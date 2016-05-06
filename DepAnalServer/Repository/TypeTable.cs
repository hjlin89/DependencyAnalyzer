/////////////////////////////////////////////////////////////////////////
// TypeTable.cs  -  Type table                                         //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    Macbook Air, Win 8.1 pro, Visual Studio 2013           //
// Application: Pr#4 DepAnalyzer, CSE681, Fall 2014                    //
// Author:      Huijun Lin, hlin14@syr.edu                             //
/////////////////////////////////////////////////////////////////////////
/*
 * Package Operations
 * ==================
 * storage class for type table
 * Table is consist of type Elem 
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   TypeTable.cs
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
    public class TypeElem
    {
        public string Namespace { get; set; }
        public string Filename { get; set; }
        public string type { get; set; }
    }

    /// <summary>
    /// Type table
    /// </summary>
    public class TypeTable
    {
        // dictionary store the typeElem with typename
        public Dictionary<string, List<TypeElem>> types { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TypeTable()
        {
            types = new Dictionary<string, List<TypeElem>>();
        }

        /// <summary>
        /// add new TypeElem into table
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Namespace"></param>
        /// <param name="Filename"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool add(string Type, string Namespace, string Filename, string type)
        {
            if (types.Keys.Contains(Type))
            {
                foreach (TypeElem te in types[Type])
                {
                    if (te.Namespace == Namespace)
                        return false;
                }
                TypeElem insert = new TypeElem();
                insert.Namespace = Namespace;
                insert.Filename = Filename;
                insert.type = type;
                types[Type].Add(insert);
                return true;
            }
            TypeElem elem = new TypeElem();
            elem.Namespace = Namespace;
            elem.Filename = Filename;
            types[Type] = new List<TypeElem>();
            types[Type].Add(elem);
            return true;
        }

        /// <summary>
        /// remove type
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public bool remove(string Type)
        {
            return types.Remove(Type);
        }

        /// <summary>
        /// check type contains
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public bool contains(string Type)
        {
            return types.Keys.Contains(Type);
        }

        /// <summary>
        /// merge type table
        /// </summary>
        /// <param name="srcTable"></param>
        /// <returns></returns>
        public uint merge(TypeTable srcTable)
        {
            uint count = 0;
            foreach (string Type in srcTable.types.Keys)
            {
                if (!types.Keys.Contains(Type))
                {
                    ++count;
                    List<TypeElem> elems = new List<TypeElem>();
                    foreach (TypeElem te in srcTable.types[Type])
                    {
                        TypeElem elem = new TypeElem();
                        elem.Filename = te.Filename;
                        elem.Namespace = te.Namespace;
                        elems.Add(elem);
                    }
                    types[Type] = elems;
                }
                else
                {
                    foreach (TypeElem te in srcTable.types[Type])
                    {
                        foreach (TypeElem te1 in types[Type])
                            if (te1.Namespace != te.Namespace)
                            {
                                TypeElem elem = new TypeElem();
                                elem.Filename = te.Filename;
                                elem.Namespace = te.Namespace;
                                types[Type].Add(elem);
                            }
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// show type table on console
        /// </summary>
        public void show()
        {
            Console.WriteLine("TypeTable Contents");
            foreach (string Type in types.Keys)
            {
                Console.Write("\n  {0}", Type);
                foreach (TypeElem te in types[Type])
                {
                    Console.Write("\n    Namespace = {0,-20} File = {1,-20}", te.Namespace, te.Filename);
                }
            }
        }
    }
}
