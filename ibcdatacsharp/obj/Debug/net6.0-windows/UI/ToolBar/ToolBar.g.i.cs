﻿#pragma checksum "..\..\..\..\..\UI\ToolBar\ToolBar.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "65C274A70AFF50CEF626560DDCC47AAFE413C614"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using OxyPlot.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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
using ibcdatacsharp.UI.Subjects;
using ibcdatacsharp.UI.TimeLine;
using ibcdatacsharp.UI.ToolBar;


namespace ibcdatacsharp.UI.ToolBar {
    
    
    /// <summary>
    /// ToolBar
    /// </summary>
    public partial class ToolBar : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 63 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button scan;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button connect;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button disconnect;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button openCamera;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button record;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image recordImage;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock recordText;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button capture;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button pause;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image pauseImage;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock pauseText;
        
        #line default
        #line hidden
        
        
        #line 107 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button stop;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button capturedFiles;
        
        #line default
        #line hidden
        
        
        #line 120 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Frame savingMenu;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ibcdatacsharp;V1.0.0.0;component/ui/toolbar/toolbar.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\UI\ToolBar\ToolBar.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.scan = ((System.Windows.Controls.Button)(target));
            return;
            case 2:
            this.connect = ((System.Windows.Controls.Button)(target));
            return;
            case 3:
            this.disconnect = ((System.Windows.Controls.Button)(target));
            return;
            case 4:
            this.openCamera = ((System.Windows.Controls.Button)(target));
            return;
            case 5:
            this.record = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.recordImage = ((System.Windows.Controls.Image)(target));
            return;
            case 7:
            this.recordText = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.capture = ((System.Windows.Controls.Button)(target));
            return;
            case 9:
            this.pause = ((System.Windows.Controls.Button)(target));
            return;
            case 10:
            this.pauseImage = ((System.Windows.Controls.Image)(target));
            return;
            case 11:
            this.pauseText = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            this.stop = ((System.Windows.Controls.Button)(target));
            return;
            case 13:
            this.capturedFiles = ((System.Windows.Controls.Button)(target));
            return;
            case 14:
            this.savingMenu = ((System.Windows.Controls.Frame)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

