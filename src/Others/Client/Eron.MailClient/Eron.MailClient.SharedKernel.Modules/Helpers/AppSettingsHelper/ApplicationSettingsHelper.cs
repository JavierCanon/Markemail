using System;
using System.Configuration;

namespace Eron.MailClient.SharedKernel.Modules.Helpers.AppSettingsHelper
{
    public static class ApplicationSettingsHelper
    {
        public static T AppSetting<T>(string key)
        {
            return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
        }

        public static string AppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
