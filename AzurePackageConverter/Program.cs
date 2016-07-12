using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CommandLine;

namespace AzurePackageConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            var options = new Options();

            if (Parser.Default.ParseArguments(args, options))
            {
                string packagePath = Path.Combine(options.PathToPackage, "package.xml");
                if (File.Exists(packagePath))
                {
                    Console.WriteLine($"Loading {packagePath}");
                    var package = XDocument.Load(packagePath);

                    XNamespace wa = "http://schemas.microsoft.com/windowsazure";

                    var layoutDefs = package.Descendants(wa + "LayoutDefinition");

                    string convertedPackage = $"{options.PathToPackage}-Parsed";

                    foreach (XElement layoutDefinition in layoutDefs)
                    {
                        string roleName = layoutDefinition.Element(wa + "Name").Value;

                        Console.WriteLine($"Parsing {roleName}");

                        string rolePath = Path.Combine(convertedPackage, roleName.Replace('/', Path.DirectorySeparatorChar));

                        if (!Directory.Exists(rolePath))
                        {
                            Console.WriteLine($"Creating directory for conversion: {rolePath}");
                            Directory.CreateDirectory(rolePath);
                        }
                        else
                        {
                            Console.WriteLine($"Path already exists! {rolePath}");
                        }

                        foreach (var fileDefinition in layoutDefinition.Descendants(wa + "FileDefinition"))
                        {
                            string fileName = fileDefinition.Element(wa + "FilePath").Value;
                            if (fileName.StartsWith("\\"))
                            {
                                fileName = fileName.Substring(1);
                            }

                            Console.WriteLine($"Creating file: {fileName}");
                            string packagedFile = fileDefinition.Descendants(wa + "DataContentReference").First().Value;

                            string filePath = Path.Combine(rolePath, fileName);
                            if (!File.Exists(filePath))
                            {
                                string directoryPath = Path.GetDirectoryName(filePath);

                                if (!Directory.Exists(directoryPath))
                                {
                                    Console.WriteLine($"Creating folder {directoryPath}");
                                    Directory.CreateDirectory(directoryPath);
                                }

                                File.Copy(Path.Combine(options.PathToPackage, packagedFile), filePath);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Unable to locate package.xml file at {options.PathToPackage}");
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
