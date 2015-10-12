using Library.Interfaces.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Library.Hardware
{
    /// <summary>
    /// Interaction logic for Printer.xaml
    /// </summary>
    public partial class Printer : Window, IPrinter
    {
        public Printer()
        {
            InitializeComponent();

            printBox.Text = "";
        }

        public void print(string printData)
        {
            printBox.Text += printData + "\n\n";
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Console.WriteLine("detected Window closing");
            e.Cancel = true;
        }
    }
}
