//-----------------------------------------------------------------------
// <copyright file="AboutWindow.xaml.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using MyToolkit.Utilities;

namespace VisualJsonEditor.Views
{
    /// <summary>Interaction logic for AboutWindow.xaml</summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();            
            Version.Text = "v" + Assembly.GetExecutingAssembly().GetVersionWithBuildTime();
        }

        private void OnOpenHyperlink(object sender, RoutedEventArgs e)
        {
            var uri = ((Hyperlink)sender).NavigateUri;
            Process.Start(uri.ToString());
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
