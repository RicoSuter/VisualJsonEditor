//-----------------------------------------------------------------------
// <copyright file="JsonPropertyModel.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using MyToolkit.Model;
using NJsonSchema;

namespace VisualJsonEditor.Models
{
    /// <summary>Describes a JSON property. </summary>
    public class JsonPropertyModel : ObservableObject
    {
        /// <summary>Initializes a new instance of the <see cref="JsonPropertyModel"/> class. </summary>
        /// <param name="name">The name of the property. </param>
        /// <param name="parent">The parent object. </param>
        /// <param name="schema">The property type as schema object. </param>
        public JsonPropertyModel(string name, JsonObjectModel parent, JsonProperty schema)
        {
            Name = name;
            Parent = parent;
            Schema = schema;

            Parent.PropertyChanged += (sender, args) => RaisePropertyChanged(() => Value);
        }

        /// <summary>Gets the property name. </summary>
        public string Name { get; private set; }

        /// <summary>Gets the parent object. </summary>
        public JsonObjectModel Parent { get; private set; }

        /// <summary>Gets the property type as schema. </summary>
        public JsonProperty Schema { get; private set; }

        /// <summary>Gets a value indicating whether the property is required. </summary>
        public bool IsRequired
        {
            get { return Schema.IsRequired; }
        }

        /// <summary>Gets or sets the value of the property. </summary>
        public object Value
        {
            get { return Parent.ContainsKey(Name) ? Parent[Name] : null; }
            set
            {
                Parent[Name] = value;

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