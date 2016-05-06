///////////////////////////////////////////////////////////////////////
// FileMgr.cs - Pass files references depend on requirement          //
// ver 1.0                                                           //
// Language:    C#, .Net Framework 4.5                               //
// Platform:    MacBook Air, Win8.1 pro, Visual Studio 2013          //
// Application: CSE681 - F14 - project#2 CodeAnalyzer                //
// Author:      Huijun Lin, hlin14@syr.edu                           //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module is to pass the files reference according to the requirement.
 * According to option "/S" to decide whether the bool recurse is true.
 * Then find the file depends on path and patterns.
 * return the file references by getFiles() method.
 * 
 */
/* Required Files:
 * FileMgr.cs
 * 
 * 
 * Build command:
 *   csc /define:TEST_FILEMGR FileMgr.cs
 *   
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 05 Oct 2014
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileManager
{
    public class FileMgr
    {
        private List<string> files = new List<string>();
        private List<string> patterns = new List<string>();
        private bool recurse;
        
        //Set recurse method
        public FileMgr(bool rec)
        {
            recurse = rec;
        }
        /// <summary>
        /// Find path and save.
        /// </summary>
        /// <param name="path"></param>
        public void findFiles(string path)
        {
            // No patterns then set it all
            if (patterns.Count == 0)
                addPattern("*.*");
            // find file according to each pattern in list

            foreach (string pattern in patterns)
            {
                string[] newFiles = Directory.GetFiles(path, pattern);
                for (int i = 0; i < newFiles.Length; ++i)
                    newFiles[i] = Path.GetFullPath(newFiles[i]);
                files.AddRange(newFiles);
            }
            //find in subdirectory
            if (recurse)
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                    findFiles(dir);
            }
        }

        /// <summary>
        /// add pattern to pattern list
        /// </summary>
        /// <param name="pattern"></param>
        public void addPattern(string pattern)
        {
            patterns.Add(pattern);
        }

        /// <summary>
        /// return the files references
        /// </summary>
        /// <returns name="files"></returns>
        public List<string> getFiles()
        {
            return files;
        }

#if(TEST_FILEMGR)
        static void Main(string[] args)
        {
            Console.Write("\n  Testing FileMgr Class");
            Console.Write("\n =======================\n");

            FileMgr fm = new FileMgr(true);
            fm.addPattern("*.cs");
            fm.findFiles("../../");
            List<string> files = fm.getFiles();
            foreach (string file in files)
                Console.Write("\n  {0}", file);
            Console.Write("\n\n");
            
        }
#endif
    }
}
