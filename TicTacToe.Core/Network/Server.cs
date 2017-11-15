using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using TicTacToe.Core.Structures;
using TicTacToe.Core.Network.Servers;

namespace TicTacToe.Core.Network
{
    public class Server : NetworkNode
    {
		public event EventHandler ClientConnected;
		public event EventHandler SocketOpened;

		Socket _mainListener;

		public static TicTacToeGameCollection Games = new TicTacToeGameCollection();

        private Services detectRequestedService(Socket socket)
        {            
            string gameInitiationMessage = ListenForMessage(socket);

			NetworkMessage message = new NetworkMessage(gameInitiationMessage);

			if (message.AnyMessageContains(CommonMessages.REQUEST_SYMBOL_2_PLAYER) && message.AnyMessageContains(CommonMessages.TICTACTOE_REQUEST_TEXT))
				return Services.TicTacToeTwoPlayer;

            if (gameInitiationMessage == CommonMessages.TICTACTOE_REQUEST_TEXT)
                return Services.TicTacToeOnePlayer;
            if (gameInitiationMessage == CommonMessages.INFO_REQUEST_TEXT || gameInitiationMessage.Contains(CommonMessages.PUTTY_CONNECTION_TEXT))
                return Services.Info;
            if (gameInitiationMessage.ToUpper().Contains(CommonMessages.WEB_SERVER_REQUEST_TEXT.ToUpper()))
                return Services.SimpleWeb;
            if (gameInitiationMessage.ToUpper().Contains(CommonMessages.WEB_SERVER_REQUEST_SPECIFIC_FILE_TEXT_1.ToUpper()) &&
                gameInitiationMessage.ToUpper().Contains(CommonMessages.WEB_SERVER_REQUEST_SPECIFIC_FILE_TEXT_1.ToUpper()))
                return Services.SimpleWeb;
                            //GET /a.html HTTP/1.1

            
            
            return Services.Invalid;
        }

        private void spinupServerThread(Socket socket)
        {
            BackgroundWorker bg = new BackgroundWorker();

            bg.DoWork += (sender, e2) =>
                {
                    try
                    {
                        Services selectedService = detectRequestedService(socket);
                        BaseServer server = null;

						if (selectedService == Services.Invalid)
						{
							base.CloseSocketConnection(socket, CommonMessages.DISCONNECT_TEXT);
							return;
						}

						if (selectedService == Services.TicTacToeOnePlayer)
						{
							server = new OnePlayerTicTacToeServer();
						}
						else if (selectedService == Services.TicTacToeTwoPlayer)
						{
							server = new TwoPlayerTicTacToeServer();
						}

						else if (selectedService == Services.Info)
						{
							server = new InfoServer();
						}
						else if (selectedService == Services.SimpleWeb)
						{
							server = new SimpleWebServer();
						}
                        server.CloneHandlers(this, server);
                        selectedService = server.PerformHandshake(socket);

                        if (selectedService != Services.Invalid)
                            server.Start(socket);
                        

						base.CloseSocketConnection(socket, CommonMessages.DISCONNECT_TEXT);
						
					}
                    catch(Exception ex)
                    {
                        try
                        {
							base.CloseSocketConnection(socket, CommonMessages.DISCONNECT_TEXT);
						}
                        catch { }
                        return;
                    }
                };
            bg.RunWorkerAsync();

        }

		/// <summary>
		/// As of right now, this only supports IPV4
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public Socket GetListener(int port)
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];

			foreach (IPAddress a in ipHostInfo.AddressList)
			{
				if (a.AddressFamily == AddressFamily.InterNetwork)
				{
					ipAddress = a;
					break;
				}
			}
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

			_mainListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_mainListener.Bind(localEndPoint);

			return _mainListener;
		}

		public void KillListener()
		{
			_mainListener.Close();			
		}


		public Socket OpenApplicationSocket(Socket listener)
		{
			if (SocketOpened != null)
				SocketOpened("Opened application Socket on " + listener.LocalEndPoint.ToString(), new EventArgs());

			Socket socket = listener.Accept();

			if (socket.Connected)
			{
				string message = "Opened application socket for client " + socket.RemoteEndPoint.ToString();

				ServerLogger.WriteToLog("CXN", message);
				if (ClientConnected != null)
				{
					ClientConnected(message, new EventArgs());
				}
			}

			return socket;

		}


		public void Start()
        {
            Socket listener = GetListener(Settings.LISTENING_PORT);

            try
            {
                listener.Listen(10);
                                
                while (true)
                {
                    Socket socket = OpenApplicationSocket(listener);
                    spinupServerThread(socket);                    
                }

            }
            catch (Exception e)
            {
                ServerLogger.WriteToLog("ERR", e.Message);
                throw;
            }
        }        
    }

    /// <summary>
    /// Types of servers
    /// </summary>
    public enum Services
    {
        TicTacToe,
		TicTacToeOnePlayer,
		TicTacToeTwoPlayer,
        Info,
        SimpleWeb,
        Invalid
    }
}
