//-----------------------------------------------------------------------
// <copyright file="JsonEditor.xaml.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VisualJsonEditor.Models;

namespace VisualJsonEditor.Controls
{
    /// <summary>A JSON editor control. </summary>
    public partial class JsonEditor
    {
        /// <summary>Initializes a new instance of the <see cref="JsonEditor"/> class. </summary>
        public JsonEditor()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(object), typeof(JsonEditor), new PropertyMetadata(default(object), (o, args) => { ((JsonEditor)o).Update(); }));

        /// <summary>Gets or sets the <see cref="JsonObject"/> to edit with the editor. </summary>
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private void Update()
        {
            Presenter.Content = Data;
        }

        private void OnAddArrayObject(object sender, RoutedEventArgs e)
        {
            var property = (JsonProperty)((Button)sender).Tag;

            if (property.Value == null)
                property.Value = new ObservableCollection<JsonToken>();

            var list = (ObservableCollection<JsonToken>)property.Value;
            var schema = property.Schema.Items.First();

            var obj = schema.Properties == null ? (JsonToken)new JsonValue { Schema = schema } : JsonObject.FromSchema(schema);
            obj.ParentList = list;

            list.Add(obj);
        }

        private void OnRemoveArrayObject(object sender, RoutedEventArgs e)
        {
            var obj = (JsonToken)((Button)sender).Tag;
            obj.ParentList.Remove(obj);
        }

        private void OnCreateObject(object sender, RoutedEventArgs e)
        {
            var property = (JsonProperty)((CheckBox)sender).Tag;
            if (property.Parent[property.Key] == null)
            {
                property.Parent[property.Key] = JsonObject.FromSchema(property.Schema);
                property.RaisePropertyChanged<JsonProperty>(i => i.HasValue);
            }
        }

        private void OnRemoveObject(object sender, RoutedEventArgs e)
        {
            var property = (JsonProperty)((CheckBox)sender).Tag;
            if (property.Parent.ContainsKey(property.Key) && property.Parent[property.Key] != null)
            {
                property.Parent[property.Key] = null;
                property.RaisePropertyChanged<JsonProperty>(i => i.HasValue);
            }
        }

        private void OnCreateArray(object sender, RoutedEventArgs e)
        {
            var property = (JsonProperty)((CheckBox)sender).Tag;
            if (property.Parent[property.Key] == null)
            {
                property.Parent[property.Key] = new ObservableCollection<JsonToken>();
                property.RaisePropertyChanged<JsonProperty>(i => i.HasValue);
            }
        }

        private void OnRemoveArray(object sender, RoutedEventArgs e)
        {
            var property = (JsonProperty)((CheckBox)sender).Tag;
            if (property.Parent.ContainsKey(property.Key) && property.Parent[property.Key] != null)
            {
                property.Parent[property.Key] = null;
                property.RaisePropertyChanged<JsonProperty>(i => i.HasValue);
            }
        }
    }
}
