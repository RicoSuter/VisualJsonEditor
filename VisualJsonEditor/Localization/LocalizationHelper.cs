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
    public static class LocalizationHelper
    {
        public static async void ShowError(Exception exception)
        {
            await ShowErrorAsync(exception);
        }
        
        public static Task ShowErrorAsync(Exception exception)
        {
            var text = string.Format(Strings.MessageErrorText, exception.Message);
            return Messenger.Default.SendAsync(new TextMessage(text, Strings.MessageErrorTitle));
        }
    }
}
