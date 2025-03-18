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
    /// Логика взаимодействия для AddQuestionForm.xaml
    /// </summary>
    public partial class AddQuestionForm : Window
    {

        private string userName;
        private DatabaseHelper databaseHelper;
        public AddQuestionForm(string userName)
        {
            InitializeComponent();
            this.userName = userName;
            userNameBox.Text = userName;
            databaseHelper = new DatabaseHelper();
            List<string> departments = databaseHelper.GetAllDepartmentsName();
            foreach(string department in departments)
            {
                depComboBox.Items.Add(department);
            }
            Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string questionText = questionTextBox.Text.ToString();
            string doc_id = doc_id_TextBox.Text.ToString();
            string dep_id = (depComboBox.SelectedIndex + 1).ToString();
            if (questionText == "")
            {
                MessageBox.Show("Введите ваш вопрос");
                return;
            }
            if (doc_id == "")
            {
                MessageBox.Show("Введите номер документа");
                return;
            }
            if (dep_id == "0")
            {
                MessageBox.Show("Укажите отдел, куда направить запрос");
                return;
            }
            try
            {
                databaseHelper.AddNewQuestion(doc_id, questionText, dep_id, databaseHelper.GetUserId(userName), DateTime.Now.ToString());
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Обратитесь к администратору");
                return;
            }
            MessageBox.Show("Вопрос успешно добавлен!");
            Close();
        }
    }
}
