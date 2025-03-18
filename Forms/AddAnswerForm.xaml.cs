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
using WorkJournal.Classses;

namespace WorkJournal.Forms
{
    /// <summary>
    /// Логика взаимодействия для AddAnswerForm.xaml
    /// </summary>
    public partial class AddAnswerForm : Window
    {
        private string userName;
        private string dep_id;
        private string request_id;
        private List<(string text, string request_id)> requests;
        private DatabaseHelper databaseHelper;
        public AddAnswerForm(string userName)
        {
            InitializeComponent();
            answerGrid.Visibility = Visibility.Hidden;
            commentGrid.Visibility = Visibility.Hidden;
            this.userName = userName;
            databaseHelper = new DatabaseHelper();
            dep_id = databaseHelper.GetDep_id(userName);
            requests = databaseHelper.GetAllStartRequests(dep_id);
            if (requests.Count() == 0)
            {
                MessageBox.Show("Нет запросов для вас!");
                Close();
            }
            else
            {
                Show();
            }
            for (int i = 0; i < requests.Count; ++i)
            {
                req_id_comboBox.Items.Add(requests[i].request_id);
            }
        }

        private void Doc_Id_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = req_id_comboBox.SelectedIndex;
            questionTextBox.Text = requests[index].text;
            request_id = requests[index].request_id;
            answerGrid.Visibility = Visibility.Visible;
            nameTextBox.Text = userName;
            string comment = databaseHelper.GetComment(request_id);
            if (comment != "")
            {
                commentGrid.Visibility = Visibility.Visible;
                commentTextBox.Text = comment;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (answerTextBox.Text == "")
            {
                MessageBox.Show("Напишите текст ответа");
                return;
            }
            try
            {
                databaseHelper.AddNewAnswer(request_id, databaseHelper.GetUserId(userName), DateTime.Now.ToString(), answerTextBox.Text);
            }
            catch {
                MessageBox.Show("Произошла ошибка! Обратитесь к администратору");
                return;
            }
            MessageBox.Show("Ответ успешно добавлен!");
            Close();
        }
    }
}
