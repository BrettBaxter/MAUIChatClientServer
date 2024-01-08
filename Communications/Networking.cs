// By Brett Baxter & Henderson Bare
// Created: 3/22/2023
// CS 3500

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communications
{
    /// <summary>
    /// Author: Henderson Bare, Brett Baxter
    /// Date: March 22, 2023
    /// Course: CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Henderson Bare - This work may not be copied for use in academic course work
    /// 
    /// I, Henderson Bare and Brett Baxter, certify that I wrote this code from scratch and did not copy it in part or in whole from another source.
    /// All references used in the completion of the assignment are cited in my README files and commented in the code.
    /// 
    /// Networking object implements abstract class Communications. Uses TCP Client objects to send and receive data.
    /// </summary>
    public class Networking : Communications
    {
        // The Client Name.
        public string ID { get; set; }

        // Logger for logging.
        private readonly ILogger? _logger;

        // Delegates
        public delegate void ReportMessageArrived(Networking channel, string message); //callback delegate for when messages arrive
        public delegate void ReportDisconnect(Networking channel); //callback delegate for when a client disconnects
        public delegate void ReportConnectionEstablished(Networking channel); //callback delegate for when a client connects to a listener
        ReportMessageArrived onMessage;
        ReportDisconnect onDisconnect;
        ReportConnectionEstablished onConnect;

        // The Client Object.
        public TcpClient? _tcpClient { get; private set; }
        // The Listener Object for servers.
        TcpListener? _tcpListener;

        // Default /n.
        private readonly char _terminationCharacter;
        //cancellation token used to stop infinite processes
        private CancellationTokenSource _WaitForCancellation;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"> Logs various things throughout the class. </param>
        /// <param name="onConnect"></param>
        /// <param name="onDisconnect"></param>
        /// <param name="onMessage"></param>
        /// <param name="terminationCharacter"> What character determines when a message ends (by default \n) </param>
        public Networking(ILogger logger, ReportConnectionEstablished onConnect, ReportDisconnect onDisconnect, ReportMessageArrived onMessage, char terminationCharacter)
        {
            this._logger = logger;
            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
            this.onMessage = onMessage;
            this._terminationCharacter = terminationCharacter;
            _WaitForCancellation = new();
            _logger?.LogTrace("Networking constructor finished.");
        }

        /// <summary>
        /// Creates the TCPClient object using the input host and port.
        /// If the host or port is invalid it will catch the exception.
        /// </summary>
        /// <param name="host"> The host name. </param>
        /// <param name="port"> The port #. </param>
        public void Connect(string host, int port)
        {
            try
            {
                //if this object hasn't connected to anything, we create a new TcpClient based on the specified host and port
                //and assign ID accordingly. onConnect callback is then called on the Client Side
                if (_tcpClient is null)
                {
                    _tcpClient = new TcpClient(host, port);
                    ID = _tcpClient.Client.RemoteEndPoint.ToString();
                    onConnect(this);
                    _logger?.LogDebug("Networking Connect method successful.");
                }
            }
            //if theres an exception, noting happens and we log that an error occurred in the code.
            catch (Exception e)
            {
                _logger?.LogError(e, "Error unable to connect... Check host or port #.");
            }
        }

        /// <summary>
        /// If infinite is true, infinitely wait for data to come in over the TCP client.
        /// If infinite is false, wait for a single message.
        /// </summary>
        /// <param name="infinite"></param>
        public async void AwaitMessagesAsync(bool infinite)
        {
            try
            {
                //creates a backlog stringbuilder, array of bytes to take stuff off of the stream, then creates 
                //a NetworkStream between _tcpClient and the listener that it is connected to.
                StringBuilder dataBacklog = new StringBuilder();
                NetworkStream stream = _tcpClient.GetStream();

                if (stream == null)
                {
                    return;
                }
                using (stream)
                {

                    while (infinite)
                    {
                        byte[] buffer = new byte[4096];
                        //takes the data on the stream, gets the string representation and puts it in current data,
                        //appends to the backlog and logs that a message was received. 
                        int total = await stream.ReadAsync(buffer, _WaitForCancellation.Token);
                        _WaitForCancellation.Token.ThrowIfCancellationRequested();
                        if (total == 0)
                        {
                            // the connection quit unexpectedly
                            Disconnect();
                            throw new Exception("End of Stream Reached. Connection must be closed.");
                        }
                        string currentData = Encoding.UTF8.GetString(buffer, 0, total);
                        dataBacklog.Append(currentData);
                        onMessage(this, currentData); //callback delegate used
                        _logger?.LogDebug("AwaitMessagesAsync: Message received: " + currentData);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger?.LogError("Await messages was canceled");
            }
            catch (Exception e)
            {
                Disconnect();
                _logger?.LogError(e, "Error, unable to await messages.");
            }

        }

        /// <summary>
        /// For use by servers, continuously wait for clients to connect using a TCP Listener.
        /// Creates a TcpListener and starts it up. When a user connects create a networking object for it and call its
        /// onConnect method.
        /// </summary>
        /// <param name="port"> The port #. </param>
        /// <param name="infinite"> If infinite wait continuously for new clients. </param>
        public async void WaitForClients(int port, bool infinite)
        {
            try
            {
                //defines a TcpListener for the server
                _tcpListener = new TcpListener(port);
                while (infinite)
                {
                    _logger?.LogDebug("Waiting For Clients started.");
                    _tcpListener.Start();
                    //creates a new networking object with the received client and changes th eID
                    TcpClient connection = await _tcpListener.AcceptTcpClientAsync(_WaitForCancellation.Token);
                    var x = new Networking(_logger, onConnect, onDisconnect, onMessage, _terminationCharacter);
                    x._tcpClient = connection;
                    x.ID = x._tcpClient.Client.RemoteEndPoint.ToString();
                    //we want the new client to be waiting for messages so that it can receive them
                    x.AwaitMessagesAsync(infinite: true);
                    //lets the server know that a new connection has been created
                    onConnect(x);
                }

            }
            //waiting for clients caused an error, so we stop listening.
            catch (Exception e)
            {
                _tcpListener?.Stop();
                _logger?.LogError(e, "Error, unable to wait for clients.");
            }
        }

        /// <summary>
        /// Cancel WaitForClients().
        /// </summary>
        public void StopWaitingForClients()
        {
            _WaitForCancellation.Cancel();
            _logger?.LogDebug("Stopped waiting for clients.");
        }

        /// <summary>
        /// Closes the connection to the remote host using the stored TCP client object.
        /// </summary>
        public void Disconnect()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Close();
                onDisconnect(this);
            }
            _WaitForCancellation.Cancel();
            _logger?.LogDebug("Networking disconnect called.");
            _tcpClient = null;
        }

        /// <summary>
        /// Send text across the network using the TCP client.
        /// </summary>
        /// <param name="text"> The message to send. </param>
        public async void Send(string text)
        {
            try
            {
                if (_tcpClient.Connected)
                {
                    //replaces the instances of the termination character and adds one to the ends
                    text = text.Replace(_terminationCharacter, '\n') + _terminationCharacter;
                    StringBuilder dataBacklog = new StringBuilder();
                    byte[] buffer = new byte[4096];
                    NetworkStream stream = _tcpClient.GetStream();
                    buffer = Encoding.UTF8.GetBytes(text);
                    //writes the message to the stream established between the client and the listener
                    await stream.WriteAsync(buffer);
                    _logger?.LogInformation("Message Successfully sent: " + $"{text}");
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Error, unable to send message.");
            }
        }
    }
}
