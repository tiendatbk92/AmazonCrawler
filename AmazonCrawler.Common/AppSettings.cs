using System.Configuration;

namespace AmazonCrawler.Common
{
    public class AppSettings
    {
        public static string GetString(string key)
        {
            var str = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrEmpty(str))
            {
                return str.Trim();
            }
            else
            {
                return String.Empty;
            }
        }

        public static int? GetInt32(string key)
        {
            var str = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrEmpty(str))
            {
                int re;
                var success = int.TryParse(str, out re);
                return success ? re : null;
            }
            else
            {
                return null;
            }
        }

        public static long? GetInt64(string key)
        {
            var str = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrEmpty(str))
            {
                long re;
                var success = long.TryParse(str, out re);
                return success ? re : null;
            }
            else
            {
                return null;
            }
        }

        public static DateTime? GetDateTime(string key)
        {
            var str = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrEmpty(str))
            {
                DateTime re;
                var success = DateTime.TryParse(str, out re);
                return success ? re : null;
            }
            else
            {
                return null;
            }
        }

        public static bool GetBool(string key)
        {
            bool re = false;
            var str = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrEmpty(str))
            {
                var success = bool.TryParse(str, out re);
                return success ? re : false;
            }
            else
            {
                return false;
            }
        }

        public static string GetConnection(string key)
        {
            return !string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[key].ConnectionString) ? ConfigurationManager.ConnectionStrings[key].ConnectionString : string.Empty;
        }
    }
}
