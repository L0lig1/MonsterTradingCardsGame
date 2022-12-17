using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;

using MonsterTradingCardsGame.ClientServer;


namespace MonsterTradingCardsGame.ClientServer
{
    internal class Server
    {
        public async Task Listen()
        {
            const int port = 10001;
            var localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener server = null;
            try
            {

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    using TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;
                    var parser = new HttpParser();

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        parser.Request += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);
                    
                        // Process the data sent by the client.
                        parser.ParseData();
                        var rh = new RequestHandler();
                        var response = rh.HandleRequest(parser);
                        
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                        
                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    //// Loop to receive all the data sent by the client.
                    //while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    //{
                    //    // Translate data bytes to a ASCII string.
                    //    parser.Request += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    //    Console.WriteLine("Received: {0}", data);
                    //
                    //}
                    //// Process the data sent by the client.
                    //parser.ParseData();
                    //var rh = new RequestHandler();
                    //rh.HandleRequest(parser);
                    //
                    //byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                    //
                    //// Send back a response.
                    //stream.Write(msg, 0, msg.Length);
                    //Console.WriteLine("Sent: {0}", data);
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