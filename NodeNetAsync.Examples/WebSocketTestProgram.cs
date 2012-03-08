using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net.Http;
using NodeNetAsync.Net.Http.Router;
using NodeNetAsync.Net.Http.Static;
using NodeNetAsync.Net.Http.WebSockets;

namespace NodeNetAsync.Examples
{
	public class WebSocketTestProgram
	{
		public class Client
		{
			public string UserName;
		}

		static public void Main(string[] Args)
		{
			Core.Loop(async () =>
			{
				var Server = new HttpServer();
				var Router = new HttpRouter();

				var ConnectedSockets = new List<WebSocket<Client>>();

				Func<String, Task> BroadcastMessage = async (Message) =>
				{
					var TaskList = new List<Task>();
					foreach (var ConnectedSocket in ConnectedSockets)
					{
						TaskList.Add(ConnectedSocket.WritePacketAsync(Message));
					}
					foreach (var Task in TaskList) await Task;
				};

				Router.AddRoute("/websocket", new HttpWebSocket<Client>(
					ConnectHandler: async (WebSocket) =>
					{
						Console.WriteLine("Socket Connected");

						WebSocket.Tag = new Client();
						WebSocket.Tag.UserName = "User" + WebSocket.UniqueId;
						ConnectedSockets.Add(WebSocket);

						await BroadcastMessage(String.Format("Global: User '{0}' connected", WebSocket.Tag.UserName));

						while (true)
						{
							var Packet = await WebSocket.ReadPacketAsStringAsync();

							if (Packet.Length > 0)
							{
								if (Packet[0] == '/')
								{
									var Parts = Packet.Split(new[] { ' ', '\t' }, 2);
									if (Parts.Length < 1) Parts = new[] { "", "" };
									if (Parts.Length < 2) Parts = new[] { Parts[0], "" };

									switch (Parts[0])
									{
										case "/nick":
											if (Parts[1].Length > 0)
											{
												var OldName = WebSocket.Tag.UserName;
												var NewName = Parts[1];
												WebSocket.Tag.UserName = NewName;
												await WebSocket.WritePacketAsync(String.Format("System: Changed nick to: {0}", Parts[1]));
												await BroadcastMessage(String.Format("Global: User '{0}' changed nick to '{1}'", OldName, NewName));
											}
											else
											{
												await WebSocket.WritePacketAsync(String.Format("{0}: {1}", "System", "Must specify nick"));
											}
											break;
										case "/help":
											await WebSocket.WritePacketAsync(String.Format("{0}: {1}", "System", "Help:"));
											await WebSocket.WritePacketAsync(String.Format("{0}: {1}", "System", "/help -- Show this help"));
											await WebSocket.WritePacketAsync(String.Format("{0}: {1}", "System", "/nick name -- For chaning nick"));
											break;
										default:
											await WebSocket.WritePacketAsync(String.Format("{0}: {1}", "System", "Unknown command '" + Parts[0] + "'"));
											break;
									}

									continue;
								}

								await BroadcastMessage(String.Format("{0}: {1}", WebSocket.Tag.UserName, Packet));

								//await WebSocket.WritePacketAsync();
							}
						}
					},
					DisconnectHandler: async (WebSocket) =>
					{
						Console.WriteLine("Socket Disconnected");
						ConnectedSockets.Remove(WebSocket);
						await BroadcastMessage(String.Format("Global: User '{0}' disconnected", WebSocket.Tag.UserName));
					}
				));

				Router.AddRoute("/", async (Request, Response) =>
				{
					await Response.WriteChunkAsync(@"
						<html>
						<head>
							<script src='http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js' type='text/javascript'></script>
							<style type='text/css'>
								#console {
									border: 1px solid black;
									padding: 8px;
									width: 400px;
									height: 300px;
									overflow: auto;
								}
							</style>
						</head>
						<body>
						<div id='connecting'>Connecting...</div>
						<form action='javascript:false' method='' id='editor' style='display:none;'><input type='text' name='text' id='input_text' /><input type='submit' value='send'></form> (write <strong>/help</strong> to display help)
						<div id='console'></div>
						<script type='text/javascript'>
							colorTable = ['red', 'blue', 'green'];
							function hash(str) {
								var hash = str.length;
								for (var n = 0; n < str.length; n++) hash ^= str.charCodeAt(n);
								return hash;
							}
							function console_write(color, message) {
								$('#console').append($('<div>').css('color', color).text(message));
								$('#console')[0].scrollTop = $('#console')[0].scrollHeight;
								//$('#console').animate({ scrollTop: $('#console')[0].scrollHeight }, 100);
							}

							if ('WebSocket' in window) {
								function reconect() {
									var ws = new WebSocket('ws://localhost/websocket');
									ws.onopen = function() {
										console_write('green', 'Websocket opened!');
										$('#connecting').hide();
										$('#editor').show();
										$('#editor').on('submit', function() {
											ws.send($('#input_text').val());
											$('#input_text').val('');
										});
										$('#input_text').focus();
									};
									ws.onmessage = function (evt) {
										var received_msg = evt.data;
										var parts = received_msg.split(':');
										var user = parts[0];
										var message = parts.slice(1).join(':');
										var user_hash = hash(user);
										var color = colorTable[user_hash % colorTable.length];
										if (user == 'System') color = 'black';
										if (user == 'Global') color = 'orange';
										console_write(color, user + ': ' + message);
									};
									ws.onclose = function() {
										console_write('red', 'Websocket closed!');
										$('#editor').off('submit');
										$('#connecting').show();
										$('#editor').hide();
										setTimeout(reconect, 1000);
									};
								}
								reconect();
							} else {
								console_write('red', 'Websocket is not supported in your browser');
							}
						</script>
						</body>
						</html>
					".Trim());
				});

				Router.AddRoute("/favicon.ico", async (Request, Response) =>
				{
				});

				Server.AddFilterLast(Router);
				await Server.ListenAsync(80, "127.0.0.1");
			});
		}
	}
}
