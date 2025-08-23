using System.CommandLine;

namespace MakeCerts;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        return await new CertCommand().InvokeAsync(args);
    }
}
