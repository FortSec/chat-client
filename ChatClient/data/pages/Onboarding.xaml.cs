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
using ChatClient;

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
        public event EventHandler ConnectingInProgress;
        public event EventHandler ConnectionInited;
        private static readonly HttpClient client = new HttpClient();
        private bool connectBtnReserved = false;
        private LoginDialog loginDialog;

        private void ChangePageEvent(EventArgs e)
        {
            if( ChangePage != null)
                ChangePage(this, e);
        }

        private void ReloadPageEvent(EventArgs e)
        {
            ReloadPage?.Invoke(this, e);
        }

        private void ConnectionInitedEvent(EventArgs e)
        {
            ConnectionInited?.Invoke(this, e);
        }

        private void ConnectingInProgressEvent(EventArgs e)
        {
            ConnectingInProgress?.Invoke(this, e);
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
            Storyboard polyAnim = FindResource("BGPolyStoryboard") as Storyboard;
            polyAnim.Begin();
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
            if (connectBtnReserved)
                return;
            connectBtnReserved = true;
            VisualExceptionArgs event_args = new VisualExceptionArgs();
            string requestedUrl = MainTextBox.GetBTBValue();
            if (requestedUrl == "localdebug")
                requestedUrl = "127.0.0.1:5000";
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
            await Task.Delay(650);
            connectBtnReserved = false;
        }

        private async void InitConnection()
        {
            ClientTools.BeginBusyCursor();
            APIPath jsonPathResponse;
            Status jsonStatusResponse;
            VisualExceptionArgs event_args = new VisualExceptionArgs();
            string requestedUrl = MainTextBox.GetBTBValue();
            if (requestedUrl == "localdebug")
                requestedUrl = "127.0.0.1:5000";
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
                    ConnectingInProgressEvent(new EventArgs());
                    if(jsonStatusResponse.Data.Accepts_Guests)
                    {
                        MainViewEntryArgs args = new MainViewEntryArgs();
                        args.ServerURL = requestedUrl;
                        args.Client = new Client("Guest", "negus@gue.st", "guest");
                        ConnectionInitedEvent(args);
                    } else
                    {
                        ClientTools.EndBusyCursor();
                        loginDialog = new LoginDialog();
                        bool? loginResult = loginDialog.ShowDialog();
                        if(!(loginResult.HasValue && loginResult.Value))
                        {
                            LoginRequired();
                        } else
                        {
                            try
                            {
                                jsonPathResponse = await client.GetFromJsonAsync<APIPath>($"{requestedUrl}path/user_get");
                            }
                            catch (HttpRequestException e)
                            {
                                ClientTools.EndBusyCursor();
                                event_args.ExceptionText = $"{e.Message}";
                                ExceptionOccuredEvent(event_args);
                                ReloadPageEvent(new EventArgs());
                                return;
                            }
                            string password = loginDialog.Password;
                            string email = loginDialog.Email;
                            string token = Utilities.ConstructToken(email, Utilities.HashSHA1(password));
                            UserGET jsonUserGetResponse = APITools.GetRequest<UserGET>(jsonPathResponse.Data.Path, token);
                            if(jsonUserGetResponse.Response == "success")
                            {
                                MainViewEntryArgs args = new MainViewEntryArgs();
                                args.ServerURL = requestedUrl;
                                args.Client = new Client(jsonUserGetResponse.Data.Name, email, password);
                                ConnectionInitedEvent(args);
                            } else
                            {
                                ClientTools.EndBusyCursor();
                                event_args.ExceptionText = "We were refused by the server, because the credentials were wrong!";
                                ExceptionOccuredEvent(event_args);
                                ReloadPageEvent(new EventArgs());
                                return;
                            }
                        }
                    }
                } else
                {
                    ClientTools.EndBusyCursor();
                    event_args.ExceptionText = "The server is currently not open or it just doesn't accept any clients.";
                    ExceptionOccuredEvent(event_args);
                    ReloadPageEvent(new EventArgs());
                }
            } else
            {
                ClientTools.EndBusyCursor();
                event_args.ExceptionText = "Everything was fine until I've reached out the server. He doesn't want us let in!";
                ExceptionOccuredEvent(event_args);
                ReloadPageEvent(new EventArgs());
            }
        }

        private void LoginRequired()
        {
            VisualExceptionArgs event_args = new VisualExceptionArgs();
            ClientTools.EndBusyCursor();
            loginDialog = new LoginDialog();
            loginDialog.ShowDialog();
            event_args.ExceptionText = "Login is required to access this server.";
            ExceptionOccuredEvent(event_args);
            ReloadPageEvent(new EventArgs());
        }
    }
}
