using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NativeSockets.Udp;
using Socket = NativeSockets.Udp.Socket;

// ReSharper disable ALL

namespace Examples
{
    // unsafe
    public sealed unsafe class Example1
    {
        public static void StartServer(ushort port, string localIP = "::0")
        {
            UdpPal.Initialize();

            Socket server = UdpPal.Create();
            server.SetSendBufferSize(256 * 1024);
            server.SetReceiveBufferSize(256 * 1024);

            SocketAddress listenAddress = new SocketAddress();
            listenAddress.Port = port;

            if (UdpPal.SetIP(ref listenAddress, localIP) == SocketError.Success)
                Console.WriteLine("SocketAddress set!");

            if (UdpPal.Bind(server, ref listenAddress) == 0)
                Console.WriteLine("Socket bound!");

            if (UdpPal.SetNonBlocking(server, true) != SocketError.Success)
                Console.WriteLine("Non-blocking option error!");

            SocketAddress localAddress = new SocketAddress();
            UdpPal.GetAddress(server, ref localAddress);
            Console.WriteLine($"Server local: {localAddress}");

            SocketAddress address = new SocketAddress();
            byte* buffer = stackalloc byte[1024];

            SocketError error = UdpPal.GetHostName(ref localAddress, MemoryMarshal.CreateSpan(ref *buffer, 1024));
            Console.WriteLine("Server HostName: " + Encoding.ASCII.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, 1024)));

            Console.WriteLine();

            while (!Console.KeyAvailable)
            {
                if (UdpPal.Poll(server, 15, SelectMode.SelectRead))
                {
                    int dataLength;

                    while ((dataLength = UdpPal.ReceiveFrom(server, ref *buffer, 1024, ref address)) > 0)
                    {
                        string data = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, dataLength));

                        Console.WriteLine($"Server received from {address.ToString()}: " + data);

                        int bytes = Encoding.UTF8.GetBytes($"send back[{data}]", MemoryMarshal.CreateSpan(ref *buffer, 1024));

                        UdpPal.SendTo(server, ref *buffer, bytes, ref address);
                    }

                    Console.WriteLine();
                }

                Thread.Sleep(100);
            }

            UdpPal.Close(ref server);

            UdpPal.Cleanup();
        }

        public static void StartClient(string serverIP, ushort port, ushort localPort)
        {
            UdpPal.Initialize();

            Socket client = UdpPal.Create();
            client.SetSendBufferSize(256 * 1024);
            client.SetReceiveBufferSize(256 * 1024);

            SocketAddress connectionAddress = new SocketAddress();

            connectionAddress.Port = port;

            SocketAddress listenAddress = new SocketAddress();
            listenAddress.Port = localPort;

            if (UdpPal.SetIP(ref listenAddress, "::0") == SocketError.Success)
                Console.WriteLine("SocketAddress set!");

            if (UdpPal.Bind(client, ref listenAddress) == 0)
                Console.WriteLine("Socket bound!");

            if (UdpPal.SetIP(ref connectionAddress, serverIP) == SocketError.Success)
                Console.WriteLine("SocketAddress set!");

            if (UdpPal.Connect(client, ref connectionAddress) == 0)
                Console.WriteLine("Socket connected!");

            if (UdpPal.SetNonBlocking(client, true) != SocketError.Success)
                Console.WriteLine("Non-blocking option error!");

            Console.WriteLine();

            byte* buffer = stackalloc byte[1024];

            int bytes = Encoding.UTF8.GetBytes("hello server.", MemoryMarshal.CreateSpan(ref *buffer, 1024));

            UdpPal.Send(client, ref *buffer, bytes);

            byte i = 0;

            while (!Console.KeyAvailable)
            {
                bytes = Encoding.UTF8.GetBytes($"test send {i++}.", MemoryMarshal.CreateSpan(ref *buffer, 1024));

                UdpPal.Send(client, ref *buffer, bytes);

                if (UdpPal.Poll(client, 15, SelectMode.SelectRead))
                {
                    int dataLength;

                    while ((dataLength = UdpPal.Receive(client, ref *buffer, 1024)) > 0)
                    {
                        string data = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, dataLength));

                        Console.WriteLine($"Client received: " + data);
                    }

                    Console.WriteLine();
                }

                Thread.Sleep(1000);
            }

            UdpPal.Close(ref client);

            UdpPal.Cleanup();
        }
    }
}