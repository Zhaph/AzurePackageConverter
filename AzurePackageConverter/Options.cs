using CommandLine;
using CommandLine.Text;

namespace AzurePackageConverter
{
    public class Options
    {
        [Option('p', "Path", Required = true, HelpText = "Path to unpacked package")]
        public string PathToPackage { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

    }
}
