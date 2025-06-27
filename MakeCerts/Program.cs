using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Security.Cryptography.X509Certificates;

namespace MakeCerts
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var command = new RootCommand("A tool for create self signed certificate")
            {
                Name = "mkcert",
            };

            command.AddGlobalOption(new Option<string>(["--output", "-o"], () => "certs"));

            command.AddOption(new Option<int>(["--year", "-y"], () => 5));
            command.AddOption(new Option<bool>(["--install", "-i"]));

            command.AddOption(new Option<bool>(["--ca"]));

            command.AddOption(new Option<string>(["--root-ca", "-root"])); 

            command.AddArgument(new Argument<string[]>("subjects"));

            command.Handler = CommandHandler.Create<CertCommandOptions>(options =>
            {
                if (options.Subjects?.Any() != true)
                {
                    return;
                }

                Console.WriteLine($"Start to generate cert for {string.Join(",", options.Subjects)} ");

                Thread.Sleep(500);

                StoreName storeName = options.CA ? StoreName.Root : StoreName.My;

                X509Certificate2 cert = null;

                X509Certificate2? rootCa = null;

                var rootcaDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mkcert");

                if (options.CA)
                {
                    cert = CertManager.GenerateRootCA(options.Subjects.First(), options.Year);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(options.RootCA))
                    {
                        var caCrtFile = Path.Combine(rootcaDir, options.RootCA + ".crt");
                        var caKeyFile = Path.Combine(rootcaDir, options.RootCA + ".key");
                        if (!File.Exists(caCrtFile))
                        {
                            Console.WriteLine($"The root ca crt file '{caCrtFile}' not exits");
                            return;
                        }
                        if (!File.Exists(caKeyFile))
                        {
                            Console.WriteLine($"The root ca key file {caKeyFile} not exits");
                            return;
                        }

                        rootCa = X509Certificate2.CreateFromPemFile(caCrtFile, caKeyFile);
                    }

                    cert = CertManager.GenerateCert(options.Subjects[0], options.Subjects.Skip(1).ToArray(), 2, rootCa);
                }

                if (options.CA)
                {
                    CertManager.Save(rootcaDir, cert, "rootCA", null);
                    CertManager.Save(rootcaDir, cert, options.Subjects[0], null);
                }

                CertManager.Save(Path.Combine(Environment.CurrentDirectory, options.Output), cert, null, rootCa);

                Console.WriteLine($"The cert {cert.FriendlyName} saved in {Path.Combine(Environment.CurrentDirectory, options.Output)} ");

                if (options.Install)
                {
                    if (CertManager.IsExists(storeName, cert.FriendlyName))
                    {
                        Console.WriteLine($"The cert {cert.FriendlyName} is installed ");
                    }
                    else
                    {
                        CertManager.Install(storeName, cert);

                        Console.WriteLine($"The cert {cert.FriendlyName} has been install. ");
                    }
                }
            });

            return command.Invoke(args);
        }
    }
}
