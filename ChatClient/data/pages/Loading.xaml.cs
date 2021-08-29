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

namespace ChatClient.data.pages
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class Loading : Page
    {

        private Storyboard storyboard;

        public Loading()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0.1d;
            opacityAnimation.To = 1.0d;
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            storyboard = new Storyboard
            {
                Duration = TimeSpan.FromMilliseconds(1700),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            storyboard.Children.Add(opacityAnimation);
            Storyboard.SetTarget(opacityAnimation, MainGrid);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Grid.OpacityProperty));
            storyboard.Begin(this);
        }

        public void ChangeText(string nText)
        {
            LoadingText.Text = nText;
        }
    }
}
