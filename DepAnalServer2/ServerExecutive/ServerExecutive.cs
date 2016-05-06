/////////////////////////////////////////////////////////////////////////
// ServerExecutive.cs  -  WPF Window of Client                         //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    Macbook Air, Win 8.1 pro, Visual Studio 2013           //
// Application: Pr#4 DepAnalyzer, CSE681, Fall 2014                    //
// Author:      Huijun Lin, hlin14@syr.edu                             //
/////////////////////////////////////////////////////////////////////////
/*
 * 
 * Module Operations
 * =================
 * Server Executive
 * Set up a server and listen
 * when a request from client come
 * It will analyzer according to cmd input
 * 
 * 
 * Methods:
 * 1. public static void writeXML(Dictionary<string, List<TypeDepElem>> typeDeps, Dictionary<string, List<string>> pkgDeps)
 * 2. public static List<string> readTypeDep()
 * 3. public static List<string> readPkgDep()
 * 
 * 
 * Build Process
 * =============
 * Required Files:
 *   ServerExecutive.cs CmdParser.cs Analyzer.cs Parser.cs Semi.cs RulesAndActions.cs Toker.cs FileMgr.cs Repository.cs
 * 
 * Compiler Command:
 * csc ServerExecutive [port]
 *  
 * Maintenance History
 * ===================
 * 
 * 
 * ver 1.0 : 17 Nov 14
 *   - first release
 * 
 */
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using Cmd;
using CodeAnalysis;
using Repositorylib;
using ServiceLibrary;

namespace DepAnalServer
{
    class ServerExecutive
    {
        Sender sndr;
        Receiver rcvr;
        Message<TypeElem,TypeDepElem> rcvdMsg;
        Message<TypeElem, TypeDepElem> localMsg;
        string localAddr;
        string localPort;
        // store client addr and port to send it back
        string clientAddr;
        string clientPort;


        Thread rcvThrd = null;
        delegate void NewMessage(Message<TypeElem, TypeDepElem> msg);
        event NewMessage onNewMessage;

        void ThreadProc()
        {
            while(true)
            {
                Console.WriteLine("Monintoring sender queue");

                rcvdMsg = rcvr.GetMessage();

                Console.WriteLine("Get Message. Invoke message handler");

                //dispatcher
                onNewMessage.Invoke(rcvdMsg);
            }
        }

