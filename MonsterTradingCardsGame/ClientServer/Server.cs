using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;

using MonsterTradingCardsGame.ClientServer;


namespace MonsterTradingCardsGame.ClientServer
{
    internal class Server
    {
        public void Listen()
        {
            const int port = 10001;
            var localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener? server = null;

            try
            {

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                var bytes = new byte[256];
                string? data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                        
                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    using var client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    var stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine($"this: {data}");

                        // Process the data sent by the client.
                        var parse = new HttpParser(data);
                        var handleRequest = new RequestHandler();
                        handleRequest.HandleRequest(parse);

                        //data = data.ToUpper();

                        var msg = System.Text.Encoding.ASCII.GetBytes("data");

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e}");
            }
            finally
            {
                server?.Stop();
            }
        }
    }
}