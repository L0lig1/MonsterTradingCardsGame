using System.IO;
using System.Text;
using Microsoft.VisualBasic;

namespace MonsterTradingCardsGame.LogsManagement
{
    public class LogFileManagement
    {

        private static string _fileName = string.Empty;
        private string _log = $"New Battle on {DateTime.Now:yyyy-MM-dd hh:mm:ss}{Environment.NewLine}";

        public LogFileManagement()
        {
            _fileName = @"C:\Users\Nahash\source\repos\MonsterTradingCardsGame\MonsterTradingCardsGame\LogsManagement\BattleLogs\" + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".log";
            using var fs = File.Create(_fileName);
            var info = new UTF8Encoding(true).GetBytes($"New Battle on {DateTime.Now:yyyy-MM-dd hh:mm:ss}\n");
            // Add some information to the file.
            fs.Write(info, 0, info.Length);
        }

        public void Log(string toLog)
        {
            _log += toLog;
        }

        public string GetLog()
        {
            return _log;
        }

        public bool WriteLog(string strMessage)
        {
            try
            {                
                var sb = new StringBuilder();
                sb.Append(strMessage);
                File.AppendAllText(_fileName, sb.ToString());
                sb.Clear();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        public bool ReadLog()
        {
            try
            {
                using var sr = File.OpenText(_fileName);
                while (sr.ReadLine() is { } s)
                {
                    Console.WriteLine(s);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
