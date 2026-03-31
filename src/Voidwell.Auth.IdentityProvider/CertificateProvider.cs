using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Voidwell.Auth.IdentityProvider;

internal static class CertificateProvider
{
    private static readonly string _certificatesDirectory = Path.Combine(AppContext.BaseDirectory, "certificates");

    public static X509Certificate2 GetSigningCertificate() =>
        GetOrCreateCertificate("signing", X509KeyUsageFlags.DigitalSignature);

    public static X509Certificate2 GetEncryptionCertificate() =>
        GetOrCreateCertificate("encryption", X509KeyUsageFlags.KeyEncipherment);

    private static X509Certificate2 GetOrCreateCertificate(string name, X509KeyUsageFlags keyUsage)
    {
        Directory.CreateDirectory(_certificatesDirectory);

        var path = Path.Combine(_certificatesDirectory, $"{name}.pfx");

        if (File.Exists(path))
        {
            var existing = X509CertificateLoader.LoadPkcs12FromFile(path, password: null);
            if (existing.NotAfter > DateTime.UtcNow)
            {
                return existing;
            }

            existing.Dispose();
        }

        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            $"CN=Voidwell Auth {name}",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(new X509KeyUsageExtension(keyUsage, critical: true));

        var cert = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(10));
        File.WriteAllBytes(path, cert.Export(X509ContentType.Pkcs12));

        return cert;
    }
}