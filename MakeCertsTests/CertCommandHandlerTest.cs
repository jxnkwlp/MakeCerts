using MakeCerts;
using Shouldly;

namespace MakeCertsTests;

public class CertCommandHandlerTest
{
    [Theory]
    [InlineData("myca", false, 1, "", false)]
    [InlineData("myca2", true, 2, "certs", false)]
    public void GenerateCA(string subject, bool pfx, uint year, string output, bool install)
    {
        var result = CertCommandHandler.Generate(new CaCertCommandOptions()
        {
            Subject = subject,
            Year = year,
            Pfx = pfx,
            Install = install,
            Output = output,
        });

        result.ShouldNotBeNull();
        result.Certificate.ShouldNotBeNull();

        result.Certificate.Subject.ShouldBe($"CN={subject} Root CA, OU={subject}, O={Environment.UserName}");
        result.Certificate.NotAfter.Date.ShouldBe(DateTime.Now.AddYears((int)year).Date);

        File.Exists(Path.Combine(result.OutputDir, $"{subject}.crt")).ShouldBeTrue();
        File.Exists(Path.Combine(result.OutputDir, $"{subject}.key")).ShouldBeTrue();
        File.Exists(Path.Combine(result.OutputDir, $"{subject}.pub")).ShouldBeTrue();

        if (pfx)
            File.Exists(Path.Combine(result.OutputDir, $"{subject}.pfx")).ShouldBeTrue();
    }

    [Theory]
    [InlineData(new[] { "mysite.com" }, false, 1, "", false)]
    [InlineData(new[] { "mysite.com", "localhost", "10.2.3.4" }, true, 2, "certs", false)]
    [InlineData(new[] { "*.mysite.com" }, true, 2, "certs", false)]
    public void Generate(string[] subjects, bool pfx, uint year, string output, bool install)
    {
        var result = CertCommandHandler.Generate(new CertCommandOptions()
        {
            Subjects = subjects,
            Year = year,
            Pfx = pfx,
            Install = install,
            Output = output,
        });

        result.ShouldNotBeNull();
        result.Certificate.ShouldNotBeNull();

        result.Subject.ShouldBe(result.Subject);

        result.Certificate.NotAfter.Date.ShouldBe(DateTime.Now.AddYears((int)year).Date);

        if (subjects[0].StartsWith("*"))
            result.FileName.ShouldBe(subjects[0].Replace("*", "_wildcard").ToLowerInvariant());
        else
            result.FileName.ShouldBe(subjects[0].ToLowerInvariant());

        File.Exists(Path.Combine(result.OutputDir, $"{result.FileName}.crt")).ShouldBeTrue();
        File.Exists(Path.Combine(result.OutputDir, $"{result.FileName}.key")).ShouldBeTrue();
        File.Exists(Path.Combine(result.OutputDir, $"{result.FileName}.pub")).ShouldBeTrue();

        if (pfx)
            File.Exists(Path.Combine(result.OutputDir, $"{result.FileName}.pfx")).ShouldBeTrue();
    }
}
