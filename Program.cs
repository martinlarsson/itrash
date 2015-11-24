using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Itrash
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Console.WriteLine("En apa och en katt, vilken underbar skatt!");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1Test());
        }
    }
}
