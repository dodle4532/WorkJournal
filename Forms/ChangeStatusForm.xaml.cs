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
    /// Логика взаимодействия для ChangeStatusForm.xaml
    /// </summary>
    public partial class ChangeStatusForm : Window
    {
        private DatabaseHelper databaseHelper;
        public ChangeStatusForm()
        {
            InitializeComponent();
            databaseHelper = new DatabaseHelper();
            List<string> requests = databaseHelper.GetAllRequests_id();
            foreach(string req in requests)
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
                MessageBox.Show("Выберите вопрос");
                return;
            }
            string selectedStatus = statusComboBox.SelectedItem.ToString();
            if (selectedStatus == "")
            {
                MessageBox.Show("Выберите новый статус");
                return;
            }
            try
            {
                databaseHelper.SetNewStatus(selectedRequest, selectedStatus);
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");
                return;
            }
            MessageBox.Show("Успешно!");
            Close();
            return;
        }
    }
}
