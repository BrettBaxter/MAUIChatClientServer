/// By Henderson Bare & Brett Baxter
/// Created: 3/22/2023
/// Course: CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Henderson Bare - This work may not be copied for use in academic course work
/// 
/// I, Henderson Bare and Brett Baxter, certify that I wrote this code from scratch and did not copy it in part or in whole from another source.
/// All references used in the completion of the assignment are cited in my README files and commented in the code.

using Communications;
using FileLogger;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Xml.Linq;
using Windows.ApplicationModel.Calls;

namespace ChatClient
{
    /// <summary>
    /// The main page logic for the chat client.
    /// Includes functionality for:
    ///     1. Connect to server button
    ///     2. Disconnect from server button
    ///     3. Retrieve user list button
    ///     4. Server address entry
    ///     5. User name entry
    ///     6. Message entry
    /// Defines delegates for handling connection, disconnection, and message receiving.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private CustomFileLogProvider _fileLoggerProvider;
        private ILogger _fileLogger;
        private readonly ILogger<MainPage> _logger;
        private readonly Networking _client;

        // A constant arbitrary port # picked for this assignment.
        private const int _port = 11000;
        // The IP address to connect to.
        private string _address;

        /// <summary>
        /// Constructor. Initializes the GUI, sets up logger.
        /// Sets default address to the local standard.
        /// </summary>
        /// <param name="logger"></param>
        public MainPage(ILogger<MainPage> logger)
        {
            _logger = logger;
            _client = new Networking(logger, onConnect, onDisconnect, onMessage, '\n');
            InitializeComponent();
            _address = "127.0.0.1";
            _logger?.LogTrace("Main Page Constructor");
            _fileLoggerProvider = new CustomFileLogProvider();
            _fileLogger = _fileLoggerProvider.CreateLogger("Chat Client");
            _fileLogger?.Log(LogLevel.Trace, "Main Page Constructor");
        }
        /// <summary>
        /// Logs that the networking object has formed a connection to a TCP listener
        /// calls AwaitMessagesAsync for the client so that it is able to receive messages after connecting
        /// sends a message with "command name [NameEntry]" so that the server knows what to 
        /// call the networking object
        /// </summary>
        /// <param name="item"></param>
        public void onConnect(Networking item)
        {
            _logger?.LogDebug("Client onConnect method called.");
            _fileLogger?.Log(LogLevel.Debug, "Client onConnect method called.");
            _client.AwaitMessagesAsync(infinite: true);
            if (NameEntry.Text.Length > 0)
            {
                _client.Send("Command Name [" + NameEntry.Text + "]");
                item.ID = NameEntry.Text;
            } //letting the server know what to call the client
            //_client.Send("has connected to server" + "\r\n");
            if (item._tcpClient.Client.Connected) { Dispatcher.Dispatch(() => { ConnectionStatusLabel.Text = "Connected to: " + _address; }); }
        }

        /// <summary>
        /// Send a message notifying users of disconnection, reset the GUI connection status label.
        /// </summary>
        /// <param name="item"></param>
        public void onDisconnect(Networking item)
        {
            //item.Send(item.ID + " has disconnected from the server");
            _logger?.LogInformation(_client.ID + "Disconnected");
            _fileLogger?.Log(LogLevel.Information, _client.ID + "Disconnected");
            Dispatcher.Dispatch(() =>
            {
                ConnectionStatusLabel.Text = "";
            });
        }

        /// <summary>
        /// When a message is received this function is called.
        /// If the message is a command it will be handled differently.
        /// Messages will be added to the Message Log GUI element.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="message"></param>
        public void onMessage(Networking item, string message)
        {
            // A message like this will be received when the user requests a list of participants.
            // This updates the participants list.
            if (message.StartsWith("Command Participants,"))
            {
                Dispatcher.Dispatch(() =>
                {
                    UserListEntry.Text = "";
                });

                while (message.IndexOf('[') != -1)
                {
                    int index = message.IndexOf('[');
                    int index2 = message.IndexOf(']');
                    string name = message.Substring(index + 1, index2 - index - 1);
                    message = message.Remove(index, index2 - index + 1);
                    Dispatcher.Dispatch(() =>
                    {
                        UserListEntry.Text += name + Environment.NewLine;
                    });
                }
            }
            // If a name change command is sent back.
            else if (message.StartsWith("Command Name "))
            {
                _logger?.LogDebug("Command Name Called: " + _client.ID);
                _fileLogger?.Log(LogLevel.Debug, "Command Name Called: " + _client.ID);
                return;
            }
            // Any other normal message is added to the message log.
            else
            {
                Dispatcher.Dispatch(() => { MessageLogEntry.Text += item.ID + ": " + message + Environment.NewLine; });
                _logger?.LogDebug("Client onMessage called.");
                _fileLogger?.Log(LogLevel.Debug, "Client onMessage called.");
            }
        }

