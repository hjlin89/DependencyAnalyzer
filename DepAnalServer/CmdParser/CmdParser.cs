/////////////////////////////////////////////////////////////////////////////
// CmdParser.cs - Parse the cmd input into path, patterns and options      //
//                                                                         //
// Platform:    Macbook Air, Win 8.1 pro, Visual Studio 2013               //
// Application: CSE681 - F14 - project#2 CodeAnalyzer                      //
// Author:      Huijun Lin, hlin14@syr.edu                                 //
/////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations
 * =================
 * 
 * cmdParser, provides method to parse the cmdline input.
 * 
 * There is one method:
 * public void parseArgs(string[] args, ref List<string> thePath, ref List<string> patterns, ref List<string> options) 
 * 
 * ParseArgs get string args from commmand line input. then parse it into three list- thePath, patterns and options.
 * Then send back the three list through ref.
 * 
 * Build Process
 * =============
 * Required Files:
 *   CmdParser.cs 
 * 
 * Compiler Command:
 *   csc /define:TEST_CmdParser CmdParser.cs
 *   
 * 
 * Maintenance History
 * ===================
 * ver 1.1 : 01 Oct 14
 * -first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cmd
{
    public class CmdParser
    {

        /// <summary>
        /// Parse args from cmdline input.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="thePath"></param>
        /// <param name="patterns"></param>
        /// <param name="options"></param>
        public void parseArgs(string arg, ref List<string> thePath, ref List<string> patterns) 
        {
            string path, patternSet, pattern;

                Console.Write("\n  Command Line Argument = \"" + arg + "\"");
                try
                {
                    // Then seperate it from path and patterns
                    int pos = arg.LastIndexOf('\\');
                    if (pos == -1)
                        pos = arg.LastIndexOf('/');
                    if (pos > -1)
                    {
                        ++pos;
                        path = arg.Remove(pos, arg.Length - pos);
                        patternSet = arg.Remove(0, pos);
                        thePath.Add(path);
                        while ((pos = patternSet.LastIndexOf('.')) > -1)
                        {
                            --pos;
                            pattern = patternSet.Remove(0, pos);
                            patterns.Add(pattern);
                            if (pos == 0)
                            {
                                break;
                            }
                            patternSet = patternSet.Remove(pos, patternSet.Length - pos);
                        }
                    }
                    else
                    {
                        patternSet = arg;
                        while ((pos = patternSet.LastIndexOf('.')) > -1)
                        {
                            --pos;
                            pattern = patternSet.Remove(0, pos);
                            patterns.Add(pattern);
                            if (pos == 0)
                            {
                                break;
                            }
                            patternSet = patternSet.Remove(pos, patternSet.Length - pos);
                        }
                    }
                }
                catch (Exception except)
                {
                    Console.Write("\n  error in command line argument: {0}", arg);
                    Console.Write("\n  {0}\n\n", except.Message);
                }
                Console.Write("\n\n");
            
            string path_ = string.Join(", ", thePath.ToArray());
            string patterns_ = string.Join(", ", patterns.ToArray());
            Console.Write("\n  path = {0}\n  file pattern = {1}\n \n", path_, patterns_);
        }
#if(TEST_CmdParser)
        static void Main(string[] args)
        {
            CmdParser cmdParser = new CmdParser();
            string arg = "../../*.**.cs";
            List<string> thePath_=new List<string>();
            List<string> patterns_ = new List<string>();
            cmdParser.parseArgs(arg, ref thePath_, ref patterns_);
            Console.WriteLine("Path:");
            foreach(string s in thePath_)
                Console.WriteLine(s);
            Console.WriteLine("Patterns:");
            foreach (string s in patterns_)
                Console.WriteLine(s);
        }
#endif
    }
}
