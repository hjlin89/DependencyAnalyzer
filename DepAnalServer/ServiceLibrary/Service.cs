/////////////////////////////////////////////////////////////////////////
// Service.cs  -  Implementation of IService                           //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    Macbook Air, Win 8.1 pro, Visual Studio 2013           //
// Application: Pr#4 DepAnalyzer, CSE681, Fall 2014                    //
// Author:      Huijun Lin, hlin14@syr.edu                             //
/////////////////////////////////////////////////////////////////////////
/*
 * Package Operations
 * ==================
 * Service for Client and Server
 * 
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   Service.cs ISerivce.cs BlockingQueue.cs
 * 
 *  
 * Maintenance History
 * ===================
 * 
 * ver 1.0 : 17 Nov 14
 *   - first release
 * 
 */
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Repositorylib;

namespace ServiceLibrary
{
    /////////////////////////////////////////////////////////////
    // Receiver hosts Communication service used by other Peers

    public class Receiver : IService
    {
        static BlockingQueue<Message<TypeElem, TypeDepElem>> rcvBlockingQ = null;
        ServiceHost service = null;

        public Receiver()
        {
            if (rcvBlockingQ == null)
                rcvBlockingQ = new BlockingQueue<Message<TypeElem, TypeDepElem>>();
        }

        public void Close()
        {
            service.Close();
        }

        //  Create ServiceHost for Communication service

        public void CreateRecvChannel(string address)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            Uri baseAddress = new Uri(address);
            binding.MaxBufferPoolSize = 213738367;
            service = new ServiceHost(typeof(Receiver), baseAddress);
            service.AddServiceEndpoint(typeof(IService), binding, baseAddress);
            service.Open();
        }

        // Implement service method to receive messages from other Peers

        public void PostMessage(Message<TypeElem, TypeDepElem> msg)
        {
            rcvBlockingQ.enQ(msg);
        }

        // Implement service method to extract messages from other Peers.
        // This will often block on empty queue, so user should provide
        // read thread.

        public Message<TypeElem, TypeDepElem> GetMessage()
        {
            return rcvBlockingQ.deQ();
        }
    }
    ///////////////////////////////////////////////////
    // client of another Peer's Communication service

    public class Sender
    {
        IService channel;
        string lastError = "";
        BlockingQueue<Message<TypeElem, TypeDepElem>> sndBlockingQ = null;
        Thread sndThrd = null;
        int tryCount = 0, MaxCount = 10;

        // Processing for sndThrd to pull msgs out of sndBlockingQ
        // and post them to another Peer's Communication service

        void ThreadProc()
        {
            while (true)
            {
                Message<TypeElem, TypeDepElem> msg = sndBlockingQ.deQ();
                channel.PostMessage(msg);
            }
        }

        // Create Communication channel proxy, sndBlockingQ, and
        // start sndThrd to send messages that client enqueues

        public Sender(string url)
        {
            sndBlockingQ = new BlockingQueue<Message<TypeElem, TypeDepElem>>();
            while (true)
            {
                try
                {
                    CreateSendChannel(url);
                    tryCount = 0;
                    break;
                }
                catch (Exception ex)
                {
                    if (++tryCount < MaxCount)
                        Thread.Sleep(100);
                    else
                    {
                        lastError = ex.Message;
                        break;
                    }
                }
            }
            sndThrd = new Thread(ThreadProc);
            sndThrd.IsBackground = true;
            sndThrd.Start();
        }

        // Create proxy to another Peer's Communicator

        public void CreateSendChannel(string address)
        {
            EndpointAddress baseAddress = new EndpointAddress(address);
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxBufferPoolSize = 213738367;
            ChannelFactory<IService> factory
              = new ChannelFactory<IService>(binding, address);
            channel = factory.CreateChannel();
        }

        // Sender posts message to another Peer's queue using
        // Communication service hosted by receipient via sndThrd

        public void PostMessage(Message<TypeElem, TypeDepElem> msg)
        {
            sndBlockingQ.enQ(msg);
        }

        public string GetLastError()
        {
            string temp = lastError;
            lastError = "";
            return temp;
        }

        public void Close()
        {
            ChannelFactory<IService> temp = (ChannelFactory<IService>)channel;
            temp.Close();
        }
    }
}