using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using System;

namespace RecklessBoon.MacroDeck.Discord.Actions
{
    public class SetDeafenOffAction : PluginAction
    {
        public override string Name => "Set Deafen Off";
        public override string Description => "Set voice server deafen state to not deafened";
        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            try
            {
                var plugin = PluginInstance.Plugin;
                if (plugin.WebSocketServer != null && plugin.WebSocketServer.IsConnected)
                {
                    _ = plugin.WebSocketServer.Send(new { command = "SET_DEAF", value = false });
                }
            }
            catch (Exception ex)
            {
                PluginInstance.Logger.Error("Unexpected Exception:\n{0}", ex);
            }
        }
    }
}
