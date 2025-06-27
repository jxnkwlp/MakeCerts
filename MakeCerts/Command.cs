namespace MakeCerts
{ 
    //public static class CertCommand
    //{
    //public static Command GetCommand()
    //{
    //    var command = new Command("ca");

    //    command.AddOption(new Option<string>(["--name", "-n"], () => Environment.MachineName));
    //    command.AddOption(new Option<int>(["--year", "-y"], () => 20));
    //    command.AddOption(new Option<bool>(["--install", "-i"], () => true));

    //    command.Handler = CommandHandler.Create<CaCommandOptions>(options =>
    //    {
    //        var cert = CertManager.GenerateRootCA(options.Name);

    //        try
    //        {
    //            CertManager.Save(cert, Environment.CurrentDirectory);

    //            if (options.Install)
    //            {
    //                if (CertManager.IsExists(StoreName.Root, cert.FriendlyName))
    //                {
    //                    Console.WriteLine($"The root ca {options.Name} is installed ");
    //                    return;
    //                }
    //                else
    //                {
    //                    CertManager.Install(StoreName.Root, cert);
    //                }
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }
    //    });

    //    return command;
    //}
    //public static Command GetCaCommand()
    //{
    //    var command = new Command("ca");

    //    command.AddOption(new Option<string>(["--name", "-n"], () => Environment.MachineName));
    //    command.AddOption(new Option<int>(["--year", "-y"], () => 20));
    //    command.AddOption(new Option<bool>(["--install", "-i"], () => true));

    //    command.Handler = CommandHandler.Create<CaCommandOptions>(options =>
    //    {
    //        var cert = CertManager.GenerateRootCA(options.Name);

    //        try
    //        {
    //            CertManager.Save(cert, Environment.CurrentDirectory);

    //            if (options.Install)
    //            {
    //                if (CertManager.IsExists(StoreName.Root, cert.FriendlyName))
    //                {
    //                    Console.WriteLine($"The root ca {options.Name} is installed ");
    //                    return;
    //                }
    //                else
    //                {
    //                    CertManager.Install(StoreName.Root, cert);
    //                }
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }
    //    });

    //    return command;
    //}

    //public static Command GetGenCommand()
    //{
    //    var command = new Command("gen");

    //    command.Handler = CommandHandler.Create<GenCommandOptions>(options =>
    //    {

    //    });

    //    return command;
    //}
    //}

    //public class CaCommandOptions
    //{
    //    public string? Output { get; set; }
    //    public string Name { get; set; } = Environment.MachineName;
    //    public int? Year { get; set; } = 20;
    //    public bool Install { get; set; } = true;
    //}

    //public class GenCommandOptions
    //{
    //    public string? Output { get; set; }

    //    public bool Ca { get; set; }

    //    public FileInfo? CaFile { get; set; }
    //    public string? CaPassword { get; set; }

    //    public int? Year { get; set; } = 20;

    //    public bool Install { get; set; }

    //    public string[] Subjects { get; set; } = null!;
    //}


    //[CliCommand(Description = "A tool for create self signed certificate", Name = "mkcert")]
    //public class Command
    //{
    //    [CliOption(Required = false)]
    //    public string? Output { get; set; }

    //    public void Run()
    //    {
    //    }


    //    [CliCommand]
    //    public class CaCommand
    //    {
    //        [CliOption]
    //        public string Name { get; set; } = Environment.MachineName;

    //        [CliOption]
    //        public int? Year { get; set; } = 20;

    //        [CliOption]
    //        public bool Install { get; set; } = true;


    //        public void Run()
    //        {
    //            var cert = CertManager.GenerateRootCA(Name);

    //            try
    //            {
    //                CertManager.Save(cert, Environment.CurrentDirectory);

    //                if (Install)
    //                {
    //                    if (CertManager.IsExists(StoreName.Root, cert.FriendlyName))
    //                    {
    //                        Console.WriteLine($"The root ca {Name} is installed ");
    //                        return;
    //                    }
    //                    else
    //                    {
    //                        CertManager.Install(StoreName.Root, cert);
    //                    }
    //                }
    //            }
    //            catch (Exception)
    //            {
    //                throw;
    //            }
    //        }
    //    }

    //    [CliCommand]
    //    public class GenCommand
    //    {
    //        [CliOption]
    //        public string Name { get; set; } = Environment.MachineName;

    //        [CliOption(Required = false)]
    //        public FileInfo? RootFile { get; set; }

    //        [CliOption(Required = false)]
    //        public string? RootPassword { get; set; }

    //        [CliOption(Required = true)]
    //        public string[] Subjects { get; set; } = null!;

    //        [CliOption]
    //        public int? Year { get; set; } = 20;

    //        [CliOption]
    //        public bool Install { get; set; }

    //        public void Run()
    //        {
    //            X509Certificate2? rootCa = null;
    //            if (RootFile?.Exists == true)
    //            {
    //                rootCa = new X509Certificate2(RootFile.FullName, this.RootPassword);
    //            }

    //            var cert = CertManager.GenerateCert(Name, Subjects[0], Subjects.Take(1).ToArray(), rootCa);

    //            try
    //            {
    //                CertManager.Save(cert, Environment.CurrentDirectory);

    //                if (Install)
    //                {
    //                    if (CertManager.IsExists(StoreName.My, cert.FriendlyName))
    //                    {
    //                        Console.WriteLine($"The root ca {Name} is installed ");
    //                        return;
    //                    }
    //                    else
    //                    {
    //                        CertManager.Install(StoreName.My, cert);
    //                    }
    //                }
    //            }
    //            catch (Exception)
    //            {
    //                throw;
    //            }
    //        }
    //    }
    //}
}
