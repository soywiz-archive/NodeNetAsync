using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NodeNetAsync.Net.Xmpp
{
	public class Xmpp
	{
		TcpSocket Socket;
		XmlReader Reader;
		private string SourceJID;
		public event Action OnConnected;
		private string User;
		private string Password;

		public Xmpp(string User, string Password)
		{
			this.User = User;
			this.Password = Password;
		}

		async private Task ConnectAsync(string User, string Password)
		{
			Socket = await TcpSocket.CreateAndConnectAsync("talk.google.com", 5222);
			bool Authenticated = false;
			while (true)
			{
				Console.WriteLine("--------------------------");
				await Socket.WriteAsync("<stream:stream to='gmail.com' xmlns:stream='http://etherx.jabber.org/streams' xmlns='jabber:client' version='1.0'>");
				//await Socket.ReadBytesAsync(138);

				Reader = XmlReader.Create(Socket.Stream, new XmlReaderSettings() { Async = true, ConformanceLevel = ConformanceLevel.Fragment });
				await Reader.ReadAsync(); await Reader.ReadAsync(); Reader = Reader.ReadSubtree(); await Reader.ReadAsync();

				var FeaturesXml = await WaitEndElementAsync("stream:features");
				var starttls = FeaturesXml.OuterXml.Contains("starttls");

				//Console.WriteLine("starttls: {0}", starttls);
				//Console.ReadKey();

				if (starttls)
				{
					await Socket.WriteAsync("<starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'><required /></starttls>");
					await WaitEndElementAsync("proceed");
					await Socket.SecureSslAsync("gmail.com");
					Console.WriteLine("StartTLS");
				}
				else if (!Authenticated)
				{
					await Socket.WriteAsync(String.Format("<auth xmlns='urn:ietf:params:xml:ns:xmpp-sasl' mechanism='PLAIN'>{0}</auth>", Convert.ToBase64String(Encoding.Default.GetBytes(String.Format("\0{0}\0{1}", User, Password)))));
					await WaitEndElementAsync("success");
					Authenticated = true;
				}
				else
				{
					await Socket.WriteAsync(String.Format("<iq xmlns='jabber:client' type='set' id='1'><bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'><resource>{0}</resource></bind></iq>", "NodeNet"));
					var IqNode = await WaitEndElementAsync("iq");
					Console.WriteLine(IqNode.OuterXml);
					Console.ReadKey();
					return;
				}
			}
		}

		async public Task SendMessageAsync(string Destination, string Message)
		{
			await Socket.WriteAsync("<presence/>");
			await Socket.WriteAsync(String.Format("<message from='{0}' to='{1}' type='chat'><body>{2}</body></message>", SourceJID, Destination, Message));
		}

		async public Task MainLoopAsync()
		{
			while (true)
			{
				await ConnectAsync(User, Password);

				OnConnected();

				await WaitEndElementAsync(null, (Node) =>
				{

				});
			}
		}

		async private Task<XmlNode> WaitEndElementAsync(string ExpectedName, Action<XmlNode> ElementHandler = null)
		{
			Console.WriteLine("-----------{0}", ExpectedName);
			while (true)
			{
				//Console.WriteLine("{0}:{1}:{2}", Reader.NodeType, Reader.Name, Reader.Value);
				var Content = await Reader.ReadOuterXmlAsync();
				var Document = new XmlDocument();
				Document.LoadXml(Content);
				var Node = Document.FirstChild;

				Console.WriteLine(Node.OuterXml);

				if (ElementHandler != null) ElementHandler(Node);
				if (Node.Name == ExpectedName) return Node;
			}
			//throw(new InvalidDataException());
			return null;
		}

		async private Task AuthenticateAsync()
		{

		}
	}
}
