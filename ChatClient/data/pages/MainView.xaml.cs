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
using SocketIOClient;
using Caliburn.Micro;

using ChatClient.Utils;
using ChatClient.Extensions;
using ChatClient.Items;

namespace ChatClient.data.pages
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Page
    {
        public event EventHandler ConnectionFailed;

        private SocketIO socket;
        public string ServerURL { get; set; }
        private Client ThisClient { get; set; }
        private string ChatEndpoint;
        private BindableCollection<ChatItem> ChatBubbles { get; set; }
        private List<ChatItem> _chatItems = new List<ChatItem>();

        private void ConnectionFailedEvent(VisualExceptionArgs e)
        {
            ConnectionFailed?.Invoke(this, e);
        }

        public MainView(string server_url, Client client)
        {
            ServerURL = server_url;
            ThisClient = client;
            InitializeComponent();
            ChatBubbles = new BindableCollection<ChatItem>(_chatItems);
            ChatBubblesBox.ItemsSource = ChatBubbles;
            InitiateConnection();
        }

        private async void InitiateConnection()
        {
            ChatEndpoint = $"{ServerURL}";
            socket = new SocketIO(ChatEndpoint);
            await socket.ConnectAsync();
            await socket.EmitAsync("join", new
            {
                ThisClient.Token
            });
            ConnectHandlers();
            LoadKeptMessages();
        }

        private void ConnectHandlers()
        {
            socket.On("joined", OnJoined);
            socket.On("leaved", OnLeft);
            socket.On("autherr", OnAuthErr);
            socket.On("conerr", OnConErr);
            socket.On("newmess", OnNewMess);
        }
        
        private void LoadKeptMessages()
        {

        }

        private void OnJoined(SocketIOResponse response)
        {
            var obj = response.GetValue();
            string user_name = obj.GetProperty("user_name").GetString();
            AppendMessage($"{user_name} has joined.");
        }

        private void OnLeft(SocketIOResponse response)
        {
            var obj = response.GetValue();
            string user_name = obj.GetProperty("user_name").GetString();
            AppendMessage($"{user_name} has left.");
        }

        private void OnNewMess(SocketIOResponse response)
        {
            var obj = response.GetValue();
            string message_content = obj.GetProperty("content").GetString();
            string sender = obj.GetProperty("sender").GetString();
            AppendMessage(message_content, sender);
        }

        private void OnAuthErr(SocketIOResponse response)
        {
            VisualExceptionArgs e_args = new VisualExceptionArgs();
            e_args.ExceptionText = "HEY! You can't just sit this server, you just can't! How did you even got there? *bonk*";
            ConnectionFailedEvent(e_args);
        }

        private void OnConErr(SocketIOResponse response)
        {
            VisualExceptionArgs e_args = new VisualExceptionArgs();
            e_args.ExceptionText = "You (we hope at least) are connected on this server on a different device";
            ConnectionFailedEvent(e_args);
        }

        private void AppendMessage(string raw_message, string sender="SYSTEM")
        {
            ChatItem chatItem = new ChatItem()
            {
                Text = raw_message,
                Sender = sender
            };
            ChatBubbles.Add(chatItem);
            BubblesScroll.ScrollToBottom();
        }

        private async void MessageToSendBox_BTBEnterDown(object sender, EventArgs e)
        {
            string message = MessageToSendBox.GetBTBValue();
            if(message.Length > 0)
            {
                MessageToSendBox.SetBTBValue("");
                await socket.EmitAsync("mess", new
                {
                    ThisClient.Token,
                    Message = message
                });
            }
        }
    }
}
