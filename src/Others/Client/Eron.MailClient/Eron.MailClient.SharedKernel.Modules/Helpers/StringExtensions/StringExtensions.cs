using System;
using System.Text.RegularExpressions;

namespace Eron.MailClient.SharedKernel.Modules.Helpers.StringExtensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        public static bool IsPopulated(this string value)
        {
            return !String.IsNullOrWhiteSpace(value) && !String.IsNullOrEmpty(value);
        }

        public static string Slugify(this string phrase)
        {
            var s = phrase.ToLower();
          //s = Regex.Replace(s, @"[^a-z0-9\s-]", "");                      // remove invalid characters
            s = Regex.Replace(s, @"\s+", " ").Trim();                       // single space
            s = s.Substring(0, s.Length <= 45 ? s.Length : 45).Trim();      // cut and trim
            s = Regex.Replace(s, @"\s", "-");                               // insert hyphens
            return s.ToLower();
        }
    }
}
