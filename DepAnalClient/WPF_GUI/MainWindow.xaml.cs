/////////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs  -  WPF Window of Client                         //
// ver 1.0                                                             //
// Language:    C#, Visual Studio 13.0, .Net Framework 4.5             //
// Platform:    Macbook Air, Win 8.1 pro, Visual Studio 2013           //
// Application: Pr#4 DepAnalyzer, CSE681, Fall 2014                    //
// Author:      Huijun Lin, hlin14@syr.edu                             //
/////////////////////////////////////////////////////////////////////////
/*
 * 
 * Module Operations
 * =================
 * Client Executive
 * Set up a client and send request according to the input from user
 * Then display analyze information from server
 * 
 * 
 * Methods:
 * 1. public static void writeXML(Dictionary<string, List<TypeDepElem>> typeDeps, Dictionary<string, List<string>> pkgDeps)
 * 2. public static List<string> readTypeDep()
 * 3. public static List<string> readPkgDep()
 * 
 * 
 * Build Process
 * =============
 * Required Files:
 *   MainWindow.xaml.cs 
 * 
 * Compiler Command:
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Repositorylib;
using XmlManager;


namespace WPF_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Initiation of sender, server and msgs
        ServiceLibrary.Receiver recvr;
        ServiceLibrary.Sender sndr;
        ServiceLibrary.Message<TypeElem, TypeDepElem> rcvdMsg;
        ServiceLibrary.Message<TypeElem, TypeDepElem> sndrMsg;
        ServiceLibrary.Message<TypeElem, TypeDepElem> localMsg;
        int countServer;


        Thread rcvThrd = null;
        delegate void NewMessage(ServiceLibrary.Message<TypeElem, TypeDepElem> msg);
        event NewMessage OnNewMessage;

        // receive thread processing
        void ThreadProc()
        {
            while(true)
            {
                // get message out of receive queue
                rcvdMsg = recvr.GetMessage();

                // call window functions on UI thread
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    OnNewMessage,
                    rcvdMsg);
            }
        }

        /// <summary>
        /// handle msg when msg arrived
        /// </summary>
        /// <param name="msg"></param>
        void OnNewMessageHandler(ServiceLibrary.Message<TypeElem, TypeDepElem> msg)
        {
            listboxMsg.Items.Insert(0, "Received a new Message.");
            countServer--;

            try
            {
                // revd Msg merge into local
                mergeType(msg.typeDep);
                mergePkg(msg.packDep);
                if (countServer == 0)  /// when msg from all server set, store it 
                {
                    Dictionary<string, List<TypeDepElem>> typeDeps = localMsg.typeDep;
                    Dictionary<string, List<string>> pkgDeps = localMsg.packDep;
                    // store msg into XML
                    XMLWR.writeXML(typeDeps, pkgDeps);
                    listboxMsg.Items.Insert(0, @"XML file save in .\DependencyTable.xml");
                    
                    List<string> PkgDepList = XMLWR.readPkgDep();;
                    List<string> TypeDepList =  XMLWR.readTypeDep();;
                    //Check which list is going to display
                    if ((bool)PackageCheckbox.IsChecked && (!(bool)TypeCheckbox.IsChecked))
                    {
                        printList(PkgDepList);
                        listboxMsg.Items.Insert(0, "Show PkgDepTable.");
                    }
                    else if ((!(bool)PackageCheckbox.IsChecked) && ((bool)TypeCheckbox.IsChecked))
                    {
                        printList(TypeDepList);
                        listboxMsg.Items.Insert(0, "Show TypeDepTable.");
                    }
                    else
                    {
                        printList(TypeDepList);  listboxMsg.Items.Insert(0, "Show TypeDepTable.");
                        printList(PkgDepList);   listboxMsg.Items.Insert(0, "Show PkgDepTable.");
                    }
                }
            }
            catch (Exception ex)
            {
                Window temp = new Window();
                temp.Content = ex.Message;
                temp.Height = 100;
                temp.Width = 500;
            }
        }

        /// <summary>
        /// print the list which is return by xml reader
        /// </summary>
        /// <param name="list"></param>
        void printList(List<string> list)
        {
            foreach (string elem in list)
                listbox.Items.Insert(0, elem);
        }

        /// <summary>
        /// Merge pkg dep table in local
        /// </summary>
        /// <param name="srcTable"></param>
        void mergePkg(Dictionary<string, List<string>> srcTable)
        {
            listboxMsg.Items.Insert(0, "Merging PkgTalbe");
            if (localMsg.packDep == null) /// if table is null , then set the srctable to it
            {
                if (srcTable == null)
                    return;
                localMsg.packDep = srcTable;
                return;
            }
            foreach (string Type in srcTable.Keys)
            {
                if (!localMsg.packDep.Keys.Contains(Type)) // if it dont have that type, then add it
                {
                    List<string> elems = new List<string>();
                    foreach (string pkgname in srcTable[Type])
                    {
                        string elem = pkgname;
                        elems.Add(elem);
                    }
                    localMsg.packDep[Type] = elems;
                }
                else // if it have the type, check pkgname, add the different one 
                {
                    foreach (string pkgname in srcTable[Type])
                        if (!localMsg.packDep[Type].Contains(pkgname))
                            localMsg.packDep[Type].Add(pkgname);
                }
            }
            listboxMsg.Items.Insert(0, "PkgTalbe merged");
        }

        /// <summary>
        /// merge type dep table
        /// </summary>
        /// <param name="srcTable"></param>
        void mergeType(Dictionary<string, List<TypeDepElem>> srcTable)
        {
            listboxMsg.Items.Insert(0, "Merging TypeTalbe");
            if (localMsg.typeDep == null) /// if table is null , then set the srctable to it
            {
                localMsg.typeDep = srcTable;
                return;
            }
            foreach (string Type in srcTable.Keys)
            {
                if (!localMsg.typeDep.Keys.Contains(Type))// if it dont have that type, then add it
                {
                    List<TypeDepElem> elems = new List<TypeDepElem>();
                    foreach (TypeDepElem te in srcTable[Type])
                    {
                        TypeDepElem elem = new TypeDepElem();
                        elem.relationship = te.relationship;
                        elem.typeName = te.typeName;
                        elems.Add(elem);
                    }
                    localMsg.typeDep[Type] = elems;
                }
                else  // if it have the type, check pkgname, add the different one 
                {
                    foreach (TypeDepElem te in srcTable[Type])
                    {
                        int count = localMsg.typeDep[Type].Count;
                        foreach (TypeDepElem te1 in localMsg.typeDep[Type])
                            if (te1.typeName != te.typeName)
                            {
                                count--;
                                if (count == 0)
                                {
                                    TypeDepElem elem = new TypeDepElem();
                                    elem.relationship = te.relationship;
                                    elem.typeName = te.typeName;
                                    localMsg.typeDep[Type].Add(elem);
                                }
                            }
                    }
                }
            }
            listboxMsg.Items.Insert(0, "TypeTalbe merged");
        }

        /// <summary>
        /// constructor with initiation
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            listboxMsg.Items.Insert(0, "Client Start");
            OnNewMessage += new NewMessage(OnNewMessageHandler);
            AddButton.IsEnabled = false;
            SendButton.IsEnabled = false;
            PackageCheckbox.IsChecked = false;
            RecurCheckbox.IsChecked = false;
            TypeCheckbox.IsChecked = false;
            sndrMsg = new ServiceLibrary.Message<TypeElem, TypeDepElem>();
            sndrMsg.serverPorts = new List<string>();
            sndrMsg.serverAddrs = new List<string>();
            localMsg = new ServiceLibrary.Message<TypeElem, TypeDepElem>();
        }

        // Set the 3 option check boxes when changed
        private void Package_checkbox(object sender, RoutedEventArgs e)
        {

        }
        private void Recursive_checkbox(object sender, RoutedEventArgs e)
        {

        }
        private void Type_checkbox(object sender, RoutedEventArgs e)
        {

        }

        // Set 4 textboxes when text_changed
        private void CmdInput_Textbox(object sender, TextChangedEventArgs e)
        {

        }
        private void Address_TextBox(object sender, TextChangedEventArgs e)
        {

        }
        private void ConnectPort_TextBox(object sender, TextChangedEventArgs e)
        {

        }
        private void ListenPort_TextBox(object sender, TextChangedEventArgs e)
        {

        }


        /// <summary>
        /// Listen button, click then set listen running
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_Button(object sender, RoutedEventArgs e)
        {
            string localPort = ListPortBox.Text;
            string endpoint = "http://localhost:" + localPort + "/IService";

            try
            {
                recvr = new ServiceLibrary.Receiver();
                recvr.CreateRecvChannel(endpoint);

                // create receive thread which calls rcvBlockingQ.deQ
                rcvThrd = new Thread(new ThreadStart(this.ThreadProc));
                rcvThrd.IsBackground = true;
                rcvThrd.Start();
                AddButton.IsEnabled = true;
                ListenButton.IsEnabled = false;
                listboxMsg.Items.Insert(0, "Starting listening:" + endpoint);
            }
            catch(Exception ex)
            {
                Window temp = new Window();
                StringBuilder msg = new StringBuilder(ex.Message);
                msg.Append("\nport = ");
                msg.Append(localPort.ToString());
                temp.Content = msg.ToString();
                temp.Height = 100;
                temp.Width = 500;
                temp.Show();
            }
        }
        /// <summary>
        /// add button, add server port and addr to endpoint pool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                sndrMsg.address = AddressBox.Text;
                sndrMsg.port = ListPortBox.Text;
                SendButton.IsEnabled = true;
                sndrMsg.serverAddrs.Add(AddressBox.Text);
                sndrMsg.serverPorts.Add(ConnectPortBox.Text);
                listboxMsg.Items.Insert(0, "Add server address and port");
                listboxMsg.Items.Insert(0, sndrMsg.address + ":" + ConnectPortBox.Text);
            }
            catch(Exception ex)
            {
                Window temp = new Window();
                StringBuilder msg = new StringBuilder(ex.Message);
                temp.Content = msg.ToString();
                temp.Height = 100;
                temp.Width = 500;
                temp.Show();
            }
        }

        /// <summary>
        /// send cmd to the server in local server pool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Send_Button(object sender, RoutedEventArgs e)
        {
            string endpoint = "";
            sndrMsg.cmd = CmdBox.Text;
            sndrMsg.recursive = (bool)RecurCheckbox.IsChecked;

            for (int i = 0; i < sndrMsg.serverAddrs.Count; i++)
            {
                string remoteAddress = sndrMsg.serverAddrs[i];
                string remotePort = sndrMsg.serverPorts[i];
                endpoint = remoteAddress + ":" + remotePort + "/IService";
                try
                {
                    countServer = sndrMsg.serverAddrs.Count;
                    sndr = new ServiceLibrary.Sender(endpoint);
                    listboxMsg.Items.Insert(0, "Sending request to all servers");
                    sndr.PostMessage(sndrMsg);
                }
                catch (Exception ex)
                {
                    Window temp = new Window();
                    temp.Content = ex.Message;
                    temp.Height = 100;
                    temp.Width = 500;
                }
            }
            SendButton.IsEnabled = true;

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// close 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            recvr.Close();
            ServiceLibrary.Message<TypeElem, TypeDepElem> quitmsg = new ServiceLibrary.Message<TypeElem, TypeDepElem>();
            quitmsg.cmd = "quit";
            sndr.PostMessage(quitmsg);
            sndr.Close();
        }
    }

}
