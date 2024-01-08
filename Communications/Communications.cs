// By Brett Baxter & Henderson Bare
// Created: 3/22/2023
// CS 3500

using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;

namespace Communications
{
    /// <summary>
    /// Abstract class for networking objects. Uses TCP Client object.
    /// </summary>
    public abstract class Communications
    {
        /// <summary>
        /// Creates the TCPClient object using the input host and port.
        /// If the host or port is invalid it will catch the exception.
        /// </summary>
        /// <param name="host"> The host name. </param>
        /// <param name="port"> The port #. </param>
        public void Connect(string host, int port)
        {

        }

        /// <summary>
        /// If infinite is true, infinitely wait for data to come in over the TCP client.
        /// If infinite is false, wait for a single message.
        /// </summary>
        /// <param name="infinite"></param>
        public async void AwaitMessagesAsync(bool infinite)
        {

        }

        /// <summary>
        /// For use by servers, continuously wait for clients to connect using a TCP Listener.
        /// </summary>
        /// <param name="port"> The port #. </param>
        /// <param name="infinite"> If infinite wait continuously for new clients. </param>
        public async void WaitForClients(int port, bool infinite)
        {

        }

        /// <summary>
        /// Cancel WaitForClients().
        /// </summary>
        public void StopWaitingForClients()
        {

        }

        /// <summary>
        /// Closes the connection to the remote host using the stored TCP client object.
        /// </summary>
        public void Disconnect()
        {

        }

        /// <summary>
        /// Send text across the network using the TCP client.
        /// </summary>
        /// <param name="text"> The message to send. </param>
        public async void Send(string text)
        {

        }
    }
}
