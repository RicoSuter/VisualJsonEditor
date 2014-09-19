//-----------------------------------------------------------------------
// <copyright file="JsonObject.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace VisualJsonEditor.Model
{
    public class JsonObject : ObservableDictionary<string, object>
    {
        public JsonSchema Schema { get; set; }
        public ObservableCollection<JsonObject> ParentList { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static JsonObject FromSchema(JsonSchema schema)
        {
            var obj = new JsonObject();
            foreach (var property in schema.Properties)
            {
                if (property.Value.Type.HasValue)
                {
                    if (property.Value.Type.Value.HasFlag(JsonSchemaType.Object))
                    {
                        if (property.Value.Required == true)
                            obj[property.Key] = FromSchema(property.Value);
                        else
                            obj[property.Key] = null;
                    }
                    else if (property.Value.Type.Value.HasFlag(JsonSchemaType.Array))
                    {
                        if (property.Value.Required == true)
                            obj[property.Key] = new ObservableCollection<JsonObject>();
                        else
                            obj[property.Key] = null;
                    }
                    else
                        obj[property.Key] = GetDefaultValue(property);
                }
            }
            obj.Schema = schema; 
            return obj;
        }

        private static object GetDefaultValue(KeyValuePair<string, JsonSchema> property)
        {
            if (property.Value.Default != null)
                return ((JValue) property.Value.Default).Value;

            if (property.Value.Type.HasValue)
            {
                if (property.Value.Type.Value.HasFlag(JsonSchemaType.Boolean))
                    return false;

                if (property.Value.Required == true)
                {
                    if (property.Value.Type.Value.HasFlag(JsonSchemaType.String) && property.Value.Format == "date-time")
                        return new DateTime();
                    if (property.Value.Type.Value.HasFlag(JsonSchemaType.String) && property.Value.Format == "date")
                        return new DateTime();
                    if (property.Value.Type.Value.HasFlag(JsonSchemaType.String) && property.Value.Format == "time")
                        return new TimeSpan();
                }
            }

            return null;
        }

        public static JsonObject FromJson(string jsonData, JsonSchema schema)
        {
            var json = JsonConvert.DeserializeObject(jsonData);
            return FromJson((JObject)json, schema);
        }

        public static JsonObject FromJson(JObject obj, JsonSchema schema)
        {
            var result = new JsonObject();
            foreach (var property in schema.Properties)
            {
                if (property.Value.Type.HasValue)
                {
                    if (property.Value.Type.Value.HasFlag(JsonSchemaType.Array))
                    {
                        var propertySchema = property.Value.Items.First();
                        var objects = obj[property.Key].Select(o => FromJson((JObject)o, propertySchema));
                        
                        var list = new ObservableCollection<JsonObject>(objects);
                        foreach (var item in list)
                            item.ParentList = list;

                        result[property.Key] = list;
                    }
                    else if (property.Value.Type.Value.HasFlag(JsonSchemaType.Object))
                    {
                        result[property.Key] = FromJson((JObject)obj[property.Key], property.Value);
                    }
                    else
                    {
                        JToken value;
                        if (obj.TryGetValue(property.Key, out value))
                            result[property.Key] = ((JValue)value).Value;
                        else
                            result[property.Key] = GetDefaultValue(property);  
                    }
                }
            }
            result.Schema = schema; 
            return result;
        }

        public Task<bool> IsValidAsync()
        {
            return Task.Run(() =>
            {
                var jsonData = JsonConvert.SerializeObject(this, Formatting.Indented);
                var obj = JToken.ReadFrom(new JsonTextReader(new StringReader(jsonData)));
                return obj.IsValid(Schema);
            });
        }
    }
}