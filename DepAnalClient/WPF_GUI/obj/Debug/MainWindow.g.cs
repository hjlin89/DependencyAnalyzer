﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "09C9B79DF768D949B20F9A529D34FA64"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace WPF_GUI {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SendButton;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddButton;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ListenButton;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox RecurCheckbox;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox TypeCheckbox;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox PackageCheckbox;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox AddressBox;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CmdBox;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ListPortBox;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ConnectPortBox;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listbox;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listboxMsg;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WPF_GUI;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.SendButton = ((System.Windows.Controls.Button)(target));
            
            #line 12 "..\..\MainWindow.xaml"
            this.SendButton.Click += new System.Windows.RoutedEventHandler(this.Send_Button);
            
            #line default
            #line hidden
            return;
            case 2:
            this.AddButton = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\MainWindow.xaml"
            this.AddButton.Click += new System.Windows.RoutedEventHandler(this.Add_Button);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ListenButton = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\MainWindow.xaml"
            this.ListenButton.Click += new System.Windows.RoutedEventHandler(this.List_Button);
            
            #line default
            #line hidden
            return;
            case 4:
            this.RecurCheckbox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 15 "..\..\MainWindow.xaml"
            this.RecurCheckbox.Checked += new System.Windows.RoutedEventHandler(this.Recursive_checkbox);
            
            #line default
            #line hidden
            return;
            case 5:
            this.TypeCheckbox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 16 "..\..\MainWindow.xaml"
            this.TypeCheckbox.Checked += new System.Windows.RoutedEventHandler(this.Type_checkbox);
            
            #line default
            #line hidden
            return;
            case 6:
            this.PackageCheckbox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 17 "..\..\MainWindow.xaml"
            this.PackageCheckbox.Checked += new System.Windows.RoutedEventHandler(this.Package_checkbox);
            
            #line default
            #line hidden
            return;
            case 7:
            this.AddressBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 18 "..\..\MainWindow.xaml"
            this.AddressBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.Address_TextBox);
            
            #line default
            #line hidden
            return;
            case 8:
            this.CmdBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 19 "..\..\MainWindow.xaml"
            this.CmdBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.CmdInput_Textbox);
            
            #line default
            #line hidden
            return;
            case 9:
            this.ListPortBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 23 "..\..\MainWindow.xaml"
            this.ListPortBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.ListenPort_TextBox);
            
            #line default
            #line hidden
            return;
            case 10:
            this.ConnectPortBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 24 "..\..\MainWindow.xaml"
            this.ConnectPortBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.ConnectPort_TextBox);
            
            #line default
            #line hidden
            return;
            case 11:
            this.listbox = ((System.Windows.Controls.ListBox)(target));
            
            #line 31 "..\..\MainWindow.xaml"
            this.listbox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ListBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 12:
            this.listboxMsg = ((System.Windows.Controls.ListBox)(target));
            
            #line 32 "..\..\MainWindow.xaml"
            this.listboxMsg.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ListBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

