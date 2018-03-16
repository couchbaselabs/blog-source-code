using System;
using System.Diagnostics;
using System.Linq;
using HtmlAgilityPack;

namespace adclean
{
    public class CrossCuttingConcernsFilter : IBlogFormattingFilter
    {
        public string Process(string htmlContent)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // TODO: prepend link to existing blog somehow?
            // this is another manual step I do, but I think it might require more work than its worth
            // unless I can turn asciidoc meta data into html somehow that's stripped out later

            // update img src and add img-responsive class
            // so I don't have to do this manually
            var imgs = doc.DocumentNode.Descendants("img").ToList();
            if (imgs.Any())
            {
                foreach (var t in imgs)
                {
                    var existingSrc = t.Attributes["src"].Value;
                    // starting URL should be of the form: images/<filename>
                    // new URL should be of the form: https://crosscuttingconcerns.blob.core.windows.net/images/<filename>
                    var filenameonly = existingSrc.Replace("images/", "");
                    var newSrc = $"https://crosscuttingconcerns.blob.core.windows.net/images/{filenameonly}";
                    t.Attributes["src"].Value = newSrc;

                    Debugger.Launch();

                    // add img-responsive to css class if it's not already on there
                    var existingCssClasses = t.GetAttributeValue("class","");
                    var classes = existingCssClasses.Split(new []{' '},StringSplitOptions.RemoveEmptyEntries).ToList();
                    if(!classes.Contains("img-responsive"))
                        classes.Add("img-responsive");
                    t.SetAttributeValue("class", string.Join(" ", classes));
                }
            }

            return doc.DocumentNode.WriteContentTo();
        }
    }
}