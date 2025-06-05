using System;
using System.Threading;
using NativeSockets.Udp;

// ReSharper disable ALL

namespace Examples
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            new Thread(() => { Example4.StartServer(7777); }) { IsBackground = true }.Start();

            Thread.Sleep(1000);

            new Thread(() => { Example4.StartClient("127.0.0.1", 7777, 7778); }) { IsBackground = true }.Start();

            Console.ReadLine();
        }

        private static void Test()
        {
            MnSocketAddress.CreateFromIP("127.0.0.1", out MnSocketAddress address);
            address.Port = 9999;
            Console.WriteLine(((SocketAddress6)address).CreateSocketAddress());
            Console.WriteLine(((SocketAddress6)address).CreateIPEndPoint().Serialize());
            Console.WriteLine(((SocketAddress6)address).CreateSocketAddress().Equals(((SocketAddress6)address).CreateIPEndPoint().Serialize()));
            MnSocketAddress.CreateFromSocketAddress(((SocketAddress6)address).CreateIPEndPoint().Serialize(), out MnSocketAddress a);
            Console.WriteLine(a.CreateIPEndPoint(true));
        }

        private static void StartExample1()
        {
            new Thread(() => { Example1.StartServer(7777); }) { IsBackground = true }.Start();

            Thread.Sleep(1000);

            new Thread(() => { Example1.StartClient("::1", 7777, 7778); }) { IsBackground = true }.Start();

            Console.ReadLine();
        }

        private static void StartExample2()
        {
            new Thread(() => { Example2.StartServer(7800); }) { IsBackground = true }.Start();

            Thread.Sleep(1000);

            new Thread(() => { Example2.StartClient("127.0.0.1", 7800, 7801); }) { IsBackground = true }.Start();

            Console.ReadLine();
        }

        private static void StartExample3()
        {
            new Thread(() => { Example2.StartServer(7777); }) { IsBackground = true }.Start();

            Thread.Sleep(1000);

            new Thread(() => { Example1.StartClient("127.0.0.1", 7777, 7778); }) { IsBackground = true }.Start();

            Console.ReadLine();
        }

        private static void StartExample4()
        {
            new Thread(() => { Example1.StartServer(7777); }) { IsBackground = true }.Start();

            Thread.Sleep(1000);

            new Thread(() => { Example2.StartClient("127.0.0.1", 7777, 7778); }) { IsBackground = true }.Start();

            Console.ReadLine();
        }
    }
}