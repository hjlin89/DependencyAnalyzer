///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// ver 2.1                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: Demonstration for CSE681, Project #2, Fall 2011      //
// Author:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 2.2 : 24 Sep 2011
 * - modified Semi package to extract compile directives (statements with #)
 *   as semiExpressions
 * - strengthened and simplified DetectFunction
 * - the previous changes fixed a bug, reported by Yu-Chi Jen, resulting in
 * - failure to properly handle a couple of special cases in DetectFunction
 * - fixed bug in PopStack, reported by Weimin Huang, that resulted in
 *   overloaded functions all being reported as ending on the same line
 * - fixed bug in isSpecialToken, in the DetectFunction class, found and
 *   solved by Zuowei Yuan, by adding "using" to the special tokens list.
 * - There is a remaining bug in Toker caused by using the @ just before
 *   quotes to allow using \ as characters so they are not interpreted as
 *   escape sequences.  You will have to avoid using this construct, e.g.,
 *   use "\\xyz" instead of @"\xyz".  Too many changes and subsequent testing
 *   are required to fix this immediately.
 * ver 2.1 : 13 Sep 2011
 * - made BuildCodeAnalyzer a public class
 * ver 2.0 : 05 Sep 2011
 * - removed old stack and added scope stack
 * - added Repository class that allows actions to save and 
 *   retrieve application specific data
 * - added rules and actions specific to Project #2, Fall 2010
 * ver 1.1 : 05 Sep 11
 * - added Repository and references to ScopeStack
 * - revised actions
 * - thought about added folding rules
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 * Planned Modifications (not needed for Project #2):
 * --------------------------------------------------
 * - add folding rules:
 *   - CSemiExp returns for(int i=0; i<len; ++i) { as three semi-expressions, e.g.:
 *       for(int i=0;
 *       i<len;
 *       ++i) {
 *     The first folding rule folds these three semi-expression into one,
 *     passed to parser. 
 *   - CToker returns operator[]( as four distinct tokens, e.g.: operator, [, ], (.
 *     The second folding rule coalesces the first three into one token so we get:
 *     operator[], ( 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Repositorylib;


namespace CodeAnalysis
{
    /////////////////////////////////////////////////////////
    // pushes scope info on stack when entering new scope

    public class PushStack : AAction
    {
        Repository repo_;

        public PushStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi, string filename)
        {
            Elem elem = new Elem();
            elem.type = semi[0];  // expects type
            elem.name = semi[1];  // expects name
            repo_.stack.push(elem);
            if (semi[0]!="namespace"&&semi[0] != "control" && semi[0] != "function" && semi[0] != "array")
                repo_.typeTable.add(semi[1], repo_.getCurrentNamespace(), filename,semi[0]);
            if (elem.type == "control" || elem.name == "anonymous")
                return;

            if (AAction.displaySemi)
            {
                Console.WriteLine();
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }
    }
    /////////////////////////////////////////////////////////
    // pops scope info from stack when leaving scope

    public class PopStack : AAction
    {
        Repository repo_;

        public PopStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi, string filename)
        {
            Elem elem;
            try
            {
                elem = repo_.stack.pop();
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            local.Add(elem.type).Add(elem.name);
            if (local[0] == "control")
                return;

            if (AAction.displaySemi)
            {
                Console.WriteLine();
                Console.Write("leaving  ");
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                Console.Write("{0}", indent);
                this.display(local); // defined in abstract action
            }
        }
    }
    ///////////////////////////////////////////////////////////
    // action to print function signatures - not used in demo

    public class PrintFunction : AAction
    {
        Repository repo_;

        public PrintFunction(Repository repo)
        {
            repo_ = repo;
        }
        public override void display(CSsemi.CSemiExp semi)
        {
            Console.Write("\n    ");
            for (int i = 0; i < semi.count; ++i)
                if (semi[i] != "\n" && !semi.isComment(semi[i]))
                    Console.Write("{0} ", semi[i]);
        }
        public override void doAction(CSsemi.CSemiExp semi, string filename)
        {
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // concrete printing action, useful for debugging

    public class Print : AAction
    {
        Repository repo_;

        public Print(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi, string filename)
        {
            this.display(semi);
        }
    }

    /////////////////////////////////////////////////////////
    // concrete printing action, useful for debugging

    public class PushPkgDep : AAction
    {
        Repository repo_;

        public PushPkgDep(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi, string filename)
        {
            if(repo_.depTable.PackDep.ContainsKey(semi[0]))
            {
                if(repo_.depTable.PackDep[semi[0]].Contains(semi[1]))
                    return;
                repo_.depTable.PackDep[semi[0]].Add(semi[1]);
            }
            else
            {
                List<string> elem = new List<string>();
                elem.Add(semi[1]);
                repo_.depTable.PackDep[semi[0]] = elem;
            }
        }
    }
    /////////////////////////////////////////////////////////
    // rule to detect namespace declarations

    public class DetectNamespace : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string filename)
        {
            int index = semi.Contains("namespace");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local,filename);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect class definitions

    public class DetectClass : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string filename)
        {
            int indexCL = semi.Contains("class");
            int indexIF = semi.Contains("interface");
            int indexST = semi.Contains("struct");
            int indexEN = semi.Contains("enum");

            int index = Math.Max(indexCL, indexIF);
            index = Math.Max(index, indexST);
            index = Math.Max(index, indexEN);

            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local, filename);
                return true;
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // rule to dectect delegate definitions
    public class DetectDelegate : ARule
    {
        //----< checks for delegate keyword >---------
        public override bool test(CSsemi.CSemiExp semi,string filename)
        {
            //searching for delegate keyword
            int index = semi.Contains("delegate");
            int indexBrace = semi.Contains("(");
            if (index != -1 && indexBrace != -1 && indexBrace > index)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[indexBrace - 1]);
                doActions(local,filename);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect function definitions


    public class DetectFunction : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi, string filename)
        {
            if (semi[semi.count - 1] != "{")
                return false;

            int index = semi.FindFirst("(");
            if (index > 0 && !isSpecialToken(semi[index - 1]))
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                local.Add("function").Add(semi[index - 1]);
                doActions(local,filename);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // detect entering anonymous scope
    // - expects namespace, class, and function scopes
    //   already handled, so put this rule after those
    public class DetectAnonymousScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string filename)
        {
            int index = semi.Contains("{");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                if (index > 0 && semi[index - 1] == "=")
                    local.Add("array").Add("anonymous");
                else
                    local.Add("control").Add("anonymous");
                doActions(local,filename);
                return true;
            }
            return false;
        }
    }
 

    /////////////////////////////////////////////////////////
    // detect leaving scope

    public class DetectLeavingScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string filename)
        {
            int index = semi.Contains("}");
            if (index != -1)
            {
                doActions(semi, filename);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// rule to detect relationship shown in semi
    /// </summary>
    public class DetectRelationship : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string filename)
        {
            Repository rep = Repository.getInstance();
            foreach (string typeName in rep.typeTable.types.Keys)
            {
                        if (semi.Contains(typeName) > -1)
                        {
                            doActions(semi, "");
                            return true;
                        }
            }
            return false;
        }
    }

    /// <summary>
    /// rule to detect package relationship shown in semi
    /// </summary>
    public class DetectPkgDep : ARule
    {
        public override bool test(CSsemi.CSemiExp semi, string filename)
        {
            Repository rep = Repository.getInstance();
            foreach (string type in rep.typeTable.types.Keys)
            {
                int index = semi.Contains(type);
                foreach (TypeElem te in rep.typeTable.types[type])
                {
                    if (index != -1 && te.Namespace!=rep.getCurrentNamespace())
                    {
                        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                        // create local semiExp with tokens for type and name
                        local.displayNewLines = false;
                        local.Add(te.Namespace).Add(rep.getCurrentNamespace());
                        doActions(local, filename);
                        return true;
                    }
                }
            }
            return false;
        }
    }

    ///////////////////////////////////////////////////////
    // decide which kind of relationship is.
    public class DecideRelationship : AAction
    {
        Repository repo_;
        public DecideRelationship(Repository repo)
        {
            repo_ = repo;
        }

        // overrided doAction to call 4 IsRelationship func tion
        public override void doAction(CSsemi.CSemiExp semi, string useless)
        {
                if (isInheritance(semi))
                    return;
                if (isAggregation(semi))
                    return;
                if (isAssociation(semi))
                    return;
                if (isComposition(semi))
                    return;
        }

        /////////////////////////////////////////////////////
        // decide whether it is a inheritance relationship
        private bool isInheritance(CSsemi.CSemiExp semi)
        {
            // Decide if it is a inheritance
            int index = semi.Contains(":");
            if (index > -1)
            {
                if (semi[index - 1] == repo_.getCurrentClass())
                {
                            string relationship = "Inheritance";
                            string typeName = semi[index + 1];
                            repo_.depTable.add(semi[index - 1], relationship, typeName);
                            return true;
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////
        /// decide whether it is a aggregation relationship
        private bool isAggregation(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("new");
            if (index > -1)
                foreach (string TYPE in repo_.typeTable.types.Keys)
                {
                    int len = repo_.stack.count;
                    // check if it is a struct
                    if (TYPE == semi[index + 1])
                        foreach (TypeElem te in repo_.typeTable.types[TYPE])
                            if (te.type == "struct")
                                return false;

                    if (TYPE == repo_.getCurrentClass())
                    {
                        string relationship = "Aggregation";
                        string typeName = semi[index + 1];
                        repo_.depTable.add(TYPE, relationship, typeName);
                        return true;
                    }
                }
            return false;
        }

        ////////////////////////////////////////////////////////
        // decide whether it is a composition relationship
        private bool isComposition(CSsemi.CSemiExp semi)
        {
            //check if it is enum
            int index = semi.Contains("enum");
            if (index < 0)
                index = semi.Contains("struct");
            if (index > -1)
                foreach (string TYPE in repo_.typeTable.types.Keys)
                {
                    if (TYPE == repo_.getCurrentClass())
                    {
                        int index1 = semi.Contains(";");
                        if (index1 > 0)
                        {
                            string relationship = "Composition";
                            if (semi.Contains("=") < 0)
                                relationship = "Composition";
                            else
                                relationship = "Association";
                            string typeName = semi[index + 1];
                            repo_.depTable.add(TYPE, relationship, typeName);
                            return true;
                        }
                    }
                }
            return false;
        }

        ///////////////////////////////////////////////////////////
        // decide whether it is a using relationship

        private bool isAssociation(CSsemi.CSemiExp semi)
        {
            if (semi[semi.count - 1] != "{")
                return false;
            foreach (string TYPE in repo_.typeTable.types.Keys)
            {
                int index = semi.Contains(TYPE);
                if(index>-1)
                {
                    foreach (TypeElem te in repo_.typeTable.types[TYPE])
                    {
                        if (te.type == "enum" || te.type == "class" || te.type == "struct" || te.type == "delegate")
                        {
                            if (semi[index - 1] == "(")
                            {
                                string typeName = semi[index];
                                string relationship = "Association";
                                repo_.depTable.add(repo_.getCurrentClass(), relationship, typeName);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }

    public class BuildTypesFinder
    {
        Repository repo = new Repository();

        public void setSemi(CSsemi.CSemiExp semi)
        {
            repo.semi = semi;
        }
        public virtual Parser build()
        {
            Parser parser = new Parser();

            // decide what to show
            AAction.displaySemi = false;
            AAction.displayStack = false;  // this is default so redundant

            // action used for namespaces, classes, and functions
            PushStack push = new PushStack(repo);

            // capture namespace info
            DetectNamespace detectNS = new DetectNamespace();
            detectNS.add(push);
            parser.add(detectNS);

            // capture class info
            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);

            // capture function info
            DetectFunction detectFN = new DetectFunction();
            detectFN.add(push);
            parser.add(detectFN);

            DetectDelegate detectDL = new DetectDelegate();
            detectDL.add(push);
            parser.add(detectDL);

            // handle entering anonymous scopes, e.g., if, while, etc.
            DetectAnonymousScope anon = new DetectAnonymousScope();
            anon.add(push);
            parser.add(anon);

            // handle leaving scopes
            DetectLeavingScope leave = new DetectLeavingScope();
            PopStack pop = new PopStack(repo);
            leave.add(pop);
            parser.add(leave);

            // parser configured
            return parser;
        }
    }

    /// <summary>
    /// build for dep analyze
    /// </summary>
    public class BuildDepAnal
    {
        Repository repo = Repository.getInstance();

        public void setSemi(CSsemi.CSemiExp semi)
        {
            repo.semi = semi;
        }
        public virtual Parser build()
        {
            Parser parser = new Parser();

            // decide what to show
            AAction.displaySemi = false;
            AAction.displayStack = false;  // this is default so redundant

            // action used for namespaces, classes, and functions
            PushStack push = new PushStack(repo);

            // capture namespace info
            DetectNamespace detectNS = new DetectNamespace();
            detectNS.add(push);
            parser.add(detectNS);

            // capture class info
            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);

            // capture function info
            DetectFunction detectFN = new DetectFunction();
            detectFN.add(push);
            parser.add(detectFN);

            DetectDelegate detectDL = new DetectDelegate();
            detectDL.add(push);
            parser.add(detectDL);

            // handle entering anonymous scopes, e.g., if, while, etc.
            DetectAnonymousScope anon = new DetectAnonymousScope();
            anon.add(push);
            parser.add(anon);

            // handle leaving scopes
            DetectLeavingScope leave = new DetectLeavingScope();
            PopStack pop = new PopStack(repo);
            leave.add(pop);
            parser.add(leave);

            // find pkg dep
            DetectPkgDep pkgDep = new DetectPkgDep();
            PushPkgDep pushPkg = new PushPkgDep(repo);
            pkgDep.add(pushPkg);
            parser.add(pkgDep);
            
            // find type dep(rel)
            DetectRelationship rel = new DetectRelationship();
            DecideRelationship decide = new DecideRelationship(repo);
            rel.add(decide);
            parser.add(rel);
            // parser configured
            return parser;
        }
    }
}

