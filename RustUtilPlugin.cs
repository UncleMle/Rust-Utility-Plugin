using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Markup;
using ConVar;
using Facepunch.Extend;
using JSON;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Database;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.MySql.Libraries;
using Oxide.Game.Rust.Libraries;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Oxide.Plugins
{
    [Info("Rust Util Plugin", "UncleMole", "0.0.1")]
    class RustUtilPlugin : RustPlugin
    {
        #region variables
        private Configuration configData;
        private StoredData storedData;

        public static string noauth = $"<font color='red'>[Not Authorized]<font color='white'> You are not authorized to use this command.";
        public static string error = $"<font color='red'>[Error]<font color='white'> ";
        public static string system = $"<font color='#61b8ff'>[System]<font color='white'> ";
        public static string adminSys = $"<font color='red'>[Admin System]<font color='white'> ";
        public static string techrustColour = "<color=#799BFF>";
        public static string orange = "<color=orange>";
        public static string stopColour = "</color>";
        #endregion

        #region config and data
        class Configuration
        {
            [JsonProperty(PropertyName = "showAdminName")]
            public bool adminMessage { get; set; }

            [JsonProperty(PropertyName = "adminPermission")]
            public string adminPermission { get; set; }
        }

        class StoredData
        {
            public List<BanData> bannedPlayers = new List<BanData>();
        }

        class BanData
        {
            public string name { get; set; }
            public string steamId { get; set; }
            public string ipAddress { get; set; }
            public string unix { get; set; }
            public string adminId { get; set; }
            public string adminName { get; set; }
            public string reason { get; set; }
        }

        private Configuration getDefault()
        {
            return new Configuration
            {
                adminMessage = true,
                adminPermission = "bans.use"
            };
        }


        protected override void LoadDefaultConfig()
        {
            Puts("Creating new config file.");

            Configuration configData = getDefault();

            SaveConfig(configData);
        }


        private bool LoadData()
        {
            try
            {
                configData = Config.ReadObject<Configuration>();
            }
            catch
            {
                return false;
            }

            SaveConfig(configData);
            return true;
        }

        void Loaded()
        {
            storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>("RustUtilPlugin");
            Interface.Oxide.DataFileSystem.WriteObject("RustUtilPlugin", storedData);
        }

        bool SaveData()
        {
            try
            {
                Interface.Oxide.DataFileSystem.WriteObject("RustUtilPlugin", storedData);
                return true;
            }
            catch (Exception e)
            {
                Puts(e.ToString());
                return false;
            }
        }

        #endregion

        #region Initialization
        void Init()
        {
            if (!LoadData())
            {
                Puts("Config or Data discrepancy.");
            }

            InitPerms();

            Puts("Util plug has been loaded successfully.");
        }

        void SaveConfig(Configuration configData)
        {
            Config.WriteObject(configData, true);

        }

        void InitPerms()
        {
            permission.RegisterPermission(configData.adminPermission, this);
        }

        private StoredData GetStoredData()
        {
            return storedData;
        }

        #endregion

        #region global events
        void OnUserConnected(IPlayer player, StoredData storedData)
        {
            bool isBanned = false;
            string adminName = null;
            string reason = null;

            foreach (BanData b in storedData.bannedPlayers)
            {
                if (b.steamId == player.Id)
                {
                    isBanned = true;
                    adminName = b.name;
                    reason = b.reason;
                    return;
                }
                else
                {
                    isBanned = false;
                    return;
                }
            }

            if (isBanned && adminName != null && reason != null)
            {
                player.Kick($"You are banned from this server for {reason} (ban from {adminName})");
            }

        }
        #endregion


        #region Commands
        [ChatCommand("players")]
        void playersCmd(BasePlayer p)
        {
            int currentPlayers = BasePlayer.activePlayerList.Count;
            int currentSleepers = BasePlayer.sleepingPlayerList.Count;

            SendReply(p, $"There are currently {techrustColour}{currentPlayers}</color> players online and {techrustColour}{currentSleepers}</color> sleepers. ");

            SaveData();
        }

        [ChatCommand("ban")]
        void banPlayerCmd(BasePlayer p, string displayName, string reason)
        {
            if (!checkAuth(p))
            {
                p.SendMessage(noauth);
                return;
            }

            if (displayName == null ||  reason == null)
            {
                p.SendMessage("Ensure parameters are satisfied.");
                return;
            }

            bool found = false;

            foreach (BasePlayer target in BasePlayer.activePlayerList)
            {
                if(target.displayName == displayName && !checkBanList(target.UserIDString)) {
                    banPlayer(p, target);
                    p.SendMessage($"{adminSys} Banned player {target.displayName} ({target.UserIDString}) with reason {orange}{reason}{stopColour}");
                    target.Kick(reason);
                    found = true;
                    return;
                }
            }

            if(!found)
            {
                p.SendMessage($"{adminSys} Player not found or player is already banned.");
            }

        }

        #endregion


        #region helpers

        bool banPlayer(BasePlayer admin, BasePlayer target)
        {
            if (!checkAuth(admin)) return false;
            
            BanData ban = new BanData();
            string unixTime = getUnix();

            ban.name = target.displayName;
            ban.steamId = target.UserIDString;
            ban.ipAddress = target.IPlayer.Address;
            ban.unix = unixTime;

            ban.adminId = admin.UserIDString;
            ban.adminName = admin.displayName;

            storedData.bannedPlayers.Add(ban);
            
            return SaveData();
        }

        bool checkAuth(BasePlayer p)
        {
            return p.IPlayer.HasPermission(configData.adminPermission);
        }

        bool checkBanList(string steamId)
        {
            bool found = false;

            foreach (BanData target in storedData.bannedPlayers)
            {
                if(target.steamId == steamId)
                {
                    found = true;
                }
            }

            return found;
        }

        string getUnix()
        {
            string unixTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            return unixTime;
        }

        #endregion

    }
}

