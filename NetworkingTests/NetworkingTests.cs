// By Brett Baxter & Henderson Bare
// Created: 3/22/2023
// CS 3500
using Communications;

namespace NetworkingTests
{
    /// <summary>
    /// Testing for the Networking class.
    /// </summary>
    [TestClass]
    public class NetworkingTests
    {
        /// <summary>
        /// Dummy onConnect delegate method
        /// </summary>
        /// <param name="item"></param>
        public void onConnect(Networking item)
        {

        }

        /// <summary>
        /// Dummy onDisconnect delegate method
        /// </summary>
        /// <param name="item"></param>
        public void onDisconnect(Networking item)
        {

        }

        /// <summary>
        /// Dummy onMessage delegate method
        /// </summary>
        /// <param name="item"></param>
        /// <param name="message"></param>
        public void onMessage(Networking item, string message)
        {

        }
        
        /// <summary>
        /// Makes sure the constructor can complete with no errors.
        /// Also tests whether or not a null logger is going to work.
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            Networking n = new Networking(null, onConnect, onDisconnect, onMessage, '\n');
        }

    }
}