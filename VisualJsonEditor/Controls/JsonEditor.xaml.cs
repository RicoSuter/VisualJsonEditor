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
using VisualJsonEditor.Model;

namespace VisualJsonEditor.Controls
{
    public partial class JsonEditor
    {
        public JsonEditor()
        {
            InitializeComponent();
            Properties = new ObservableCollection<JsonProperty>();
        }

        public ObservableCollection<JsonProperty> Properties { get; set; }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(JsonObject), typeof(JsonEditor), new PropertyMetadata(default(JsonObject), PropertyChanged));

        public JsonObject Data
        {
            get { return (JsonObject)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        
        private static void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((JsonEditor)obj).UpdateProperties();
        }

        private void UpdateProperties()
        {
            Properties.Clear();
            if (Data != null && Data.Schema.Properties != null)
            {
                foreach (var propertyInfo in Data.Schema.Properties)
                {
                    var property = new JsonProperty(propertyInfo.Key, Data, propertyInfo.Value);
                    if (property.Value is ObservableCollection<JsonObject>)
                    {
                        foreach (var obj in (ObservableCollection<JsonObject>)property.Value)
                            obj.Schema = propertyInfo.Value.Items.First();
                    }

                    Properties.Add(property);
                }
            }
        }

        private void OnAddArrayObject(object sender, RoutedEventArgs e)
        {
            var property = (JsonProperty)((Button)sender).Tag;

            if (property.Value == null)
                property.Value = new ObservableCollection<JsonObject>();

            var list = (ObservableCollection<JsonObject>)property.Value;
            var schema = property.Schema.Items.First();

            var obj = JsonObject.FromSchema(schema);
            obj.ParentList = list;

            list.Add(obj);
        }

        private void OnRemoveArrayObject(object sender, RoutedEventArgs e)
        {
            var obj = (JsonObject)((Button)sender).Tag;
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
                property.Parent[property.Key] = new ObservableCollection<JsonObject>();
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
