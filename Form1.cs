//THERE IS A LINE IN MAINPANEL THAT USES AN ABSOLUTE PATH.  WHEN THE PROGRAM IS MOVED, FIX THIS PATH!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Timers;
using System.Threading;

namespace Hangman
{
    public partial class mainPanel : Form
    {
        public static mainFuncs mainFuncs { get; set; }
        public static char c { get; set; }
        public static Boolean gamePlayed { get; set; }
        public static Boolean isGameActive { get; set; }
        public static Boolean isGameOver { get; set; }
        public static Boolean noMoreWords { get; set; }
        public static char[] currentWord { get; set; }
        public static char[] displayedWord { get; set; }
        public static String[] gameWords { get; set; }
        public static String statusText { get; set; }
        public static String promptText { get; set; }
        public static int numWins { get; set; }
        public static int numGames { get; set; }
        public static int wordCount { get; set; }
        public static int wordsLeft { get; set; }
        public static int guessesLeft { get; set; }
        public static Random rnd { get; set; }

        public static int timerIndex { get; set; }
        public static int scrollCount { get; set; }
        System.Timers.Timer serialSend { get; set; }
        public static Byte[] sendBytes { get; set; }
        public static char[] spaceArray { get; set; } //for use in scrolling text across the screen
        private static SerialPort comPort { get; set; }

        delegate void SetTextCallback(Label label, string text);

        public mainPanel()
        {
            InitializeComponent();
            mainFuncs = new mainFuncs();
            gamePlayed = false;
            isGameActive = false;
            isGameOver = false;
            noMoreWords = false;
            guessesLeft = 0;
            numWins = 0;
            numGames = 0;
            currentWord = null;
            displayedWord = null;
            //THIS LINE USES AN ABSOLUTE PATH THAT WILL BREAK IF THIS PROGRAM IS MOVED
            gameWords = File.ReadAllLines("C:\\Users\\benlo\\Desktop\\Hangman\\Hangman\\Resources\\game_words.txt");
            wordCount = gameWords.Length;
            wordsLeft = wordCount;
            rnd = new Random();
            initCOM();
            serialSend = new System.Timers.Timer(250);
            serialSend.AutoReset = true;
            serialSend.Enabled = false;
            spaceArray = new char[19];
            for (int i=0; i<19; i++)
            {
                spaceArray[i] = ' ';
            }
            //initLCD();
            try {
                serialWrite("New Game?".ToCharArray());
            }catch (Exception e)
            {
                ChangeLabelText(statusLabel, "No Device Connected!");
            }
        }

        private void ChangeLabelText(Label label, string newText)
        {
            if (label.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ChangeLabelText);
                this.Invoke(d, new object[] { label, newText });
            }
            else
            {
                label.Text = newText;
            }
        }

        //connects to first COM port in list of all available COM Ports, as long as there is at least 1 COM port available.
        private void initCOM()
        {
            if (SerialPort.GetPortNames().Length != 0)
            {
                comPort = new SerialPort((SerialPort.GetPortNames())[0], 9600, Parity.None, 8, StopBits.One);
                comPort.Open();
                comPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            }
        }

        private void initLCD()
        {
            //comPort.Write(new Byte[] { 0x6d }, 0, 1);
            try {
                comPort.Write(new Byte[] { 0x0c, 0x8c, 0x0c, 0x0c, 0x8c, 0x0c, 0x0c, 0x8c, 0x0c,
                                       0x08, 0x88, 0x08, 0x08, 0x88, 0x08, 0x00, 0x80, 0x00,
                                       0x00, 0x80, 0x00, 0x30, 0xb0, 0x30,
                                       0x00, 0x80, 0x00, 0x18, 0x98, 0x18}, 0, 30);
            }catch (Exception e)
            {
                ChangeLabelText(statusLabel, "No Device Connected!");
            }
        }

