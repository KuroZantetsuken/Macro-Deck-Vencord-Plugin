using DiscordRPC;
using RecklessBoon.MacroDeck.Discord.UI;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;
using System;

namespace RecklessBoon.MacroDeck.Discord.Actions
{
    public class SetRichPresenceAction : PluginAction
    {
        public override string Name => "Set Rich Presence";
        public override string Description => "Set Rich Presence in Discord (Not supported in WS mode)";
        public override bool CanConfigure => true;

        public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
        {
            return new SetRichPresenceConfig(this, actionConfigurator);
        }

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            // Disabled in WebSocket version
            return;
        }

        public override void OnActionButtonDelete() { }
        public override void OnActionButtonLoaded() { }
    }
}
