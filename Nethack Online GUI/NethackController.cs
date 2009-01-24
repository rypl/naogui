using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Nethack_Online_GUI
{
    class NethackController
    {
        const int NUM_TILES_COL = 40;
        const int CONSOLE_WIDTH = 80;
        const int CONSOLE_HEIGHT = 24;

        SocketConnection client;
        ASCIIEncoding encoding;

        byte[] Wills = {
                            0xff, 0xfb, 0x1f,	// Will negotitate about window size
                            0xff, 0xfb, 0x20,	// Will Terminal Speed
                            0xff, 0xfb, 0x18,	// Will Terminal Type
                            0xff, 0xfb, 0x27,	// Will New Enviroment Option
                            0xff, 0xfb, 0x03,	// Will Suppress Go Ahead
                            0xff, 0xfc, 0x01,	// Won't Echo
                            0xff, 0xfc, 0x23,	// Won't X Display Location
                            0xff, 0xfc, 0x01,	// Won't Echo
                            0xff, 0xfc, 0x21,	// Won't Remote Flow Control
                      },

                        Dos =
                        {
                            0xff, 0xfd, 0x01,	// Do Echo
                                      0xff, 0xfd, 0x03,	// Do Supress Go Ahead
                                      0xff, 0xfe, 0x05,	// Don't Status
                        },

                      SoBs =
                      {
                          
                          0xff, 0xfa, 0x1f, 0x00, 0x05, 0x00, 0x18, 0xff, 0xf0,	// SB: Negotitate About Window Size
                          0xff, 0xfa, 0x20, 0x00, 0x33, 0x38, 0x34, 0x30, 0x30, 0x2c, 0x33, 0x38, 0x34, 0x30, 0x30, 0xff, 0xf0,	// SB: Terminal Speed 38400,38400
                          0xff, 0xfa, 0x18, 0x00, 0x58, 0x54, 0x45, 0x52, 0x4d, 0xff, 0xf0,	// SB: Terminal Type \0XTERM
                      };

        Bitmap tiles;

        public NethackController()
        {
            tiles = new Bitmap(@"..\..\nhtiles.bmp");
            client = new SocketConnection();
            encoding = new ASCIIEncoding();
        }

        public void Paint(Graphics graph)
        {
            //graph.FillRectangle(new SolidBrush(Color.Black), new Rectangle(10, 110, 1280, 384)); //Dungeon Area
            graph.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, 1280, 384)); //Dungeon Area

            //drawTile(0, 0, graph);
            drawTile(0, graph);

            graph.Dispose();
        }

        public void drawTile(int col, int row, Graphics graph)
        {
//            graph.DrawImage(tiles, new Rectangle(10, 110, 16, 16), new Rectangle(col * 16, row * 16, 16, 16), GraphicsUnit.Pixel);
            graph.DrawImage(tiles, new Rectangle(0, 0, 16, 16), new Rectangle(col * 16, row * 16, 16, 16), GraphicsUnit.Pixel);
        }

        public void drawTile(int tileNum, Graphics graph)
        {
            int row = tileNum / NUM_TILES_COL;
            int col = tileNum - (row * NUM_TILES_COL);

            drawTile(col, row, graph);
        }

        // Connect to server and send initial responses
        public void Connect(string host, int port)
        {
            byte[] recvData;
            int recvBytes;

            client.Connect(host,port);

            recvBytes = client.ReceiveData();
            recvData = client.GetData();

            // Initial things our client will do
            SendCommand(TelnetHelper.WILL, 0x03);   // WILL Supress Go Ahead
            SendCommand(TelnetHelper.DO,   0x03);   // DO Suppress Go Ahead
            SendCommand(TelnetHelper.WILL, 0x1F);   // WILL Negotiate about window size 
            SendCommand(TelnetHelper.WILL, 0x20);   // WILL Terminal Speed 
            SendCommand(TelnetHelper.WILL, 0x18);   // WILL Terminal Type 
            SendCommand(TelnetHelper.WILL, 0x27);   // WILL New Enviroment Option
            SendCommand(TelnetHelper.WILL, 0x01);   // WILL Echo
 
            DisplayReceivedData(recvData, recvBytes);

            ProcessReceivedData(recvData, recvBytes);
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

        // Process Received Data, do not use for printing
        // This is where all the magic happens
        public void ProcessReceivedData(byte[] data, int dataLength)
        {
            //foreach (byte dataByte in data)
            for (int i = 0; i < dataLength; ++i)
            {
                byte dataByte = data[i];
                byte dataNextByte = 0;
                if (i + 1 < data.Length) dataNextByte = data[i + 1];


                if (dataByte != 0)
                {
                    if (dataByte == TelnetHelper.SB)
                    {

                    }
                    else if (dataByte == TelnetHelper.SE)
                    { }
                    else if (dataByte == TelnetHelper.WILL)
                    { }
                    else if (dataByte == TelnetHelper.WONT)
                    { }
                    else if (dataByte == TelnetHelper.DO)
                    {
                        if (TelnetHelper.GetOptionDescription(dataNextByte) == "Terminal Type")
                            SendSubNegotiation(dataNextByte, encoding.GetBytes("\0XTERM"));
                        else if (TelnetHelper.GetOptionDescription(dataNextByte) == "Terminal Speed")
                            SendSubNegotiation(dataNextByte, encoding.GetBytes("\038400,38400"));
                        else if (TelnetHelper.GetOptionDescription(dataNextByte) == "X Display Location")
                            SendCommand(TelnetHelper.WONT, 0x23); // Won't display X Location
                        else if (TelnetHelper.GetOptionDescription(dataNextByte) == "New Environment Option")
                            SendSubNegotiation(dataNextByte, encoding.GetBytes("\0"));
                    }
                    else if (dataByte == TelnetHelper.DONT)
                    {}
                    else if (dataByte == TelnetHelper.IAC)
                    {}
                    else if (dataByte == TelnetHelper.ESC)
                    {
                        int endLocation = (data.ToString()).IndexOf("\0", i);

                        // The next packet will contain this, but for now throw exception
                        if (endLocation == -1)
                        {
                            throw new Exception("Check Next Packet");
                        }

                        byte[] dataLine = new byte[endLocation - i];

                        Array.Copy(data, dataLine, dataLine.Length);

                        Console.Write(dataLine);
                    }
                    else
                    {
                        Console.Write(TelnetHelper.GetOptionDescription(dataByte) + " "); //dataByte.ToString("X")
                    }
                }

                else
                    break;
            }
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

                else // no more data in buffer, break out of loop
                    break;
            }

            Console.WriteLine();
        }
    }
}