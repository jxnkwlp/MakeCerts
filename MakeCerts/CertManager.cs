using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MakeCerts;

public static class CertManager
{
    public static void Install(StoreName storeName, X509Certificate2 certificate)
    {
        using (var store = new X509Store(storeName, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
        {
            store.Add(certificate);
        }
    }

    public static void Remove(StoreName storeName, string name)
    {
        using (var store = new X509Store(storeName, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
        {
            var find = store.Certificates.FirstOrDefault(x => x.FriendlyName == name);

            if (find != null)
            {
                store.Remove(find);
            }
        }
    }

    public static bool IsExists(StoreName storeName, string name)
    {
        using (var store = new X509Store(storeName, StoreLocation.CurrentUser, OpenFlags.ReadOnly))
        {
            return store.Certificates.Any(x => x.FriendlyName == name);
        }
    }

    public static X509Certificate2? Find(StoreName storeName, string name)
    {
        using (var store = new X509Store(storeName, StoreLocation.CurrentUser, OpenFlags.ReadOnly))
        {
            return store.Certificates.FirstOrDefault(x => x.FriendlyName == name);
        }
    }

    public static void Save(string rootDir, X509Certificate2 certificate, string? fileName, X509Certificate2? root)
    {
        if (!Directory.Exists(rootDir))
            Directory.CreateDirectory(rootDir);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = certificate.FriendlyName;
            if (fileName.StartsWith("*"))
                fileName = fileName.Replace("*", "_wildcard");
        }

        var password = Random.Shared.Next(100000, 999999).ToString();

        File.WriteAllBytes(Path.Combine(rootDir, fileName + ".pdfx"), certificate.Export(X509ContentType.Pfx, password));
        File.WriteAllText(Path.Combine(rootDir, fileName + " password.txt"), password);

        var key = certificate.GetRSAPrivateKey();

        File.WriteAllText(Path.Combine(rootDir, fileName + ".key"), string.Concat(PemEncoding.Write("PRIVATE KEY", key!.ExportPkcs8PrivateKey())));
        File.WriteAllText(Path.Combine(rootDir, fileName + ".pub"), string.Concat(PemEncoding.Write("PUBLIC KEY", key.ExportSubjectPublicKeyInfo())));

        //if (root != null)
        //{
        //    File.WriteAllText(Path.Combine(rootDir, fileName + ".crt"), string.Concat(PemEncoding.Write("CERTIFICATE", certificate.RawData)));
        //    File.AppendAllText(Path.Combine(rootDir, fileName + ".crt"), "\n");
        //    File.AppendAllText(Path.Combine(rootDir, fileName + ".crt"), string.Concat(PemEncoding.Write("CERTIFICATE", root.RawData)));
        //}
        //else
        //{
        File.WriteAllText(Path.Combine(rootDir, fileName + ".crt"), string.Concat(PemEncoding.Write("CERTIFICATE", certificate.RawData)));
        //}
    }

    public static X509Certificate2 GenerateRootCA(string subject, int year = 10)
    {
        var rsa = RSA.Create(keySizeInBits: 4096);
        var req = new CertificateRequest($"CN={subject} Root CA, OU={subject}, O={Environment.UserName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        req.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, false));
        req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, false));
        // req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1"), new Oid("1.3.6.1.5.5.7.3.2") }, false));
        req.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(req.PublicKey, false));
        SubjectAlternativeNameBuilder sanBuilder = new SubjectAlternativeNameBuilder();
        AddAlises(req, [subject]);

        var cert = req.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.Now.AddYears(year));

#pragma warning disable CA1416 // Validate platform compatibility
        cert.FriendlyName = subject;
#pragma warning restore CA1416 // Validate platform compatibility

        return cert;
    }

