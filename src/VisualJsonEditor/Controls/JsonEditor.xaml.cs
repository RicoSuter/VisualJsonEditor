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
using NJsonSchema;
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

        /// <summary>Gets or sets the <see cref="JsonObjectModel"/> to edit with the editor. </summary>
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
            var property = (JsonPropertyModel)((Button)sender).Tag;

            if (property.Value == null)
                property.Value = new ObservableCollection<JsonTokenModel>();

            var list = (ObservableCollection<JsonTokenModel>)property.Value;
            var schema = property.Schema.ActualPropertySchema.Item.ActualSchema;

            var obj = !schema.Type.HasFlag(JsonObjectType.Object) && !schema.Type.HasFlag(JsonObjectType.Array) ? 
                (JsonTokenModel)new JsonValueModel { Schema = schema } : JsonObjectModel.FromSchema(schema);
            obj.ParentList = list;

            list.Add(obj);
        }

        private void OnRemoveArrayObject(object sender, RoutedEventArgs e)
        {
            var obj = (JsonTokenModel)((Button)sender).Tag;
            obj.ParentList.Remove(obj);
        }

        private void OnCreateObject(object sender, RoutedEventArgs e)
        {
            var property = (JsonPropertyModel)((CheckBox)sender).Tag;
            if (property.Parent[property.Name] == null)
            {
                property.Parent[property.Name] = JsonObjectModel.FromSchema(property.Schema.ActualPropertySchema);
                property.RaisePropertyChanged<JsonPropertyModel>(i => i.HasValue);
            }
        }

        private void OnRemoveObject(object sender, RoutedEventArgs e)
        {
            var property = (JsonPropertyModel)((CheckBox)sender).Tag;
            if (property.Parent.ContainsKey(property.Name) && property.Parent[property.Name] != null)
            {
                property.Parent[property.Name] = null;
                property.RaisePropertyChanged<JsonPropertyModel>(i => i.HasValue);
            }
        }

        private void OnCreateArray(object sender, RoutedEventArgs e)
        {
            var property = (JsonPropertyModel)((CheckBox)sender).Tag;
            if (property.Parent[property.Name] == null)
            {
                property.Parent[property.Name] = new ObservableCollection<JsonTokenModel>();
                property.RaisePropertyChanged<JsonPropertyModel>(i => i.HasValue);
            }
        }

        private void OnRemoveArray(object sender, RoutedEventArgs e)
        {
            var property = (JsonPropertyModel)((CheckBox)sender).Tag;
            if (property.Parent.ContainsKey(property.Name) && property.Parent[property.Name] != null)
            {
                property.Parent[property.Name] = null;
                property.RaisePropertyChanged<JsonPropertyModel>(i => i.HasValue);
            }
        }
    }
}
