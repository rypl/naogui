using System;

using System.IO;
using System.Net.Sockets;
//using System.Net;

class SocketConnection
{
    Socket socket;
    bool connected;

    public SocketConnection()
    {
        connected = false;
    }

    public SocketConnection(string host, int port)
    {
        Connect(host, port);
    }

    public bool Connect(string host, int port)
    {
        connected = false;

        try
        {
            connected = true;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(host, port);
        }

        catch (Exception e)
        {
            connected = false;
            Console.WriteLine("Error: " + e.ToString());
        }

        return connected;
    }

    public bool Disconnect()
    {
        if (connected == true)
        {
            socket.Close();

            connected = false;
        }

        return connected;
    }

    public bool SendData(byte[] data)
    {
        socket.Send(data);

        return true;
    }

    public bool SendData(byte[] data, bool partial)
    {
        if (partial == true)
            socket.Send(data, SocketFlags.Partial);
        else
            socket.Send(data);

        return true;
    }

    public byte[] GetData()
    {
        byte[] buffer = new byte[socket.ReceiveBufferSize];

        socket.Receive(buffer);

        return buffer;
    }

    public bool isConnected()
    {
        return connected;
    }
}
