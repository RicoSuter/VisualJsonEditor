//-----------------------------------------------------------------------
// <copyright file="ExpandingGroupBox.cs" company="Visual JSON Editor">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://visualjsoneditor.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using MyToolkit.Utilities;

namespace VisualJsonEditor.Controls
{
    /// <summary>A group box with expander. </summary>
    public class ExpandingGroupBox : ContentControl
    {
        static ExpandingGroupBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpandingGroupBox), new FrameworkPropertyMetadata(typeof(ExpandingGroupBox)));
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof (object), typeof (ExpandingGroupBox), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the header object (data context of header template). </summary>
        public object Header
        {
            get { return (object) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate", typeof (DataTemplate), typeof (ExpandingGroupBox), new PropertyMetadata(default(DataTemplate)));

        /// <summary>Gets or sets the header template (always visible). </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded", typeof (bool), typeof (ExpandingGroupBox), new PropertyMetadata(true));

        /// <summary>Gets or sets a value indicating whether the group box is expanded. </summary>
        public bool IsExpanded
        {
            get { return (bool) GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal 
        /// processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>. </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += delegate
            {
                var button = this.FindVisualChild<Button>();
                button.Click += OnToggleExpanded;
            };
        }

        private void OnToggleExpanded(object sender, RoutedEventArgs args)
        {
            IsExpanded = !IsExpanded;
            args.Handled = true; 
        }
    }
}
