///////////////////////////////////////////////////////////////////////
//Analyzer.cs - Manages Code Analysis                                //
//                                                                   //
//Platform:    MacBook Air, Win 8.1 pro, Visual Studio 2013          //
//Application: CSE681 - F14 - project#2 CodeAnalyzer                 //
//Author:      Huijun Lin, hlin14@syr.edu                            //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations
 * =================
 * 
 * Analyzer, provides method to make a builder, which consists of rules and actions.
 * According to the rules and actions, the builder will deal with the semi.
 * 
 * 
 * There are two analysis methods:
 * 1. static public Repository findTypes(string[] files)
 * 2. static public Repository findRelationship(string[] files)
 * 
 * Each method will return the repository instance.
 * One is findTypes(string[] files). It is used to aggregate BuildTypesFinder to find 
 * information like type, name, endline, startline and complexity. 
 * The other one is findRelationships(string[] files). It is used to aggregate BuildRelationshipFinder,
 * which provides rules and actions to figure out relationships between types in the set of file.
 * 
 * Build Process
 * =============
 * Required Files:
 *   Analyzer.cs 
 * 
 * Compiler Command:
 *   csc /define:TEST_Analyzer Analyzer.cs Parser.cs IRulesAndActions.cs RulesAndActions.cs 
 *                                           Semi.cs Toker.cs
 * 
 * Maintenance History
 * ===================
 * 
 * ver 1.2 : 17 Nov 14
 * - modify doAnalysis in order to find dependency
 * ver 1.1 : 01 Oct 14
 * -modify doAnalysis() method to make it more specific.
 * ver 1.0 : Sep 14
 * -first release by Jim Fawcett
 */
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FileManager;
using Repositorylib;

namespace CodeAnalysis
{
    public class Analyzer
    {
        /// <summary>
        /// Get files references according to path and patterns
        /// Aggregate class FileMgr to get files references
        /// </summary>
        /// <param name="path"></param>
        /// <param name="patterns"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        static public string[] getFiles(List<string> paths, List<string> patterns, bool recurse)
        {
            FileMgr fm = new FileMgr(recurse);
            
            foreach (string pattern in patterns)
                fm.addPattern(pattern);

            foreach(string path in paths)
                fm.findFiles(path);

            return fm.getFiles().ToArray();
        }

        /// <summary>
        /// do analysis to find types in the set of file
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        static public Repository findTypes(string[] files)
        {
            Console.Write("\n Find all types in the file set.");
            Console.Write("\n ================================\n");
            BuildTypesFinder builder = new BuildTypesFinder();
            Parser parser = builder.build();
            foreach (object file in files) 
            {
                // parse file name out
                string filename = file as string;
                Console.WriteLine(filename);
                int pos = filename.LastIndexOf('\\');
                if (pos == -1)
                    pos = filename.LastIndexOf('/');
                if (pos > -1)
                {
                    filename = filename.Remove(0, pos+1);
                }
                Console.Write("\n  Processing file {0}\n", file as string);
                CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
                semi.displayNewLines = false;
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", file);
                    continue;
                }
                Console.Write("\n  Type and Function Analysis");
                Console.Write("\n ----------------------------");
                builder.setSemi(semi);
                try 
                {
                    // use parser.parseTypes to analyze every semi to get type information
                    while (semi.getSemi())
                        parser.parseTypes(semi,filename);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                }
                Console.WriteLine();
                semi.close();
            }
            Repository rep = Repository.getInstance();
            return rep;
        }

        /// <summary>
        /// find types dependency in the set of file.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        static public Repository DepAnal(string[] files, Dictionary<string, List<TypeElem>> types)
        {
            Repository.getInstance().typeTable.show();
            Console.Write("\n Find relationship between types in the file set.");
            Console.Write("\n ================================================\n");
            BuildDepAnal builder = new BuildDepAnal();
            Parser parser = builder.build();

            foreach (object file in files)
            {
                //Console.Write("\n  Processing file {0}\n", file as string);
                CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
                semi.displayNewLines = false;
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", file);
                    return null;
                }
                //Console.Write("\n Relationsihp Analysis");
                builder.setSemi(semi);
                try
                {
                    // use parser.parseRelationship to find is there a relationship in each semi
                    while (semi.getSemi())
                        parser.parseDep(semi, file as string);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                }
                //Console.WriteLine();
                semi.close();
            }
            Repository rep = Repository.getInstance();
            return rep;
        }


#if(TEST_Analyzer)
        static void Main(string[] args)
        {
            Analyzer a = new Analyzer();
            string path = "../../";
            List<string> patterns = new List<string>();
            patterns.Add("*.cs");
            string[] files = Directory.GetFiles(path, "*.cs");
            try
            {
                findTypes(files);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
#endif
    }
}
