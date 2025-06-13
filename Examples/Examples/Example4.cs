using System;
using System.Net;
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
    public sealed unsafe class Example4
    {
        public static void StartServer(ushort port, string localIP = "0.0.0.0", bool ipv6 = true)
        {
            SocketError error;

            MnUdpPal.Initialize();

            Socket server = new Socket(ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            if (ipv6)
                server.DualMode = true;
            server.SendBufferSize = 256 * 1024;
            server.ReceiveBufferSize = 256 * 1024;

            MnSocketAddress listenAddress = new MnSocketAddress();
            listenAddress.Port = port;

            if (ipv6 && localIP == "0.0.0.0")
                localIP = "::0";

            if (MnUdpPal.SetHostName(ref listenAddress, localIP) == SocketError.Success)
                Console.WriteLine($"SocketAddress set! {listenAddress}");

            error = MnUdpPal.Bind(server, ref listenAddress);
            if (error == 0)
                Console.WriteLine("Socket bound!");
            else
                Console.WriteLine(error + " " + SocketPal.GetLastSocketError());

            if (MnUdpPal.SetNonBlocking(server, true) != SocketError.Success)
                Console.WriteLine("Non-blocking option error!");

            MnSocketAddress localAddress = new MnSocketAddress();
            error = MnUdpPal.GetAddress(server, ref localAddress);
            Console.WriteLine($"Server local: {error} {localAddress}");

            Span<byte> address = stackalloc byte[28];
            Span<byte> buffer = stackalloc byte[1024];

            error = MnUdpPal.GetHostName(ref localAddress, buffer);
            Console.WriteLine("Server HostName: " + Encoding.ASCII.GetString(buffer));

            Console.WriteLine();

            while (!Console.KeyAvailable)
            {
                if (MnUdpPal.Poll(server, 15, SelectMode.SelectRead))
                {
                    int dataLength;

                    while (true)
                    {
                        Span<byte> addressSnapshot = address;

                        if ((dataLength = server.ReceiveFromNonAlloc(buffer, ref addressSnapshot)) > 0)
                        {
                            string data = Encoding.UTF8.GetString(buffer.Slice(0, dataLength));

                            UdpPal.CreateIPEndPoint(addressSnapshot, out IPEndPoint? ipEndPoint);
                            Console.WriteLine($"Server received from {ipEndPoint}: " + data);

                            int bytes = Encoding.UTF8.GetBytes($"send back[{data}]", buffer);

                            server.SendToNonAlloc(buffer.Slice(0, bytes), addressSnapshot);
                        }
                        else
                            break;
                    }

                    Console.WriteLine();
                }

                Thread.Sleep(100);
            }

            server.Close();

            MnUdpPal.Cleanup();
        }

        public static void StartClient(string serverIP, ushort port, ushort localPort, bool ipv6 = false)
        {
            MnUdpPal.Initialize();

            MnSocket client = new Socket(ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            if (ipv6)
                client.SetDualMode(true);
            client.SetSendBufferSize(256 * 1024);
            client.SetReceiveBufferSize(256 * 1024);

            MnSocketAddress.CreateFromIPEndPoint(new IPEndPoint(IPAddress.Parse(serverIP), port), out MnSocketAddress connectionAddress);

            Console.WriteLine(connectionAddress);

            MnSocketAddress listenAddress = new MnSocketAddress();
            listenAddress.Port = localPort;

            if (MnUdpPal.SetIP(ref listenAddress, ipv6 ? "::0" : "0.0.0.0") == SocketError.Success)
                Console.WriteLine("SocketAddress set!");

            if (MnUdpPal.Bind(client, ref listenAddress) == 0)
                Console.WriteLine("Socket bound!");

            if (MnUdpPal.Connect(client, ref connectionAddress) == 0)
                Console.WriteLine("Socket connected!");
            else
                Console.WriteLine(SocketPal.GetLastSocketError() + " " + connectionAddress);

            if (MnUdpPal.SetNonBlocking(client, true) != SocketError.Success)
                Console.WriteLine("Non-blocking option error!");

            Console.WriteLine();

            byte* buffer = stackalloc byte[1024];

            int bytes = Encoding.UTF8.GetBytes("hello server.", MemoryMarshal.CreateSpan(ref *buffer, 1024));

            int a = MnUdpPal.Send(client, ref *buffer, bytes);

            byte i = 0;

            while (!Console.KeyAvailable)
            {
                bytes = Encoding.UTF8.GetBytes($"test send {i++}.", MemoryMarshal.CreateSpan(ref *buffer, 1024));

                MnUdpPal.Send(client, ref *buffer, bytes);

                if (MnUdpPal.Poll(client, 15, SelectMode.SelectRead))
                {
                    int dataLength;

                    while ((dataLength = MnUdpPal.Receive(client, ref *buffer, 1024)) > 0)
                    {
                        string data = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, dataLength));

                        Console.WriteLine($"Client received: " + data);
                    }

                    Console.WriteLine();
                }

                Thread.Sleep(1000);
            }

            MnUdpPal.Close(ref client);

            MnUdpPal.Cleanup();
        }
    }
}