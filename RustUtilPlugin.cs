using Oxide.Core.Libraries.Covalence;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Rust Admin Plugin", "UncleMole", "0.0.1")]
    class RustUtilPlugin : RustPlugin
    { 
        void Init()
        {
            Puts("Plugin has started.");
            Puts($"Current Players online: {getPlayers()} ");
        }


        [ChatCommand("players")]
        void playersCmd(BasePlayer p)
        {
            SendReply(p, $"{getPlayers()}");
        } 

        public static string[] getPlayers()
        {
            int count = 0;
            var players = new List<string>();
            foreach(BasePlayer p in BasePlayer.activePlayerList)
            {
                 players.Add($"{count}. Player {p.displayName}");
                count++;
            }

            return players.ToArray();
        }


    }
}