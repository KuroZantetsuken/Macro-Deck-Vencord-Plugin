using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using System;

namespace RecklessBoon.MacroDeck.Discord.Actions
{
    public class SetMuteOnAction : PluginAction
    {
        public override string Name => "Set Mute On";
        public override string Description => "Set voice server mute state to muted";
        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            try
            {
                var plugin = PluginInstance.Plugin;
                if (plugin.WebSocketServer != null && plugin.WebSocketServer.IsConnected)
                {
                    _ = plugin.WebSocketServer.Send(new { command = "SET_MUTE", value = true });
                }
            }
            catch (Exception ex)
            {
                PluginInstance.Logger.Error("Unexpected Exception:\n{0}", ex);
            }
        }
    }
}
