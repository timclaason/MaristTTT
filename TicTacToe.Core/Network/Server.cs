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
        private ServerApplication performClientHandshake(Socket socket)
        {            
            ///Client sends <TICTACTOE>
            string gameInitiationMessage = ListenForMessage(socket);
            if (gameInitiationMessage == NetworkMessages.TICTACTOE_REQUEST_TEXT)
                return ServerApplication.TicTacToe;
            if (gameInitiationMessage == NetworkMessages.INFO_REQUEST_TEXT)
                return ServerApplication.Info;

            return ServerApplication.Invalid;
        }

        private void spinupServerThread(Socket socket)
        {
            BackgroundWorker bg = new BackgroundWorker();

            bg.DoWork += (sender, e2) =>
                {
                    try
                    {
                        ServerApplication selectedServer = performClientHandshake(socket);

                        if (selectedServer == ServerApplication.Invalid)
                        {
                            this.CloseSocketConnection(socket, NetworkMessages.CLOSING_SOCKET_TEXT);
                        }

                        if(selectedServer == ServerApplication.TicTacToe)
                        {
                            TicTacToeServer tttServer = new TicTacToeServer();
                            tttServer.CloneHandlers(this, tttServer);
                            selectedServer = tttServer.PerformHandshake(socket);

                            if(selectedServer == ServerApplication.TicTacToe)
                                tttServer.Start(socket);
                        }

                        if(selectedServer == ServerApplication.Info)
                        {
                            InfoServer infoServer = new InfoServer();
                            infoServer.CloneHandlers(this, infoServer);
                            selectedServer = infoServer.PerformHandshake(socket);

                            if (selectedServer == ServerApplication.Info)
                                infoServer.Start(socket);

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

                // Start listening for connections.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.

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
    public enum ServerApplication
    {
        TicTacToe,
        Info,
        Invalid
    }
}
