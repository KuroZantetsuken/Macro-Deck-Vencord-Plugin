using RecklessBoon.MacroDeck.Discord.Actions;
using RecklessBoon.MacroDeck.Discord.Models;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace RecklessBoon.MacroDeck.Discord
{
    public static class PluginInstance
    {
        public static AppLogger Logger;
        public static DiscordPlugin Plugin;
        public static StateCache cache = new StateCache()
        {
            VoiceState = new VoiceState()
        };
    }

    public class DiscordPlugin : MacroDeckPlugin
    {
        public Configuration configuration;

        public override bool CanConfigure => true;

        public WebSocketServer WebSocketServer { get; private set; }

        private ContentSelectorButton statusButton = new ContentSelectorButton();
        private ToolTip statusToolTip = new ToolTip();

        private MainWindow mainWindow;

        public class VariableState
        {
            public string Name { get; set; }
            protected VariableType _type = VariableType.Bool;
            public VariableType Type { get { return _type; } set { _type = value; } }
            protected object _value = false;
            public object Value { get { return _value; } set { _value = value; } }
            protected bool _save = true;
            public bool Save { get { return _save; } set { _save = value; } }

        }

        public void SetVariable(VariableState variableState)
        {
            // Empty list for suggestions
            VariableManager.SetValue(string.Format("discord_{0}", variableState.Name), variableState.Value, variableState.Type, this, new string[] { });
        }

        public string GetVariable(string key)
        {
            var name = String.Format("discord_{0}", key);
            return VariableManager.Variables.Where(x => x.Name == name).ToString();
        }

        public void SetVariable(VariableState[] variableStates)
        {
            foreach (VariableState state in variableStates)
            {
                SetVariable(state);
            }
        }

        public void UpdateVoiceStateVariables(VoiceState voiceState)
        {
            if (voiceState == null)
                voiceState = new VoiceState();

            PluginInstance.cache.VoiceState = voiceState;
            SetVariable(new[] {
                new VariableState { Name = "is_self_muted", Value = voiceState.SelfMute },
                new VariableState { Name = "is_self_deafened", Value = voiceState.SelfDeaf },
                new VariableState { Name = "is_server_muted", Value = voiceState.Mute },
                new VariableState { Name = "is_server_deafened", Value = voiceState.Deaf },
                new VariableState { Name = "is_any_muted", Value = voiceState.Mute || voiceState.SelfMute },
                new VariableState { Name = "is_any_deafened", Value = voiceState.Deaf || voiceState.SelfDeaf },
            });
        }

        public DiscordPlugin()
        {
            PluginInstance.Plugin ??= this;
            PluginInstance.Logger ??= new AppLogger();
            SuchByte.MacroDeck.MacroDeck.OnMainWindowLoad += MacroDeck_OnMainWindowLoad;
        }

        // Gets called when the plugin is loaded
        public override void Enable()
        {
            try
            {
                configuration ??= new Configuration(PluginInstance.Plugin);
                ResetVariables();
                InitServer();

                Actions = new List<PluginAction>
                {
                    // add the instances of your actions here
                    new SetMuteOnAction(),
                    new SetMuteOffAction(),
                    new ToggleMuteAction(),
                    new SetDeafenOnAction(),
                    new SetDeafenOffAction(),
                    new ToggleDeafenAction(),
                    // Rich Presence actions disabled for WS version
                    // new SetRichPresenceAction(),
                    // new ClearRichPresenceAction(),
                };
            }
            catch (Exception ex)
            {
                PluginInstance.Logger.Error("Unexpected Exception:\n{0}", ex);
            }
        }

        private void MacroDeck_OnMainWindowLoad(object sender, EventArgs e)
        {
            try
            {
                mainWindow = sender as MainWindow;
                statusButton = new ContentSelectorButton();
                statusButton.BackgroundImageLayout = ImageLayout.Zoom;
                UpdateStatusButton(WebSocketServer != null && WebSocketServer.IsConnected);
                statusButton.Click += StatusButton_Click;
                mainWindow.contentButtonPanel.Controls.Add(statusButton);
            }
            catch (Exception ex)
            {
                PluginInstance.Logger.Error("Unexpected Exception:\n{0}", ex);
            }
        }

        private void StatusButton_Click(object sender, EventArgs e)
        {
            // Reconnect logic or config?
            if (configuration == null || !configuration.IsFullySet)
            {
                OpenConfigurator();
            }
        }

        /**
         * Pad a bitmap with default padding
         */
        private Bitmap PadBitmap(Bitmap bm)
        {
            return PadBitmap(bm, 1.3f, 1.3f);
        }

        /**
         * Pad a bitmap with equal percentage on x and y axis
         */
        private Bitmap PadBitmap(Bitmap bm, float padding)
        {
            return PadBitmap(bm, padding, padding);
        }

        /**
         * Pad a bitmap with explicit percentage on x and y axis
         */
        private Bitmap PadBitmap(Bitmap bm, float xPadding, float yPadding)
        {
            Bitmap paddedBm = new Bitmap((int)(bm.Width * xPadding), (int)(bm.Height * yPadding));
            using (Graphics graphics = Graphics.FromImage(paddedBm))
            {
                graphics.Clear(Color.Transparent);
                int x = (paddedBm.Width - bm.Width) / 2;
                int y = (paddedBm.Height - bm.Height) / 2;
                graphics.DrawImage(bm, x, y, bm.Width, bm.Height);
            }
            return paddedBm;
        }

        private void UpdateStatusButton(bool connected)
        {
            try
            {
                if (mainWindow != null && mainWindow.IsHandleCreated)
                {
                    mainWindow.BeginInvoke(new Action(() =>
                    {
                        Bitmap bm = connected ? Properties.Resources.Discord_Logo_Color_64x49 : Properties.Resources.Discord_Logo_White_64x49;
                        statusButton.BackgroundImage = PadBitmap(bm, 1.35f);
                        statusToolTip.SetToolTip(statusButton, "Vencord " + (connected ? "Connected" : "Disconnected"));
                    }));
                }
            }
            catch (Exception ex)
            {
                PluginInstance.Logger.Error("Unexpected Exception:\n{0}", ex);
            }
        }

        protected void InitServer()
        {
            try
            {
                if (WebSocketServer == null)
                {
                    WebSocketServer = new WebSocketServer();
                    WebSocketServer.OnMessage += OnWebSocketMessage;
                    WebSocketServer.OnConnectionStateChanged += OnConnectionStateChanged;
                    WebSocketServer.Start(configuration.Port);
                }
            }
            catch (Exception ex)
            {
                PluginInstance.Logger.Error("Unexpected Exception ocurred while initializing the WS server:\n{0}", ex);
            }
        }

        private void OnWebSocketMessage(object sender, JObject json)
        {
            try
            {
                if (json["event"]?.ToString() == "VOICE_STATE_UPDATE" && json["data"] != null)
                {
                    var voiceState = json["data"].ToObject<VoiceState>();
                    UpdateVoiceStateVariables(voiceState);
                }
            }
            catch (Exception ex)
            {
                PluginInstance.Logger.Error("Error processing WS message: " + ex.Message);
            }
        }

        private void OnConnectionStateChanged(object sender, bool connected)
        {
            UpdateStatusButton(connected);
        }

        protected void ResetVariables()
        {
            SetVariable(new[] {
                new VariableState { Name = "is_self_muted" },
                new VariableState { Name = "is_self_deafened" },
                new VariableState { Name = "is_server_muted" },
                new VariableState { Name = "is_server_deafened" },
                new VariableState { Name = "is_any_muted" },
                new VariableState { Name = "is_any_deafened" }
            });
        }

        public override void OpenConfigurator()
        {
            using var configurator = new ConfigurationForm(configuration);
            configurator.ShowDialog();
        }
    }
}
