/////////////////////////////////////////////////////////////////////////
// Repository.cs  -  Repository of information                         //
// ver 1.1                                                             //
// Language:    C#, Visual Studio 10.0, .Net Framework 4.5             //
// Platform:    Macbook Air, Win 8.1 pro, Visual Studio 2013           //
// Application: Pr#4 DepAnalyzer, CSE681, Fall 2014                    //
// Author:      Huijun Lin, hlin14@syr.edu                             //
//              Jim Fawcett, CST 2-187, Syracuse University            //
//              (315) 443-3948, jfawcett@twcny.rr.com                  //
/////////////////////////////////////////////////////////////////////////
/*
 * Package Operations
 * ==================
 * Repository provides a class of information.
 * It contains stack using in parser
 * typetable and deptable using in depAnalyzer
 * 
 */
/*
 * Build Process
 * =============
 * Required Files:
 *   Repository.cs DepTable.cs TypeTable.cs ScopeStack.cs
 * 
 * Compiler Command:
 * It is a class for storing information, there is no entry point for test
 * 
 *  
 * Maintenance History
 * ===================
 * 
 * ver 1.1 : 17 Nov 14
 *   - add depTable and Typetable
 * 
 * ver 1.0 : 05 Sep 11
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
    public class Elem  // holds scope information
    {
        public string type { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");

            temp.Append("}");
            return temp.ToString();
        }
    }

    public class Repository
    {
        TypeTable typeTable_ = new TypeTable();
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        DepTable deptable_ = new DepTable();

        static Repository instance;

        public Repository()
        {
            instance = this;
        }

        public static Repository getInstance()
        {
            return instance;
        }
        // provides all actions access to current semiExp

        public CSsemi.CSemiExp semi
        {
            get;
            set;
        }

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }

        public TypeTable typeTable
        {
            get { return typeTable_; }
        }

        public DepTable depTable
        {
            get { return deptable_; }
        }


        // get current class
        public string getCurrentClass()
        {
            for (int i = stack.count - 1; i >= 0; i--)
            {
                if (stack[i].type == "class")
                {
                    return stack[i].name;
                }
            }
            return "";
        }

        //  get current Namespace
        public string getCurrentNamespace()
        {
            //checks from the stack the recent namespace and returns it.
            for (int i = stack.count - 1; i >= 0; i--)
            {
                if (stack[i].type == "namespace")
                {
                    return stack[i].name;
                }
            }
            return "";

        }

#if(TEST_REP)
        static void Main()
        {
            Console.Write("\n  Test Repository");
            Console.Write("\n =================\n");

            Console.Write("\n\n");

            TypeTable tt = new TypeTable();
            tt.add("Type1", "myNamespace", "someFile.cs", "class");
            tt.show();

            TypeTable ntt = new TypeTable();
            ntt.add("Type1", "myNamespace", "someFile.cs", "struct");

            Console.Write("\n\n  after merging with:\n");
            ntt.show();

            Console.Write("\n\n  the merged results are:\n");
            tt.merge(ntt);
            tt.show();

            Console.Write("\n\n");
        }
#endif
    }
}
