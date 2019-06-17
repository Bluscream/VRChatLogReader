using System;
using System.IO;
using System.Linq;

namespace VRChatLogReader
{
    public class Utils
    {
        public static void Log(params object[] msgs) {
            var msg = $"[{DateTime.Now}]";
            foreach (var _msg in msgs) {
                try {
                    msg += $" {_msg}";
                } catch {
                    msg += $" {_msg.ToString()}";
                }
            }
            Console.WriteLine(msg);
        }
        public static string CombinePaths(string source, params string[] paths)  {
            if (source == null) throw new ArgumentNullException("source");
            if (paths == null) throw new ArgumentNullException("paths");
            return paths.Aggregate(source, (acc, p) => Path.Combine(acc, p));
        }
    }
}
