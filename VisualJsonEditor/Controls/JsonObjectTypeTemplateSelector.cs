//-----------------------------------------------------------------------
// <copyright file="JsonObjectTypeTemplateSelector.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Schema;
using VisualJsonEditor.Models;

namespace VisualJsonEditor.Controls
{
    public class JsonObjectTypeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var presenter = (FrameworkElement)container;

            if (item is JsonObject)
                return (DataTemplate)presenter.Resources["RootTemplate"];

            JsonSchema schema = null;
            if (item is JsonToken)
                schema = ((JsonToken)item).Schema;

            if (item is JsonProperty)
                schema = ((JsonProperty)item).Schema;

            if (schema != null && schema.Type.HasValue)
            {
                var type = schema.Type.Value;

                if (type.HasFlag(JsonSchemaType.String) && schema.Format == "date-time")
                    return (DataTemplate)presenter.Resources["DateTimeTemplate"];
                if (type.HasFlag(JsonSchemaType.String) && schema.Format == "date")
                    return (DataTemplate)presenter.Resources["DateTemplate"];
                if (type.HasFlag(JsonSchemaType.String) && schema.Format == "time")
                    return (DataTemplate)presenter.Resources["TimeTemplate"];
                if (type.HasFlag(JsonSchemaType.String) && schema.Enum != null)
                    return (DataTemplate)presenter.Resources["EnumTemplate"];
                if (type.HasFlag(JsonSchemaType.String))
                    return (DataTemplate)presenter.Resources["StringTemplate"];

                if (type.HasFlag(JsonSchemaType.Integer) && schema.Enum != null)
                    return (DataTemplate)presenter.Resources["EnumTemplate"];
                if (type.HasFlag(JsonSchemaType.Integer))
                    return (DataTemplate)presenter.Resources["IntegerTemplate"];

                if (type.HasFlag(JsonSchemaType.Float))
                    return (DataTemplate)presenter.Resources["NumberTemplate"];
                if (type.HasFlag(JsonSchemaType.Boolean))
                    return (DataTemplate)presenter.Resources["BooleanTemplate"];
                if (type.HasFlag(JsonSchemaType.Object))
                    return (DataTemplate)presenter.Resources["ObjectTemplate"];
                if (type.HasFlag(JsonSchemaType.Array))
                    return (DataTemplate)presenter.Resources["ArrayTemplate"];
            }

            return base.SelectTemplate(item, container);
        }
    }
}