using System;
using System.Diagnostics;
using System.IO;

namespace adclean
{
    class Program
    {
        const string HELP_MESSAGE = @"

            ADClean: Runs asciidoctor on the given adoc file
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
            var newHtmlFile = Path.ChangeExtension(originalFullPath, "html");

            // which needs to be run through the cleaner
            var originalFilename = Path.GetFileName(newHtmlFile);
            var newFilename = "clean_" + originalFilename;
            var newPathRoot = Path.GetPathRoot(originalFullPath);
            var newFullPath = Path.Combine(newPathRoot, newFilename);

            var cleaner = new Cleaner(newHtmlFile);
            cleaner.OutputTo(newFullPath);

            // finally, open it up in Notepad++
            var notepadCommand = "notepad++ " + newFullPath;
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
