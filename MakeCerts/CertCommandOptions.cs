using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeCerts;

public class CertCommandOptions
{
    public string Output { get; set; } = null!;

    public bool CA { get; set; }

    public string? RootCA { get; set; }

    public int Year { get; set; } = 20;
    public bool Install { get; set; }

    public string[] Subjects { get; set; } = null!;
}
