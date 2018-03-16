using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HtmlAgilityPack;

namespace adclean
{
    class Program
    {
        const string HELP_MESSAGE = @"

            ADClean 2: Runs asciidoctor on the given adoc file
            Then runs it through my custom HTML cleanup for Couchbase blogging
            Then opens the result in Notepad++

            Usage: adclean [filename] <OPTIONS>

            [filename] is an asciidoc file (defaults to blogpost.adoc)

            Options:
                --debug launches debugger
                --help shows this message
        ";

        static void Main(string[] args)
        {
            string originalFullPath = "";

            // if no string specified, assume blogpost.adoc
            var allArgs = "";
            if (args != null)
                allArgs = string.Join(" ", args).Trim();

            if (allArgs == "" || allArgs == "--debug" || allArgs == "--help")
                originalFullPath = ".\\blogpost.adoc";
            else
                originalFullPath = args[0];

            if(allArgs.Contains("--debug"))
                Debugger.Launch();

            if (allArgs.Contains("--help"))
            {
                Console.WriteLine(HELP_MESSAGE);
                return;
            }

            // check to make sure file exists
            if (!File.Exists(originalFullPath))
            {
                Console.WriteLine($"File '{originalFullPath}' not found.");
                Console.WriteLine(HELP_MESSAGE);
                return;
            }

            // run asciidoctor command line
            var asciidoctorCommand = "asciidoctor " + originalFullPath;
            var result = ExecuteCommandLine(asciidoctorCommand);
            
            // make sure asciidoctor is installed/in path
            if (result.Contains("is not recognized as an internal"))
            {
                Console.WriteLine("You must have asciidoctor installed (and in the path, probably)");
                return;
            }

            // this should result in filename.html 
            // load the contents of that file
            var newHtmlFile = Path.ChangeExtension(originalFullPath, "html");
            var newPathRoot = Path.GetPathRoot(originalFullPath);
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(newHtmlFile);

            // run it through asciidoc cleaner
            var cleaner = new CleanUpAsciiDoctorGeneratedHtml();
            var cleanedContent = cleaner.Process(htmlDocument);

            // now run it through each blog platform specific filter
            IBlogFormattingFilter wordpressFilter = new WordPressFilter();
            var wordpressContent = wordpressFilter.Process(cleanedContent);
            var wordpressFilterFullPath = Path.Combine(newPathRoot, "blogpost_wordpress.html");
            File.WriteAllText(wordpressFilterFullPath, wordpressContent);

            IBlogFormattingFilter cccFilter = new CrossCuttingConcernsFilter();
            var cccContent = cccFilter.Process(cleanedContent);
            var cccFilterFullPath = Path.Combine(newPathRoot, "blogpost_ccc.html");
            File.WriteAllText(cccFilterFullPath, cccContent);

            // finally, open them up in Notepad++
            var notepadCommand = "notepad++ " + wordpressFilterFullPath;
            ExecuteCommandLine(notepadCommand);
            notepadCommand = "notepad++ " + cccFilterFullPath;
            ExecuteCommandLine(notepadCommand);
        }

        // returns error (if there is any)
        private static string ExecuteCommandLine(string command, bool createNoWindow = true)
        {
            var processStartInfo = new ProcessStartInfo("CMD.exe", "/c " + command);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = createNoWindow;
            var proc = new Process();
            proc.StartInfo = processStartInfo;
            proc.Start();
            return proc.StandardError.ReadToEnd();
        }
    }
}
