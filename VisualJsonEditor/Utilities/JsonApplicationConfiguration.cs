//-----------------------------------------------------------------------
// <copyright file="JsonApplicationConfiguration.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace VisualJsonEditor.Utilities
{
    public static class JsonApplicationConfiguration
    {
        private const string ConfigExtension = ".json";
        private const string SchemaExtension = ".schema.json";

        public static T Load<T>(string fileNameWithoutExtension, bool alwaysCreateNewSchemaFile, bool storeInAppData) where T : new()
        {
            var configPath = CreateFilePath(fileNameWithoutExtension, ConfigExtension, storeInAppData);
            var schemaPath = CreateFilePath(fileNameWithoutExtension, SchemaExtension, storeInAppData);

            if (alwaysCreateNewSchemaFile || !File.Exists(schemaPath))
                CreateSchemaFile<T>(fileNameWithoutExtension, storeInAppData);

            if (!File.Exists(configPath))
                return CreateDefaultConfigurationFile<T>(fileNameWithoutExtension, storeInAppData);

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(configPath, Encoding.UTF8));
        }

        public static void Save<T>(string fileNameWithoutExtension, T configuration, bool storeInAppData) where T : new()
        {
            CreateSchemaFile<T>(fileNameWithoutExtension, storeInAppData);

            var configPath = CreateFilePath(fileNameWithoutExtension, ConfigExtension, storeInAppData);
            File.WriteAllText(configPath, JsonConvert.SerializeObject(configuration), Encoding.UTF8);
        }

        private static string CreateFilePath(string fileNameWithoutExtension, string extension, bool storeInAppData)
        {
            if (storeInAppData)
            {
                var appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appDataDirectory, fileNameWithoutExtension) + extension;
            }
            return fileNameWithoutExtension + extension;
        }

        private static T CreateDefaultConfigurationFile<T>(string fileNameWithoutExtension, bool storeInAppData) where T : new()
        {
            var config = new T();
            var configData = JsonConvert.SerializeObject(config);
            var configPath = CreateFilePath(fileNameWithoutExtension, ConfigExtension, storeInAppData);

            File.WriteAllText(configPath, configData, Encoding.UTF8);

            return config;
        }

        private static void CreateSchemaFile<T>(string fileNameWithoutExtension, bool storeInAppData) where T : new()
        {
            var schemaPath = CreateFilePath(fileNameWithoutExtension, SchemaExtension, storeInAppData);
            var generator = new JsonSchemaGenerator();
            var schema = generator.Generate(typeof (T));

            File.WriteAllText(schemaPath, schema.ToString(), Encoding.UTF8);
        }
    }
}