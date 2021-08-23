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
using System.Net.Http;
using System.Net.Http.Json;
using ChatClient.Responses;
using ChatClient.Extensions;

namespace ChatClient.data.pages
{
    /// <summary>
    /// Interaction logic for Onboarding.xaml
    /// </summary>
    public partial class Onboarding : Page
    {
        private Storyboard storyboard;
        public event EventHandler ChangePage;
        public event EventHandler ExceptionOccured;
        public event EventHandler ReloadPage;
        private static readonly HttpClient client = new HttpClient();

        private void ChangePageEvent(EventArgs e)
        {
            if( ChangePage != null)
                ChangePage(this, e);
        }

        private void ReloadPageEvent(EventArgs e)
        {
            if (ReloadPage != null)
                ReloadPage(this, e);
        }

        private void ExceptionOccuredEvent(VisualExceptionArgs e)
        {
            if (ExceptionOccured != null)
                ExceptionOccured(this, e);
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
            VisualExceptionArgs event_args = new VisualExceptionArgs();
            string requestedUrl = MainTextBox.GetBTBValue();
            requestedUrl = ClientTools.NormalizeURL(requestedUrl);
            if (requestedUrl == "invalid url")
            {
                event_args.ExceptionText = "This text is unknown to me. I can only understand URL.";
                ExceptionOccuredEvent(event_args);
            }else if (requestedUrl == "parse error")
            {
                event_args.ExceptionText = "Let's just say that I have some trouble understanding the enetred address.";
                ExceptionOccuredEvent(event_args);
            } else
            {
                AnimateAllOut();
                await Task.Delay(700);
                ChangePageEvent(new EventArgs());
                InitConnection();
            }
        }

        private async void InitConnection()
        {
            ClientTools.BeginBusyCursor();
            APIPath jsonPathResponse;
            Status jsonStatusResponse;
            VisualExceptionArgs event_args = new VisualExceptionArgs();
            string requestedUrl = MainTextBox.GetBTBValue();
            requestedUrl = ClientTools.NormalizeURL(requestedUrl);
            try
            {
                jsonPathResponse = await client.GetFromJsonAsync<APIPath>($"{requestedUrl}path/status");
            } catch(HttpRequestException e)
            {
                ClientTools.EndBusyCursor();
                event_args.ExceptionText = $"{e.Message}";
                ExceptionOccuredEvent(event_args);
                ReloadPageEvent(new EventArgs());
                return;
            }
            string statusRequestUrl = jsonPathResponse.Data.Path;
            try
            {
                jsonStatusResponse = await client.GetFromJsonAsync<Status>(statusRequestUrl);
            }
            catch (HttpRequestException e)
            {
                ClientTools.EndBusyCursor();
                event_args.ExceptionText = $"{e.Message}";
                ExceptionOccuredEvent(event_args);
                ReloadPageEvent(new EventArgs());
                return;
            }
            if(jsonStatusResponse.Response == "success")
            {
                if(jsonStatusResponse.Data.Status == "open" && jsonStatusResponse.Data.Accepts_Clients)
                {
                    ClientTools.EndBusyCursor();
                    MessageBox.Show("connecting");
                } else
                {
                    ClientTools.EndBusyCursor();
                    MessageBox.Show($"Status: {jsonStatusResponse.Data.Status}\nAccepts clients: {jsonStatusResponse.Data.Accepts_Clients.ToString()}");
                }
            } else
            {
                ClientTools.EndBusyCursor();
                event_args.ExceptionText = "Everything was fine until I've reached out the server. He doesn't want us let in!";
                ExceptionOccuredEvent(event_args);
                ReloadPageEvent(new EventArgs());
            }
        }
    }
}
