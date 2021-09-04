using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Diagnostics;

using ChatClient.Extensions;
using ChatClient.CustomControls;

namespace ChatClient.CustomControls
{
    /// <summary>
    /// Interaction logic for BetterTextBox.xaml
    /// </summary>
    public partial class BetterPwdBox : UserControl
    {

        private DoubleAnimation cursorAnimation = new DoubleAnimation();
        public static readonly DependencyProperty BPBfontSizeProperty = DependencyProperty.Register("BPBFontSize", typeof(int), typeof(BetterPwdBox), new PropertyMetadata(16));
        public static readonly DependencyProperty BPBpaddingProperty = DependencyProperty.Register("BPBPadding", typeof(string), typeof(BetterPwdBox), new PropertyMetadata("5,5,5,5"));
        public static readonly DependencyProperty BPBvContentAlignProperty = DependencyProperty.Register("BPBVContentAlign", typeof(string), typeof(BetterPwdBox), new PropertyMetadata("Top"));
        public static readonly DependencyProperty BPBborderColorProperty = DependencyProperty.Register("BPBBorderColor", typeof(string), typeof(BetterPwdBox), new PropertyMetadata("White"));
        public static readonly DependencyProperty BPBcornerRadiusProperty = DependencyProperty.Register("BPBCornerRadius", typeof(int), typeof(BetterPwdBox), new PropertyMetadata(2));

        public int BPBFontSize
        {
            get { return (int)GetValue(BPBfontSizeProperty);  }
            set { SetValue(BPBfontSizeProperty, value); }
        }
        public string BPBPadding
        {
            get { return (string)GetValue(BPBpaddingProperty); }
            set { SetValue(BPBpaddingProperty, value); }
        }
        public string BPBVContentAlign
        {
            get { return (string)GetValue(BPBvContentAlignProperty); }
            set { SetValue(BPBvContentAlignProperty, value); }
        }

        public string BPBBorderColor
        {
            get { return (string)GetValue(BPBborderColorProperty); }
            set { SetValue(BPBborderColorProperty, value); }
        }

        public int BPBCornerRadius
        {
            get { return (int)GetValue(BPBcornerRadiusProperty); }
            set { SetValue(BPBcornerRadiusProperty, value); }
        }

        public BetterPwdBox()
        {
            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseEnterEvent, new RoutedEventHandler(TextBox_MouseEnter));
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseLeaveEvent, new RoutedEventHandler(TextBox_LeaveEvent));
        }

        private void TextBox_MouseEnter(object sender, RoutedEventArgs e)
        {
            ClientTools.BeginTextCursor();
        }

        private void TextBox_LeaveEvent(object sender, RoutedEventArgs e)
        {
            ClientTools.ResetCursor();
        }

        public string GetBPBValue()
        {
            return MainTextBox.Password;
        }

        public Brush BPBBackground
        {
            get { return MainTextBox.Background; }
            set { MainTextBox.Background = value; }
        }

        private void MainTextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            ClientTools.BeginTextCursor();
        }

        private void MainTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            ClientTools.ResetCursor();
        }
    }
}
