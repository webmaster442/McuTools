using MarkdownSharp;
using McuShell.Kernel;
using System;
using System.IO;
using System.Text;

namespace markdown
{
    class MARKDOWN
    {
        class Settings
        {
            [ParameterArgument(ShortName = "f", LongName = "file", Required = true, Description = "Input markdown file to process")]
            public string FileName { get; set; }

            [SwitchArgument(ShortName="d", LongName="document", Required = false, Description="Output a valid html document instead of raw code")]
            public bool HTMLFrame { get; set; }
        }

        static void Main(string[] args)
        {
            Settings settings = new Settings();
            CommandParser.CommandDescription = "Renders markdown files to html and writes the result to the standard output";
            CommandParser.Parse(settings, args);

            try
            {
                using (var text = File.OpenText(settings.FileName))
                {
                    string input = text.ReadToEnd();
                    Markdown renderer = new Markdown();
                    if (settings.HTMLFrame)
                    {
                        StringBuilder html = new StringBuilder();
                        html.Append(Properties.Resources.RawHTML);
                        html.Replace("{{title}}", Path.GetFileName(settings.FileName));
                        html.Replace("{{content}}", renderer.Transform(input));
                        Console.WriteLine(html.ToString());
                    }
                    else Console.WriteLine(renderer.Transform(input));
                }
            }
            catch (IOException ex)
            {
                CommandParser.Error(ex);
            }

#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
