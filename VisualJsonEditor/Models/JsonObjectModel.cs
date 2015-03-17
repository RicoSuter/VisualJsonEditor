//-----------------------------------------------------------------------
// <copyright file="JsonObjectModel.cs" company="Visual JSON Editor">
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
using NJsonSchema;

namespace VisualJsonEditor.Models
{
    /// <summary>Represents a JSON object. </summary>
    public class JsonObjectModel : JsonTokenModel
    {
        /// <summary>Creates a default <see cref="JsonObjectModel"/> from the given schema. </summary>
        /// <param name="schema">The <see cref="JsonSchema4"/>. </param>
        /// <returns>The <see cref="JsonObjectModel"/>. </returns>
        public static JsonObjectModel FromSchema(JsonSchema4 schema)
        {
            var obj = new JsonObjectModel();
            foreach (var property in schema.Properties)
            {
                if (property.Value.Type.HasFlag(JsonObjectType.Object))
                {
                    if (property.Value.IsRequired)
                        obj[property.Key] = FromSchema(property.Value);
                    else
                        obj[property.Key] = null;
                }
                else if (property.Value.Type.HasFlag(JsonObjectType.Array))
                {
                    if (property.Value.IsRequired)
                        obj[property.Key] = new ObservableCollection<JsonTokenModel>();
                    else
                        obj[property.Key] = null;
                }
                else
                    obj[property.Key] = GetDefaultValue(property);
            }
            obj.Schema = schema; 
            return obj;
        }

        /// <summary>Creates a <see cref="JsonObjectModel"/> from the given JSON data and <see cref="JsonSchema4"/>. </summary>
        /// <param name="jsonData">The JSON data. </param>
        /// <param name="schema">The <see cref="JsonSchema4"/>. </param>
        /// <returns>The <see cref="JsonObjectModel"/>. </returns>
        public static JsonObjectModel FromJson(string jsonData, JsonSchema4 schema)
        {
            var json = JsonConvert.DeserializeObject(jsonData);
            return FromJson((JObject)json, schema);
        }

        /// <summary>Creates a <see cref="JsonObjectModel"/> from the given <see cref="JObject"/> and <see cref="JsonSchema4"/>. </summary>
        /// <param name="obj">The <see cref="JObject"/>. </param>
        /// <param name="schema">The <see cref="JsonSchema4"/>. </param>
        /// <returns>The <see cref="JsonObjectModel"/>. </returns>
        public static JsonObjectModel FromJson(JObject obj, JsonSchema4 schema)
        {
            var result = new JsonObjectModel();
            foreach (var property in schema.Properties)
            {
                if (property.Value.Type.HasFlag(JsonObjectType.Array))
                {
                    var propertySchema = property.Value.Item;
                    var objects = obj[property.Key].Select(o => o is JObject ?
                        (JsonTokenModel)FromJson((JObject)o, propertySchema) : JsonValueModel.FromJson((JValue)o, propertySchema));

                    var list = new ObservableCollection<JsonTokenModel>(objects);
                    foreach (var item in list)
                        item.ParentList = list;

                    result[property.Key] = list;
                }
                else if (property.Value.Type.HasFlag(JsonObjectType.Object))
                {
                    var token = obj[property.Key];
                    if (token is JObject)
                        result[property.Key] = FromJson((JObject)token, property.Value);
                    else
                        result[property.Key] = null;
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
            result.Schema = schema; 
            return result;
        }

        private static object GetDefaultValue(KeyValuePair<string, JsonProperty> property)
        {
            if (property.Value.Default != null)
                return ((JValue) property.Value.Default).Value;

            if (property.Value.Type.HasFlag(JsonObjectType.Boolean))
                return false;

            if (property.Value.Type.HasFlag(JsonObjectType.String) && property.Value.Format == JsonFormatStrings.DateTime)
                return new DateTime();
            if (property.Value.Type.HasFlag(JsonObjectType.String) && property.Value.Format == "date") // TODO: What to do with date/time?
                return new DateTime();
            if (property.Value.Type.HasFlag(JsonObjectType.String) && property.Value.Format == "time")
                return new TimeSpan();

            return null;
        }

        /// <summary>Gets the object's properties. </summary>
        public IEnumerable<JsonPropertyModel> Properties
        {
            get
            {
                var properties = new List<JsonPropertyModel>();
                if (Schema.Properties != null)
                {
                    foreach (var propertyInfo in Schema.Properties)
                    {
                        var property = new JsonPropertyModel(propertyInfo.Key, this, propertyInfo.Value);
                        if (property.Value is ObservableCollection<JsonTokenModel>)
                        {
                            foreach (var obj in (ObservableCollection<JsonTokenModel>)property.Value)
                                obj.Schema = propertyInfo.Value.Item;
                        }
                        properties.Add(property);
                    }
                }
                return properties;
            }
        }

        /// <summary>Validates the data of the <see cref="JsonObjectModel"/>. </summary>
        /// <returns>The list of validation errors. </returns>
        public Task<string[]> ValidateAsync()
        {
            return Task.Run(() =>
            {
                var obj = JToken.ReadFrom(new JsonTextReader(new StringReader(ToJson())));
                var errors = Schema.Validate(obj);
                return errors.Select(e => e.Path + ": " + e.Kind).ToArray();
            });
        }

        /// <summary>Converts the <see cref="JsonTokenModel"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public override JToken ToJToken()
        {
            var obj = new JObject();
            foreach (var pair in this)
            {
                if (pair.Value is ObservableCollection<JsonTokenModel>)
                {
                    var array = (ObservableCollection<JsonTokenModel>) pair.Value;
                    var jArray = new JArray();
                    foreach (var item in array)
                        jArray.Add(item.ToJToken());
                    obj[pair.Key] = jArray;
                }
                else if (pair.Value is JsonTokenModel)
                    obj[pair.Key] = ((JsonTokenModel)pair.Value).ToJToken();
                else
                    obj[pair.Key] = new JValue(pair.Value);
            }
            return obj; 
        }
    }
}