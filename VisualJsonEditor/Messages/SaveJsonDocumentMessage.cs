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
    public class SaveJsonDocumentMessage : CallbackMessage<string>
    {
        public string FileName { get; private set; }

        public SaveJsonDocumentMessage(string fileName)
        {
            FileName = fileName;
        }
    }
}