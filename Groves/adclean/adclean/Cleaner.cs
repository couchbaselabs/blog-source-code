using HtmlAgilityPack;
using System.IO;
using System.Linq;
using System.Text;

namespace adclean
{
    public class Cleaner
    {
        readonly string _filename;

        public Cleaner(string filename)
        {
            _filename = filename;
        }

        public void OutputTo(string filenameOutput)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(_filename);

            // get just the content from inside <div id="content"> ... </div>
            var content = htmlDocument.DocumentNode.Descendants("div")
                .ToList()
                .Where(d => d.Attributes["id"].Value == "content")
                .First()
                .WriteContentTo();

            // replace <pre class="highlight"> with <pre class="highlight decode:true">
            content = content.Replace("<pre class=\"highlight\">", "<pre class=\"highlight decode:true\">");

            var sb = new StringBuilder();
            sb.Append(content);

            // overwrite existing file
            File.Delete(filenameOutput);
            File.WriteAllText(filenameOutput, sb.ToString());
        }
    }
}
