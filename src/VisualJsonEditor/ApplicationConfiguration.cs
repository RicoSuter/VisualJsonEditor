﻿//-----------------------------------------------------------------------
// <copyright file="ApplicationConfiguration.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------
 
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace VisualJsonEditor
{
    /// <summary>Stores the application configuration. </summary>
    public class ApplicationConfiguration
    {
        public ApplicationConfiguration()
        {
            IsFirstStart = true;
            
            WindowHeight = 600;
            WindowWidth = 700;
            WindowState = WindowState.Normal;

            RecentFiles = new ObservableCollection<RecentFile>();
        }

        public bool IsFirstStart { get; set; }
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        public WindowState WindowState { get; set; }
        public ObservableCollection<RecentFile> RecentFiles { get; set; }
    }

    /// <summary>Describes a recently opened file. </summary>
    public class RecentFile
    {
        /// <summary>Gets or sets the file path. </summary>
        public string FilePath { get; set; }

        [JsonIgnore]
        public string FileName { get { return Path.GetFileName(FilePath); } }
    }
}