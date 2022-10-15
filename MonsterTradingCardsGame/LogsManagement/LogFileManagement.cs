using System.IO;
using System.Text;
using Microsoft.VisualBasic;

namespace MonsterTradingCardsGame.LogsManagement
{
    public class LogFileManagement
    {

        private static string _fileName;

        public LogFileManagement()
        {
            _fileName = @"C:\Users\Nahash\source\repos\MonsterTradingCardsGame\MonsterTradingCardsGame\LogsManagement\BattleLogs\" + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".log";
            using (FileStream fs = File.Create(_fileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes($"New Battle on {DateTime.Now:yyyy-MM-dd hh:mm:ss}\n");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
        }

        public bool WriteLog(string strMessage)
        {
            try
            {                
                StringBuilder sb = new StringBuilder();
                sb.Append(strMessage);
                File.AppendAllText(_fileName, sb.ToString());
                sb.Clear();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public bool ReadLog()
        {
            try
            {
                using (StreamReader sr = File.OpenText(_fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
