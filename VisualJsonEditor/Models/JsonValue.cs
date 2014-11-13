//-----------------------------------------------------------------------
// <copyright file="JsonObject.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace VisualJsonEditor.Models
{
    /// <summary>Represents a single JSON value. </summary>
    public class JsonValue : JsonToken
    {
        /// <summary>Initializes a new instance of the <see cref="JsonValue"/> class. </summary>
        public JsonValue()
        {
            this["Value"] = null;
        }

        /// <summary>Creates a <see cref="JsonValue"/> from a <see cref="JValue"/> and a given schema. </summary>
        /// <param name="value">The value. </param>
        /// <param name="schema">The schema. </param>
        /// <returns>The <see cref="JsonValue"/>. </returns>
        public static JsonValue FromJson(JValue value, JsonSchema schema)
        {
            return new JsonValue
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
        
        /// <summary>Converts the <see cref="JsonToken"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public override JToken ToJToken()
        {
            return new JValue(Value);
        }
    }
}