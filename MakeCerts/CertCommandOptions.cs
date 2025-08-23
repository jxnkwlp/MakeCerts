namespace MakeCerts;

public class CertCommandOptions
{
    public string[] Subjects { get; set; } = null!;

    /// <summary>
    ///  CA certificate name
    /// </summary>
    public string? CA { get; set; }

    /// <summary>
    ///  File save directory location
    ///  Default is current directory
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    ///  Certificate validity period (years)
    ///  Default 2
    /// </summary>
    public uint Year { get; set; }

    /// <summary>
    ///  Indicates whether to install
    /// </summary>
    public bool Install { get; set; }

    /// <summary>
    ///  Indicates whether to generate a PFX file
    /// </summary>
    public bool? Pfx { get; set; }

}

public class CaCertCommandOptions
{
    /// <summary>
    ///  The cert file output
    ///  Default is current directory
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    ///  The certificate validity period (years) 
    /// </summary>
    public uint Year { get; set; }

    /// <summary>
    ///  Indicates whether to install
    ///  Default is true
    /// </summary>
    public bool Install { get; set; }

    /// <summary>
    ///  Indicates whether to generate a PFX file
    /// </summary>
    public bool? Pfx { get; set; }

    public string Subject { get; set; } = null!;
}

public class InstallCommandOptions
{
    public string Type { get; set; } = null!;

    public string? Name { get; set; }

    public string File { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string? Password { get; set; }

    public bool Ca { get; set; }

}

public class UninstallCommandOptions
{
    public string Name { get; set; } = null!;
}
