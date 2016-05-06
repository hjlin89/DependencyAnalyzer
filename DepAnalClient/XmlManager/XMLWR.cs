/////////////////////////////////////////////////////////////////////////////
// XMLWR.cs - Helping client to Read and Write XML file                    //
//                                                                         //
// Platform:    Macbook Air, Win 8.1 pro, Visual Studio 2013               //
// Application: CSE681 - F14 - project#4 DepAnalyzer                       //
// Author:      Huijun Lin, hlin14@syr.edu                                 //
/////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations
 * =================
 * XMLWR, Helping client to Read and Write XML file.
 * 
 * 
 * Methods:
 * 1. public static void writeXML(Dictionary<string, List<TypeDepElem>> typeDeps, Dictionary<string, List<string>> pkgDeps)
 * 2. public static List<string> readTypeDep()
 * 3. public static List<string> readPkgDep()
 * 
 * Those three static method is for client to write TypeTable and PkgTalbe into XML file
 * or Read XML with such information just write.
 * 
 * Build Process
 * =============
 * Required Files:
 *   XMLWR.cs 
 * 
 * Compiler Command:
 *   csc /define:TEST_XML XMLWR.cs
 *   
 * 
 * Maintenance History
 * ===================
 * ver 1.1 : 10 Nov 14
 * -first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Repositorylib;

namespace XmlManager
{
    public class XMLWR
    {
        /// <summary>
        /// Write XML using information from two tables.
        /// </summary>
        /// <param name="typeDeps"></param>
        /// <param name="pkgDeps"></param>
        public static void writeXML(Dictionary<string, List<TypeDepElem>> typeDeps, Dictionary<string, List<string>> pkgDeps)
        {
            // initiation of XML elements
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement root = new XElement("root");
            xml.Add(root);
            XElement TypeDep = new XElement("TypeDepTable");
            XElement PkgDep = new XElement("PkgDepTable");

            // write typeDepTable
            foreach(string type in typeDeps.Keys)
            {
                XElement theType = new XElement("Type", new XAttribute("Name",type));
                foreach(TypeDepElem te in typeDeps[type])
                {
                    XElement relationship = new XElement("relationship",te.relationship);
                    XElement typename = new XElement("DepType", new XAttribute("Name",te.typeName));
                    typename.Add(relationship);
                    theType.Add(typename);
                }
                TypeDep.Add(theType);
            }

            // write PkgDepTable
            foreach (string type in pkgDeps.Keys)
            {
                XElement Parent = new XElement("Parent", new XAttribute("Name", type));
                foreach (string s in pkgDeps[type])
                {
                    XElement child = new XElement("Child", s);
                    Parent.Add(child);
                }
                PkgDep.Add(Parent);
            }

            // add to root and save
            root.Add(TypeDep);
            root.Add(PkgDep);
            xml.Save(@".\DependencyTable.xml");
        }

        /// <summary>
        /// read type dependency table in xml
        /// and output a List<stirng> with arranged information in it
        /// </summary>
        /// <returns>List<string></returns>
        public static List<string> readTypeDep()
        {
            List<string> inserts = new List<string>();
            XDocument doc = XDocument.Load(@".\DependencyTable.xml");

            // read Type in table
            var Types = from x in
                            doc.Elements("root")
                            .Elements("TypeDepTable")
                            .Elements("Type")
                        select x;

            foreach (var Type in Types)
            {
                // Read each Deptype and print
                var DepTypes = from x in
                                   Type.Elements("DepType")
                               select x;
                foreach (var DepType in DepTypes)
                {
                    string insert = Type.Attribute("Name").Value + " "
                                    + DepType.Value + " "
                                    + DepType.Attribute("Name").Value;
                    inserts.Add(insert);
                }
            }
            return inserts;
        }

        /// <summary>
        /// read Package dependency table in xml
        /// and output a List<stirng> with arranged information in it
        /// </summary>
        /// <returns>List<string></returns>
        public static List<string> readPkgDep()
        {
            List<string> inserts = new List<string>();
            XDocument doc = XDocument.Load(@".\DependencyTable.xml");

            // read parents in table
            var Parents = from x in
                              doc.Elements("root")
                              .Elements("PkgDepTable")
                              .Elements("Parent")
                          select x;

            foreach (var Parent in Parents)
            {
                // read childs for each parents
                var Childs = from x in
                                 Parent.Elements("Child")
                             select x;
                foreach (var Child in Childs)
                {
                    string insert = "Parent " + Parent.Attribute("Name").Value +
                                    " is depend on Child "
                                    + Child.Value;
                    inserts.Add(insert);
                }
            }
            return inserts;
        }

#if(TEST_XML)
        public static void Main(string[] args)
        {
            Dictionary<string, List<TypeDepElem>> typeDeps = new Dictionary<string,List<TypeDepElem>>();
            List<TypeDepElem> list = new List<TypeDepElem>();
            TypeDepElem tde = new TypeDepElem();
            tde.relationship ="Inheritance";
            tde.typeName = "type2";
            list.Add(tde);
            typeDeps["type1"] = list;


            Dictionary<string, List<string>> pkgDeps = new Dictionary<string, List<string>>();
            List<string> list2 = new List<string>();
            string str = "childtype";
            list2.Add(str);
            pkgDeps["typeParent"] = list2;

            Console.WriteLine(typeDeps.ToString());
            Console.WriteLine(pkgDeps.ToString());

            XMLWR.writeXML(typeDeps, pkgDeps);

            List<string> PkgDepList = XMLWR.readPkgDep(); ;
            List<string> TypeDepList = XMLWR.readTypeDep(); ;
            foreach (string elem in PkgDepList)
                Console.WriteLine(elem);
            foreach (string elem in TypeDepList)
                Console.WriteLine(elem);
        }
#endif
    }
}
