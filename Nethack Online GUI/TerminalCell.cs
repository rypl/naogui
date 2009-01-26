using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Nethack_Online_GUI
{
    public class TerminalCell
    {
        public int row, col;
        public Color color;
        public FontStyle fontStyle;
        public char character;

        public TerminalCell()
        {
            row = -1;
            col = -1;
            color = Color.White;
            fontStyle = new FontStyle();
            character = ' ';
        }
    }
}