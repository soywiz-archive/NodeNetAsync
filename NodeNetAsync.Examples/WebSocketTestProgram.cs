using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;
using NodeNetAsync.Net.Http;
using NodeNetAsync.Net.Http.Router;
using NodeNetAsync.Net.Http.Static;
using NodeNetAsync.Net.Http.WebSockets;

namespace NodeNetAsync.Examples
{
	public class WebSocketTestProgram
	{
		//const int BindPort = 3333;
		//const string BindAddress = "0.0.0.0";
		const int BindPort = 80;
		const string BindAddress = "127.0.0.1";

		public class Client
		{
			/// <summary>
			/// Name of the user.
			/// </summary>
			public string UserName;
		}

		public class WebsocketChatHandler : IHttpWebSocketHandler<Client>
		{
			/// <summary>
			/// List of connected sockets.
			/// </summary>
			List<WebSocket<Client>> ConnectedSockets = new List<WebSocket<Client>>();

			public WebsocketChatHandler()
			{
				Core.SetInterval(async () =>
				{
					await SendMessageToAllAsync(String.Format("Global: Timer Tick 4 seconds. Connected users: {0}", ConnectedSockets.Count));
				}, TimeSpan.FromSeconds(4));
			}

			/// <summary>
			/// Executed when an user connects.
			/// </summary>
			/// <param name="WebSocket"></param>
			/// <returns></returns>
			async public Task OnOpen(WebSocket<Client> WebSocket)
			{
				// Creates a client and add the socket to the list of sockets.
				WebSocket.Tag = new Client();
				WebSocket.Tag.UserName = "User" + WebSocket.UniqueId;
				ConnectedSockets.Add(WebSocket);

				// Send information about the new connection to all sockets but the current one.
				await SendMessageToAllExceptAsync(
					String.Format("Global: User '{0}' connected", WebSocket.Tag.UserName),
					WebSocket
				);

				// Sends information just to the current user.
				await SendMessageToAsync(
					String.Format("System: Connected as '{0}'", WebSocket.Tag.UserName),
					WebSocket
				);

				// Loop forever
				while (true)
				{
					// Read Packet
					var Packet = await WebSocket.ReadPacketAsync();
					if (Packet.Opcode != WebSocketPacket.OpcodeEnum.TextFrame) continue;
					var Message = Packet.Text;

					// Skip empty packets
					if (Message.Length <= 0) continue;

					// Checks if the first character is a '/'
					if (Message[0] == '/')
					{
						var Parts = Message.Split(new[] { ' ', '\t' }, 2);
						if (Parts.Length < 1) Parts = new[] { "", "" };
						if (Parts.Length < 2) Parts = new[] { Parts[0], "" };

						var Command = Parts[0];
						var Parameter = Parts[1];

						// Check the available commands.
						switch (Command)
						{
							case "/nick": await HandleNick(WebSocket, Parameter); break;
							case "/help": await HandleHelpAsync(WebSocket); break;
							default:
								await SendMessageToAsync(String.Format("System: Unknown command '{0}'", Command), WebSocket);
								break;
						}

						continue;
					}

					await SendMessageToAllAsync(String.Format("{0}: {1}", WebSocket.Tag.UserName, Message));
				}
			}

			/// <summary>
			/// Handles the /nick command
			/// </summary>
			/// <param name="WebSocket"></param>
			/// <param name="NickName"></param>
			/// <returns></returns>
			async protected Task HandleNick(WebSocket<Client> WebSocket, string NickName)
			{
				// Specified a nick
				if (NickName.Length > 0)
				{
					var OldName = WebSocket.Tag.UserName;
					var NewName = NickName;

					// Check if the new name is available to use
					if (!IsNameUsed(NewName))
					{
						WebSocket.Tag.UserName = NewName;
						await SendMessageToAsync(String.Format("System: Changed nick to: {0}", NewName), WebSocket);
						await SendMessageToAllExceptAsync(String.Format("Global: User '{0}' changed nick to '{1}'", OldName, NewName));
					}
					// Can't use that nick
					else
					{
						await SendMessageToAsync(String.Format("System: Can't change Nick. Nick '{0}' already used", NewName), WebSocket);
					}
				}
				// No nick specified
				else
				{
					await SendMessageToAsync(String.Format("System: Must specify nick"), WebSocket);
				}
			}

			/// <summary>
			/// Handles the /help command
			/// </summary>
			/// <param name="WebSocket"></param>
			/// <returns></returns>
			async protected Task HandleHelpAsync(WebSocket<Client> WebSocket)
			{
				await SendMessageToAsync("System: Help:", WebSocket);
				await SendMessageToAsync("System: /help -- Show this help", WebSocket);
				await SendMessageToAsync("System: /nick name -- For chaning nick", WebSocket);
			}

