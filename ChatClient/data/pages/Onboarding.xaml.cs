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
using System.Windows.Media.Animation;

namespace ChatClient.data.pages
{
    /// <summary>
    /// Interaction logic for Onboarding.xaml
    /// </summary>
    public partial class Onboarding : Page
    {
        private Storyboard storyboard;

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
            anim.To = new Thickness(124, 419, 353, 146);
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
            anim.To = new Thickness(80, 419, 397, 146);
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            storyboard = new Storyboard();
            storyboard.Children.Add(anim);
            Storyboard.SetTarget(anim, EnterButton);
            Storyboard.SetTargetProperty(anim, new PropertyPath(Button.MarginProperty));
            storyboard.Begin(this);
        }
    }
}
