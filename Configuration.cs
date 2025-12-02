using Newtonsoft.Json;
using SuchByte.MacroDeck.Plugins;

namespace RecklessBoon.MacroDeck.Discord
{
    public class Configuration
    {
        [JsonIgnore]
        DiscordPlugin _plugin;

        public int Port { get; set; } = 8124;
        public bool Debug { get; set; } = false;

        public Configuration(DiscordPlugin plugin)
        {
            if (plugin != null)
            {
                _plugin = plugin;
                Reload();
            }
        }

        public bool IsFullySet => true;

        public void Save()
        {
            SaveConfig();
            LoadConfig();
        }

        public void Reload()
        {
            LoadConfig();
        }

        protected void LoadConfig()
        {
            var json = PluginConfiguration.GetValue(_plugin, "config");
            if (json != null)
            {
                try
                {
                    var config = JsonConvert.DeserializeObject<Configuration>(json);
                    Port = config != null ? config.Port : 8124;
                    Debug = config != null && config.Debug;
                }
                catch { }
            }
        }

        protected void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(this);
            PluginConfiguration.SetValue(_plugin, "config", json);
        }
    }
}
