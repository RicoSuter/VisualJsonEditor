//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Forms;
using Microsoft.ApplicationInsights;
using MyToolkit.Composition;
using MyToolkit.Messaging;
using MyToolkit.Mvvm;
using MyToolkit.Storage;
using MyToolkit.UI;
using VisualJsonEditor.Localization;
using VisualJsonEditor.Messages;

namespace VisualJsonEditor
{
    /// <summary>Interaction logic for App.xaml</summary>
    public partial class App
    {
        public static TelemetryClient Telemetry = new TelemetryClient();

        /// <summary>Initializes a new instance of the <see cref="App"/> class.</summary>
        public App()
        {
            InitializeTelemetry();
        }

        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup"/> event. </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceLocator.Default.RegisterSingleton<IDispatcher, UiDispatcher>(new UiDispatcher(Dispatcher));

            Messenger.Default.Register(DefaultActions.GetTextMessageAction());
            Messenger.Default.Register<OpenJsonDocumentMessage>(OpenJsonDocument);
            Messenger.Default.Register<SaveJsonDocumentMessage>(SaveJsonDocument);

            Telemetry.TrackEvent("ApplicationStart");

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Telemetry.TrackEvent("ApplicationExit");
            Telemetry.Flush();
        }

        private void InitializeTelemetry()
        {
#if !DEBUG
            Telemetry.InstrumentationKey = "471bd0a4-e71d-454a-aa2b-89658b685df3";
            Telemetry.Context.User.Id = ApplicationSettings.GetSetting("TelemetryUserId", Guid.NewGuid().ToString());
            Telemetry.Context.Session.Id = Guid.NewGuid().ToString();
            Telemetry.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            Telemetry.Context.Component.Version = GetType().Assembly.GetName().Version.ToString();

            ApplicationSettings.SetSetting("TelemetryUserId", Telemetry.Context.User.Id);
#endif
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Telemetry.TrackException(args.ExceptionObject as Exception);
        }

        private void SaveJsonDocument(SaveJsonDocumentMessage msg)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = msg.FileName;
            dlg.Filter = Strings.FileDialogFilter;
            dlg.RestoreDirectory = true;
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                msg.CallSuccessCallback(dlg.FileName);
                Telemetry.TrackEvent("FileSave");
            }
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
            {
                msg.CallSuccessCallback(dlg.FileName);
                Telemetry.TrackEvent("FileOpen");
            }
            else
                msg.CallFailCallback(null);
        }
    }
}