        /// <summary>
        /// When the connect button is clicked.
        /// Call connect on the networking object.
        /// Update the connection status indicator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectBtn_Clicked(object sender, EventArgs e)
        {
            _logger?.LogDebug("Connect Button Clicked, attempting connection to address: " + _address + " Port: " + _port);
            _fileLogger?.Log(LogLevel.Debug, "Connect Button Clicked, attempting connection to address: " + _address + " Port: " + _port);
            _client.Connect(_address, _port);
        }

        /// <summary>
        /// When the retrieve users button is clicked.
        /// Reset the user list text and then send a command to the server.
        /// This should trigger a response in onMessage and update the user list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UsersBtn_Clicked(object sender, EventArgs e)
        {
            bool answer = true;
            if (ConnectionStatusLabel.Text == "")
            {
                answer = await DisplayAlert("Not Connected", "Not connected to server, proceed anyway?", "Yes", "No");
            }
            if (answer)
            {
                _logger?.LogDebug("Retrieve Users button clicked");
                _fileLogger?.Log(LogLevel.Debug, "Retrieve Users button clicked");
                UserListEntry.Text = "";
                _client.Send("Command Participants");
            }
        }

        /// <summary>
        /// When the server name / address is entered.
        /// Set the address to whatever has been entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressEntry_Completed(object sender, EventArgs e)
        {
            _address = AddressEntry.Text;
            _logger?.LogInformation("Address set to: " + _address);
            _fileLogger?.Log(LogLevel.Information, "Address set to: " + _address);

        }

        /// <summary>
        /// When the address entry is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressEntry_Changed(object sender, TextChangedEventArgs e)
        {
            AddressEntry.Text = e.NewTextValue;
        }

        /// <summary>
        /// When the user name is entered.
        /// Set the ID to whatever has been entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameEntry_Completed(object sender, EventArgs e)
        {
            _client.ID = NameEntry.Text;
            _logger?.LogInformation("Name set to: " + _client.ID);
            _fileLogger?.Log(LogLevel.Information, "Name set to: " + _client.ID);
            if (_client != null && _client._tcpClient != null && _client._tcpClient.Connected)
            {
                _client.Send("Command Name [" + NameEntry.Text + "]");
            }
        }

        /// <summary>
        /// When the name entry is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameEntry_Changed(object sender, TextChangedEventArgs e)
        {
            NameEntry.Text = e.NewTextValue;
        }

        /// <summary>
        /// When the message is entered.
        /// Sends whatever has been entered to the server.
        /// This should call the onMessage method in the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MessageEntry_Completed(object sender, EventArgs e)
        {
            try
            {
                bool answer = true;
                if (ConnectionStatusLabel.Text == "")
                {
                    answer = await DisplayAlert("Not Connected", "Not connected to server, proceed anyway?", "Yes", "No");
                }
                if (answer)
                {
                    _client.Send(MessageEntry.Text);
                    _logger?.LogDebug("Message entry completed: sent: " + MessageEntry.Text);
                    _fileLogger?.Log(LogLevel.Debug, "Message entry completed: sent: " + MessageEntry.Text);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogDebug("Error: Message entry completed: Could not send message." + ex.Message);
                _fileLogger?.Log(LogLevel.Debug, "Error: Message entry completed: Could not send message." + ex.Message);
            }
        }

        /// <summary>
        /// When the message entry is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageEntry_Changed(object sender, TextChangedEventArgs e)
        {
            MessageEntry.Text = e.NewTextValue;
        }

        //// <summary>
        /// When the help button is pressed display a popup with information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuFlyoutItem_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Commands", "1. Command Name [YourNameHere] will set your username. 2. Command Participants will retrieve a list of users. These commands are case sensitive and should be entered exactly as they appear here.", "Ok");
        }
    }

}