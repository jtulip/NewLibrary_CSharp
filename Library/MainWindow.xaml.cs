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
using System.ComponentModel;

using Library.Interfaces.Hardware;
using Library.Controllers;
using Library.Hardware;
using Library.Interfaces.Daos;
//using Library.Daos;
//using Library.Entities;
using Library.Interfaces.Entities;

namespace Library
{


    public partial class MainWindow : Window, IDisplay
    {
        private UserControl _currentControl;
        private CardReader _reader;
        private Scanner _scanner;
        private Printer _printer;

        private IBookDAO _bookDAO;
        private ILoanDAO _loanDAO;
        private IMemberDAO _memberDAO;

        public MainWindow()
        {
            _reader = new CardReader();
            _scanner = new Scanner();
            _printer = new Printer();
            InitializeComponent();
            _reader.Show();
            _scanner.Show();
            _printer.Show();


            //SetUpTestData();

            MainMenuController mainController =
                new MainMenuController(this, _reader, _scanner, _printer, 
                                        null, null, null);
            mainController.initialise();
        }


        public UserControl Display
        {
            get
            {
                Console.WriteLine("Getting Display");
                return _currentControl;
            }
            set
            {
                Console.WriteLine("Setting Display : " + value);
                Panel.Children.Remove(_currentControl);
                _currentControl = value;
                Panel.Children.Add(_currentControl);

            }
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Console.WriteLine("detected Window closing");
            Application.Current.Shutdown();
        }

        private void SetUpTestData()
        {
            IBook[] book = new IBook[15];
            IMember[] member = new IMember[6];

            book[0]  = _bookDAO.AddBook("author1", "title1", "callNo1");
            book[1]  = _bookDAO.AddBook("author1", "title2", "callNo2");
            book[2]  = _bookDAO.AddBook("author1", "title3", "callNo3");
            book[3]  = _bookDAO.AddBook("author1", "title4", "callNo4");
            book[4]  = _bookDAO.AddBook("author2", "title5", "callNo5");
            book[5]  = _bookDAO.AddBook("author2", "title6", "callNo6");
            book[6]  = _bookDAO.AddBook("author2", "title7", "callNo7");
            book[7]  = _bookDAO.AddBook("author2", "title8", "callNo8");
            book[8]  = _bookDAO.AddBook("author3", "title9", "callNo9");
            book[9]  = _bookDAO.AddBook("author3", "title10", "callNo10");
            book[10] = _bookDAO.AddBook("author4", "title11", "callNo11");
            book[11] = _bookDAO.AddBook("author4", "title12", "callNo12");
            book[12] = _bookDAO.AddBook("author5", "title13", "callNo13");
            book[13] = _bookDAO.AddBook("author5", "title14", "callNo14");
            book[14] = _bookDAO.AddBook("author5", "title15", "callNo15");

            member[0] = _memberDAO.AddMember("fName1", "lName1", "0001", "email1");
            member[1] = _memberDAO.AddMember("fName2", "lName2", "0002", "email2");
            member[2] = _memberDAO.AddMember("fName3", "lName3", "0003", "email3");
            member[3] = _memberDAO.AddMember("fName4", "lName4", "0004", "email4");
            member[4] = _memberDAO.AddMember("fName5", "lName5", "0005", "email5");
            member[5] = _memberDAO.AddMember("fName6", "lName6", "0006", "email6");

            DateTime borrowDate = DateTime.Now;
            TimeSpan loanPeriod = new TimeSpan(LoanConstants.LOAN_PERIOD, 0, 0, 0);
            DateTime dueDate = borrowDate.Add(loanPeriod);

            //create a member with overdue loans		
            for (int i = 0; i < 2; i++)
            {
                ILoan loan = _loanDAO.CreateLoan(member[1], book[i], borrowDate,dueDate);
                _loanDAO.CommitLoan(loan);
            }
            DateTime checkDate = dueDate.Add(new TimeSpan(1, 0, 0, 0));
            _loanDAO.UpdateOverDueStatus(checkDate);

            //create a member with maxed out unpaid fines
            member[2].AddFine(10.0f);

            //create a member with maxed out loans
            for (int i = 2; i < 7; i++)
            {
                ILoan loan = _loanDAO.CreateLoan(member[3], book[i], borrowDate, dueDate);
                _loanDAO.CommitLoan(loan);
            }

            //a member with a fine, but not over the limit
            member[4].AddFine(5.0f);

            //a member with a couple of loans but not over the limit
            for (int i = 7; i < 9; i++)
            {
                ILoan loan = _loanDAO.CreateLoan(member[5], book[i], borrowDate, dueDate);
                _loanDAO.CommitLoan(loan);
            }
        }

    }

}