			/// <summary>
			/// Executed when an user disconnects.
			/// </summary>
			/// <param name="WebSocket"></param>
			/// <returns></returns>
			async public Task OnClose(WebSocket<Client> WebSocket)
			{
				ConnectedSockets.Remove(WebSocket);
				await SendMessageToAllExceptAsync(String.Format("Global: User '{0}' disconnected", WebSocket.Tag.UserName));
			}

			/// <summary>
			/// Check if the user name can be used.
			/// </summary>
			/// <param name="Name"></param>
			/// <returns></returns>
			public bool IsNameUsed(string Name)
			{
				return ConnectedSockets.Any(Socket => Socket.Tag.UserName == Name);
			}

			/// <summary>
			/// Sends a message to all connected sockets.
			/// </summary>
			/// <param name="Message"></param>
			/// <param name="ExceptList"></param>
			/// <returns></returns>
			async Task SendMessageToAllAsync(string Message)
			{
				await SendMessageToAllExceptAsync(Message);
			}

			/// <summary>
			/// Sends a message to all connected sockets except a list of sockets.
			/// </summary>
			/// <param name="Message"></param>
			/// <param name="ExceptList"></param>
			/// <returns></returns>
			async Task SendMessageToAllExceptAsync(string Message, params WebSocket<Client>[] ExceptList)
			{
				var TaskList = new List<Task>();
				foreach (var ConnectedSocket in ConnectedSockets)
				{
					if (ExceptList.Contains(ConnectedSocket)) continue;
					TaskList.Add(ConnectedSocket.WritePacketAsync(Message));
				}
				await Task.WhenAll(TaskList);
			}

			/// <summary>
			/// Sends a message to a list of sockets.
			/// </summary>
			/// <param name="Message"></param>
			/// <param name="List"></param>
			/// <returns></returns>
			async Task SendMessageToAsync(string Message, params WebSocket<Client>[] List)
			{
				var TaskList = new List<Task>();
				foreach (var Socket in List)
				{
					TaskList.Add(Socket.WritePacketAsync(Message));
				}
				await Task.WhenAll(TaskList);
			}
		}

		static public void Main2(string[] Args)
		{
			Core.Loop(async () =>
			{
				var Server = new HttpServer();
				var Router = new HttpRouter();

				Router.AddRoute("/websocket", new HttpWebSocket<Client>(new WebsocketChatHandler()));

				Router.AddRoute("/", async (Request, Response) =>
				{
					Response.Buffering = true;

					await Response.WriteAsync(@"
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
								input#input_text, #console {
									font: 14px 'Lucida Console';
								}
							</style>
						</head>
						<body>
						<div id='connecting'>Connecting...</div>
						<form action='' method='' id='editor' style='display:none;'>
							<input type='text' name='text' id='input_text' autocomplete='off' />
							<input type='submit' value='send'>
							(write <strong>/help</strong> to display help)
						</form>
						<div id='console'></div>
						<script type='text/javascript'>
							colorTable = ['red', 'blue', 'green'];
							function hash(str) {
								var hash = str.length;
								for (var n = 0; n < str.length; n++) hash ^= str.charCodeAt(n);
								return hash;
							}
							function console_clear() {
								$('#console').html('');
							}

							function console_write(color, message) {
								$('#console').append($('<div>').css('color', color).text(message));
								$('#console')[0].scrollTop = $('#console')[0].scrollHeight;
								//$('#console').animate({ scrollTop: $('#console')[0].scrollHeight }, 100);
							}

							if ('MozWebSocket' in window) window.WebSocket = window.MozWebSocket;

							if ('WebSocket' in window) {
								function reconect() {
									var ws = new WebSocket('ws://' + document.location.host + '/websocket');
									ws.onopen = function() {
										console_clear();
										console_write('green', 'Websocket opened!');
										$('#connecting').hide();
										$('#editor').show();
										$('#editor').on('submit', function() {
											ws.send($('#input_text').val());
											$('#input_text').val('');
											return false;
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
				await Server.ListenAsync(BindPort, BindAddress);
			});
		}

		static public void Main(string[] Args)
		{
			/*
			try
			{
				new NodeNetAsync.Db.Redis.RedisClientTest().TestConnection().Wait();
			}
			catch (Exception Exception)
			{
				Console.WriteLine(Exception);
				Console.ReadKey();
			}
			*/

			Main2(Args);
		}
	}
}
