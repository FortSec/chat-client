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

using ChatClient.Utils;

namespace ChatClient.data.pages
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Page
    {
        private SocketIO socket;
        public string ServerURL { get; set; }
        private string ChatEndpoint;

        public MainView(string server_url)
        {
            ServerURL = server_url;
            InitializeComponent();
            InitiateConnection();
        }

        private async void InitiateConnection()
        {
            ChatEndpoint = $"{ServerURL}";
            socket = new SocketIO(ChatEndpoint);
            await socket.ConnectAsync();
            socket.OnConnected += async (sender, e) =>
            {
                await socket.EmitAsync("join", new
                {
                    
                });
            };
            LoadKeptMessages();
        }

        private void LoadKeptMessages()
        {

        }

        private async void SendMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            //string message = MainMessageBox.Text;
            //await socket.EmitAsync("message", message);
        }
    }
}
