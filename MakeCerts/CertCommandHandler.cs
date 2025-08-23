using System.Security.Cryptography.X509Certificates;

namespace MakeCerts;

public class CertCommandHandler
{
    public static CertGenerateCommandHandleResult? Generate(CertCommandOptions options)
    {
        var subjects = options.Subjects?.Select(x => x.Trim())?.Distinct()?.ToArray();

        if (subjects?.Any() != true)
        {
            Console.WriteLine("❌ Please enter the host name");
            return null;
        }

        //foreach (var item in subjects)
        //{
        //    if (!item.IsHostName() && !IPAddress.TryParse(item, out var _))
        //    {
        //        Console.WriteLine($"❌ The host name or ip address '{item}' is invalid");
        //        return null;
        //    }
        //}

        Console.WriteLine($"💦 Start generating certificate for {string.Join(", ", subjects)} ");

        X509Certificate2? rootCa = null;

        if (!string.IsNullOrWhiteSpace(options.CA))
        {
            var cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mkcert");
            var certFile = Path.Combine(cacheDir, $"{options.CA}.crt");
            var certKeyFile = Path.Combine(cacheDir, $"{options.CA}.key");

            if (!File.Exists(certFile))
            {
                Console.WriteLine($"❌ Root certificate '{options.CA}' not found. Did you miss installing it?");
                return null;
            }
            if (!File.Exists(certKeyFile))
            {
                Console.WriteLine($"❌ Root certificate key file not found. Did you miss installing it?");
                return null;
            }

            rootCa = CertManager.LoadPemFile(certFile, certKeyFile);

        }

        var certName = subjects[0].ToCertName();
        var subject = CertManager.GetSubjectName(subjects, rootCa);

        if (options.Install && CertManager.IsExists(StoreName.My, certName))
        {
            Console.WriteLine($"❌ Certificate '{certName}' is already installed");

            return null;
        }

        var savedDir = Path.Combine(Environment.CurrentDirectory, options.Output ?? string.Empty);

        var cert = CertManager.GenerateCert(subjects, options.Year, rootCa);

        CertManager.Save(certificate: cert, saveDir: savedDir, fileName: certName, pfx: options.Pfx ?? false);

        Console.WriteLine($"🎉 Certificate '{certName}' saved to {savedDir}");

        if (options.Install)
        {
            CertManager.Install(StoreName.My, cert);
            Console.WriteLine($"🔐 Certificate '{certName}' has been install. ");
        }

        return new CertGenerateCommandHandleResult(cert, savedDir, certName, subject);
    }

    public static CertGenerateCommandHandleResult? Generate(CaCertCommandOptions options)
    {
        string subject = options.Subject;

        if (string.IsNullOrWhiteSpace(subject))
        {
            Console.WriteLine("❌ Please enter the subject name");
            return null;
        }

        if (!subject.IsHostName())
        {
            Console.WriteLine($"❌ Invalid hostname '{subject}'");
            return null;
        }

        Console.WriteLine($"💦 Start generating certificate for '{subject}'");

        var certName = subject.ToCertName();

        if (options.Install && CertManager.IsExists(StoreName.Root, certName))
        {
            Console.WriteLine($"❌ Certificate '{certName}' is already installed");
            return null;
        }

        var cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mkcert");
        var savedDir = Path.Combine(Environment.CurrentDirectory, options.Output ?? string.Empty);

        var cert = CertManager.GenerateRootCA(subject, options.Year);

        CertManager.Save(cert, cacheDir, certName);
        CertManager.Save(certificate: cert, saveDir: savedDir, fileName: certName, pfx: options.Pfx ?? false);

        Console.WriteLine($"🎉 Certificate '{certName}' saved to {savedDir}");

        if (options.Install)
        {
            CertManager.Install(StoreName.Root, cert);
            Console.WriteLine($"🔐 Certificate '{certName}' has been install. ");
        }

        return new CertGenerateCommandHandleResult(cert, savedDir, certName, subject);
    }

    public static void Install(InstallCommandOptions options)
    {
        var file = Path.Combine(Environment.CurrentDirectory, options.File ?? string.Empty);

        if (!File.Exists(file))
        {
            Console.WriteLine($"❌ File '{file}' not found");
            return;
        }

        X509Certificate2 certificate;

        if (options.Type == "pem")
        {
            string? keyFile = Path.Combine(Environment.CurrentDirectory, Path.ChangeExtension(file, ".key"));

            if (!string.IsNullOrWhiteSpace(options.Key))
            {
                keyFile = Path.Combine(Environment.CurrentDirectory, options.Key);
            }

            if (!File.Exists(keyFile))
            {
                Console.WriteLine($"❌ Key file '{Path.GetFileName(keyFile)}' not found");
                return;
            }

            Console.WriteLine($"💦 Loading key '{Path.GetFileName(keyFile)}' ... ");

            // certificate = X509CertificateLoader.LoadCertificateFromFile(file, options.Password);
            certificate = CertManager.LoadPemFile(file, keyFile);
        }
        else if (options.Type == "pfx")
        {
#if NET9_0_OR_GREATER
            certificate = X509CertificateLoader.LoadPkcs12FromFile(file, options.Password);
#else
            certificate = new X509Certificate2(file, options.Password);
#endif
        }
        else
        {
            Console.WriteLine("❌ Only supprt pfx or pem format");
            return;
        }

        var certSubject = CertSubject.Parse(certificate.Subject);
        var certName = certSubject.CN;

        if (!string.IsNullOrWhiteSpace(options.Name))
            certName = options.Name;

        CertManager.Install(options.Ca ? StoreName.Root : StoreName.My, certificate);

        if (options.Ca)
        {
            var cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mkcert");
            CertManager.Save(certificate, cacheDir, certName);
        }

        Console.WriteLine($"🔐 Certificate '{certificate.SubjectName.Name}' has been install. ");
    }

    public static void Uninstall(UninstallCommandOptions options)
    {
        var cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mkcert");

        DeleteFile(Path.Combine(cacheDir, $"{options.Name.ToLowerInvariant()}.crt"));
        DeleteFile(Path.Combine(cacheDir, $"{options.Name.ToLowerInvariant()}.key"));
        DeleteFile(Path.Combine(cacheDir, $"{options.Name.ToLowerInvariant()}.pub"));

        CertManager.Remove(StoreName.My, options.Name);
        CertManager.Remove(StoreName.Root, options.Name);
    }

    private static bool DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        return false;
    }
}

public class CertGenerateCommandHandleResult
{
    public CertGenerateCommandHandleResult(X509Certificate2 certificate, string outputDir, string fileName, string subject)
    {
        Certificate = certificate;
        OutputDir = outputDir;
        FileName = fileName;
        Subject = subject;
    }

    public X509Certificate2 Certificate { get; }
    public string OutputDir { get; }
    public string FileName { get; }
    public string Subject { get; }
}
