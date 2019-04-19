using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NDesk.Options;
/*using VRChatApi;
using VRChatApi.Classes;
using VRChatApi.Endpoints;*/

namespace VRChatLogReader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "VRChat Log Reader";
            DirectoryInfo logDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "vrchat"));
            var lastLog = logDir.GetFiles().OrderByDescending(f => f.LastWriteTime).First().FullName;
            //ClearLog(file);
            /*
            VRChatApi.VRChatApi api = new VRChatApi.VRChatApi("", "");
            System.Console.WriteLine(api);
            System.Console.WriteLine("Logging in...");
            UserResponse user = await api.UserApi.Login();
            System.Console.WriteLine("Logged in as {0}", user.username);
            */
            bool joins_only = false;

            var p = new OptionSet() { { "j|joins|joinsonly", "Only show joins/leaves", v => joins_only = v != null }
            };
            List<string> extra;
            extra = p.Parse(args);

            var wh = new AutoResetEvent(false);
            var fsw = new FileSystemWatcher(".");
            fsw.Filter = lastLog;
            fsw.EnableRaisingEvents = true;
            fsw.Changed += (s, e) => wh.Set();

            var fs = new FileStream(fsw.Filter, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            //UInt64 i = 0;
            using (var sr = new StreamReader(fs))
            {
                var s = "";
                while (true)
                {
                    s = sr.ReadLine();
                    if (s != null && !string.IsNullOrWhiteSpace(s)) {
                        if (joins_only && (!s.Contains("OnPlayerJoined") && !s.Contains("OnPlayerLeft") && !s.Contains("OnJoinedRoom") && !s.Contains("OnLeftRoom"))) continue;
                        /*
                        LogLine line = ParseLine(s);
                        if (joins_only)
                        {
                            if (line.Logger != Logger.NetworkManager) continue;
                            if (line.EventType != EventType.OnPlayerJoined && line.EventType != EventType.OnPlayerLeft &&
                                line.EventType != EventType.OnJoinedRoom && line.EventType != EventType.OnLeftRoom) continue;
                        }
                        Console.WriteLine(@"<{0}> [{1}] {2} ({3}): {4}", line.DateTime.ToString(), line.Category.ToString(), line.Logger.ToString(), line.EventType.ToString(), line.Message);
                        */
                        Console.WriteLine(s);
                        //i += 1;
                    } else {
                        wh.WaitOne(50);
                    }
                }
            }
            wh.Close();
        }
        public static LogLine ParseLine(string line)
        {
            if (line == null || string.IsNullOrWhiteSpace(line))
                return null;
            var groups = Regex.Split(line, @"(\d\d\d\d.\d\d.\d\d \d\d:\d\d:\d\d) (\w+)\s+-\s+(.*)");
            if (groups.Length < 2) {
                return new LogLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Unknown", line);
                }
            var LogLine = new LogLine(groups[1], groups[2], groups[3]);
            return LogLine;
        }
        public static void ClearLog(string path)
        {
            try {
                //System.IO.File.WriteAllText(path,string.Empty);
                FileStream fileStream = File.Open(path, FileMode.Open);
                fileStream.SetLength(0);
                fileStream.Close();
                Console.WriteLine("Cleared \"{0}\".", path);
            }
            catch(IOException)
            {
               Console.WriteLine("Unable to clear \"{0}\"!", path);
            }
        }
    }
}
