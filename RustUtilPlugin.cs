using Oxide.Core.Libraries.Covalence;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using ConVar;
using System.Dynamic;
using Oxide.Core.Libraries;
using Newtonsoft.Json;

namespace Oxide.Plugins
{
    [Info("Rust Util Plugin", "UncleMole", "0.0.1")]
    class RustUtilPlugin : RustPlugin
    {



        #region Initialization
        void Init()
        {
            Puts("Util plug has been loaded successfully.");
        }

        #endregion

        #region events
        void OnPlayerConnected(BasePlayer player)
        {
            sendMsgToAll($"{adminSys} {player.displayName} has joined. Players online: {BasePlayer.activePlayerList.ToString().Length}", "admins");
        }

        void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            sendMsgToAll($"{adminSys} {player.displayName} has disconnected. Players online: {BasePlayer.activePlayerList.ToString().Length}", "admins");
        }

        #endregion


        #region Commands
        [ChatCommand("players")]
        void playersCmd(BasePlayer p)
        {
            SendReply(p, $"{getPlayers()}");
        }

        [ChatCommand("aduty")]
        void adutyCmd(BasePlayer p)
        {
            SendReply(p, $"Isadmin: {p.IsAdmin} aduty: ");
        }


        #endregion

        #region GlobalMethods
        public static string[] getPlayers()
        {
            int count = 0;
            var players = new List<string>();
            foreach(BasePlayer p in BasePlayer.activePlayerList)
            {
                players.Add($"{count}. {p.displayName}");
                count++;
            }

            return players.ToArray();
        }

        public void sendMsgToAll(string message, string exception)
        {
            switch(exception)
            {
                case "admins":
                    foreach (BasePlayer player in BasePlayer.activePlayerList)
                    {
                        if(player.IsAdmin)
                        {
                            player.SendMessage($"{message}");
                        }
                    }
                    break;
                case "players":
                    foreach (BasePlayer player in BasePlayer.activePlayerList)
                    {
                        player.SendMessage($"{message}");
                    }
                    break;
                default: break;
            }
        }

        public static string returnTime()
        {
            DateTime today = DateTime.Today;
            return $"<font color='grey'>{today.Hour}:{today.Minute}:{today.Second}<font color='white'> ";
        }



        public static void sendPerm(BasePlayer localPlayer, string commandName)
        {
            localPlayer.SendMessage("<font color='red'>[Authentication]<font color='white'> You are not authorized to use command "+ commandName);
        }
        #endregion

        #region prefixes

        public static string noauth = $"{returnTime()}<font color='red'>[Not Authorized]<font color='white'> You are not authorized to use this command.";
        public static string error = $"{returnTime()}<font color='red'>[Error]<font color='white'> ";
        public static string system = $"{returnTime()}<font color='#61b8ff'>[System]<font color='white'> ";
        public static string adminSys = $"{returnTime()}<font color='red'>[Admin System]<font color='white'> ";
        #endregion

        #region playerData


        #endregion
    }
}