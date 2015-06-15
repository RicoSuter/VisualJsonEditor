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
    /// <summary>Message to show the file open dialog to open a JSON document. </summary>
    public class OpenJsonDocumentMessage : CallbackMessage<string>
    {
        /// <summary>Initializes a new instance of the <see cref="OpenJsonDocumentMessage"/> class. </summary>
        /// <param name="title">The title to show in the dialog. </param>
        public OpenJsonDocumentMessage(string title)
        {
            Title = title; 
        }

        /// <summary>Gets or sets the file name. </summary>
        public string Title { get; private set; }
    }
}
