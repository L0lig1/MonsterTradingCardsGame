using HttpResponseHeader = MonsterTradingCardsGame.ClientServer.Http.Response.HttpResponseHeader;
using MonsterTradingCardsGame.ClientServer.Http.Response;
using MonsterTradingCardsGame.ClientServer.Http;
using MonsterTradingCardsGame.Authorization;
using System.Net.Sockets;
using System.Net;


namespace MonsterTradingCardsGame.Server
{
    internal class Server : HttpParser
    {
        private const int Port = 10001;
        private static readonly IPAddress LocalAddr = IPAddress.Loopback; // localhost
        private readonly TcpListener _serverSocket = new (LocalAddr, Port);
        private readonly AuthorizationHandler _authorization = new();

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

                Console.WriteLine("Connected!");
                
                var stream = client.GetStream();

                // Process the data sent by the client.
                var request = GetRequest(stream);
                var parsedRequest = ParseHttpData(request);
                if (parsedRequest.IsValid)
                {
                    var response = rh.HandleRequest(parsedRequest, _authorization);
                    WriteResponse(response, stream);
                }
                else
                {
                    WriteResponse(new HttpResponse{Header = new HttpResponseHeader(HttpStatusCode.BadRequest, "text/plain", 16), Body = new HttpResponseBody("Invalid Request!")}, stream);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e}");
            }
        }

        private string GetRequest(Stream stream)
        {
            var i = 0;
            var recvData = " ";
            const int bufferSize = 4096;
            var bytes = new byte[bufferSize];
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
            return recvData;
        }

        private void WriteResponse(HttpResponse response, NetworkStream stream)
        {
            object lockFlag = new();
            lock (lockFlag)
            {
                var msg = System.Text.Encoding.ASCII.GetBytes(response.Header.GetResponse() + response.Body?.GetHttpBody());

                // Send back a response.
                stream.Write(msg, 0, msg.Length);
            }
        }

    }
}