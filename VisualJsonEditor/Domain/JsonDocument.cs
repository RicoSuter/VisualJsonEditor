//-----------------------------------------------------------------------
// <copyright file="JsonDocument.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MyToolkit.Data;
using MyToolkit.Model;
using MyToolkit.Mvvm;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace VisualJsonEditor.Domain
{
    public class JsonDocument : ObservableObject
    {
        private string _filePath;
        private JsonObject _data;
        private bool _isReadOnly;

        protected JsonDocument() { }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (Set(ref _filePath, value))
                {
                    RaisePropertyChanged(() => HasFileLocation);
                    RaisePropertyChanged(() => DisplayTitle);
                }
            }
        }

        public string SchemaPath { get; private set; }

        public string DisplayTitle
        {
            get
            {
                if (HasFileLocation)
                    return Path.GetFileName(FilePath);
                return "Unsaved";
            }
        }

        public bool HasFileLocation
        {
            get { return _filePath != null; }
        }

        public JsonObject Data
        {
            get { return _data; }
            private set { Set(ref _data, value); }
        }

        public void Initialize(JsonObject data, IDispatcher dispatcher)
        {
            UndoRedoManager = new UndoRedoManager(data, dispatcher);
            Data = data; 
        }

        public UndoRedoManager UndoRedoManager { get; set; }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { Set(ref _isReadOnly, value); }
        }

        public static Task<JsonDocument> LoadAsync(string filePath, IDispatcher dispatcher)
        {
            var schemaPath = GetDefaultSchemaPath(filePath);
            return LoadAsync(filePath, schemaPath, dispatcher);
        }

        public static string GetDefaultSchemaPath(string filePath)
        {
            var directoryName = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(directoryName))
                return Path.GetFileNameWithoutExtension(filePath) + ".schema" + Path.GetExtension(filePath);
            return Path.Combine(directoryName, Path.GetFileNameWithoutExtension(filePath) + ".schema" + Path.GetExtension(filePath));
        }

        public static Task<JsonDocument> LoadAsync(string filePath, string schemaPath, IDispatcher dispatcher)
        {
            return Task.Run(() =>
            {
                var schema = JsonSchema.Parse(File.ReadAllText(schemaPath, Encoding.UTF8));
                var data = JsonObject.FromJson(File.ReadAllText(filePath, Encoding.UTF8), schema); 

                var document = new JsonDocument();
                document.Initialize(data, dispatcher);
                document.FilePath = filePath;
                document.SchemaPath = schemaPath;
                return document;
            });
        }

        public static Task<JsonDocument> CreateAsync(string schemaPath, IDispatcher dispatcher)
        {
            return Task.Run(() =>
            {
                var schema = JsonSchema.Parse(File.ReadAllText(schemaPath, Encoding.UTF8));
                var data = JsonObject.FromSchema(schema);

                var document = new JsonDocument();
                document.Initialize(data, dispatcher);
                return document;
            });
        }

        public async Task SaveAsync(bool saveSchema)
        {
            if (!HasFileLocation)
                throw new Exception("Document has no file location");

            await Task.Run(() =>
            {
                var jsonData = JsonConvert.SerializeObject(Data, Formatting.Indented);
                File.WriteAllText(FilePath, jsonData, Encoding.UTF8);

                if (saveSchema)
                {
                    var schemaPath = GetDefaultSchemaPath(FilePath);
                    File.WriteAllText(schemaPath, Data.Schema.ToString());
                }
            });

            UndoRedoManager.Reset();
        }

        /// <summary>Returns a string that represents the current object. </summary>
        /// <returns>A string that represents the current object. </returns>
        public override string ToString()
        {
            if (HasFileLocation)
                return Path.GetFileName(FilePath);
            return "Unsaved";
        }
    }
}
