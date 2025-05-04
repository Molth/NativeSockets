using System;
using System.Threading;

// ReSharper disable ALL

namespace Examples
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            StartExample1();
        }

        private static void StartExample1()
        {
            new Thread(() => { Example1.StartServer(7777); }) { IsBackground = true }.Start();

            Thread.Sleep(1000);

            new Thread(() => { Example1.StartClient("::1", 7777, 7778); }) { IsBackground = true }.Start();

            Console.ReadLine();
        }
    }
}