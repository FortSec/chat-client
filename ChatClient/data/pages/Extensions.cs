using System;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;
using System.Text;

namespace ChatClient.Extensions
{
    class Extensions : DependencyObject
    {
        public static readonly DependencyProperty BTBfontSizeProperty = DependencyProperty.RegisterAttached("BTBFontSize", typeof(int), typeof(Extensions), new PropertyMetadata(16));

        public static void SetBTBFontSize(UIElement element, int value)
        {
            Console.WriteLine(value);
            element.SetValue(BTBfontSizeProperty, value);
        }

        public static int GetBTBFontSize(UIElement element)
        {
            Console.WriteLine((int)element.GetValue(BTBfontSizeProperty));
            return (int)element.GetValue(BTBfontSizeProperty);
        }
    }
}
