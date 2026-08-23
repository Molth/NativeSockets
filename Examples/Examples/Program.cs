using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NativeSockets;

namespace Examples
{
    internal sealed class Program
    {
        private static void Main()
        {
            Console.WriteLine("Server + Client");
            Console.WriteLine();

            new Thread(ServerLoop) { IsBackground = true }.Start();

            new Thread(ClientLoop) { IsBackground = true }.Start();

            while (true)
            {
                ConsoleKeyInfo str = Console.ReadKey();
                if (str.Key == ConsoleKey.Backspace)
                    break;
            }
        }

        private static void ServerLoop()
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint serverEndpoint = new(IPAddress.Loopback, 12345);
            socket.Bind(serverEndpoint);

            Console.WriteLine($"[Server] Started: {serverEndpoint}");

            byte[] buffer = new byte[1024];

            NativeSocketAddress socketAddress = new NativeSocketAddress();

            try
            {
                while (true)
                {
                    int received = socket.ReceiveFromNonAlloc(buffer, ref socketAddress);
                    IPEndPoint remoteEndPoint = socketAddress.ToIpEndPoint();
                    string receivedText = Encoding.UTF8.GetString(buffer, 0, received);

                    Console.WriteLine($"[Server] Receive from: [{remoteEndPoint}]: [{receivedText}]");

                    string reply = $"[Server]: {receivedText}";
                    byte[] replyData = Encoding.UTF8.GetBytes(reply);
                    socket.SendToNonAlloc(replyData, socketAddress);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[Server]: {ex.Message}");
            }
        }

        private static void ClientLoop()
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));

            IPEndPoint localEndPoint = (IPEndPoint)socket.LocalEndPoint!;
            Console.WriteLine($"[Client] Started: {localEndPoint}");
            Console.WriteLine();

            byte[] receiveBuffer = new byte[1024];
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 12345);
            int counter = 1;

            NativeSocketAddress socketAddress = new NativeSocketAddress();
            NativeSocketAddress serverAddress = new NativeSocketAddress();
            serverAddress.SetIp(serverEndPoint);

            try
            {
                while (true)
                {
                    byte[] sendBuffer = Encoding.UTF8.GetBytes($"Hello world! {counter++}");

                    socket.SendToNonAlloc(sendBuffer, serverAddress);

                    socket.ReceiveTimeout = 2000;
                    try
                    {
                        int received = socket.ReceiveFromNonAlloc(receiveBuffer, ref socketAddress);
                        IPEndPoint remoteEndPoint = socketAddress.ToIpEndPoint();
                        string receivedText = Encoding.UTF8.GetString(receiveBuffer, 0, received);

                        Console.WriteLine($"[Client] Receive from: [{remoteEndPoint}]: [{receivedText}]");
                        Console.WriteLine();
                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        Console.WriteLine("[Client]: Timeout.");
                    }

                    Thread.Sleep(3000);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[Client]: {ex.Message}.");
            }
        }
    }
}