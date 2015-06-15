//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Forms;
using MyToolkit.Composition;
using MyToolkit.Messaging;
using MyToolkit.Mvvm;
using MyToolkit.UI;
using VisualJsonEditor.Localization;
using VisualJsonEditor.Messages;

namespace VisualJsonEditor
{
    /// <summary>Interaction logic for App.xaml</summary>
    public partial class App
    {
        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup"/> event. </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceLocator.Default.RegisterSingleton<IDispatcher, UiDispatcher>(new UiDispatcher(Dispatcher));

            Messenger.Default.Register(DefaultActions.GetTextMessageAction());
            Messenger.Default.Register<OpenJsonDocumentMessage>(OpenJsonDocument);
            Messenger.Default.Register<SaveJsonDocumentMessage>(SaveJsonDocument);
        }

        private void SaveJsonDocument(SaveJsonDocumentMessage msg)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = msg.FileName;
            dlg.Filter = Strings.FileDialogFilter;
            dlg.RestoreDirectory = true;
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == DialogResult.OK)
                msg.CallSuccessCallback(dlg.FileName);
            else
                msg.CallFailCallback(null);
        }

        private void OpenJsonDocument(OpenJsonDocumentMessage msg)
        {
            var dlg = new OpenFileDialog();
            dlg.Title = msg.Title;
            dlg.Filter = Strings.FileDialogFilter; 
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
                msg.CallSuccessCallback(dlg.FileName);
            else
                msg.CallFailCallback(null);
        }
    }
}
