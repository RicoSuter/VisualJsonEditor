//-----------------------------------------------------------------------
// <copyright file="DoubleUpDownRangeConverter.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace VisualJsonEditor.Converters
{
    public class DoubleUpDownRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return parameter.ToString() == "min" ? double.MinValue : double.MaxValue;
            return (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}