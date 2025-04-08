using System;
using System.Collections.Generic;
using System.Windows;
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
        JournalWindow journalWindow;
        public AddQuestionForm(string userName, JournalWindow journalWindow)
        {
            InitializeComponent();
            this.journalWindow = journalWindow;
            this.userName = userName;
            userNameBox.Text = userName;
            databaseHelper = new DatabaseHelper();
            List<string> departments = databaseHelper.GetAllConstuctorDepartmentsName();
            foreach (string department in departments)
            {
                depComboBox.Items.Add(department);
            }
            Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string questionText = questionTextBox.Text.ToString();
            string doc_id = doc_id_TextBox.Text.ToString();
            string dep_id = databaseHelper.GetDep_idFromDepName(depComboBox.SelectedItem.ToString());
            if (questionText == "")
            {
                MessageBox.Show("Введите ваш вопрос", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (doc_id == "")
            {
                MessageBox.Show("Введите номер документа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (dep_id == "0")
            {
                MessageBox.Show("Укажите отдел, куда направить запрос", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                databaseHelper.AddNewQuestion(doc_id, questionText, dep_id, databaseHelper.GetUserId(userName), DateTime.Now.ToString());
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Обратитесь к администратору", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("Вопрос успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            journalWindow.FillData();
            Close();
        }
    }
}
