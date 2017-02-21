//-----------------------------------------------------------------------
// <copyright file="LocalizationHelper.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using MyToolkit.Dialogs;

namespace VisualJsonEditor.Localization
{
    /// <summary>Helper methods for localizing the application. </summary>
    public static class LocalizationHelper
    {
        /// <summary>Shows an exception in a message box. </summary>
        /// <param name="exception">The exception to show. </param>
        public static void ShowError(Exception exception)
        {
            ExceptionBox.Show("An error has occurred", exception, Application.Current.MainWindow);
        }
    }
}
