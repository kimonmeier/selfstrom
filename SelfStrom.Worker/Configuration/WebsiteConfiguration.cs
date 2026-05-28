using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Configuration;

internal class WebsiteConfiguration
{
    public string BaseUrl { get; set; } = null!;

    public string ApiKey { get; set; } = null!;
}
