using System.Net;
using System.Net.Sockets;
using MonsterTradingCardsGame.ClientServer.Http;


namespace MonsterTradingCardsGame.ClientServer
{
    internal class Server : HttpParser
    {

        private const int Port = 10001;
        private static readonly IPAddress LocalAddr = IPAddress.Loopback; // localhost
        private readonly TcpListener _serverSocket = new (LocalAddr, Port);
        private readonly Dictionary<string, DateTime> _authorization = new();

        public void Start()
        {
            try
            {
                _serverSocket.Start();
                var rh = new RequestHandler();
                while (true)
                {
                    // Wait for a client to connect
                    Console.WriteLine("Waiting for a client to connect...");
                    var clientSocket = _serverSocket.AcceptTcpClient();

                    // Start a new thread to handle the client connection
                    var clientThread = new Thread(() => HandleClient(clientSocket, rh));
                    clientThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _serverSocket.Stop();
            }
        }

        private void HandleClient(object? ct, RequestHandler rh)
        {
            try
            {
                // Convert the object to a TcpClient
                if (ct == null) throw new Exception("Server error");
                var client = (TcpClient)ct;

                // Buffer for reading data
                const int bufferSize = 4096;
                var bytes = new byte[bufferSize];
                Console.WriteLine("Connected!");

                // Get a stream object for reading and writing
                var stream = client.GetStream();

                var i = 0;
                var recvData = " ";

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    recvData += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    // end zeichen (doesn't work when actual message == buffer size cz flag not init)
                    if (i is not 0 and < bufferSize)
                    {
                        break;
                    }
                }
                // Process the data sent by the client.
                
                var response = rh.HandleRequest(ParseHttpData(recvData), _authorization);
                object lockFlag = new();
                lock (lockFlag)
                {
                    var msg = System.Text.Encoding.ASCII.GetBytes(response.Header.GetResponse() + response.Body?.GetHttpBody());
                    
                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e}");
            }

        }

    }
}