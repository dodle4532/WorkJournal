using System.Collections.Generic;
using System.Windows;
using WorkJournal.Classses;

namespace WorkJournal.Forms
{
    /// <summary>
    /// Логика взаимодействия для ChangeStatusForm.xaml
    /// </summary>
    public partial class ChangeStatusForm : Window
    {
        private DatabaseHelper databaseHelper;
        private JournalWindow journalWindow;
        public ChangeStatusForm(JournalWindow journalWindow)
        {
            InitializeComponent();
            this.journalWindow = journalWindow;
            databaseHelper = new DatabaseHelper();
            List<string> requests = databaseHelper.GetAllRequests_id();
            foreach (string req in requests)
            {
                questionComboBox.Items.Add(req);
            }
            statusComboBox.Items.Add("created");
            statusComboBox.Items.Add("solved");
            statusComboBox.Items.Add("closed");
            Show();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedRequest = questionComboBox.SelectedItem.ToString();
            if (selectedRequest == "")
            {
                MessageBox.Show("Выберите вопрос", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            string selectedStatus = statusComboBox.SelectedItem.ToString();
            if (selectedStatus == "")
            {
                MessageBox.Show("Выберите новый статус", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                databaseHelper.SetNewStatus(selectedRequest, selectedStatus);
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("Успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
            journalWindow.FillData();
            return;
        }
    }
}
