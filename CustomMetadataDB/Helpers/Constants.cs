using System.Text.RegularExpressions;

namespace CustomMetadataDB.Helpers
{
    public class Constants
    {
        public const string PLUGIN_NAME = "CMetadataDB";
        public const string PLUGIN_EXTERNAL_ID = "cmdb";
        public const string PLUGIN_DESCRIPTION = "Custom metadata agent db.";
        public const string PLUGIN_GUID = "83b77e24-9fce-4ee0-a794-73fdfa972e66";
        public static readonly Regex[] EPISODE_MATCHERS = {
            // YY?YY(-._)MM(-._)DD -? series -? epNumber -? title
            new(@"^(?<year>\d{2,4})(\-|\.|_)?(?<month>\d{2})(\-|\.|_)?(?<day>\d{2})\s-?(?<series>.+?)(?<epNumber>\#(\d+)|ep(\d+)|DVD[0-9.-]+|DISC[0-9.-]+|SP[0-9.-]+|Episode\s(\d+)) -?(?<title>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            // YY?YY(-._)MM(-._)DD -? title
            new(@"^(?<year>\d{2,4})(\-|\.|_)?(?<month>\d{2})(\-|\.|_)?(?<day>\d{2})\s?-?(?<title>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            // title YY?YY(-._)MM(-._)DD at end of filename.
            new(@"(?<title>.+?)(?<year>\d{2,4})(\-|\.|_)?(?<month>\d{2})(\-|\.|_)?(?<day>\d{2})$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            // series - YY?YY(-._)MM(-._)DD -? title
            new(@"(?<series>.+?)(?<year>\d{2,4})(\-|\.|_)?(?<month>\d{2})(\-|\.|_)?(?<day>\d{2})\s?-?(?<title>.+)?", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            // series - SXXEYY - title
            new(@"^(?<series>.+?)[Ss](?<season>[0-9]{1,})[Ee](?<episode>[0-9]{1,})\s?-?(?<title>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        };
    }
}
