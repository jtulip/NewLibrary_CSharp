using Library.Interfaces.Controllers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library.Controls
{
    public partial class MainMenuControl : UserControl, IMainMenuListener
    {
        internal IMainMenuListener _listener;

        public MainMenuControl(IMainMenuListener listener)
        {
            _listener = listener;
            InitializeComponent();
        }

        public void borrowBook()
        {
            throw new NotImplementedException();
        }

        private void borrowButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Main Menu : Borrow Button clicked");
            _listener.borrowBook();
        }
    }
}
