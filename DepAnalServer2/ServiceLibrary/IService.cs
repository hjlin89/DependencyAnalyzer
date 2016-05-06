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
    [ServiceContract]
    public interface IService
    {
        [OperationContract(IsOneWay = true)]
        void PostMessage(Message<TypeElem,TypeDepElem> msg);

        // used only locally so not exposed as service method

        Message<TypeElem,TypeDepElem> GetMessage();
    }

    [DataContract]
    public class Message<T,C>
    {
        [DataMember]
        string cmdInput = "../../*.cs";


        [DataMember]
        public string port { get; set; }

        [DataMember]
        public string address { get; set; }

        [DataMember]
        public string cmd { get { return cmdInput; } set { cmdInput = value; } }

        [DataMember]
        public bool recursive { get; set; }

        [DataMember]
        public string[] filename { get; set; }

        [DataMember]
        public List<string> serverPorts { get; set; }

        [DataMember]
        public List<string> serverAddrs { get; set; }

        [DataMember]
        public Dictionary<string, List<T>> types { get; set; }


        [DataMember]
        public Dictionary<string, List<C>> typeDep { get; set; }


        [DataMember]
        public Dictionary<string, List<string>> packDep { get; set; }

    }
}

