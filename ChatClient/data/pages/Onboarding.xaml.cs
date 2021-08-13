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
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatClient.CustomControls;
using ChatClient.Utils;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace ChatClient.data.pages
{
    /// <summary>
    /// Interaction logic for Onboarding.xaml
    /// </summary>
    public partial class Onboarding : Page
    {
        private Storyboard storyboard;
        public event EventHandler ChangePage;

        private void ChangePageEvent(EventArgs e)
        {
            if( ChangePage != null)
                ChangePage(this, e);
        }

        public Onboarding()
        {
            InitializeComponent();
        }

        private void MainTextBox_BTBTextChanged(object sender, EventArgs e)
        {
            BTBTextChangedEventArgs eArgs = (BTBTextChangedEventArgs)e;
            if(eArgs.BoxContent == "")
            {
                AnimateContinueOut();
            } else
            {
                AnimateContinueIn();
            }
        }

        private void AnimateContinueIn()
        {
            var anim = new ThicknessAnimation();
            anim.To = new Thickness(645, 419, 353, 146);
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            storyboard = new Storyboard();
            storyboard.Children.Add(anim);
            Storyboard.SetTarget(anim, EnterButton);
            Storyboard.SetTargetProperty(anim, new PropertyPath(Button.MarginProperty));
            storyboard.Begin(this);
        }

        private void AnimateContinueOut()
        {
            var anim = new ThicknessAnimation();
            anim.To = new Thickness(601, 419, 397, 146);
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            storyboard = new Storyboard();
            storyboard.Children.Add(anim);
            Storyboard.SetTarget(anim, EnterButton);
            Storyboard.SetTargetProperty(anim, new PropertyPath(Button.MarginProperty));
            storyboard.Begin(this);
        }

        private void AnimateAllOut()
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.To = 0.0d;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(400));
            storyboard = new Storyboard();
            storyboard.Children.Add(anim);
            Storyboard.SetTarget(anim, Content);
            Storyboard.SetTargetProperty(anim, new PropertyPath(Grid.OpacityProperty));
            storyboard.Begin(this);
        }

        private async void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            AnimateAllOut();
            await Task.Delay(700);
            ChangePageEvent(new EventArgs());
        }
    }
}
