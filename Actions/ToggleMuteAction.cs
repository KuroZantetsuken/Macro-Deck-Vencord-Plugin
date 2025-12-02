using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using System;

namespace RecklessBoon.MacroDeck.Discord.Actions
{
    public class ToggleMuteAction : PluginAction
    {
        public override string Name => "Toggle Mute";
        public override string Description => "Toggle voice server mute state";
        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            try
            {
                var plugin = PluginInstance.Plugin;
                if (plugin.WebSocketServer != null && plugin.WebSocketServer.IsConnected)
                {
                    _ = plugin.WebSocketServer.Send(new { command = "TOGGLE_MUTE" });
                }
            }
            catch (Exception ex)
            {
                PluginInstance.Logger.Error("Unexpected Exception:\n{0}", ex);
            }
        }
    }
}
