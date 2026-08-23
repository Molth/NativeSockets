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
                var str = Console.ReadKey();
                if (str.Key == ConsoleKey.Backspace)
                    break;
            }
        }

        private static void ServerLoop()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint serverEndpoint = new(IPAddress.Loopback, 12345);
            socket.Bind(serverEndpoint);

            Console.WriteLine($"[Server] Started: {serverEndpoint}");

            var buffer = new byte[1024];

            var socketAddress = new NativeSocketAddress();

            try
            {
                while (true)
                {
                    var received = socket.ReceiveFromNonAlloc(buffer, ref socketAddress);
                    var remoteEndPoint = socketAddress.ToIpEndPoint();
                    var receivedText = Encoding.UTF8.GetString(buffer, 0, received);

                    Console.WriteLine($"[Server] Receive from: [{remoteEndPoint}]: [{receivedText}]");

                    var reply = $"[Server]: {receivedText}";
                    var replyData = Encoding.UTF8.GetBytes(reply);
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
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));

            var localEndPoint = (IPEndPoint)socket.LocalEndPoint!;
            Console.WriteLine($"[Client] Started: {localEndPoint}");
            Console.WriteLine();

            var receiveBuffer = new byte[1024];
            var serverEndPoint = new IPEndPoint(IPAddress.Loopback, 12345);
            var counter = 1;

            var socketAddress = new NativeSocketAddress();
            var serverAddress = new NativeSocketAddress();
            serverAddress.SetIp(serverEndPoint);

            try
            {
                while (true)
                {
                    var sendBuffer = Encoding.UTF8.GetBytes($"Hello world! {counter++}");

                    socket.SendToNonAlloc(sendBuffer, serverAddress);

                    socket.ReceiveTimeout = 2000;
                    try
                    {
                        var received = socket.ReceiveFromNonAlloc(receiveBuffer, ref socketAddress);
                        var remoteEndPoint = socketAddress.ToIpEndPoint();
                        var receivedText = Encoding.UTF8.GetString(receiveBuffer, 0, received);

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