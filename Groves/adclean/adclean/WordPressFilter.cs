
using System;
using System.Linq;
using HtmlAgilityPack;

namespace adclean
{
    public class WordPressFilter : IBlogFormattingFilter
    {
        public string Process(string htmlContent)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // update img src to use the couchbase blog path so I don't have to do this manually
            var imgs = doc.DocumentNode.Descendants("img").ToList();
            if (imgs.Any())
            {
                foreach (var t in imgs)
                {
                    var existingSrc = t.Attributes["src"].Value;
                    // starting URL should be of the form: images/<filename>
                    // new URL should be of the form: http://blog.couchbase.com/wp-content/uploads/2018/03/filename.png

                    // note that the wordpress URL has year and month in it
                    // this could cause problems on the edge days of the month
                    // but I think it's generally a safe assumption to make that I'm generating this HTML in the same
                    // month I'm uploading images
                    var currentYear = DateTime.Now.Year;
                    var currentMonth = DateTime.Now.Month.ToString().PadLeft(2, '0');
                    var filenameonly = existingSrc.Replace("images/", "");
                    var newSrc = $"http://blog.couchbase.com/wp-content/uploads/{currentYear}/{currentMonth}/{filenameonly}";
                    t.Attributes["src"].Value = newSrc;
                }
            }

            return doc.DocumentNode.WriteContentTo();
        }
    }
}