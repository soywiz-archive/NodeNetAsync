using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net.Http;

namespace NodeNetAsync.Net.Https
{
	/// <summary>
	/// 
	/// </summary>
	/// <see cref="http://www.codeproject.com/Articles/162194/Certificates-to-DB-and-Back"/>
	/// <seealso cref="openssl pkcs12 -export -in public.cer -inkeyprivate.key –out cert_key.p12"/>
	/// <seealso cref="var HttpCert = new X509Certificate2(@"C:\htdocs\core.bak\certs\certificate.xml");"/>
	public partial class HttpsServer : HttpServer
	{
		protected X509Certificate2 DefaultX509Certificate;
		protected Dictionary<string, X509Certificate2> HostCertificates = new Dictionary<string, X509Certificate2>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="X509Certificate"></param>
		public HttpsServer(X509Certificate2 X509Certificate)
			: base(null)
		{
			this.DefaultX509Certificate = X509Certificate;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="PublicCertificateString">String containing -----BEGIN CERTIFICATE-----</param>
		/// <param name="PrivateKeyString">String containing -----BEGIN RSA PRIVATE KEY-----</param>
		/// <param name="Password">Password of the private key</param>
		public HttpsServer(string PublicCertificateString, string PrivateKeyString, string Password = "")
			: base(null)
		{
			AddDefaultCertificate(PublicCertificateString, PrivateKeyString, Password);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="PublicCertificateString">String containing -----BEGIN CERTIFICATE-----</param>
		/// <param name="PrivateKeyString">String containing -----BEGIN RSA PRIVATE KEY-----</param>
		/// <param name="Password">Password of the private key</param>
		public void AddDefaultCertificate(string PublicCertificateString, string PrivateKeyString, string Password = "")
		{
			this.DefaultX509Certificate = GetCertificateFromPEMstring(PublicCertificateString, PrivateKeyString, Password);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Host"></param>
		/// <param name="PublicCertificateString">String containing -----BEGIN CERTIFICATE-----</param>
		/// <param name="PrivateKeyString">String containing -----BEGIN RSA PRIVATE KEY-----</param>
		/// <param name="Password">Password of the private key</param>
		public void AddCertificateForHost(string Host, string PublicCertificateString, string PrivateKeyString, string Password = "")
		{
			HostCertificates[Host] = GetCertificateFromPEMstring(PublicCertificateString, PrivateKeyString, Password);
		}

		async protected override Task InitializeConnectionAsync(TcpSocket Client)
		{
			var Stream = new SslStream(
				innerStream: Client.Stream,
				leaveInnerStreamOpen: false,
				userCertificateValidationCallback: (Sender, Certificate, Chain, SslPolicyErrors) =>
				{
					//Console.WriteLine("userCertificateValidationCallback");
					return true;
				},
				userCertificateSelectionCallback: (Sender, TargetHost, LocalCertificates, RemoteCertificate, AcceptableIssuers) =>
				{
					//Console.WriteLine("Host: '{0}'", targetHost);
					//Console.WriteLine(String.Join(",", acceptableIssuers));

					if (HostCertificates.ContainsKey(TargetHost))
					{
						return HostCertificates[TargetHost];
					}
					else
					{
						return DefaultX509Certificate;
					}
				}
			);
			await Stream.AuthenticateAsServerAsync(DefaultX509Certificate);

			//await Stream.AuthenticateAsServerAsync(X509Certificate, true, SslProtocols.Tls, true);
			Client.UnsafeSetStream(Stream);
		}

		protected override async Task ReadHeadersAsync(TcpSocket Client, HttpRequest Request, HttpResponse Response)
		{
			Request.Ssl = true;
			await base.ReadHeadersAsync(Client, Request, Response);
		}

		async public override Task ListenAsync(ushort Port = 443, string Host = "0.0.0.0")
		{
			await base.ListenAsync(Port, Host);
		}
	}
}
