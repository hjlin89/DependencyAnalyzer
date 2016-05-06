/////////////////////////////////////////////////////////////////////////
// IService.cs  -  Service for communication between client and server //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    Macbook Air, Win 8.1 pro, Visual Studio 2013           //
// Application: Pr#4 DepAnalyzer, CSE681, Fall 2014                    //
// Author:      Huijun Lin, hlin14@syr.edu                             //
/////////////////////////////////////////////////////////////////////////
/*
 * 
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using Repositorylib;

namespace ServiceLibrary
{
    /// <summary>
    /// service contract 
    /// </summary>
    [ServiceContract]
    public interface IService
    {
        [OperationContract(IsOneWay = true)]
        void PostMessage(Message<TypeElem, TypeDepElem> msg);

        Message<TypeElem, TypeDepElem> GetMessage();
    }

    [DataContract]
    public class Message<T, C>
    {
        [DataMember]
        string cmdInput = "../../*.cs"; 


        // store client port
        [DataMember]
        public string port { get; set; }

        // store client address
        [DataMember]
        public string address { get; set; }

        // store cmd input
        [DataMember]
        public string cmd { get { return cmdInput; } set { cmdInput = value; } }

        // store option recursive
        [DataMember]
        public bool recursive { get; set; }

        // store filenames - for test, not used in here
        [DataMember]
        public string[] filename { get; set; }

        // store server's Ports 
        [DataMember]
        public List<string> serverPorts { get; set; }

        // store server's Addr
        [DataMember]
        public List<string> serverAddrs { get; set; }


        // types table
        [DataMember]
        public Dictionary<string, List<T>> types { get; set; }

        // typeDep table
        [DataMember]
        public Dictionary<string, List<C>> typeDep { get; set; }

        // PkgDep table
        [DataMember]
        public Dictionary<string, List<string>> packDep { get; set; }

    }
}

