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
        /// merge another table into one single table
        /// </summary>
        /// <param name="srcTable"></param>
        /// <returns></returns>
        public uint merge(DepTable srcTable)
        {
            uint count = 0;
            foreach (string Type in srcTable.typeDep.Keys)
            {
                if (!typeDep.Keys.Contains(Type))
                {
                    ++count;
                    List<TypeDepElem> elems = new List<TypeDepElem>();
                    foreach (TypeDepElem te in srcTable.typeDep[Type])
                    {
                        TypeDepElem elem = new TypeDepElem();
                        elem.relationship = te.relationship;
                        elem.typeName = te.typeName;
                        elems.Add(elem);
                    }
                    typeDep[Type] = elems;
                }
                else
                {   // for Type in the table but with different namespace
                    foreach (TypeDepElem te in srcTable.typeDep[Type])
                    {
                        foreach (TypeDepElem te1 in typeDep[Type])
                            if (te1.typeName != te.typeName)
                            {
                                TypeDepElem elem = new TypeDepElem();
                                elem.relationship = te.relationship;
                                elem.typeName = te.typeName;
                                typeDep[Type].Add(elem);
                            }
                    }
                }
            }
            return count;
        }

    }

}
