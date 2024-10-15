using Oxide.Core;
using Oxide.Core.Plugins;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Oxide.Plugins
{
    [Info("Pop", "RustFlash", "1.0.0")]
    [Description("The most comprehensive /pop plugin with many setting options for PvP/PvE or RP")]
    public class Pop : RustPlugin
    {
        private Configuration config;

        private class Configuration
        {
            [JsonProperty("Show Server Name")]
            public bool ShowServerName = true;

            [JsonProperty("Show Online Players Count")]
            public bool ShowOnlineCount = true;

            [JsonProperty("Show Sleeping Players Count")]
            public bool ShowSleepingCount = true;

            [JsonProperty("Show Joining Players Count")]
            public bool ShowJoiningCount = true;

            [JsonProperty("Show Queued Players Count")]
            public bool ShowQueuedCount = true;

            [JsonProperty("Show Online Player Names")]
            public bool ShowPlayerNames = true;

            [JsonProperty("Server Name Color")]
            public string ServerNameColor = "#ffed00";

            [JsonProperty("Label Color")]
            public string LabelColor = "#00BFFF";

            [JsonProperty("Value Color")]
            public string ValueColor = "#ffffff";
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();
                if (config == null) throw new JsonException();
                ValidateColors();
                SaveConfig();
            }
            catch
            {
                PrintError("Your configuration file contains an error. Using default configuration values.");
                LoadDefaultConfig();
            }
        }

        private void ValidateColors()
        {
            Regex hexColorRegex = new Regex("^#(?:[0-9a-fA-F]{3}){1,2}$");
            if (!hexColorRegex.IsMatch(config.ServerNameColor)) config.ServerNameColor = "#ffed00";
            if (!hexColorRegex.IsMatch(config.LabelColor)) config.LabelColor = "#00BFFF";
            if (!hexColorRegex.IsMatch(config.ValueColor)) config.ValueColor = "#ffffff";
        }

        protected override void LoadDefaultConfig() => config = new Configuration();

        protected override void SaveConfig() => Config.WriteObject(config);

        private void Init()
        {
            cmd.AddChatCommand("pop", this, "CmdPop");
        }

        private void CmdPop(BasePlayer player, string command, string[] args)
        {
            player.ChatMessage(GetPopInfoString());
        }

        private string GetPopInfoString()
        {
            string message = "";

            if (config.ShowServerName)
                message += $"<color={config.ServerNameColor}>{ConVar.Server.hostname}</color>\n";

            if (config.ShowOnlineCount)
                message += FormatLine("Online", BasePlayer.activePlayerList.Count);

            if (config.ShowSleepingCount)
                message += FormatLine("Sleeping", BasePlayer.sleepingPlayerList.Count);

            if (config.ShowJoiningCount)
                message += FormatLine("Joining", ServerMgr.Instance.connectionQueue.Joining);

            if (config.ShowQueuedCount)
                message += FormatLine("Queued", ServerMgr.Instance.connectionQueue.Queued);

            if (config.ShowPlayerNames)
            {
                string onlinePlayerNames = string.Join(", ", BasePlayer.activePlayerList.Select(p => p.displayName));
                message += FormatLine("Players", onlinePlayerNames, false);
            }

            return message.TrimEnd('\n');
        }

        private string FormatLine(string label, object value, bool addNewLine = true)
        {
            return $"<color={config.LabelColor}>{label}:</color> <color={config.ValueColor}>{value} Player</color>{(addNewLine ? "\n" : "")}";
        }
    }
}