        /// <summary>
        /// handle new coming msg
        /// </summary>
        /// <param name="msg"></param>
        void onNewMessageHandler(Message<TypeElem, TypeDepElem> msg)
        {
            try
            {
                // after that send back msg to sender
                if (msg.types == null)
                {
                    clientAddr = msg.address;
                    clientPort = msg.port;
                    localMsg.cmd = msg.cmd;
                    localMsg.recursive = msg.recursive;
                    msg = findType(msg);
                    localMsg.types = msg.types;
                    msg.address = localAddr;
                    msg.port = localPort;
                    int count = msg.serverAddrs.Count;
                    for (int i = 0; i < count; i++)
                    {
                        //Store Server's port and addr in list  
                        if (msg.serverPorts[i] == localPort && msg.serverAddrs[i] == localAddr)
                            continue;
                        localMsg.serverAddrs.Add(msg.serverAddrs[i]);
                        localMsg.serverPorts.Add(msg.serverPorts[i]);
                        string endpoint = msg.serverAddrs[i] + ":" + msg.serverPorts[i] + "/IService";
                        sndr = new Sender(endpoint);
                        sndr.PostMessage(msg);
                    }
                }
                else
                {
                    //merge local type table
                    localMsg.serverPorts.RemoveAt(0);
                    localMsg.serverAddrs.RemoveAt(0);
                    merge(msg.types);
                }
                if (localMsg.serverPorts.Count == 0)
                {
                    localMsg = doDepAnal(localMsg);
                    Message<TypeElem, TypeDepElem> sendMsg;
                    sendMsg = new Message<TypeElem, TypeDepElem>();
                    sendMsg.typeDep = localMsg.typeDep;
                    sendMsg.packDep = localMsg.packDep;
                    string endpoint = clientAddr + ":" + clientPort + "/IService";
                    sndr = new Sender(endpoint);
                    sndr.PostMessage(sendMsg);
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }

        /// <summary>
        /// call Analyzer method to do Dependency analyze
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        Message<TypeElem, TypeDepElem> doDepAnal(Message<TypeElem, TypeDepElem> msg)
        {
            List<string> paths = new List<string>();
            List<string> patterns = new List<string>();
            CmdParser cmdparser = new CmdParser();
            cmdparser.parseArgs(msg.cmd, ref paths, ref patterns);
            Console.WriteLine("Start dependency analyzing:");

            try
            {
                string[] files = Analyzer.getFiles(paths, patterns, msg.recursive);
                msg.filename = files;
                Repository rep_ = Analyzer.DepAnal(files, msg.types);
                /// store the rep into the Message
                Console.WriteLine();
                msg.typeDep = rep_.depTable.typeDep;
                msg.packDep = rep_.depTable.PackDep;
                rep_.depTable.show();
                rep_.depTable.pkgShow();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return msg;
        }

        /// <summary>
        /// merge type table
        /// </summary>
        /// <param name="src"></param>
        void merge(Dictionary<string,List<TypeElem>> src)
        {
            foreach (string Type in src.Keys)
            {
                if (!localMsg.types.Keys.Contains(Type))
                {
                    List<TypeElem> elems = new List<TypeElem>();
                    foreach (TypeElem te in src[Type])
                    {
                        TypeElem elem = new TypeElem();
                        elem.Filename = te.Filename;
                        elem.Namespace = te.Namespace;
                        elems.Add(elem);
                    }
                    localMsg.types[Type] = elems;
                }

                if(localMsg.types.Keys.Contains(Type))
                {
                    List<TypeElem> elems = new List<TypeElem>();
                    foreach (TypeElem te in src[Type])
                    {
                        foreach(TypeElem te1 in localMsg.types[Type])
                        {
                            if(te1.Namespace != te.Namespace)
                            {
                                TypeElem elem = new TypeElem();
                                elem.Filename = te.Filename;
                                elem.Namespace = te.Namespace;
                                localMsg.types[Type].Add(elem);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// call Analyzer to find Type
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        Message<TypeElem, TypeDepElem> findType(Message<TypeElem, TypeDepElem> msg)
        {
            List<string> paths = new List<string>();
            List<string> patterns = new List<string>();
            CmdParser cmdparser = new CmdParser();
            cmdparser.parseArgs(msg.cmd, ref paths, ref patterns);

            Console.WriteLine("Start analyzing");

            try
            {
                string[] files = Analyzer.getFiles(paths, patterns, msg.recursive);
                msg.filename = files;
                Repository rep_ = Analyzer.findTypes(files);

                /// store the rep into the Message
                Console.WriteLine();
                msg.types = rep_.typeTable.types;
                rep_.typeTable.show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return msg;
        }


        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
          
            ServerExecutive server = new ServerExecutive();

            server.localMsg = new Message<TypeElem, TypeDepElem>();
            server.localMsg.serverPorts = new List<string>();
            server.localMsg.serverAddrs = new List<string>();

            server.onNewMessage += server.onNewMessageHandler;

            // Listen msg from client
            server.localPort = args[0];
            server.localAddr = "http://localhost";
            string endpoint = server.localAddr + ":" +server.localPort + "/IService";

            try
            {
                server.rcvr = new Receiver();
                server.rcvr.CreateRecvChannel(endpoint);

                // creat receive thread which call rcvrBlockinQ.deQ
                server.rcvThrd = new Thread(new ThreadStart(server.ThreadProc));
                server.rcvThrd.IsBackground = true;
                server.rcvThrd.Start();
                Console.WriteLine("Server Start: Listening port " + server.localPort);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();

        }
    }
}
