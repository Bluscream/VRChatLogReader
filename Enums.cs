using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChatLogReader
{
    public enum LogCategory {
        Unknown, Log, Trace, Debug, Info, Warning, Error
    }
    public enum PlayerType {
        Unknown, Local,Remote
    }
    public enum EventType {
        Unknown, OnPlayerJoined, OnPlayerLeft, OnConnectedToPhoton, OnConnectedToMaster, OnPhotonPlayerPropertiesChanged, OnJoinedRoom, OnLeftRoom, 
        OnPhotonCustomRoomPropertiesChanged, OnPhotonPlayerConnected, OnPhotonPlayerDisconnected, OnMasterClientSwitched, OnOwnershipTransfered
    }
    public enum Logger {
        Unknown, USpeaker, ApiWorldUpdate, NetworkManager, AssetBundleDownloadManager, VRC_EventDispatcherRFC, ObjectInstantiator
    }
    /*
    public class LogCategory {
        private LogCategory(string value) { Value = value; }
        public string Value { get; set; }
        public static LogCategory Log { get { return new LogCategory("Log"); } }
        public static LogCategory Trace { get { return new LogCategory("Trace"); } }
        public static LogCategory Debug { get { return new LogCategory("Debug"); } }
        public static LogCategory Info { get { return new LogCategory("Info"); } }
        public static LogCategory Warning { get { return new LogCategory("Warning"); } }
        public static LogCategory Error { get { return new LogCategory("Error"); } }
    }
    public class PlayerType {
        private PlayerType(string value) { Value = value; }
        public string Value { get; set; }
        public static PlayerType Local { get { return new PlayerType("VRCPlayer[Local]"); } }
        public static PlayerType Remote { get { return new PlayerType("VRCPlayer[Remote]"); } }
    }
    public class EventType {
        private EventType(string value) { Value = value; }
        public string Value { get; set; }
        public static EventType OnPlayerJoined { get { return new EventType("OnPlayerJoined"); } }
        public static EventType OnPlayerLeft { get { return new EventType("OnPlayerLeft"); } }
    }
    public class Logger
    {
        private Logger(string value) { Value = value; }
        public string Value { get; set; }
        public static Logger USpeaker { get { return new Logger("USpeaker"); } }
        public static Logger ApiWorldUpdate { get { return new Logger("ApiWorldUpdate"); } }
        public static Logger NetworkManager { get { return new Logger("NetworkManager"); } }
        public static Logger AssetBundleDownloadManager { get { return new Logger("AssetBundleDownloadManager"); } }
        public static Logger VRC_EventDispatcherRFC { get { return new Logger("VRC_EventDispatcherRFC"); } }
        public static Logger ObjectInstantiator { get { return new Logger("ObjectInstantiator"); } }
    }
    */
}
