//-----------------------------------------------------------------------
// <copyright file="JsonObjectTypeTemplateSelector.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using NJsonSchema;
using VisualJsonEditor.Models;

namespace VisualJsonEditor.Controls
{
    /// <summary>Template selector which selects the template based on the given JSON object type. </summary>
    public class JsonObjectTypeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var presenter = (FrameworkElement)container;

            if (item is JsonObjectModel)
                return (DataTemplate)presenter.Resources["RootTemplate"];

            JsonSchema4 schema = null;
            if (item is JsonTokenModel)
                schema = ((JsonTokenModel)item).Schema.ActualSchema;
            else if (item is JsonPropertyModel)
                schema = ((JsonPropertyModel)item).Schema.ActualSchema;
            else 
                throw new NotImplementedException("The item given item type is not supported.");

            var type = schema.Type;

            if (type.HasFlag(JsonObjectType.String) && schema.Format == JsonFormatStrings.DateTime) // TODO: What to do with date/time?
                return (DataTemplate)presenter.Resources["DateTimeTemplate"];
            if (type.HasFlag(JsonObjectType.String) && schema.Format == "date")
                return (DataTemplate)presenter.Resources["DateTemplate"];
            if (type.HasFlag(JsonObjectType.String) && schema.Format == "time")
                return (DataTemplate)presenter.Resources["TimeTemplate"];
            if (type.HasFlag(JsonObjectType.String) && schema.Enumeration.Count > 0)
                return (DataTemplate)presenter.Resources["EnumTemplate"];
            if (type.HasFlag(JsonObjectType.String))
                return (DataTemplate)presenter.Resources["StringTemplate"];

            if (type.HasFlag(JsonObjectType.Integer) && schema.Enumeration.Count > 0)
                return (DataTemplate)presenter.Resources["EnumTemplate"];
            if (type.HasFlag(JsonObjectType.Integer))
                return (DataTemplate)presenter.Resources["IntegerTemplate"];

            if (type.HasFlag(JsonObjectType.Number))
                return (DataTemplate)presenter.Resources["NumberTemplate"];
            if (type.HasFlag(JsonObjectType.Boolean))
                return (DataTemplate)presenter.Resources["BooleanTemplate"];
            if (type.HasFlag(JsonObjectType.Object))
                return (DataTemplate)presenter.Resources["ObjectTemplate"];
            if (type.HasFlag(JsonObjectType.Array))
                return (DataTemplate)presenter.Resources["ArrayTemplate"];

            return base.SelectTemplate(item, container);
        }
    }
}