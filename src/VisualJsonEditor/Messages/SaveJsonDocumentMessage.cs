//-----------------------------------------------------------------------
// <copyright file="SaveJsonDocumentMessage.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using MyToolkit.Messaging;

namespace VisualJsonEditor.Messages
{
    /// <summary>Message to show the save file dialog for a JSON document. </summary>
    public class SaveJsonDocumentMessage : CallbackMessage<string>
    {
        /// <summary>Initializes a new instance of the <see cref="SaveJsonDocumentMessage"/> class. </summary>
        /// <param name="fileName">The default file path of the JSON document. </param>
        public SaveJsonDocumentMessage(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>Gets or sets the file name. </summary>
        public string FileName { get; private set; }
    }
}