// See https://aka.ms/new-console-template for more information

using System.Reflection.Metadata;
using System;
using System.Net;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using CardInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MonsterTradingCardsGame.Stack;

namespace HelloWorld
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            var stack = new Stack();
            stack.CreateRandomStack();
            stack.PrintStack();
        }
        
    }
}

