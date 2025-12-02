using SuchByte.MacroDeck.Logging;

namespace RecklessBoon.MacroDeck.Discord
{
    public class AppLogger
    {
        public void Info(string message)
        {
            MacroDeckLogger.Info(PluginInstance.Plugin, message);
        }

        public void Error(string message, params object[] args)
        {
            MacroDeckLogger.Error(PluginInstance.Plugin, string.Format(message, args));
        }

        public void Trace(string message)
        {
            // Optional trace logging
            MacroDeckLogger.Trace(PluginInstance.Plugin, message);
        }
    }
}
