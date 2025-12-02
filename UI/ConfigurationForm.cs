using SuchByte.MacroDeck.GUI.CustomControls;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RecklessBoon.MacroDeck.Discord
{
    partial class ConfigurationForm : DialogForm
    {
        protected static Configuration _config;

        public event EventHandler OnSecretChanged; // Keeping event to avoid breakages if used elsewhere
        public event EventHandler OnDebugLoggingChanged;

        public ConfigurationForm(Configuration config)
        {
            _config = config ?? _config;
            InitializeComponent();

            clientId.Text = config.Port.ToString();
            cbxDebugLogging.Checked = config.Debug;

            // Hide unused (removed from designer)
            // lblClientSecret.Visible = false;
            // clientSecret.Visible = false;
            label1.Text = "Port:";
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (int.TryParse(clientId.Text, out int port))
            {
                _config.Port = port;
            }

            var oldDebugLogging = _config.Debug;
            _config.Debug = cbxDebugLogging.Checked;

            if (oldDebugLogging != _config.Debug)
            {
                OnDebugLoggingChanged?.Invoke(this, EventArgs.Empty);
            }

            _config.Save();

            // Signal change (reusing SecretChanged event for simplicity)
            OnSecretChanged?.Invoke(this, EventArgs.Empty);

            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl("https://github.com/KuroZantetsuken/Macro-Deck-Vencord-Plugin");
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
            }
        }
    }
}
