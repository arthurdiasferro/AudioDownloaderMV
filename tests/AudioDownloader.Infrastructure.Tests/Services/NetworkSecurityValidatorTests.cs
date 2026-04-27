using AudioDownloader.Infrastructure.Services;

namespace AudioDownloader.Infrastructure.Tests.Services;

public class NetworkSecurityValidatorTests
{
    private readonly INetworkSecurityValidator _validator = new NetworkSecurityValidator();

    [Fact]
    public void IsPublicIP_ShouldBlockLocalhost()
    {
        Assert.False(_validator.IsPublicIP("localhost"));
        Assert.False(_validator.IsPublicIP("LOCALHOST"));
        Assert.False(_validator.IsPublicIP("LocalHost"));
    }

    [Fact]
    public void IsPublicIP_ShouldBlockLocalhostDomains()
    {
        Assert.False(_validator.IsPublicIP("test.localhost"));
        Assert.False(_validator.IsPublicIP("sub.test.localhost"));
    }

    [Fact]
    public void IsPublicIP_ShouldBlockLoopbackIPv4()
    {
        Assert.False(_validator.IsPublicIP("127.0.0.1"));
        Assert.False(_validator.IsPublicIP("127.0.0.0"));
        Assert.False(_validator.IsPublicIP("127.255.255.255"));
    }

    [Fact]
    public void IsPublicIP_ShouldBlockPrivateNetworks()
    {
        // 10.0.0.0/8
        Assert.False(_validator.IsPublicIP("10.0.0.0"));
        Assert.False(_validator.IsPublicIP("10.0.0.1"));
        Assert.False(_validator.IsPublicIP("10.255.255.255"));

        // 172.16.0.0/12
        Assert.False(_validator.IsPublicIP("172.16.0.0"));
        Assert.False(_validator.IsPublicIP("172.16.0.1"));
        Assert.False(_validator.IsPublicIP("172.31.255.255"));

        // 192.168.0.0/16
        Assert.False(_validator.IsPublicIP("192.168.0.0"));
        Assert.False(_validator.IsPublicIP("192.168.0.1"));
        Assert.False(_validator.IsPublicIP("192.168.255.255"));
    }

    [Fact]
    public void IsPublicIP_ShouldBlockLinkLocal()
    {
        Assert.False(_validator.IsPublicIP("169.254.0.0"));
        Assert.False(_validator.IsPublicIP("169.254.0.1"));
        Assert.False(_validator.IsPublicIP("169.254.255.255"));
    }

    [Fact]
    public void IsPublicIP_ShouldBlockReservedAndMulticast()
    {
        // Multicast
        Assert.False(_validator.IsPublicIP("224.0.0.0"));
        Assert.False(_validator.IsPublicIP("239.255.255.255"));

        // Reserved
        Assert.False(_validator.IsPublicIP("240.0.0.0"));
        Assert.False(_validator.IsPublicIP("255.255.255.255"));

        // 0.0.0.0/8
        Assert.False(_validator.IsPublicIP("0.0.0.1"));
        Assert.False(_validator.IsPublicIP("0.255.255.255"));
    }

    [Fact]
    public void IsPublicIP_ShouldAllowPublicIPv4()
    {
        Assert.True(_validator.IsPublicIP("8.8.8.8"));         // Google DNS
        Assert.True(_validator.IsPublicIP("1.1.1.1"));         // Cloudflare DNS
        Assert.True(_validator.IsPublicIP("208.67.222.222"));  // OpenDNS
        Assert.True(_validator.IsPublicIP("93.184.216.34"));   // example.com
    }

    [Fact]
    public void IsPublicIP_ShouldAllowHostnames()
    {
        Assert.True(_validator.IsPublicIP("google.com"));
        Assert.True(_validator.IsPublicIP("github.com"));
        Assert.True(_validator.IsPublicIP("example.com"));
        Assert.True(_validator.IsPublicIP("subdomain.example.com"));
    }

    [Fact]
    public void IsPublicIP_ShouldBlockIPv6Loopback()
    {
        Assert.False(_validator.IsPublicIP("::1"));
    }

    [Fact]
    public void IsPublicIP_ShouldBlockIPv6LinkLocal()
    {
        Assert.False(_validator.IsPublicIP("fe80::1"));
        Assert.False(_validator.IsPublicIP("fe80::ffff:ffff:ffff:ffff"));
    }

    [Fact]
    public void IsPublicIP_ShouldBlockIPv6SiteLocal()
    {
        Assert.False(_validator.IsPublicIP("fec0::1"));
        Assert.False(_validator.IsPublicIP("fec0::ffff:ffff:ffff:ffff"));
    }

    [Fact]
    public void IsPublicIP_ShouldBlockIPv6Multicast()
    {
        Assert.False(_validator.IsPublicIP("ff00::1"));
        Assert.False(_validator.IsPublicIP("ff02::1"));
    }

    [Fact]
    public void IsPublicIP_ShouldAllowIPv6Public()
    {
        Assert.True(_validator.IsPublicIP("2001:4860:4860::8888")); // Google IPv6 DNS
        Assert.True(_validator.IsPublicIP("2001:4860:4860::8844")); // Google IPv6 DNS
        Assert.True(_validator.IsPublicIP("2606:4700:4700::1111")); // Cloudflare IPv6 DNS
        Assert.True(_validator.IsPublicIP("2606:4700:4700::1001")); // Cloudflare IPv6 DNS
    }
}