    public static X509Certificate2 GenerateCert(string subject, string[] alises, int year = 5, X509Certificate2? rootCa = null)
    {
        var rsa = RSA.Create(keySizeInBits: 4096);

        string subjectName = $"CN={subject}";
        if (rootCa != null)
        {
            var rootSubject = CertSubject.Parse(rootCa.Subject);

            subjectName += $", OU={rootSubject.OU ?? rootSubject.CN}, O={Environment.UserName},";
        }

        var req = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        req.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));
        req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection {
            new Oid("1.3.6.1.5.5.7.3.1"),
           // new Oid("1.3.6.1.5.5.7.3.2")
        }, false));
        req.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

        AddAlises(req, alises.Concat([subject]).ToArray());

        X509Certificate2 cert = null!;

        if (rootCa != null)
        {
            Span<byte> span = stackalloc byte[8];
            RandomNumberGenerator.Fill(span);

            foreach (var item in rootCa.Extensions)
            {
                if (item.Oid?.Value == "2.5.29.14") //  "Subject Key Identifier"
                {
                    var issuerSubjectKey = item.RawData;
                    //var issuerSubjectKey = signingCertificate.Extensions["Subject Key Identifier"].RawData;
                    var segment = new ArraySegment<byte>(issuerSubjectKey, 2, issuerSubjectKey.Length - 2);
                    var authorityKeyIdentifier = new byte[segment.Count + 4];
                    // "KeyID" bytes
                    authorityKeyIdentifier[0] = 0x30;
                    authorityKeyIdentifier[1] = 0x16;
                    authorityKeyIdentifier[2] = 0x80;
                    authorityKeyIdentifier[3] = 0x14;
                    segment.CopyTo(authorityKeyIdentifier, 4);
                    req.CertificateExtensions.Add(new X509Extension("2.5.29.35", authorityKeyIdentifier, false));
                    break;
                }
            }

            cert = req.Create(rootCa, DateTimeOffset.UtcNow, DateTimeOffset.Now.AddYears(year), span).CopyWithPrivateKey(rsa);
        }
        else
        {
            cert = req.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.Now.AddYears(year));
        }

#pragma warning disable CA1416 // Validate platform compatibility
        cert.FriendlyName = subject;
#pragma warning restore CA1416 // Validate platform compatibility

        return cert;
    }

    static void AddAlises(CertificateRequest request, params string[] alises)
    {
        SubjectAlternativeNameBuilder sanBuilder = new SubjectAlternativeNameBuilder();
        foreach (var item in alises)
        {
            if (IPAddress.TryParse(item, out var ip))
            {
                sanBuilder.AddIpAddress(ip);
            }
            else
            {
                sanBuilder.AddDnsName(item);
            }
        }
        request.CertificateExtensions.Add(sanBuilder.Build());
    }
}

public class Cert
{
    public Cert(X509Certificate2 certificate, string password)
    {
        Certificate = certificate;
        Password = password;
    }

    public X509Certificate2 Certificate { get; }
    public string Password { get; }
}

public class CertSubject
{
    public CertSubject(string cN)
    {
        CN = cN;
    }

    public string CN { get; } = null!;
    public string? OU { get; set; }
    public string? O { get; set; }
    public string? L { get; set; }
    public string? S { get; set; }
    public string? C { get; set; }

    public static CertSubject Parse(string value)
    {
        var data = value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split('=', StringSplitOptions.RemoveEmptyEntries)).ToDictionary(x => x[0], x => x[1], StringComparer.InvariantCultureIgnoreCase);

        return new CertSubject(data["CN"])
        {
            OU = data.ContainsKey("OU") ? data["OU"] : null,
            O = data.ContainsKey("O") ? data["O"] : null,
            L = data.ContainsKey("L") ? data["L"] : null,
            S = data.ContainsKey("S") ? data["S"] : null,
            C = data.ContainsKey("C") ? data["C"] : null,
        };
    }

    public string Build()
    {
        StringBuilder sb = new StringBuilder("CN=" + CN);

        if (!string.IsNullOrWhiteSpace(OU)) sb.Append(", OU=" + OU);
        if (!string.IsNullOrWhiteSpace(O)) sb.Append(", O=" + O);
        if (!string.IsNullOrWhiteSpace(L)) sb.Append(", L=" + L);
        if (!string.IsNullOrWhiteSpace(S)) sb.Append(", S=" + S);
        if (!string.IsNullOrWhiteSpace(C)) sb.Append(", C=" + C);


        return sb.ToString();
    }
}
