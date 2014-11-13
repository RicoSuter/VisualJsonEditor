//-----------------------------------------------------------------------
// <copyright file="JsonApplicationConfiguration.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json.Schema;

namespace VisualJsonEditor.Utilities
{
    /// <summary>Extension methods for the <see cref="JsonSchema"/> class. </summary>
    public static class JsonSchemaExtensions
    {
        /// <summary>Checks whether the type in the schema is required. </summary>
        /// <param name="schema">The schema. </param>
        /// <returns><c>true</c> if the type is required; otherwise, <c>false</c>. </returns>
        public static bool IsRequired(this JsonSchema schema)
        {
            return schema.Required.HasValue && schema.Required.Value && (!schema.Type.HasValue || !schema.Type.Value.HasFlag(JsonSchemaType.Null));
        }
    }
}
