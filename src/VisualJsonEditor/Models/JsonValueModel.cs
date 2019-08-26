//-----------------------------------------------------------------------
// <copyright file="JsonObject.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace VisualJsonEditor.Models
{
    /// <summary>Represents a single JSON value. </summary>
    public class JsonValueModel : JsonTokenModel
    {
        /// <summary>Initializes a new instance of the <see cref="JsonValueModel"/> class. </summary>
        public JsonValueModel()
        {
            this["Value"] = null;
        }

        /// <summary>Creates a <see cref="JsonValueModel"/> from a <see cref="JValue"/> and a given schema. </summary>
        /// <param name="value">The value. </param>
        /// <param name="schema">The schema. </param>
        /// <returns>The <see cref="JsonValueModel"/>. </returns>
        public static JsonValueModel FromJson(JValue value, JsonSchema schema)
        {
            return new JsonValueModel
            {
                Schema = schema,
                Value = value.Value
            };
        }

        /// <summary>Gets or sets the value. </summary>
        public object Value
        {
            get { return ContainsKey("Value") ? this["Value"] : null; }
            set { this["Value"] = value; }
        }
        
        /// <summary>Converts the <see cref="JsonTokenModel"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public override JToken ToJToken()
        {
            return new JValue(Value);
        }
    }
}