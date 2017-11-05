import java.net.*;
import java.io.*;

public class NetworkNode
    {
       
        public Socket GetClientSocket(String server, int port)
        {
        	try {
                System.out.println("Connecting to " + server + " on port " + port);
                Socket client = new Socket(server, port);
                
                System.out.println("Just connected to " + client.getRemoteSocketAddress());
                
                return client;                
             } catch (IOException e) {
                e.printStackTrace();
                return null;
             }

        }

        public String ListenForMessage(Socket socket)
        {        	
        	try
        	{
        		byte[] received = new byte[100]; 
        				
        		DataInputStream dataInputStream = new DataInputStream(socket.getInputStream());
        		dataInputStream.read(received);
        		
        		return new String(received).trim();
        		
        	}
        	catch(IOException e)
        	{        		
        		System.out.println("Error: " + e.getMessage());
        		return "";
        	}
        	
        }

        public void SendMessageThroughSocket(Socket socket, String message)
        {
			try
			{
				OutputStream outToServer = socket.getOutputStream();				
				outToServer.write(message.getBytes());
				outToServer.flush();				
            }
			catch(IOException e)
			{
			}
        }

        public void CloseSocketConnection(Socket socket)
        {            
        	try
        	{
        		socket.close();
        	}
        	catch(IOException e)
        	{
        	}
        	
        }
    };