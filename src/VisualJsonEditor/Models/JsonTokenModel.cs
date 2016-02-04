//-----------------------------------------------------------------------
// <copyright file="JsonObject.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using MyToolkit.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace VisualJsonEditor.Models
{
    /// <summary>Represents a JSON token which may be an object or a single value. </summary>
    public abstract class JsonTokenModel : ObservableDictionary<string, object>
    {
        /// <summary>Gets or sets the schema of the token. </summary>
        public JsonSchema4 Schema { get; set; }

        /// <summary>Gets or sets the parent list if applicable (may be null). </summary>
        public ObservableCollection<JsonTokenModel> ParentList { get; set; }

        /// <summary>Converts the token to a JSON string. </summary>
        /// <returns>The JSON string. </returns>
        public string ToJson()
        {
            var token = ToJToken();
            return JsonConvert.SerializeObject(token, Formatting.Indented);
        }

        /// <summary>Converts the <see cref="JsonTokenModel"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public abstract JToken ToJToken();
    }
}