using Newtonsoft.Json;

namespace RecklessBoon.MacroDeck.Discord.Models
{
    public class VoiceState
    {
        [JsonProperty("self_mute")]
        public bool SelfMute { get; set; }

        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; set; }

        [JsonProperty("mute")]
        public bool Mute { get; set; }

        [JsonProperty("deaf")]
        public bool Deaf { get; set; }
    }
}
