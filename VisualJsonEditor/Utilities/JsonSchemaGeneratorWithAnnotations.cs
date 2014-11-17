//-----------------------------------------------------------------------
// <copyright file="JsonSchemaGeneratorWithAnnotations.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace VisualJsonEditor.Utilities
{
    /// <summary>Generates a <see cref="JsonSchema"/> from a specified <see cref="Type"/>. </summary>
    public class JsonSchemaGeneratorWithAnnotations
    {
        /// <summary>Initializes a new instance of the <see cref="JsonSchemaGeneratorWithAnnotations"/> class. </summary>
        public JsonSchemaGeneratorWithAnnotations()
        {
            Generator = new JsonSchemaGenerator();
        }

        /// <summary>Gets the internal generator. </summary>
        public JsonSchemaGenerator Generator { get; private set; }

        /// <summary>Generate a <see cref="JsonSchema"/> from the specified type. </summary>
        /// <param name="type">The type to generate a <see cref="JsonSchema"/> from.</param>
        /// <returns>A <see cref="JsonSchema"/> generated from the specified type.</returns>
        public JsonSchema Generate(Type type)
        {
            var schema = Generator.Generate(type);
            UpdateSchemaWithCustomAttributes(type, schema);
            return schema;
        }

        private static void UpdateSchemaWithCustomAttributes(Type type, JsonSchema schema)
        {
            if (schema.Properties != null)
            {
                foreach (var pair in schema.Properties)
                {
                    var property = GetPropertyInfo(type, pair.Key);
                    if (property != null)
                    {
                        ProcessPropertyAttributes(property, pair.Value);
                        UpdateSchemaWithCustomAttributes(property.PropertyType, pair.Value);
                    }
                }
            }

            if (schema.Items != null)
            {
                foreach (var item in schema.Items)
                    UpdateSchemaWithCustomAttributes(type.GetElementType() ?? type.GenericTypeArguments[0], item);
            }
        }

        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            foreach (var property in type.GetProperties())
            {
                var propertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                if (propertyAttribute != null && propertyAttribute.PropertyName == propertyName)
                    return property;
                if (property.Name == propertyName)
                    return property;
            }
            return null;
        }

        private static void ProcessPropertyAttributes(PropertyInfo property, JsonSchema schema)
        {
            ProcessDescriptionAttribute(property, schema);
            ProcessRangeAttribute(property, schema);
            ProcessRegularExpressionAttribute(property, schema);
            ProcessRequiredAttribute(property, schema);
        }

        private static void ProcessDescriptionAttribute(PropertyInfo property, JsonSchema schema)
        {
            var descriptionAttribute = property.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
                schema.Description = descriptionAttribute.Description;
        }

        private static void ProcessRangeAttribute(PropertyInfo property, JsonSchema schema)
        {
            var rangeAttribute = property.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute != null)
            {
                if (rangeAttribute.Minimum != null)
                    schema.Minimum = double.Parse(rangeAttribute.Minimum.ToString());
                if (rangeAttribute.Maximum != null)
                    schema.Maximum = double.Parse(rangeAttribute.Maximum.ToString());
            }
        }

        private static void ProcessRegularExpressionAttribute(PropertyInfo property, JsonSchema schema)
        {
            var regularExpressionAttribute = property.GetCustomAttribute<RegularExpressionAttribute>();
            if (regularExpressionAttribute != null)
                schema.Pattern = regularExpressionAttribute.Pattern;
        }

        private static void ProcessRequiredAttribute(PropertyInfo property, JsonSchema schema)
        {
            var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
            if (requiredAttribute != null)
            {
                schema.Required = true;
                if (schema.Type.HasValue)
                {
                    schema.Type &= ~JsonSchemaType.Null;
                    if (schema.Type.Value.HasFlag(JsonSchemaType.String) && !requiredAttribute.AllowEmptyStrings)
                        schema.MinimumLength = 1;
                }
            }
        }
    }
}