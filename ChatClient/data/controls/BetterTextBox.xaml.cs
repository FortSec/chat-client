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
    public partial class BetterTextBox : UserControl
    {

        private DoubleAnimation cursorAnimation = new DoubleAnimation();
        public static readonly DependencyProperty BTBfontSizeProperty = DependencyProperty.Register("BTBFontSize", typeof(int), typeof(BetterTextBox), new PropertyMetadata(16));
        public static readonly DependencyProperty BTBpaddingProperty = DependencyProperty.Register("BTBPadding", typeof(string), typeof(BetterTextBox), new PropertyMetadata("5,5,5,5"));
        public static readonly DependencyProperty BTBvContentAlignProperty = DependencyProperty.Register("BTBVContentAlign", typeof(string), typeof(BetterTextBox), new PropertyMetadata("Top"));
        public static readonly DependencyProperty BTBborderColorProperty = DependencyProperty.Register("BTBBorderColor", typeof(string), typeof(BetterTextBox), new PropertyMetadata("White"));
        public static readonly DependencyProperty BTBcornerRadiusProperty = DependencyProperty.Register("BTBCornerRadius", typeof(int), typeof(BetterTextBox), new PropertyMetadata(2));
        public event EventHandler BTBTextChanged;
        public event EventHandler BTBEnterDown;

        public int BTBFontSize
        {
            get { return (int)GetValue(BTBfontSizeProperty);  }
            set { SetValue(BTBfontSizeProperty, value); }
        }
        public string BTBPadding
        {
            get { return (string)GetValue(BTBpaddingProperty); }
            set { SetValue(BTBpaddingProperty, value); }
        }
        public string BTBVContentAlign
        {
            get { return (string)GetValue(BTBvContentAlignProperty); }
            set { SetValue(BTBvContentAlignProperty, value); }
        }

        public string BTBBorderColor
        {
            get { return (string)GetValue(BTBborderColorProperty); }
            set { SetValue(BTBborderColorProperty, value); }
        }

        public int BTBCornerRadius
        {
            get { return (int)GetValue(BTBcornerRadiusProperty); }
            set { SetValue(BTBcornerRadiusProperty, value); }
        }

        public BetterTextBox()
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

        public string GetBTBValue()
        {
            return MainTextBox.Text;
        }

        public void SetBTBValue(string newVal)
        {
            MainTextBox.Text = newVal;
        }

        public Brush BTBBackground
        {
            get { return MainTextBox.Background; }
            set { MainTextBox.Background = value; }
        }

        private void UpdateCaretPosition()
        {
            var rectangle = MainTextBox.GetRectFromCharacterIndex(MainTextBox.CaretIndex);
            Caret.Height = rectangle.Bottom - rectangle.Top;
            Canvas.SetTop(Caret, rectangle.Top);
            Canvas.SetBottom(Caret, rectangle.Bottom);

            var left = Canvas.GetLeft(Caret);
            if (!double.IsNaN(left))
            {
                cursorAnimation.From = left;
                cursorAnimation.To = rectangle.Right;
                cursorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.1));

                Caret.BeginAnimation(Canvas.LeftProperty, cursorAnimation);
            }
            else
            {
                Canvas.SetLeft(Caret, rectangle.Right);
            }
        }

        private void MainTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateCaretPosition();
        }

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BTBTextChangedEventArgs eventArgs = new BTBTextChangedEventArgs();
            eventArgs.BoxContent = MainTextBox.Text;
            UpdateCaretPosition();
            if(BTBTextChanged != null)
                BTBTextChanged(this, eventArgs);
        }

        private void MainTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UpdateCaretPosition();
        }

        private void MainTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                BTBEnterDown?.Invoke(this, e);
            }
        }
    }
}
