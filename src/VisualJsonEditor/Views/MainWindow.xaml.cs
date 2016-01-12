//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Fluent;
using MyToolkit.Mvvm;
using MyToolkit.UI;
using MyToolkit.Utilities;
using VisualJsonEditor.Models;
using VisualJsonEditor.Utilities;
using VisualJsonEditor.ViewModels;
using Xceed.Wpf.AvalonDock;

namespace VisualJsonEditor.Views
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow
    {
        private ApplicationConfiguration _configuration;

        public MainWindow()
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);

            RegisterFileOpenHandler();
            RegisterShortcuts();

            LoadConfiguration();
            CheckForApplicationUpdate();

            Closing += OnWindowClosing;

            // TODO: How to implement this
            //Loaded += delegate
            //{
            //    foreach (QuickAccessMenuItem menuItem in Ribbon.QuickAccessItems)
            //    {
            //        if (!Ribbon.IsInQuickAccessToolBar(menuItem))
            //            Ribbon.AddToQuickAccessToolBar(menuItem);
            //    }
            //};

#if DEBUG
            if (Debugger.IsAttached)
                Dispatcher.InvokeAsync(delegate { Model.OpenDocumentAsync(@"Samples/Sample.json"); });
#endif

        }

        /// <summary>Gets the view model. </summary>
        public MainWindowModel Model { get { return (MainWindowModel)Resources["ViewModel"]; } }

        /// <summary>Gets the configuration file name. </summary>
        public string ConfigurationFileName
        {
            get { return "VisualJsonEditor/Config"; }
        }

        private void RegisterFileOpenHandler()
        {
            var fileHandler = new FileOpenHandler();
            fileHandler.FileOpen += (sender, args) =>
            {
                Dispatcher.InvokeAsync(() => { Model.OpenDocumentAsync(args.FileName); }); // TODO: Dispatcher.InvokeAsync not needed when using MyToolkit 2.3.31
            };
            fileHandler.Initialize(this);
        }

        private void RegisterShortcuts()
        {
            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.N, ModifierKeys.Control),
                () => Model.CreateDocumentCommand.TryExecute());
            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.O, ModifierKeys.Control),
                () => Model.OpenDocumentCommand.TryExecute());
            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.S, ModifierKeys.Control),
                () => Model.SaveDocumentCommand.TryExecute(Model.SelectedDocument));

            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.W, ModifierKeys.Control),
                () => Model.CloseDocumentCommand.TryExecute(Model.SelectedDocument));

            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.Z, ModifierKeys.Control),
                () => Model.UndoCommand.TryExecute(Model.SelectedDocument));
            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.Y, ModifierKeys.Control),
                () => Model.RedoCommand.TryExecute(Model.SelectedDocument));

            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.U, ModifierKeys.Control),
                () => Model.ValidateDocumentCommand.TryExecute(Model.SelectedDocument));
        }

        private async void LoadConfiguration()
        {
            _configuration = JsonApplicationConfiguration.Load<ApplicationConfiguration>(ConfigurationFileName, true, true);

            Width = _configuration.WindowWidth;
            Height = _configuration.WindowHeight;
            WindowState = _configuration.WindowState;

            Model.Configuration = _configuration;

            if (_configuration.IsFirstStart)
            {
                _configuration.IsFirstStart = false;
                await Model.OpenDocumentAsync("Samples/Sample.json", true);
            }
        }

        private void SaveConfiguration()
        {
            _configuration.WindowWidth = Width;
            _configuration.WindowHeight = Height;
            _configuration.WindowState = WindowState;

            JsonApplicationConfiguration.Save(ConfigurationFileName, _configuration, true);
        }

        private async void CheckForApplicationUpdate()
        {
            var updater = new ApplicationUpdater("VisualJsonEditor.msi", GetType().Assembly, 
                "http://rsuter.com/Projects/VisualJsonEditor/updates.php");
            await updater.CheckForUpdate(this);
        }

        private async void OnDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            args.Cancel = true;
            await Model.CloseDocumentAsync((JsonDocumentModel)args.Document.Content);
        }

        private async void OnWindowClosing(object sender, CancelEventArgs args)
        {
            args.Cancel = true;

            foreach (var document in Model.Documents.ToArray())
            {
                var result = await Model.CloseDocumentAsync(document);
                if (!result)
                    return;
            }

            Closing -= OnWindowClosing;
            SaveConfiguration();
            await Dispatcher.InvokeAsync(Close);
        }

        private void OnShowAboutWindow(object sender, RoutedEventArgs e)
        {
            var dlg = new AboutWindow();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void OnExitApplication(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnOpenDocument(object sender, RoutedEventArgs e)
        {
            ((Backstage) Ribbon.Menu).IsOpen = false; 
        }
    }
}
