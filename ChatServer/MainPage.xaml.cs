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
using Microsoft.Maui.Networking;
using System.Text;

namespace ChatServer
{
    /// <summary>
    /// MainPage for chat server program
    /// Displays participants and allows the user to change the IP Address, Server name, and
    /// view messages sent across the server.
    /// Defines delegates onConnect, onDisconnect, and onMessage for the server side.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private CustomFileLogProvider _fileLoggerProvider;
        private ILogger _fileLogger;
        private readonly ILogger<MainPage> _logger;
        public Networking _server { get; private set; }

        // A constant arbitrary port # defined for the assignment.
        private const int _port = 11000;
        // A list of clients connected to the server.
        public List<Networking> _clients { get; private set; }

        /// <summary>
        /// Constructor more the mainpage that takes in a logger.
        /// Creates a server networking object and immediately stars waiting
        /// for clients.
        /// </summary>
        /// <param name="logger"></param>
        public MainPage(ILogger<MainPage> logger)
        {
            InitializeComponent();
            _logger = logger;
            _clients = new();
            _server = new Networking(logger, onConnect, onDisconnect, onMessage, Environment.NewLine.ToCharArray()[0]);
            _server.WaitForClients(_port, true);
            _logger?.LogTrace("Main Page Constructor");
            _fileLoggerProvider = new CustomFileLogProvider();
            _fileLogger = _fileLoggerProvider.CreateLogger("Chat Server");
            _fileLogger?.Log(LogLevel.Trace, "Main Page Constructor");
        }

        /// <summary>
        /// When a connection happens add the client to the list.
        /// Add a message to the log notifying of a connection.
        /// Add the clients ID to the participants list.
        /// </summary>
        /// <param name="item"></param>
        public void onConnect(Networking item)
        {
            lock (this._clients)
            {
                _clients.Add(item);
            }
            Dispatcher.Dispatch(() =>
            {
                //sending a message from the client instead to see if the names will work properly
                ConsoleLog.Text += item.ID + " has connected to server" + Environment.NewLine;
                Participants.Text += item.ID;
            });
            _logger?.LogDebug("Server onConnect called.");
            _fileLogger?.Log(LogLevel.Debug, "Server onConnect called.");
        }

        /// <summary>
        /// When a disconnection happens, remove the client from the list.
        /// Add a message to the log notifying of a disconnection.
        /// Remove the clients ID from the participants list.
        /// </summary>
        /// <param name="item"></param>
        public void onDisconnect(Networking item)
        {
            _clients.Remove(item);
            _logger?.LogDebug("Server client count: " + _clients.Count);
            _fileLogger?.Log(LogLevel.Debug, "Server client count: " + _clients.Count);
            _logger?.LogDebug("Server onDisconnect called.");
            _fileLogger?.Log(LogLevel.Debug, "Server onDisconnect called.");
            Dispatcher.Dispatch(() =>
            {
                ConsoleLog.Text += item.ID + " has disconnected from the server." + Environment.NewLine;
            });

            UpdateParticipants();
        }
        /// <summary>
        /// Handles the 2 types of commands that can be received by the server. If it is not one of
        /// these 3 commands, then it sends out the message to every client so that the message will be displayed 
        /// on the message board, along with the ID of the person who sent it on each client and the server GUIs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="message"></param>
        public void onMessage(Networking item, string message)
        {
            if (message.StartsWith("Command Name "))
            {
                //changes the id of the networking item to the text withing the square brackets of the command
                item.ID = message.Substring(message.LastIndexOf("[") + 1, message.LastIndexOf("]") - message.LastIndexOf("[") - 1);
                _logger?.LogDebug("Command Name Received by server, New ID: " + item.ID);
                _fileLogger?.Log(LogLevel.Debug, "Command Name Received by server, New ID: " + item.ID);
                UpdateParticipants();
                return;
            }
            // When the Command Participants message command is received:
            // send a message back to the client with a list of clients for use in the user list element.
            else if (message.StartsWith("Command Participants"))
            {
                StringBuilder participants = new StringBuilder();
                participants.Append("Command Participants,");
                foreach (Networking client in _clients)
                {
                    participants.Append("[" + client.ID + "],");
                }
                item.Send(participants.ToString());
                _logger?.LogDebug("Command Participants Received and Sent.");
                _fileLogger?.Log(LogLevel.Debug, "Command Participants Received and Sent.");
                return;
            }
            //appends the ID and the message sent to the log for the server
            Dispatcher.Dispatch(() =>
            {
                ConsoleLog.Text += item.ID + ": " + message;
            });
            //sends the message back to every client so that the message is displayed on their end. 
            foreach (Networking client in _clients)
            {
                client.Send(message);
                _logger?.LogDebug("Message sent to: " + client.ID);
                _fileLogger?.Log(LogLevel.Debug, "Message sent to: " + client.ID);
            }
            _logger?.LogDebug("Server onMessage called.");
            _fileLogger?.Log(LogLevel.Debug, "Server onMessage called.");
        }

        /// <summary>
        /// Used when the "Shutdown Server" button is clicked
        /// If there are clients connected, loop through all of them and call disconnect,
        /// then clear the list and stop waiting for clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShutdownButton_Clicked(object sender, EventArgs e)
        {
            _logger?.LogDebug("Shutdown Server Button Clicked");
            _fileLogger?.Log(LogLevel.Debug, "Shutdown Server Button Clicked");
            // Code to handle button clicked
            if (_clients.Count > 0)
            {
                _logger?.LogDebug("Client count: " + _clients.Count);
                _fileLogger?.Log(LogLevel.Debug, "Client count: " + _clients.Count);
                List<Networking> clientsCopy = _clients.ToList();
                foreach (Networking client in clientsCopy)
                {
                    client.Disconnect();
                }
                _clients.Clear();
            }
            _server.StopWaitingForClients();
            UpdateParticipants();
            //Application.Current.Quit();
        }
        /// <summary>
        /// Used when the IP Address is changed by the user.
        /// Set what has been entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IPAddress_Changed(object sender, TextChangedEventArgs e)
        {
            _logger?.LogInformation("IP Address changed");
            _fileLogger?.Log(LogLevel.Information, "IP Address changed");
            Dispatcher.Dispatch(() =>
            {
                IPAddress.Text = e.NewTextValue;
            });
            // Code to handle IPAddress entry changed
        }
        /// <summary>
        /// Used when the name of the server is changed by the user
        /// Updates the value of the ServerName entry to the new value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _logger?.LogInformation("Server Name Changed");
            _fileLogger?.Log(LogLevel.Information, "Server Name Changed");
            Dispatcher.Dispatch(() =>
            {
                ServerName.Text = e.NewTextValue;
            });
            // Code to handle Server Name entry changed
        }

        /// <summary>
        /// Clears the participants list UI element and then loops through
        /// each client and adds the ID to the list.
        /// </summary>
        private void UpdateParticipants()
        {
            Dispatcher.Dispatch(() =>
            {
                Participants.Text = "";
            });
            foreach (Networking client in _clients)
            {
                Dispatcher.Dispatch(() =>
                {
                    Participants.Text += client.ID + " : " + client._tcpClient.Client.RemoteEndPoint + Environment.NewLine;
                });
            }
            _logger?.LogDebug("Participants List Updated");
            _fileLogger?.Log(LogLevel.Debug, "Participants List Updated");
        }

    }
}