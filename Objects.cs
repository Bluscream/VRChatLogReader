using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VRChatLogReader
{
    public class LogLine
    {
        public DateTime DateTime { get; set; }
        public LogCategory Category { get; set; }
        public EventType EventType { get; set; }
        public Logger Logger { get; set; }
        public Player Player { get; set; }
        public string Message {get; set; }
        public LogLine() { }
        public LogLine(string timestamp, string category, string message)
        {
            message = message.Trim();
            Message = message;
            DateTime = DateTime.ParseExact(timestamp, "yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            switch(category.Trim()) {
                case "Log": Category = LogCategory.Log;break;
                case "Trace": Category = LogCategory.Trace;break;
                case "Debug": Category = LogCategory.Debug;break;
                case "Info": Category = LogCategory.Info;break;
                case "Warning": Category = LogCategory.Warning;break;
                case "Error": Category = LogCategory.Error;break;
                default: Category = LogCategory.Unknown;break;
            }
            var groups = Regex.Split(message, @"^\[(.*)\] ");
            /*Log: [NetworkManager] OnPlayerLeft VRCPlayer[Remote] Atlanik 99
            [13:06:22] Log: UnityActivityWatchdog: OnApplicationFocus [True]
            [13:06:25] Log: UnityActivityWatchdog: OnApplicationFocus [False]
            [13:06:25] Error: AmplitudeAPI: upload failed with exception - The requested address is not valid in its context.
              at VRC.Core.BestHTTP.Platform*/
            if (groups.Length < 2)
                groups = Regex.Split(message, @"^(.*): ");
            if (groups.Length > 1){
                var logger = groups[1].Split(new char[] { ' ' }, 2);
                if (logger.Length > 1) Message = logger[1];
                switch (logger[0])
                {
                    case "USpeaker": Logger = Logger.USpeaker;break;
                    case "VRC_EventDispatcherRFC": Logger = Logger.VRC_EventDispatcherRFC;break;
                    case "ObjectInstantiator": Logger = Logger.ObjectInstantiator;break;
                    case "NetworkManager":
                        string[] NetworkEvent = groups[2].Split(new char[] { ' ' }, 2);
                        if (NetworkEvent.Length > 1) Message = NetworkEvent[1];
                        else { message = string.Empty; }
                        switch (NetworkEvent[0])
                        {
                            case "OnConnectedToPhoton": EventType = EventType.OnConnectedToPhoton;break;
                            case "OnPlayerJoined": EventType = EventType.OnPlayerJoined;break;
                            case "OnPlayerLeft": EventType = EventType.OnPlayerLeft;break;
                            case "OnConnectedToMaster": EventType = EventType.OnConnectedToMaster;break;
                            case "OnJoinedRoom": EventType = EventType.OnJoinedRoom;break;
                            case "OnLeftRoom": EventType = EventType.OnLeftRoom;break;
                            case "OnPhotonPlayerConnected": EventType = EventType.OnPhotonPlayerConnected;break;
                            case "OnPhotonPlayerDisconnected": EventType = EventType.OnPhotonPlayerDisconnected;break;
                            case "OnPhotonPlayerPropertiesChanged": EventType = EventType.OnPhotonPlayerPropertiesChanged;break;
                            case "OnPhotonCustomRoomPropertiesChanged": EventType = EventType.OnPhotonCustomRoomPropertiesChanged;break;
                            case "OnMasterClientSwitched": EventType = EventType.OnMasterClientSwitched;break;
                            case "OnOwnershipTransfered": EventType = EventType.OnOwnershipTransfered;break;
                            default:
                                Console.WriteLine("case \"{0}\": EventType = EventType.{0};break;", NetworkEvent[0]);
                                EventType = EventType.Unknown;
                                break; 
                        }
                        Logger = Logger.NetworkManager;
                        break;
                    case "ApiWorldUpdate": Logger = Logger.ApiWorldUpdate;break;
                    case "AssetBundleDownloadManager": Logger = Logger.AssetBundleDownloadManager;break;
                    default: Logger = Logger.Unknown;break;
                }
            }
        }
    }
    public class PlayerName
    {
        public string Fullname { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public bool IsSteam { get; set; }
        public PlayerName() { }
        public PlayerName(string name) {
            Fullname = name;
            IsSteam = false;
            if (name.Contains(" ")) {
                int idx = name.LastIndexOf(' ');
                if (idx != -1) {
                    var substr = name.Substring(idx + 1);
                    if (substr.Length == 4) {
                        var match = Regex.Match(substr, @"([a-z0-9]+)");
                        if (match.Success)
                            Tag = substr;
                            IsSteam = true;
                    }
                }
            }
        }
    }

    public class Player
    {
        public PlayerName Name { get; set; }
        public int Id { get; set; }
        public PlayerType Type { get; set; }
        public Player() { }
        public Player(string name, int id, PlayerType type)
        {
            Name = new PlayerName(name);
            Id = id;
            Type = type;
        }
    }
}
