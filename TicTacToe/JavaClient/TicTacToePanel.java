/************************************************************************************************************************************************
 * Game originally taken and adapted from code at https://courses.washington.edu/css161/nash/slides/games/gamePak/gamePak/src/TicTacToe.java
 * Original author:  Rob Nash
 ************************************************************************************************************************************************* 
 */
import java.awt.*;
import java.awt.event.*;
import javax.swing.*;

public class TicTacToePanel {
	
	
	
	static String IP_ADDRESS = "199.89.63.162";
	static int PORT = 42424;
	static String BLANK_SPACE = " ";
	static String X_SPACE = "X";
	static String O_SPACE = "O";
	
	
	private static Boolean isLocked = false;
	private static MyButton buttons[] = new MyButton[9]; //create 9 buttons

	private static TicTacToeGame theGame;
	
	public static void main (String[] args)
	{
		startGame(true);
	}

	private static void startGame(Boolean firstLoad)
	{
		theGame = new TicTacToeGame(IP_ADDRESS,  PORT);
		Boolean successfullyInitialized = theGame.Initialize();	
		String currentBoard = theGame.ListenForBoard();
		updateBoard(currentBoard);
		
		if(!successfullyInitialized)
		{
			System.out.println("Error initializing");
			return;
		}	
		
		if(firstLoad)
			initializePanel(); //launch game
	}
	
	private static String assembleMessageBasedOnBoard()
	{
		String returnMessage = "";
		
		for(int i=0; i<=8; i++){// clear all 8 buttons
			returnMessage += i + ":";
			
			
			if(buttons[i].getText().trim() == X_SPACE || buttons[i].getText().trim() == O_SPACE)
				returnMessage += buttons[i].getText();
			else
				returnMessage += "B";
			if(i < 8)
				returnMessage += ";";
		}
		return returnMessage;
	}


	private static void updateBoard(String message)
	{
		String[] split = message.split(";");
		
		for (int i = 0; i < split.length; i++) 
		{		
			if(split[i].contains(X_SPACE))
				buttons[i].setText(X_SPACE);
			if(split[i].contains(O_SPACE))
				buttons[i].setText(O_SPACE);
		}
	}

	private static void initializePanel(){
		JFrame frame = new JFrame ("Tic Tac Toe");
		frame.setDefaultCloseOperation (JFrame.EXIT_ON_CLOSE);
	
	
		JPanel panel = new JPanel(); //creating a panel with a box like a tic tac toe board
		panel.setLayout (new GridLayout (3, 3));
		panel.setBorder (BorderFactory.createLineBorder (Color.gray, 3));
		panel.setBackground (Color.white);
	
		for(int i=0; i<=8; i++){ //placing the button onto the board
			buttons[i] = new MyButton();
			panel.add(buttons[i]);			
		}
	
		frame.getContentPane().add (panel);
		frame.pack();
		frame.setVisible(true);
		frame.setSize(500, 500);// set frame size and let teh game begin
	}

	public static void clearButtons()
	{		
		for(int i=0; i<=8; i++){// clear all 8 buttons
			buttons[i].setText(BLANK_SPACE);						
		}
	}

	private static class MyButton extends JButton 
implements ActionListener {
	
		int again=1000;//set again at 1000 so we don't make the mistake we can play again
		
		String letter; // x or o
		
		public MyButton() {	// creating blank board
			super();
			letter=BLANK_SPACE;
			setFont(new Font("Dialog", 1, 60));
			setText(letter);
			addActionListener(this);
		}
		public void actionPerformed(ActionEvent e) { // placing x or o's		
			
			if(isLocked == true)
				return;
			
			if(this.getText().contains(X_SPACE) || this.getText().contains(O_SPACE))
				return;
			
			setText(X_SPACE); // place the x or the o on the actual board
			
			theGame.SendBoard(assembleMessageBasedOnBoard());
					
			isLocked = true;
			String receivedMessage = theGame.ListenForBoard();
			updateBoard(receivedMessage);
			isLocked = false;
			
			Boolean xWins = receivedMessage.contains("XWIN");
			Boolean oWins = receivedMessage.contains("OWIN");
			Boolean catscratch = receivedMessage.contains("CATSCR");
			Boolean gameOver = xWins || oWins || catscratch;
			
			
			if(xWins || oWins){ // if the game ends let the user know who wins and give option to play again
				letter = O_SPACE;
				if(xWins)
					letter = X_SPACE;
				again=JOptionPane.showConfirmDialog(null, letter + " wins the game!  Do you want to play again?",letter + "won!",JOptionPane.YES_NO_OPTION);
				
			} else if(catscratch){//tie game, announce and ask if the user want to play again
				again=JOptionPane.showConfirmDialog(null, "The game was tie!  Do you want to play again?","Tie game!",JOptionPane.YES_NO_OPTION);			
			}	
			
			if(again==JOptionPane.YES_OPTION && gameOver == true){ // if the user want to play again clear all the button and start over
					clearButtons();
					startGame(false);
			}
			else if(again==JOptionPane.NO_OPTION){
				System.exit(0); // exit game if the user do not want to play again
			}
		}
	}


	
}
