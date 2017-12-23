using System;
using System.Text.RegularExpressions;
using Nop.Core.Html.CodeFormatter;
using Iddd.Data.Model;

namespace Nop.Core.Html
{
    /// <summary>
    /// Represents a BBCode helper
    /// </summary>
    public partial class BBCodeHelper
    {
        #region Fields
        private static readonly Regex regexBold = new Regex(@"\[b\](.+?)\[/b\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexItalic = new Regex(@"\[i\](.+?)\[/i\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexUnderLine = new Regex(@"\[u\](.+?)\[/u\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexUrl1 = new Regex(@"\[url\=([^\]]+)\]([^\]]+)\[/url\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexUrl2 = new Regex(@"\[url\](.+?)\[/url\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexQuote = new Regex(@"\[quote=(.+?)\](.+?)\[/quote\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex regexImg = new Regex(@"\[img\](.+?)\[/img\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Methods
       
        /// <summary>
        /// Removes all quotes from string
        /// </summary>
        /// <param name="str">Source string</param>
        /// <returns>string</returns>
        public static string RemoveQuotes(string str)
        {
            str = Regex.Replace(str, @"\[quote=(.+?)\]", String.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\[/quote\]", String.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return str;
        }

        #endregion
    }
}
