using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace MakeCerts;

public class CertCommand : RootCommand
{
    public CertCommand()
    {
        Name = "mkcert";
        Description = "A tool for create self signed certificate";

        AddCommand(new GenCommand());
        AddCommand(new CaCommand());
        AddCommand(new InstallCommand());
        AddCommand(new UninstallCommand());
    }
}

public class GenCommand : Command
{
    public GenCommand() : this("g", "Generate a certificate")
    {
        AddOption(new Option<string>(["--output", "-o"], "File save directory location"));
        AddOption(new Option<uint>(["--year", "-y"], () => 1, "Certificate validity period (years)"));
        AddOption(new Option<string>(["--ca"], "CA certificate name"));
        AddOption(new Option<bool>(["--pfx"], "Whether to generate a pfx certificate"));
        AddOption(new Option<bool>(["--install", "-i"], "Whether to install to the system"));

        AddArgument(new Argument<string[]>("subjects", "DNS name of the certificate"));

        Handler = CommandHandler.Create(Handle);
    }

    public GenCommand(string name, string? description = null) : base(name, description)
    {
    }

    private void Handle(CertCommandOptions options)
    {
        try
        {
            CertCommandHandler.Generate(options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ {ex.Message}");
        }
    }
}

public class CaCommand : Command
{
    public CaCommand() : this("ca", "Generate a CA certificate")
    {
        AddOption(new Option<string>(["--output", "-o"], "File save directory location"));
        AddOption(new Option<uint>(["--year", "-y"], () => 10, "Certificate validity period (years)"));
        AddOption(new Option<bool>(["--pfx"], "Whether to generate a pfx certificate"));
        AddOption(new Option<bool>(["--install", "-i"], "Whether to install to the system"));

        AddArgument(new Argument<string>("subject", "DNS name of the certificate"));
    }

    public CaCommand(string name, string? description = null) : base(name, description)
    {
        Handler = CommandHandler.Create(Handle);
    }

    private void Handle(CaCertCommandOptions options)
    {
        try
        {
            CertCommandHandler.Generate(options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ {ex.Message}");
        }
    }
}

public class InstallCommand : Command
{
    public InstallCommand() : this("install", "Install the certificate")
    {
        AddAlias("i");

        AddOption(new Option<string>(["--type", "-t"], () => "pem", "Certificate Type. (pem or pfx)"));
        AddOption(new Option<string>(["--name", "-n"], "The name of the certificate to be saved"));
        AddOption(new Option<string>(["--file", "-f"], "Certificate pem or pfx file location") { IsRequired = true });
        AddOption(new Option<string>(["--key", "-k"], "Certificate key pem file location"));
        AddOption(new Option<string>(["--password", "-p"], "PFX certificate password if there is"));
        AddOption(new Option<bool>(["--ca", "-ca"], "Is it a certificate"));
    }

    public InstallCommand(string name, string? description = null) : base(name, description)
    {
        Handler = CommandHandler.Create(Handle);
    }

    private void Handle(InstallCommandOptions options)
    {
        try
        {
            CertCommandHandler.Install(options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ {ex.Message}");
        }
    }
}

public class UninstallCommand : Command
{
    public UninstallCommand() : this("uninstall", "remove the certificate from system")
    {
        AddOption(new Option<string>(["--name", "-n"], "The name of the certificate to be saved"));
    }

    public UninstallCommand(string name, string? description = null) : base(name, description)
    {
        Handler = CommandHandler.Create(Handle);
    }

    private void Handle(UninstallCommandOptions options)
    {
        try
        {
            CertCommandHandler.Uninstall(options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ {ex.Message}");
        }
    }
}
