using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using System;

namespace RecklessBoon.MacroDeck.Discord.Actions
{
    public class SetDeafenOnAction : PluginAction
    {
        public override string Name => "Set Deafen On";
        public override string Description => "Set voice server deafen state to deafened";
        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            try
            {
                var plugin = PluginInstance.Plugin;
                if (plugin.WebSocketServer != null && plugin.WebSocketServer.IsConnected)
                {
                    _ = plugin.WebSocketServer.Send(new { command = "SET_DEAF", value = true });
                }
            }
            catch (Exception ex)
            {
                PluginInstance.Logger.Error("Unexpected Exception:\n{0}", ex);
            }
        }
    }
}
