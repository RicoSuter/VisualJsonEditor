//-----------------------------------------------------------------------
// <copyright file="JsonProperty.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using MyToolkit.Model;
using Newtonsoft.Json.Schema;
using VisualJsonEditor.Utilities;

namespace VisualJsonEditor.Models
{
    /// <summary>Describes a JSON property. </summary>
    public class JsonProperty : ObservableObject
    {
        /// <summary>Initializes a new instance of the <see cref="JsonProperty"/> class. </summary>
        /// <param name="key">The key of the property. </param>
        /// <param name="parent">The parent object. </param>
        /// <param name="schema">The property type as schema object. </param>
        public JsonProperty(string key, JsonObject parent, JsonSchema schema)
        {
            Key = key;
            Parent = parent;
            Schema = schema;

            Parent.PropertyChanged += (sender, args) => RaisePropertyChanged(() => Value);
        }

        /// <summary>Gets the property key. </summary>
        public string Key { get; private set; }

        /// <summary>Gets the parent object. </summary>
        public JsonObject Parent { get; private set; }

        /// <summary>Gets the property type as schema. </summary>
        public JsonSchema Schema { get; private set; }

        /// <summary>Gets a value indicating whether the property is required. </summary>
        public bool IsRequired
        {
            get { return Schema.IsRequired(); }
        }

        /// <summary>Gets or sets the value of the property. </summary>
        public object Value
        {
            get { return Parent.ContainsKey(Key) ? Parent[Key] : null; }
            set
            {
                Parent[Key] = value;

                RaisePropertyChanged(() => Value);
                RaisePropertyChanged(() => HasValue);
            }
        }

        /// <summary>Gets a value indicating whether the property has a value. </summary>
        public bool HasValue
        {
            get { return Value != null; }
        }
    }
}