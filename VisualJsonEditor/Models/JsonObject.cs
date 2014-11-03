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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace VisualJsonEditor.Models
{
    public class JsonObject : JsonToken
    {
        public IEnumerable<JsonProperty> Properties
        {
            get
            {
                var properties = new List<JsonProperty>();
                if (Schema.Properties != null)
                {
                    foreach (var propertyInfo in Schema.Properties)
                    {
                        var property = new JsonProperty(propertyInfo.Key, this, propertyInfo.Value);
                        if (property.Value is ObservableCollection<JsonToken>)
                        {
                            foreach (var obj in (ObservableCollection<JsonToken>)property.Value)
                                obj.Schema = propertyInfo.Value.Items.First();
                        }
                        properties.Add(property);
                    }
                }
                return properties; 
            }
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
                            obj[property.Key] = new ObservableCollection<JsonToken>();
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
                        var objects = obj[property.Key].Select(o => o is JObject ? 
                            (JsonToken)FromJson((JObject)o, propertySchema) : FromJson((JValue)o, propertySchema));
                        
                        var list = new ObservableCollection<JsonToken>(objects);
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

        public static JsonValue FromJson(JValue value, JsonSchema schema)
        {
            return new JsonValue
            {
                Schema = schema,
                Value = value.Value
            };
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

        public override JToken ToJToken()
        {
            var obj = new JObject();
            foreach (var pair in this)
            {
                if (pair.Value is ObservableCollection<JsonToken>)
                {
                    var array = (ObservableCollection<JsonToken>) pair.Value;
                    var jArray = new JArray();
                    foreach (var item in array)
                        jArray.Add(item.ToJToken());
                    obj[pair.Key] = jArray;
                }
                else if (pair.Value is JsonToken)
                    obj[pair.Key] = ((JsonToken)pair.Value).ToJToken();
                else
                    obj[pair.Key] = new JValue(pair.Value);
            }
            return obj; 
        }
    }
}