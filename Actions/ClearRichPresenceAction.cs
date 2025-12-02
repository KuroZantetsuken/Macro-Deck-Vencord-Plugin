using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;
using System;
using System.Diagnostics;

namespace RecklessBoon.MacroDeck.Discord.Actions
{
    public class ClearRichPresenceAction : PluginAction
    {
        public override string Name => "Clear Rich Presence";
        public override string Description => "Clear Rich Presence in Discord (Not supported in WS mode)";
        public override bool CanConfigure => false;

        public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
        {
            return null;
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
