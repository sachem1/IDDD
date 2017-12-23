using System.Configuration;

namespace Coralcode.Framework.ConfigManager
{
    public class ConfigManager
    {
        public static readonly ConfigManager Instance = new ConfigManager();
        private ConfigManager()
        {
        }


        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
