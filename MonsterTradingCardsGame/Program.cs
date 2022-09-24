// See https://aka.ms/new-console-template for more information

using System.Reflection.Metadata;
using System;
using System.Net;
using user;
using CardInterface;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var me = new User();
            me.Name = "BRUH";
            Console.WriteLine(me.Name);
            var user = new Card
            {
                Name = "Hallo!"
            };
            user.ElementType = 1;
            user.ElementType = 5;
            Console.WriteLine(user.ElementType);
            Console.WriteLine(user.Name);
        }
    }
}

