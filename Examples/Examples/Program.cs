using System;
using System.Threading;

// ReSharper disable ALL

namespace Examples
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            StartExample2();
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
    }
}