
import java.net.*;
import java.io.*;

public class TicTacToeGame extends NetworkNode {
	NetworkNode theNode;
	private Socket theSocket;
	
	String _ip;
	int _port;
	
	public TicTacToeGame(String ipAddress, int port)
	{		
		theNode = new NetworkNode();
		_ip = ipAddress;
		_port = port;
	}
	
	public Boolean Initialize()
	{
		String requestMessage = "<TICTACTOE>";
		String ackMessage = "<ACK>";
		String pieceOfferMessage = "<WHATCHAWANT>";
		String xRequestMessage = "<X>";
		
		theSocket = theNode.GetClientSocket(_ip, _port);
		System.out.println("Got client socket");
		theNode.SendMessageThroughSocket(theSocket, requestMessage);
		System.out.println("Sent message through socket: " + requestMessage);
		String received = theNode.ListenForMessage(theSocket);
		System.out.println("Received message through socket: " + received);
		if(received.trim().contains("TICTACTOE") == false)
		{
			System.out.println(received + " != " + requestMessage);
			return false;
		}
		System.out.println("Sending ack");
		theNode.SendMessageThroughSocket(theSocket, ackMessage);
		String nextReceipt = theNode.ListenForMessage(theSocket);
		if(nextReceipt.contains("WHATCHA") == false)
			return false;
		
		theNode.SendMessageThroughSocket(theSocket, xRequestMessage);
		

		
		return true;
	}
	
	public String ListenForBoard()
	{
		String board = theNode.ListenForMessage(theSocket);
		return board;	
	}
	
	public void SendBoard(String board) 
	{
		theNode.SendMessageThroughSocket(theSocket, board);
	}

}
