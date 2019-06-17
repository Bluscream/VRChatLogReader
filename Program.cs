using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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
        public static string watching; public static string watching_vrcml;
        static bool config_joinsonly = false;
        static async Task Main(string[] args)
        {
            Console.Title = "VRChat Log Reader";
            /*var p = new OptionSet() { { "j|joins|joinsonly", "Only show joins/leaves", v => config_joinsonly = v != null } };
            List<string> extra;
            extra = p.Parse(args);*/
            var logDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "vrchat"));
            StartWatcher(logDir, "output_log_*-*-*_??.txt");
            logDir = new DirectoryInfo(@"G:\Steam\steamapps\common\VRChat\logs");
            StartWatcher(logDir, "VRCModLoader_????-??-??-??-??-??-???.log");
            Console.ReadLine();
        }

        private static void StartWatcher(DirectoryInfo logDir, string logPattern)
        {
            Utils.Log("Log Directory:", logDir);
            Utils.Log("Log Pattern:", logPattern);
            var wh = new AutoResetEvent(false);
            var fsw = new FileSystemWatcher(".");
            fsw.Path = logDir.FullName;
            fsw.Filter = logPattern;
            fsw.EnableRaisingEvents = true;
            fsw.Created += logFileCreated;
            fsw.Changed += logFileChanged; ;
            var lastLog = logDir.GetFiles(logPattern).OrderByDescending(f => f.LastWriteTime).First().FullName;
            StartTailing(lastLog);
        }

        private static void logFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Created && e.ChangeType != WatcherChangeTypes.Changed)
            {
                Utils.Log("File Removed:", e.Name, e.ChangeType.ToString().Enclose());
            }
        }

        private static void logFileCreated(object sender, FileSystemEventArgs e)
        {
            Utils.Log("File Created:", e.Name, e.ChangeType.ToString().Enclose());
            StartTailing(e.FullPath);
        }

        private static void StartTailing(string path)
        {
            watching = Path.GetFileName(path);
            Utils.Log("Now watching:", watching);
            ThreadWithState tws = new ThreadWithState(path);
            Thread thread = new Thread(tws.ReadFile);
            thread.Start();
        }

        /*public static LogLine ParseLine(string line)
        {
            if (line == null || string.IsNullOrWhiteSpace(line))
                return null;
            var groups = Regex.Split(line, @"(\d\d\d\d.\d\d.\d\d \d\d:\d\d:\d\d) (\w+)\s+-\s+(.*)");
            if (groups.Length < 2) {
                return new LogLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Unknown", line);
                }
            var LogLine = new LogLine(groups[1], groups[2], groups[3]);
            return LogLine;
        }*/
        /*public static void ClearLog(string path)
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
        }*/
    }
    public class ThreadWithState {
        private static string logPath;

        public ThreadWithState(string path) {
            logPath = path;
        }

        public void ReadFile()
        {
            try {
                var name = Path.GetFileName(logPath);
                var fs = new FileStream(logPath, FileMode.Open, FileSystemRights.ReadData, FileShare.ReadWrite, 50000, FileOptions.SequentialScan);
                if (!fs.CanRead) { Utils.Log("ERROR: Can't read stream:", fs.Name, fs.Length.ToString().Enclose()); }
                using (var sr = new StreamReader(fs)) {
                    var s = "";
                    while (true) {
                        s = sr.ReadLine();
                        try {
                            // if (!sr.EndOfStream) continue;
                            if (name != Program.watching && name != Program.watching_vrcml) break;
                            if (s != null && !string.IsNullOrWhiteSpace(s))
                            {
                                // if (config_joinsonly && (!s.Contains("OnPlayerJoined") && !s.Contains("OnPlayerLeft") && !s.Contains("OnJoinedRoom") && !s.Contains("OnLeftRoom"))) continue;
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
                            }
                        } catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
                    }
                }
            } catch (Exception ex) { Utils.Log("ERROR: Can't read file:", ex.Message + Environment.NewLine + ex.StackTrace); }
        }
    }
}
