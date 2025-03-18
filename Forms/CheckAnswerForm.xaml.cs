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
    /// Логика взаимодействия для CheckAnswerForm.xaml
    /// </summary>
    /// 

    public partial class CheckAnswerForm : Window
    {
        private List<(string text, string answer_id)> answer;
        private DatabaseHelper databaseHelper;
        private string userName;


        public CheckAnswerForm(string userName)
        {
            InitializeComponent();
            decisionGrid.Visibility = Visibility.Hidden;
            this.userName = userName;
            databaseHelper = new DatabaseHelper();
            answer = databaseHelper.GetAnswer(userName);
            if (answer.Count() == 0)
            {
                MessageBox.Show("Нет ответов для вас!");
                Close();
            }
            else
            {
                Show();
            }
            for (int i = 0; i < answer.Count; ++i)
            {
                answ_id_comboBox.Items.Add(answer[i].answer_id);
            }
        }

        private void doc_Id_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = answ_id_comboBox.SelectedIndex;
            answerTextBox.Text = answer[index].text;
            decisionGrid.Visibility = Visibility.Visible;
        }

        private void declineButton_Click(object sender, RoutedEventArgs e)
        {
            string ans_id = answ_id_comboBox.SelectedItem.ToString();
            if (commentTextBox.Text != "")
            {
                string req_id = databaseHelper.GetRequestId(ans_id);
                databaseHelper.AddComment(req_id, commentTextBox.Text);
            }
            databaseHelper.ChangeStatusOfRequest(ans_id, false);
            MessageBox.Show("Статус вопроса успешно изменен!");
            Close();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            databaseHelper.ChangeStatusOfRequest(answ_id_comboBox.SelectedItem.ToString(), true);
            MessageBox.Show("Статус вопроса успешно изменен!");
            Close();
        }
    }
}
