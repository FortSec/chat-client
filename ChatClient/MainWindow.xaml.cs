using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

using ChatClient.data.pages;
using ChatClient.Extensions;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /*
         * Pages init
         */
        private Onboarding onboarding = new Onboarding();

        protected int dragMoveThresh = 25;
        private Storyboard storyboard;
        public bool visualExceptionShown = false;
        public Loading loading;
        public Client client;

        public MainWindow()
        {
            InitializeComponent();
            onboarding.ChangePage += new EventHandler(onboarding_ChangePage);
            onboarding.ExceptionOccured += new EventHandler(DisplayException);
            onboarding.ReloadPage += new EventHandler(onboarding_ReloadPage);
            onboarding.ConnectingInProgress += new EventHandler(onboarding_ConnectingInProgress);
            onboarding.ConnectionInited += new EventHandler(onboarding_ConnectionInited);
            ExceptionBox.Visibility = Visibility.Collapsed;
            EventManager.RegisterClassHandler(typeof(Button), Button.MouseEnterEvent, new RoutedEventHandler(Button_MouseEnter));
            EventManager.RegisterClassHandler(typeof(Button), Button.MouseLeaveEvent, new RoutedEventHandler(Button_MouseLeave));
        }

        private void MainWindow1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Point mPoints = Mouse.GetPosition(MainWindow1);
                if (mPoints.Y <= dragMoveThresh)
                    this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MinimalizeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow1.WindowState = WindowState.Minimized;
        }

        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            onboarding.OnboardingPage.Opacity = -2.0d;
            MainFrame.NavigationService.Navigate(onboarding);
            var opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 1.0d;
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);
            Storyboard.SetTarget(opacityAnimation, onboarding.OnboardingPage);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Page.OpacityProperty));
            storyboard.Begin(this);
        }

        public void onboarding_ChangePage(object sender, EventArgs e)
        {
            loading = new Loading();
            MainFrame.NavigationService.Navigate(loading);
        }

        public void onboarding_ReloadPage(object sender, EventArgs e)
        {
            onboarding = new Onboarding();
            onboarding.ChangePage += new EventHandler(onboarding_ChangePage);
            onboarding.ExceptionOccured += new EventHandler(DisplayException);
            onboarding.ReloadPage += new EventHandler(onboarding_ReloadPage);
            onboarding.ConnectingInProgress += new EventHandler(onboarding_ConnectingInProgress);
            onboarding.ConnectionInited += new EventHandler(onboarding_ConnectionInited);
            MainFrame.NavigationService.Navigate(onboarding);
            var opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0d;
            opacityAnimation.To = 1.0d;
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);
            Storyboard.SetTarget(opacityAnimation, onboarding.OnboardingPage);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Page.OpacityProperty));
            storyboard.Begin(this);
        }

        public void DisplayException(object sender, EventArgs e)
        {
            VisualExceptionArgs e_args = e as VisualExceptionArgs;
            DisplayExceptionDirect(e_args.ExceptionText);
        }

        public async void DisplayExceptionDirect(string exceptionText)
        {
            if(visualExceptionShown)
            {
                HideExceptionDirect();
                await Task.Delay(600);
            }
            ExceptionTextBlock.Text = exceptionText;
            ExceptionBox.Visibility = Visibility.Visible;
            ThicknessAnimation anim = new ThicknessAnimation();
            anim.From = new Thickness(34, 659, 34, -52);
            anim.To = new Thickness(34, 560, 34, 27);
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(400));
            BackEase ease = new BackEase();
            ease.EasingMode = EasingMode.EaseOut;
            ease.Amplitude = 0.3d;
            anim.EasingFunction = ease;
            anim.DecelerationRatio = 0.1d;
            storyboard = new Storyboard();
            storyboard.Children.Add(anim);
            Storyboard.SetTarget(anim, ExceptionBox);
            Storyboard.SetTargetProperty(anim, new PropertyPath(Border.MarginProperty));
            storyboard.Begin(this);
            visualExceptionShown = true;
        }

        public void HideException(object sender, EventArgs e)
        {
            HideExceptionDirect();
        }

        public void HideExceptionDirect()
        {
            ThicknessAnimation anim = new ThicknessAnimation();
            anim.To = new Thickness(34, 659, 34, -52);
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(400));
            BackEase ease = new BackEase();
            ease.EasingMode = EasingMode.EaseIn;
            ease.Amplitude = 0.3d;
            anim.EasingFunction = ease;
            anim.AccelerationRatio = 0.1d;
            storyboard = new Storyboard();
            storyboard.Children.Add(anim);
            Storyboard.SetTarget(anim, ExceptionBox);
            Storyboard.SetTargetProperty(anim, new PropertyPath(Border.MarginProperty));
            storyboard.Begin(this);
            visualExceptionShown = false;
        }

        public void onboarding_ConnectingInProgress(object sender, EventArgs e)
        {
            loading.ChangeText("Connecting you...");
        }

        private void Button_MouseEnter(object sender, RoutedEventArgs e)
        {
            ClientTools.BeginHandCursor();
        }

        private void Button_MouseLeave(object sender, RoutedEventArgs e)
        {
            ClientTools.ResetCursor();
        }

        private void onboarding_ConnectionInited(object sender, EventArgs e)
        {
            MainViewEntryArgs args = e as MainViewEntryArgs;
            MainView mainview = new MainView(args.ServerURL, args.Client);
            mainview.ConnectionFailed += Mainview_ConnectionFailed;
            MainFrame.NavigationService.Navigate(mainview);
        }

        private void Mainview_ConnectionFailed(object sender, EventArgs e)
        {
            onboarding_ReloadPage(new object(), new EventArgs());
            VisualExceptionArgs e_args = e as VisualExceptionArgs;
            DisplayExceptionDirect(e_args.ExceptionText);
        }
    }
}
