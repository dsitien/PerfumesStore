using Microsoft.AspNetCore.SignalR.Client;
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

namespace ChatWithAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HubConnection connection;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSignalR();
        }

        private async void InitializeSignalR()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7004/chathub")
                .Build();

            connection.On<string, string, string>("ReceiveMessage", (user, message, timestamp) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{timestamp} {user}: {message}";
                    ChatListBox.Items.Add(newMessage);
                });
            });

            try
            {
                await connection.StartAsync();
                ChatListBox.Items.Add("Connection started");
            }
            catch (Exception ex)
            {
                ChatListBox.Items.Add($"Error starting connection: {ex.Message}");
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                await connection.InvokeAsync("SendMessage", "Admin", MessageInput.Text);
                MessageInput.Clear();
            }
            else
            {
                ChatListBox.Items.Add("Connection not established");
            }
        }
    }
}