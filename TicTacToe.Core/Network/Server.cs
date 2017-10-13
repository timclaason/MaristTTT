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
        private Services detectRequestedService(Socket socket)
        {            
            string gameInitiationMessage = ListenForMessage(socket);
            if (gameInitiationMessage == NetworkMessages.TICTACTOE_REQUEST_TEXT)
                return Services.TicTacToe;
            if (gameInitiationMessage == NetworkMessages.INFO_REQUEST_TEXT)
                return Services.Info;
            if (gameInitiationMessage.ToUpper().Contains(NetworkMessages.WEB_SERVER_REQUEST_TEXT.ToUpper()))
                return Services.SimpleWeb;
            if (gameInitiationMessage.ToUpper().Contains(NetworkMessages.WEB_SERVER_REQUEST_SPECIFIC_FILE_TEXT_1.ToUpper()) &&
                gameInitiationMessage.ToUpper().Contains(NetworkMessages.WEB_SERVER_REQUEST_SPECIFIC_FILE_TEXT_1.ToUpper()))
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
                        
                        if(selectedService != Services.Invalid)
                        {
                            if (selectedService == Services.TicTacToe)
                            {
                                server = new TicTacToeServer();
                            }
                            else if(selectedService == Services.Info)
                            {
                                server = new InfoServer();
                            }
                            else if(selectedService == Services.SimpleWeb)
                            {
                                server = new SimpleWebServer();
                            }
                            server.CloneHandlers(this, server);
                            selectedService = server.PerformHandshake(socket);

                            if (selectedService != Services.Invalid)
                                server.Start(socket);
                        }                        

                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    catch
                    {
                        try
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                        }
                        catch { }
                        return;
                    }
                };
            bg.RunWorkerAsync();

        }
        
        public void Start()
        {
            Socket listener = base.GetListener(Settings.LISTENING_PORT);

            try
            {
                listener.Listen(10);
                                
                while (true)
                {
                    Socket socket = base.ListenForConnection(listener);
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
        Info,
        SimpleWeb,
        Invalid
    }
}
