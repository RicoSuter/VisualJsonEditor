//-----------------------------------------------------------------------
// <copyright file="OpenJsonDocumentMessage.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using MyToolkit.Messaging;

namespace VisualJsonEditor.Messages
{
    public class OpenJsonDocumentMessage : CallbackMessage<string>
    {
        public OpenJsonDocumentMessage(string title)
        {
            Title = title; 
        }

        public string Title { get; private set; }
    }
}
