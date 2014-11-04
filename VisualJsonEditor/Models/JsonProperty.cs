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
    public class JsonProperty : ObservableObject
    {
        public JsonProperty(string key, JsonObject parent, JsonSchema schema)
        {
            Key = key;
            Parent = parent;
            Schema = schema;

            Parent.PropertyChanged += (sender, args) => RaisePropertyChanged(() => Value);
        }

        public string Key { get; private set; }

        public JsonObject Parent { get; private set; }

        public JsonSchema Schema { get; private set; }

        public bool IsRequired
        {
            get { return Schema.IsRequired(); }
        }

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

        public bool HasValue
        {
            get { return Value != null; }
        }
    }
}