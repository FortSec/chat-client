using System;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

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

    public class VisualExceptionArgs : EventArgs
    {
        public string ExceptionText { get; set; }
    }

    static class ClientTools
    {
        public static string NormalizeURL(string url)
        {
            Regex urlRegex = new Regex(@"(^(https?:\/\/)?(www\.)?(([a-zA-Z0-9]+(-?[a-zA-Z0-9])*\.)+([\w]{2,}(\/\S*)?))$)|(^(http:\/\/www\.)?(https:\/\/www\.)?(http:\/\/)?(https:\/\/)?(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(:([0-9])([0-9])?([0-9])?([0-9])?([0-9])?)?(\/.*)?$)");
            if (!urlRegex.IsMatch(url))
            {
                return "invalid url";
            }

            Uri uri;
            if(!url.StartsWith("http://") || !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }
            if(!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return "parse error";
            } else
            {
                return uri.ToString();
            }
        }

        public static void BeginBusyCursor()
        {
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public static void EndBusyCursor()
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