        //when data is Received, calls gameMain with the ASCII character entered
        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int inData = comPort.ReadByte();
            //Any uppercase or lowercase letter
            if ((inData >= 65 && inData <= 90) || (inData >= 97 && inData <= 122))
            {
                char c = Convert.ToChar(inData);
                Console.WriteLine(char.ToLower(c));
                //MessageBox.Show(char.ToLower(c).ToString());
                gameMain(char.ToLower(c));
            }

        }

        private void mainPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            gameMain(e.KeyChar);
        }

        private void gameMain(char c)
        {
            //MessageBox.Show(SerialPort.GetPortNames().Length.ToString());
            //MessageBox.Show((SerialPort.GetPortNames())[0]);
            //Start a new game case if in pre-game state and there are more words to use.
            if (!isGameActive && !isGameOver && !noMoreWords && c == 'y')
            {
                scrollText(false);
                gamePlayed = true;
                changeImage(1);
                int index;
                while (true) //make sure we get a new word before moving on
                {
                    index = rnd.Next(0, wordCount);
                    if (gameWords[index] != null)
                    {
                        String newWord = gameWords[index];
                        gameWords[index] = null;
                        currentWord = newWord.ToCharArray();
                        displayedWord = mainFuncs.newDispWord(currentWord.Length);
                        //wordLabel.Text = mainFuncs.charsToString(displayedWord);
                        ChangeLabelText(wordLabel, mainFuncs.charsToString(displayedWord));
                        wordsLeft--;
                        break;
                    }
                }
                if (wordsLeft == 0) noMoreWords = true;
                isGameActive = true;
                serialWrite(displayedWord);
                //Reset the counter of the on-board 7-Seg to 6
                try {
                    comPort.Write(new Byte[] { 0x02, 0x00, 0x02, 0x00 }, 0, 4);
                } catch (Exception e)
                {
                    ChangeLabelText(statusLabel, "No Device Connected!");
                }
                //Console.WriteLine(0x02+" "+0x00);
                guessesLeft = 6;
                //statusLabel.Text = "6 Incorrect Guesses Remaining";
                ChangeLabelText(statusLabel, "6 Incorrect Guesses Remaining");
                //promptLabel.Text = "Enter a Letter Guess [A-Z]";
                ChangeLabelText(promptLabel, "Type a Letter Guess [A-Z]");
            }
            else if (!isGameActive && c == 'n' && gamePlayed == true)
            {
                scrollText(false);
                isGameOver = true;
                statusText = numWins + " correct out of " + numGames;
                //statusLabel.Text = "Final Score: " + numWins + " out of " + numGames + ".";
                ChangeLabelText(statusLabel, "Final Score: " + numWins + " out of " + numGames + ".");
                serialWrite(statusText.ToCharArray());
                //promptLabel.Text = "GAME OVER";
                ChangeLabelText(promptLabel, "GAME OVER");
            }
            //If game is active, check for letter, then update displayed word or turn count.
            else if (isGameActive == true)
            {
                Boolean isPresent = currentWord.Contains(c);
                //successful guess case
                if (isPresent)
                {
                    displayedWord = mainFuncs.insertLetter(c, currentWord, displayedWord);
                    serialWrite(displayedWord);
                    //wordLabel.Text = mainFuncs.charsToString(displayedWord);
                    ChangeLabelText(wordLabel, mainFuncs.charsToString(displayedWord));
                    if (mainFuncs.isFull(displayedWord))
                    {
                        numWins++;
                        numGames++;
                        statusText = "Well done! You have solved " + numWins + " puzzle(s) out of " + numGames;
                        //statusLabel.Text = statusText;
                        ChangeLabelText(statusLabel, statusText);
                        scrollText(true);
                        isGameActive = false;
                    }
                    //unsuccessful guess case
                } else
                {
                    guessesLeft--;
                    changeImage(7 - guessesLeft);
                    //decrement the on-board 7-Seg
                    try {
                        comPort.Write(new Byte[] { 0x01 , 0x00 } , 0, 2);
                    }
                    catch (Exception e)
                    {
                        ChangeLabelText(statusLabel, "No Device Connected!");
                    }
                    //Console.WriteLine(0x01+" "+0x00);
                    if (guessesLeft > 0)
                    {
                        //statusLabel.Text = guessesLeft + " " + "Incorrect Guesses Remaining";
                        ChangeLabelText(statusLabel, guessesLeft + " " + "Incorrect Guesses Remaining");
                        
                    } else
                    {
                        numGames++;
                        statusText = "Sorry! The correct word was " + mainFuncs.charsToString(currentWord) + ". You have solved " + numWins + " puzzle(s) out of " + numGames;
                        //statusLabel.Text = statusText;
                        ChangeLabelText(statusLabel, statusText);
                        scrollText(true);
                        isGameActive = false;
                    }
                }
                if (!isGameActive && !noMoreWords)
                {
                    //promptLabel.Text = "Play Another Game? [Y/N]";
                    ChangeLabelText(promptLabel, "Play Another Game? [Y/N]");
                }
                else if (!isGameActive && noMoreWords)
                {
                    //promptLabel.Text = "No More Words. Press [N] to End.";
                    ChangeLabelText(promptLabel, "No More Words. Press [N] to End.");
                    isGameOver = true;
                }
            }
        }

        //controls scrolling of text that is longer than 20 characters
        public void scrollText(bool startStop)
        {
            if (startStop && serialSend.Enabled == false)
            {
                serialSend = new System.Timers.Timer(250);
                serialSend.AutoReset = true;
                timerIndex = 1;
                serialSend.Elapsed += onTimedEvent;
                scrollCount = 2;
                serialSend.Start();
            }
            else if (!startStop && serialSend.Enabled == true)
            {
                serialSend.Stop();
            }
        }

        private void onTimedEvent(object source, ElapsedEventArgs e)
        {
            int textLength = statusText.Length;
            int remainingLength;
            //while text hasn't fully covered the LCD yet
            if(timerIndex < 20)
            {
                serialWrite(spaceArray, statusText.ToCharArray(), 20 - timerIndex);
            }
            //while the text fully covers the LCD
            else if(timerIndex <= textLength)
            {
                char[] toSend = new char[20];
                for(int i=0; i<20; i++)
                {
                    toSend[i] = statusText[timerIndex - 20 + i];
                }
                serialWrite(toSend);
            }
            //while the text is leaving the LCD
            else if((remainingLength = textLength + 20 - timerIndex) > 0)
            {
                char[] toSend = new char[remainingLength];
                int charIndex = textLength - 1;
                for (int i = remainingLength-1; i >= 0; i--)
                {
                    toSend[i] = statusText[charIndex];
                    charIndex--;
                }
                serialWrite(toSend);
            }
            else
            {
                if (scrollCount > 0)
                {
                    scrollCount--;
                    timerIndex = 0; //Restart the scrolling
                }
                else if (!noMoreWords)
                {
                    serialWrite("New Game?".ToCharArray());
                    serialSend.Stop();
                }
                else {
                    serialWrite("No Words Left".ToCharArray());
                    serialSend.Stop();
                }
            }
            timerIndex++;
        }

        //Combines two different strings for sending to LCD.  First string is sent up to but not including
        // breakpoint index.  Second string is sent from breakpoint to index 20.
        private void serialWrite(char[] first, char[] last, int breakpoint)
        {
            char[] toSend = new char[20];
            for (int i=0; i<breakpoint; i++)
            {
                toSend[i] = first[i];
            }
            for(int i=breakpoint; i<20; i++)
            {
                toSend[i] = last[i-breakpoint];
            }
            serialWrite(toSend);
        }

        //writes the desired data to the LCD from the first address.  Clears the display before writing.
        public void serialWrite(char[] c)
        {
            //make byte array of each of the characters in the word
            Byte[] charBytes = new Byte[c.Length];
            for (int i = 0; i < c.Length; i++)
            {
                charBytes[i] = Convert.ToByte(c[i]);
            }

            sendBytes = new Byte[(charBytes.Length * 6) + 12];
            Byte[] temp = new Byte[6];
            //Send command to clear the display with RS = 0
            temp = byteSend(Convert.ToByte(0x01), 0);
            for (int k = 0; k < 6; k++)
            {
                sendBytes[k] = temp[k];
            }
            //Send command to move to LCD First character to byteSend with RS = 0
            temp = byteSend(Convert.ToByte(0x80), 0);
            for (int k = 6; k < 12; k++)
            {
                sendBytes[k] = temp[k-6];
            }
            //send each character to byteSend with RS = 1
            for (int j = 0; j < charBytes.Length; j++)
            {
                temp = byteSend(charBytes[j], 1);
                for (int k = 0; k < 6; k++)
                {
                    sendBytes[6 * j + 12 + k] = temp[k];
                }
            }
            for (int i = 0; i < sendBytes.Length / 6; i++)
            {
                //Console.WriteLine(sendBytes[6 * i].ToString() + " " + sendBytes[6 * i + 1].ToString() + " " + sendBytes[6 * i + 2].ToString() + " " + sendBytes[6 * i + 3].ToString() + " " + sendBytes[6 * i + 4].ToString() + " " + sendBytes[6 * i + 5].ToString());
            }

            try {
                //comPort.Write(sendBytes, 0, sendBytes.Length);
                comPort.Write(c, 0, c.Length); //For LogicStudio Probing ONLY
            }
            catch (Exception e)
            {
                ChangeLabelText(statusLabel, "No Device Connected!");
            }



        }

        private Byte[] byteSend(Byte b, int RS)
        {
            Byte[] charSend = new Byte[6];
            //Both Nybbles are stored in the middle 4 bits of a byte
            int highNybble = (b & 0xF0) >> 2;
            int lowNybble = (b & 0x0F) << 2;
            charSend[0] = Convert.ToByte(highNybble | 0x00 | ((RS & 0x01) << 6));
            charSend[1] = Convert.ToByte(highNybble | 0x80 | (RS << 6));
            charSend[2] = Convert.ToByte(highNybble | 0x00 | (RS << 6));
            charSend[3] = Convert.ToByte(lowNybble | 0x00 | (RS << 6));
            charSend[4] = Convert.ToByte(lowNybble | 0x80 | (RS << 6));
            charSend[5] = Convert.ToByte(lowNybble | 0x00 | (RS << 6));
            return charSend;
        }

        public void changeImage(int state)
        {
            if (state.Equals(1))
            {
                statusPic.Image = Hangman.Properties.Resources.State1;
            }
            else if (state.Equals(2))
            {
                statusPic.Image = Hangman.Properties.Resources.State2;
            }
            else if (state.Equals(3))
            {
                statusPic.Image = Hangman.Properties.Resources.State3;
            }
            else if (state.Equals(4))
            {
                statusPic.Image = Hangman.Properties.Resources.State4;
            }
            else if (state.Equals(5))
            {
                statusPic.Image = Hangman.Properties.Resources.State5;
            }
            else if (state.Equals(6))
            {
                statusPic.Image = Hangman.Properties.Resources.State6;
            }
            else if (state.Equals(7))
            {
                statusPic.Image = Hangman.Properties.Resources.State7;
            }
        }
    }





    public class mainFuncs {

        public mainFuncs()
        {

        }

        public String charsToString(char[] cArray)
        {
            String s = "";
            foreach (char c in cArray)
            {
                s = s + c.ToString();
            }
            return s;
        }

        public char[] newDispWord(int length)
        {
            char[] cArray = new char[length];
            for (int i=0; i<cArray.Length; i++)
            {
                cArray[i] = '_';
            }

            return cArray;
        }

        public char[] insertLetter (char c, char[] gameWord, char[] displayedWord)
        {
            for (int i=0; i<gameWord.Length; i++)
            {
                if (gameWord[i] == c)
                {
                    displayedWord[i] = c;
                }
            }
            return displayedWord;
        }
        public Boolean isFull (char[] word)
        {
            foreach (char ch in word)
            {
                if (ch == '_') return false;
            }
            return true;
        }
    }
}
