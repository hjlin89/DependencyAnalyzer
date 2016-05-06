/////////////////////////////////////////////////////////////////////////
// DepTable.cs  -  Dependency table                                    //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    Macbook Air, Win 8.1 pro, Visual Studio 2013           //
// Application: Pr#4 DepAnalyzer, CSE681, Fall 2014                    //
// Author:      Huijun Lin, hlin14@syr.edu                             //
/////////////////////////////////////////////////////////////////////////
/*
 * Package Operations
 * ==================
 * storage class for Dependency table
 * There are two dependency table 
 * one is typedep table
 * another one is Pkgdep table
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   DepTable.cs
 * 
 * Compiler Command:
 * It is a class for storing information, there is no entry point for test
 * 
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
using System.Threading.Tasks;

namespace Repositorylib
{
    // basic type dependency elem
    public class TypeDepElem
    {
        public string relationship { get; set; }
        public string typeName { get; set; }
        public string type { get; set; }

    }

    // Dependency table
    public class DepTable
    {
        // consist of two dictionary, in which one is type and another is package 
        public Dictionary<string, List<TypeDepElem>> typeDep { get; set; }
        public Dictionary<string, List<string>> PackDep { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public DepTable()
        {
            typeDep = new Dictionary<string, List<TypeDepElem>>();
            PackDep = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// add a new depElem in table
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="relationship"></param>
        /// <param name="typename"></param>
        /// <returns></returns>
        public bool add(string Type, string relationship, string typename)
        {
            if (typeDep.Keys.Contains(Type))
            {
                foreach(TypeDepElem te in typeDep[Type])
                {
                    if(te.relationship==relationship&&te.typeName==typename)
                        return false;
                }
                TypeDepElem insert = new TypeDepElem();
                insert.relationship = relationship;
                insert.typeName = typename;
                typeDep[Type].Add(insert);
                return true;
            }
            TypeDepElem elem = new TypeDepElem();
            elem.relationship = relationship;
            elem.typeName = typename;
            typeDep[Type] = new List<TypeDepElem>();
            typeDep[Type].Add(elem);
            return true;
        }

        /// <summary>
        /// display type table on console
        /// </summary>
        /// <returns></returns>
        public void show()
        {
            Console.WriteLine("TypeDepTable Contents");
            foreach (string Type in typeDep.Keys)
            {
                Console.Write("\n  {0}", Type);
                foreach (TypeDepElem te in typeDep[Type])
                {
                    Console.Write("\n    relationship = {0,-20} typename = {1,-20}", te.relationship, te.typeName);
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// display pkg table on console
        /// </summary>
        /// <returns></returns>
        public void pkgShow()
        {
            Console.WriteLine("PkgDepTable Contents");
            foreach (string Type in PackDep.Keys)
            {
                Console.Write("\n Parent {0}:", Type);
                foreach (string t in PackDep[Type])
                {
                    Console.Write("\n    Child = {0,-20} ", t);
                }
            }
            Console.WriteLine();
        }
    }

#if(TEST_TypeDepTable)
    static void Main()
    {
      DepTable dt1 = new DepTable();
      dt.add("Type1", "relationship", "Type2");
      dt1.show();

      DepTable dt2 = new DepTable();
      dt2.add("Type3", "relationship", "Type2");

      Console.Write("\n\n  after merging with:\n");
      dt2.show();

      Console.Write("\n\n  the merged results are:\n");
      dt.merge(dt2);
      dt.show();

      Console.Write("\n\n");
    }
#endif

}
