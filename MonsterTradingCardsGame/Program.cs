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

namespace HelloWorld
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ReadJsonFile("C:\\Users\\Nahash\\source\\repos\\MonsterTradingCardsGame\\MonsterTradingCardsGame\\Card\\CardData.json");
        }
        public static void ReadJsonFile(string jsonFileIn)
        {
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(jsonFileIn));
            
            Console.WriteLine(jsonFile["fire"][0][0]["name"]);
        }
        
    }
}

