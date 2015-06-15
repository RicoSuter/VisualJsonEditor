//-----------------------------------------------------------------------
// <copyright file="LocalizationHelper.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyToolkit.Messaging;

namespace VisualJsonEditor.Localization
{
    /// <summary>Helper methods for localizing the application. </summary>
    public static class LocalizationHelper
    {
        /// <summary>Shows an exception in a message box. </summary>
        /// <param name="exception">The exception to show. </param>
        public static async void ShowError(Exception exception)
        {
            await ShowErrorAsync(exception);
        }

        /// <summary>Shows an exception in a message box. </summary>
        /// <param name="exception">The exception to show. </param>
        /// <returns>The task. </returns>
        public static Task ShowErrorAsync(Exception exception)
        {
            var text = string.Format(Strings.MessageErrorText, exception.Message);
            return Messenger.Default.SendAsync(new TextMessage(text, Strings.MessageErrorTitle));
        }
    }
}
