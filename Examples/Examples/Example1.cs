using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NativeSockets.Udp;

// ReSharper disable ALL

namespace Examples
{
    // ipv6
    public sealed unsafe class Example1
    {
        public static void StartServer(ushort port, string localIP = "::0")
        {
            UdpPal6.Initialize();

            Socket6 server = UdpPal6.Create();
            server.SetDualMode(true);
            server.SetSendBufferSize(256 * 1024);
            server.SetReceiveBufferSize(256 * 1024);

            SocketAddress6 listenAddress = new SocketAddress6();
            listenAddress.Port = port;

            if (UdpPal6.SetIP(ref listenAddress, localIP) == SocketError.Success)
                Console.WriteLine("SocketAddress set!");

            if (UdpPal6.Bind(server, ref listenAddress) == 0)
                Console.WriteLine("Socket bound!");

            if (UdpPal6.SetNonBlocking(server, true) != SocketError.Success)
                Console.WriteLine("Non-blocking option error!");

            SocketAddress6 localAddress = new SocketAddress6();
            UdpPal6.GetAddress(server, ref localAddress);
            Console.WriteLine($"Server local: {localAddress}");

            SocketAddress6 address = new SocketAddress6();
            byte* buffer = stackalloc byte[1024];

            SocketError error = UdpPal6.GetHostName(ref localAddress, MemoryMarshal.CreateSpan(ref *buffer, 1024));
            Console.WriteLine("Server HostName: " + Encoding.ASCII.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, 1024)));

            Console.WriteLine();

            while (!Console.KeyAvailable)
            {
                if (UdpPal6.Poll(server, 15, SelectMode.SelectRead))
                {
                    int dataLength;

                    while ((dataLength = UdpPal6.ReceiveFrom(server, ref *buffer, 1024, ref address)) > 0)
                    {
                        string data = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, dataLength));

                        Console.WriteLine($"Server received from {address.ToString()}: " + data);

                        int bytes = Encoding.UTF8.GetBytes($"send back[{data}]", MemoryMarshal.CreateSpan(ref *buffer, 1024));

                        UdpPal6.SendTo(server, ref *buffer, bytes, ref address);
                    }

                    Console.WriteLine();
                }

                Thread.Sleep(100);
            }

            UdpPal6.Close(ref server);

            UdpPal6.Cleanup();
        }

        public static void StartClient(string serverIP, ushort port, ushort localPort)
        {
            UdpPal6.Initialize();

            Socket6 client = UdpPal6.Create();
            client.SetDualMode(true);
            client.SetSendBufferSize(256 * 1024);
            client.SetReceiveBufferSize(256 * 1024);

            SocketAddress6 connectionAddress = new SocketAddress6();

            connectionAddress.Port = port;

            SocketAddress6 listenAddress = new SocketAddress6();
            listenAddress.Port = localPort;

            if (UdpPal6.SetIP(ref listenAddress, "::0") == SocketError.Success)
                Console.WriteLine("SocketAddress set!");

            if (UdpPal6.Bind(client, ref listenAddress) == 0)
                Console.WriteLine("Socket bound!");

            if (UdpPal6.SetIP(ref connectionAddress, serverIP) == SocketError.Success)
                Console.WriteLine("SocketAddress set!");

            if (UdpPal6.Connect(client, ref connectionAddress) == 0)
                Console.WriteLine("Socket connected!");

            if (UdpPal6.SetNonBlocking(client, true) != SocketError.Success)
                Console.WriteLine("Non-blocking option error!");

            Console.WriteLine();

            byte* buffer = stackalloc byte[1024];

            int bytes = Encoding.UTF8.GetBytes("hello server.", MemoryMarshal.CreateSpan(ref *buffer, 1024));

            UdpPal6.Send(client, ref *buffer, bytes);

            byte i = 0;

            while (!Console.KeyAvailable)
            {
                bytes = Encoding.UTF8.GetBytes($"test send {i++}.", MemoryMarshal.CreateSpan(ref *buffer, 1024));

                UdpPal6.Send(client, ref *buffer, bytes);

                if (UdpPal6.Poll(client, 15, SelectMode.SelectRead))
                {
                    int dataLength;

                    while ((dataLength = UdpPal6.Receive(client, ref *buffer, 1024)) > 0)
                    {
                        string data = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, dataLength));

                        Console.WriteLine($"Client received: " + data);
                    }

                    Console.WriteLine();
                }

                Thread.Sleep(1000);
            }

            UdpPal6.Close(ref client);

            UdpPal6.Cleanup();
        }
    }
}