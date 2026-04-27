using System.Net;

namespace AudioDownloader.Infrastructure.Services;

public interface INetworkSecurityValidator
{
    bool IsPublicIP(string host);
}

public class NetworkSecurityValidator : INetworkSecurityValidator
{
    public bool IsPublicIP(string host)
    {
        // Block localhost and variations
        if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase) ||
            host.EndsWith(".localhost", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Try to parse as IP address
        if (!IPAddress.TryParse(host, out var ipAddress))
        {
            // If it's not an IP address, it's a hostname - we allow it (DNS resolution will happen elsewhere)
            return true;
        }

        // Check for IP address ranges that should be blocked
        var bytes = ipAddress.GetAddressBytes();

        // IPv4
        if (bytes.Length == 4)
        {
            // Block 0.0.0.0/8 (except for 0.0.0.0 which is invalid anyway)
            if (bytes[0] == 0) return false;
            
            // Block 127.0.0.0/8 (loopback)
            if (bytes[0] == 127) return false;
            
            // Block 10.0.0.0/8 (private network)
            if (bytes[0] == 10) return false;
            
            // Block 172.16.0.0/12 (private network)
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return false;
            
            // Block 192.168.0.0/16 (private network)
            if (bytes[0] == 192 && bytes[1] == 168) return false;
            
            // Block 169.254.0.0/16 (link-local)
            if (bytes[0] == 169 && bytes[1] == 254) return false;
            
            // Block 224.0.0.0/4 (multicast)
            if (bytes[0] >= 224) return false;
            
            // Block 240.0.0.0/4 (reserved)
            if (bytes[0] >= 240) return false;
        }
        // IPv6 (basic checks)
        else if (bytes.Length == 16)
        {
            // Block ::1 (loopback)
            if (ipAddress.Equals(IPAddress.IPv6Loopback)) return false;
            
            // Block ::ffff:0:0/96 (IPv4-mapped addresses) - check the IPv4 part
            if (bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 0 && bytes[3] == 0 && 
                bytes[4] == 0 && bytes[5] == 0 && bytes[6] == 0 && bytes[7] == 0 && 
                bytes[8] == 0 && bytes[9] == 0 && bytes[10] == 0xff && bytes[11] == 0xff)
            {
                // Recursively check the IPv4 part
                var ipv4Bytes = new byte[] { bytes[12], bytes[13], bytes[14], bytes[15] };
                var ipv4Address = new IPAddress(ipv4Bytes);
                return IsPublicIP(ipv4Address.ToString());
            }
            
            // Block local unicast (fc00::/7)
            if ((bytes[0] & 0xfe) == 0xfc) return false;
            
            // Block link-local (fe80::/10)
            if ((bytes[0] & 0xc0) == 0x80) return false;
            
            // Block site-local (fec0::/10) - deprecated but still check
            if ((bytes[0] & 0xc0) == 0xc0) return false;
            
            // Block multicast (ff00::/8)
            if (bytes[0] == 0xff) return false;
        }

        // If we got here, it's considered a public IP
        return true;
    }
}