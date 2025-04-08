using Npgsql;
using System.Windows;
using WorkJournal.Classses;

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
                MessageBox.Show("Не введен логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (passwordStr == "")
            {
                MessageBox.Show("Не введен пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            (string, string) userInfo = ("", "");
            try
            {
                userInfo = databaseHelper.GetUserTypeAndUserName(loginStr, passwordStr);
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Обратитесь к администратору", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            string userName = userInfo.Item1;
            string userType = userInfo.Item2;
            if (userType == "")
            {
                MessageBox.Show("Неправильный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            JournalWindow window = new JournalWindow(userType, userName);
            Close();
            window.Show();
        }
    }
}
