using System;
using System.Windows.Forms;
using Nethack_Online_GUI;
using System.Drawing;

class Controller
{
    static void Main()
    {
        Form MainWindow = new MainWindow();

        Application.EnableVisualStyles();
        Application.Run(MainWindow);
    }
}