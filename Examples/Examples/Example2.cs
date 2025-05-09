using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NativeSockets;
using NativeSockets.Udp;

// ReSharper disable ALL

namespace Examples
{
    // ipv4
    public sealed unsafe class Example2
    {
        public static void StartServer(ushort port, string localIP = "0.0.0.0")
        {
            SocketError error;

            UdpPal4.Initialize();

            Socket4 server = UdpPal4.Create();
            server.SetSendBufferSize(256 * 1024);
            server.SetReceiveBufferSize(256 * 1024);

            SocketAddress4 listenAddress = new SocketAddress4();
            listenAddress.Port = port;

            if (UdpPal4.SetIP(ref listenAddress, localIP) == SocketError.Success)
                Console.WriteLine($"SocketAddress set! {listenAddress}");

            error = UdpPal4.Bind(server, ref listenAddress);
            if (error == 0)
                Console.WriteLine("Socket bound!");
            else
                Console.WriteLine(error + " " + SocketPal.GetLastSocketError());

            if (UdpPal4.SetNonBlocking(server, true) != SocketError.Success)
                Console.WriteLine("Non-blocking option error!");

            SocketAddress4 localAddress = new SocketAddress4();
            error = UdpPal4.GetAddress(server, ref localAddress);
            Console.WriteLine($"Server local: {error} {localAddress}");

            SocketAddress4 address = new SocketAddress4();
            byte* buffer = stackalloc byte[1024];

            error = UdpPal4.GetHostName(ref localAddress, MemoryMarshal.CreateSpan(ref *buffer, 1024));
            Console.WriteLine("Server HostName: " + Encoding.ASCII.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, 1024)));

            Console.WriteLine();

            while (!Console.KeyAvailable)
            {
                if (UdpPal4.Poll(server, 15, SelectMode.SelectRead))
                {
                    int dataLength;

                    while ((dataLength = UdpPal4.ReceiveFrom(server, ref *buffer, 1024, ref address)) > 0)
                    {
                        string data = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, dataLength));

                        Console.WriteLine($"Server received from {address.ToString()}: " + data);

                        int bytes = Encoding.UTF8.GetBytes($"send back[{data}]", MemoryMarshal.CreateSpan(ref *buffer, 1024));

                        UdpPal4.SendTo(server, ref *buffer, bytes, ref address);
                    }

                    Console.WriteLine();
                }

                Thread.Sleep(100);
            }

            UdpPal4.Close(ref server);

            UdpPal4.Cleanup();
        }

        public static void StartClient(string serverIP, ushort port, ushort localPort)
        {
            UdpPal4.Initialize();

            Socket4 client = UdpPal4.Create();
            client.SetSendBufferSize(256 * 1024);
            client.SetReceiveBufferSize(256 * 1024);

            SocketAddress4 connectionAddress = new SocketAddress4();

            connectionAddress.Port = port;

            SocketAddress4 listenAddress = new SocketAddress4();
            listenAddress.Port = localPort;

            if (UdpPal4.SetIP(ref listenAddress, "0.0.0.0") == SocketError.Success)
                Console.WriteLine("SocketAddress set!");

            if (UdpPal4.Bind(client, ref listenAddress) == 0)
                Console.WriteLine("Socket bound!");

            if (UdpPal4.SetIP(ref connectionAddress, serverIP) == SocketError.Success)
                Console.WriteLine("SocketAddress set!");

            if (UdpPal4.Connect(client, ref connectionAddress) == 0)
                Console.WriteLine("Socket connected!");

            if (UdpPal4.SetNonBlocking(client, true) != SocketError.Success)
                Console.WriteLine("Non-blocking option error!");

            Console.WriteLine();

            byte* buffer = stackalloc byte[1024];

            int bytes = Encoding.UTF8.GetBytes("hello server.", MemoryMarshal.CreateSpan(ref *buffer, 1024));

            var a = UdpPal4.Send(client, ref *buffer, bytes);

            byte i = 0;

            while (!Console.KeyAvailable)
            {
                bytes = Encoding.UTF8.GetBytes($"test send {i++}.", MemoryMarshal.CreateSpan(ref *buffer, 1024));

                UdpPal4.Send(client, ref *buffer, bytes);

                if (UdpPal4.Poll(client, 15, SelectMode.SelectRead))
                {
                    int dataLength;

                    while ((dataLength = UdpPal4.Receive(client, ref *buffer, 1024)) > 0)
                    {
                        string data = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, dataLength));

                        Console.WriteLine($"Client received: " + data);
                    }

                    Console.WriteLine();
                }

                Thread.Sleep(1000);
            }

            UdpPal4.Close(ref client);

            UdpPal4.Cleanup();
        }
    }
}