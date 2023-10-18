using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oxide.Plugins
{
    [Info("Rust Util Plugin", "UncleMole", "0.0.1")]
    class RustUtilPlugin : RustPlugin
    {
        #region variables
        private ConfigData configData;
        #endregion

        #region config
        class ConfigData
        {
            [JsonProperty(PropertyName = "Reply Message")]
            public string rep = "Reply config data";


        }

        private bool LoadConfigVariables()
        {
            try
            {
                configData = Config.ReadObject<ConfigData>();
            }
            catch
            {
                return false;
            }
            SaveConfig(configData);
            return true;
        }
        #endregion

        #region Initialization
        void Init()
        {
            if (!LoadConfigVariables())
            {
                Puts("Config File issue.");
            }
            Puts("Util plug has been loaded successfully.", configData.rep);
        }

        protected override void LoadDefaultConfig()
        {
            Puts("Creating new config file.");
            configData = new ConfigData();

            SaveConfig(configData);
        }

        void SaveConfig(ConfigData configData)
        {
            Config.WriteObject(configData, true);

        }

        #endregion


        #region events
        void OnPlayerConnected(BasePlayer player)
        {
            player.SendMessage($"Welcome {player.displayName} ", configData.rep);
            //sendMsgToAll($"{adminSys} {player.displayName} has joined. Players online: {BasePlayer.activePlayerList.ToString().Length}", "admins");
        }

        void OnMessagePlayer(string message, BasePlayer player)
        {
            // player.SendMessage($"NULL {player.displayName} {configData.rep}");
        }

        void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            //sendMsgToAll($"{adminSys} {player.displayName} has disconnected. Players online: {BasePlayer.activePlayerList.ToString().Length}", "admins");
        }

        #endregion


        #region Commands
        [ChatCommand("players")]
        void playersCmd(BasePlayer p)
        {
            SendReply(p, $"There are currently {techrustColour}{getPlayers()}</color> players online and {techrustColour}{getSleepers()}</color> sleepers.");
        }


        #endregion

        #region GlobalMethods
        public static int getPlayers()
        {
            int count = 0;
            foreach (BasePlayer p in BasePlayer.activePlayerList)
            {
                count++;
            }

            return count;
        }

        public static int getSleepers()
        {
            int count = 0;
            foreach(BasePlayer p in BasePlayer.sleepingPlayerList)
            {
                count++;
            }

            return count;

        }

        public void sendMsgToAll(string message, string exception)
        {
            switch (exception)
            {
                case "admins":
                    foreach (BasePlayer player in BasePlayer.activePlayerList)
                    {
                        if (player.IsAdmin)
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
            localPlayer.SendMessage("<font color='red'>[Authentication]<font color='white'> You are not authorized to use command " + commandName);
        }
        #endregion

        #region prefixes

        public static string noauth = $"{returnTime()}<font color='red'>[Not Authorized]<font color='white'> You are not authorized to use this command.";
        public static string error = $"{returnTime()}<font color='red'>[Error]<font color='white'> ";
        public static string system = $"{returnTime()}<font color='#61b8ff'>[System]<font color='white'> ";
        public static string adminSys = $"{returnTime()}<font color='red'>[Admin System]<font color='white'> ";
        #endregion

        #region colours
        public static string techrustColour = "<color=#799BFF>";
        #endregion

        #region playerData


        #endregion
    }
}