using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Extensions;

internal static class IPAddressExtensions
{
    public static bool IsPortOpen(this IPAddress address, int port)
    {
        try
        {
            using (var client = new TcpClient())
            {
                client.Connect(address, port);
                return true;
            }
        }
        catch (SocketException)
        {
            // Port is not open
            return false;
        }
    }
}
