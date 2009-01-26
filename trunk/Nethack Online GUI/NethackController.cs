using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Nethack_Online_GUI
{
    public class NethackController
    {
        public const int NUM_TILES_COL = 40; // number of tiles across tileset
        public const int TERMINAL_COLS = 80; // terminal width in characters
        public const int TERMINAL_ROWS = 24; // terminal height in characters
        public const int TILE_SIZE = 16; // n x n tile

        TCPConnection client;
        ASCIIEncoding encoding;
        Bitmap tileSet;
        bool isConnected;
        Font font;
        
        TerminalCell[,] termCells;

        public NethackController()
        {
            tileSet = new Bitmap(@"..\..\nhtiles.bmp");
            client = new TCPConnection();
            encoding = new ASCIIEncoding();
            isConnected = false;
            font = new Font(FontFamily.GenericMonospace, 16, GraphicsUnit.Pixel);

            termCells = new TerminalCell[TERMINAL_COLS, TERMINAL_ROWS];

            for (int r = 0; r < TERMINAL_ROWS; ++r)
                for (int c = 0; c < TERMINAL_COLS; ++c)
                    termCells[c,r] = new TerminalCell(c,r);
        }

        public TerminalCell[,] getTermCells()
        {
            return termCells;
        }

        public void Paint(Graphics graph)
        {
            //graph.FillRectangle(new SolidBrush(Color.Black), new Rectangle(10, 110, 1280, 384)); //Dungeon Area
            graph.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, 1280, 384)); //Dungeon Area

            //drawTile(0, 0, graph);
            drawTile(0, graph);
            drawText("hello world", 0, graph);
            drawText("Line 2", 1, graph);
            drawText("Line 23", 23, graph);
            graph.Dispose();
        }

        public void drawTile(int col, int row, Graphics graph)
        {
            graph.DrawImage(tileSet, new Rectangle(0, 0, TILE_SIZE, TILE_SIZE), new Rectangle(col * TILE_SIZE, row * TILE_SIZE, TILE_SIZE, TILE_SIZE), GraphicsUnit.Pixel);
        }

        public void drawTile(int tileNum, Graphics graph)
        {
            int row = tileNum / NUM_TILES_COL;
            int col = tileNum - (row * NUM_TILES_COL);

            drawTile(col, row, graph);
        }

        public void drawText(string text, int row, Graphics graph)
        {
            drawText(text, 0, row, graph);
        }

        public void drawText(string text, int col, int row, Graphics graph)
        {
            graph.DrawString(text, font, Brushes.White, col * TILE_SIZE, row * TILE_SIZE);
        }

        // Connect to server and send initial responses
        public void Connect(string username, string password, string host, int port)
        {
            if (client.Connected)
            {
                throw new Exception("Already Connected");
            }
            
            client.Connect(host,port);

            // Initial things our client will do /////////////////////////////////////////
            SendCommand(TelnetHelper.WILL, 0x03);   // WILL Supress Go Ahead
            SendCommand(TelnetHelper.DO, 0x03);   // DO Suppress Go Ahead
            SendCommand(TelnetHelper.WILL, 0x1F);   // WILL Negotiate about window size 
            SendCommand(TelnetHelper.WILL, 0x20);   // WILL Terminal Speed 
            SendCommand(TelnetHelper.WILL, 0x18);   // WILL Terminal Type 
            SendCommand(TelnetHelper.WILL, 0x27);   // WILL New Enviroment Option
            SendCommand(TelnetHelper.WILL, 0x01);   // WILL Echo
            //////////////////////////////////////////////////////////////////////////////

            Console.WriteLine("Connected!");
        }

        public void PollTelnetServer()
        {
            if (!client.Connected)
                return;

            int dataLength;
            byte[] dataBytes;

            while (client.DataAvailable)
            {
                dataLength = client.ReceiveData();
                dataBytes = client.GetData();

                for (int i = 0; i < dataLength; ++i)
                {

                    // Interpret As Command
                    if (dataBytes[i] == TelnetHelper.IAC)
                    {
                        // IAC Negotiation Option
                        byte negotiation = dataBytes[i + 1];
                        byte option = dataBytes[i + 2];

                        if (negotiation == TelnetHelper.DO)
                        {
                            if (TelnetHelper.GetOptionDescription(option) == "Terminal Type")
                                SendSubNegotiation(option, encoding.GetBytes("\0XTERM"));
                            else if (TelnetHelper.GetOptionDescription(option) == "Terminal Speed")
                                SendSubNegotiation(option, encoding.GetBytes("\038400,38400"));
                            else if (TelnetHelper.GetOptionDescription(option) == "X Display Location")
                                SendCommand(TelnetHelper.WONT, 0x23); // Won't display X Location
                            else if (TelnetHelper.GetOptionDescription(option) == "New Environment Option")
                                SendSubNegotiation(option, encoding.GetBytes("\0"));
                        }

                        i += 2;
                    }

                    // Terminal Command
                    else if (dataBytes[i] == TelnetHelper.ESC)
                    {
                        // Find end of the command
                        int endOfLine;
                        int endOfCommand = i;

                        for (endOfLine = i; endOfLine < dataLength; ++endOfLine)
                        {
                            if (dataBytes[endOfCommand] != 0x20 && dataBytes[endOfLine] == 0x20) // space
                                endOfCommand = endOfLine;

                            if (dataBytes[endOfLine] == 0)
                                break;
                        }

                        // Copy the data over
                        byte[] terminalLine = new byte[endOfLine - i - (endOfCommand - i)];
                        byte[] terminalCommand = new byte[endOfCommand - i > 0 ? endOfCommand - i - 1 : 0];

                        // Terminal line
                        for (int j = endOfCommand + 1; j < endOfLine; ++j)
                            terminalLine[j - endOfCommand] = dataBytes[j];

                        // Terminal command
                        for (int j = i + 1; j < endOfCommand; ++j)
                            terminalCommand[j - (i + 1)] = dataBytes[j];

                        Console.WriteLine("[" + encoding.GetString(terminalCommand) + "]" + encoding.GetString(terminalLine));

                        i += endOfLine - i - 1;
                    }
                }

                Console.WriteLine();
            }
        }

        public bool DataAvailable
        {
            get
            {
                return client.DataAvailable;
            }
        }

        public bool Connected()
        {
            return isConnected;
        }

        public bool SendCommand(int negotiation, int option)
        {
            byte[] sendData;

            sendData = TelnetHelper.GetCommand(negotiation, option);

            return client.SendData(sendData);
        }

        public bool SendSubNegotiation(int option, byte[] data)
        {
            byte[] SubNegotiationCommand;

            SubNegotiationCommand = TelnetHelper.GetSubNegotiationCommand(option, data);

            return client.SendData(SubNegotiationCommand);
        }

        // Possible needs to be rewritten
        public void DisplayReceivedData(byte[] data, int dataLength)
        {
            for (int i = 0; i < dataLength; ++i)
            {
                byte dataByte = data[i];

                if (dataByte != 0)
                {
                    if (dataByte == TelnetHelper.SB)
                        Console.Write("SB:   ");
                    else if (dataByte == TelnetHelper.SE)
                        Console.Write("SE:    ");
                    else if (dataByte == TelnetHelper.WILL)
                        Console.Write("WILL:  ");
                    else if (dataByte == TelnetHelper.WONT)
                        Console.Write("WON'T: ");
                    else if (dataByte == TelnetHelper.DO)
                        Console.Write("DO:    ");
                    else if (dataByte == TelnetHelper.DONT)
                        Console.Write("DON'T: ");
                    else if (dataByte == TelnetHelper.IAC)
                        Console.Write("\nIAC  ");
                    else if (dataByte == TelnetHelper.ESC)
                    {
                        Console.Write("\nESC: ");
                    }
                    else // Get Option Description of Command
                        Console.Write(TelnetHelper.GetOptionDescription(dataByte) + " "); //dataByte.ToString("X")
                }
            }

            Console.WriteLine();
        }
    }
}