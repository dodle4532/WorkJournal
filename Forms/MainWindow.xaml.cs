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
using WorkJournal.Classses;
using Npgsql;

namespace WorkJournal.Forms
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DatabaseHelper databaseHelper;

        public MainWindow()
        {
            try
            {
                databaseHelper = new DatabaseHelper();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            InitializeComponent();
            Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string loginStr = login.Text;
            string passwordStr = password.Password;
            if (loginStr == "")
            {
                MessageBox.Show("Не введен логин");
                return;
            }
            if (passwordStr == "")
            {
                MessageBox.Show("Не введен пароль");
                return;
            }
            (string, string) userInfo = ("", "");
            try
            {
                userInfo = databaseHelper.GetUserTypeAndUserName(loginStr, passwordStr);
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Обратитесь к администратору");
                return;
            }
            string userName = userInfo.Item1;
            string userType = userInfo.Item2;
            if (userType == "")
            {
                MessageBox.Show("Неправильный логин или пароль!");
                return;
            }
            JournalWindow window = new JournalWindow(userType, userName);
            Close();
            window.Show();
        }
    }
}
