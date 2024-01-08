```
Authors:    Brett Baxter & Henderson Bare
Date:       3/22/2023
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  BrettBaxter1 & hendersonBare
Repo:       https://github.com/uofu-cs3500-spring23/assignment-seven---chatting-bretthenderson.git
Date:       3/22/2023
Solution:   Networking and Logging
Copyright:  CS 3500, Henderson Bare, and Brett Baxter - This work may not be copied for use in Academic Coursework.
```

# Overview of the Chat Functionality

    Two GUIs: Server and Client.
    Networking object: For use by servers and clients using Tcp objects. Combines the functionality of both the 
    client-side and server-side to create one class that fully encapsulates all interaction between the client and server. 
    Chat client: has fields for the name of the client (how it is identified by the server), The IP address of the server it will
    attempt to connect to, and a field that allows for the sending of messages. These messages, IPs, and names are changed after
    enter key is pressed on the keyboard. The message box on the bottom left displays the ID of the client who sends a message
    followed by the message that was sent. The message box on the bottom right displays the ID of the clients who are connected to the
    server, when requested. The connect button tries to connect to the server on the specified IP address.

    Chat server: Has 2 editable text entries that allow for the user to change the name and the IP address of the server.
    The message box on the bottom right displays the ID of the client who sends a message followed by the message that was sent.
    the text box on the bottom left displays who is connected to the server. The shutdown server button disconnects all clients 
    from the server, then shuts down the server so that clients can no longer connect to it. 

    

# Time Expenditures:

     Predicted Hours:    11              Actual Hours:   12 (+ 1.5 hours for each of us individually)
     Henderson Time spent individually: 1.5 hours
     Brett Time spent individually:     1.5 hours

#Partnership Information

     The majority of our programming was done collaboratively using pair programming principles with discord and visual studio live-share. 
     However, we did the GUIs individually with Henderson doing the server GUI and Brett doing the client GUI. 

# Branching

    Branches ClientGUI and ServerGUI were the two branches created when we worked on the GUIs individually. Other than that, no other branches besides
    main were utilized or pushed to.

#Testing

    Due to the fact that a test suite was not required for this assignment, we did not write any tests. However, we did test our code by running the server and client,
    and placing breakpoints as needed to see how certain methods functioned individually. 

# Comments to Evaluators:

     Initially, the server states the ID of the remote endpoint of the client that has joined, because do to the order in which
     methods are called, the server initiates the connection before it is able to receive a name for the client, thus displaying
     the remote endpoint. I discussed this issue with one of the TAs and they said that we would still get full credit because
     everything else functions as intended. 

     Combining the server and client functionality into the same networking object is  a weird choice. Encountered many difficulties with GUI, solving a problem with it
     always led to finding something else wrong. It was hard to see the separation of concerns initially, but after some time we developed a conceptual understanding
     of how our code is expected to behave.

# Assignment Writeups:

     N/A 

# Examples of Good Software Practice (GSP):

     1. DRY: In the server code we added a private helper method to update the user list.
     2. Made often and descriptive GitHub pushes with explanatory commit messages to better track the production of the assignment.
     3. Used pair programming principles to work collaboratively on the assignment
     4. Commented and added XML documentation as we progressed through the assignment to improve debugging and readability. 

# Peers/References:

# Consulted Peers:

    N/A

# References

    Assignment Seven:
        1. https://stackoverflow.com/questions/26761729/is-there-a-way-to-use-using-but-leave-file-open: Showed me how to use StreamWriter for logging files.
        2. How to close a TcpClient cleanly so the server reads end of stream instead of throwing a System.IO.IOException - https://stackoverflow.com/questions/73709861/how-to-close-a-tcpclient-cleanly-so-the-server-reads-end-of-stream-instead-of-th
        3. TcpClient Class - https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=net-7.0
