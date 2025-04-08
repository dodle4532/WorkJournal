using System;
using System.Windows;
using System.Windows.Controls;
using WorkJournal.Classses;

namespace WorkJournal.Forms
{
    /// <summary>
    /// Логика взаимодействия для QuestionChangeForm.xaml
    /// </summary>
    public partial class QuestionChangeForm : Window
    {
        private RequestData requestData;
        private DatabaseHelper databaseHelper;
        private string userName;
        JournalWindow journalWindow;

        public QuestionChangeForm(RequestData requestData, string userName, JournalWindow journalWindow)
        {
            InitializeComponent();
            databaseHelper = new DatabaseHelper();
            this.requestData = requestData;
            this.userName = userName;
            this.journalWindow = journalWindow;
            string status = requestData.Status;
            if (status == "created")
            {
                if (databaseHelper.GetUserType(userName) == "constructor" &&
                    databaseHelper.GetDepName(databaseHelper.GetDep_idFromUserName(userName)) == requestData.Department)
                {
                    SetFormToCreateAnswerStage();
                }
                else
                {
                    SetQuestionPage();
                    if (requestData.Answer != "")
                    {
                        SetAnswerPage();
                    }
                }
            }
            if (status == "solved")
            {
                if (databaseHelper.GetUserType(userName) == "technolog" && userName == requestData.Technolog)
                {
                    SetFormToCreateDecisionStage();
                }
                else
                {
                    SetQuestionPage();
                    SetAnswerPage();
                }
            }
            if (status == "closed")
            {
                SetQuestionPage();
                SetAnswerPage();
            }
            Show();
        }

        private void SetQuestionPage()
        {
            userNameBox.Text = requestData.Technolog;
            userNameBox.IsEnabled = false;
            questionTextBox.Text = requestData.Question;
            questionTextBox.IsEnabled = false;
            doc_id_TextBox.Text = requestData.DocId;
            doc_id_TextBox.IsEnabled = false;

            if (requestData.Comment != "")
            {
                SetCommentPage();
            }
        }

        private void SetAnswerPage()
        {
            TabItem secondTab = (TabItem)tabControl.Items[1];
            secondTab.Visibility = Visibility.Visible;
            nameTextBox.Text = requestData.Constructor;
            nameTextBox.IsEnabled = false;
            questionTextBoxInAnswerPage.Text = requestData.Question;
            questionTextBoxInAnswerPage.IsEnabled = false;
            answerTextBox.Text = requestData.Answer;
            answerTextBox.IsEnabled = false;
        }

        private void SetCommentPage()
        {
            TabItem thirdTab = (TabItem)tabControl.Items[2];
            thirdTab.Visibility = Visibility.Visible;
            commentTextBox.Text = requestData.Comment;
            commentTextBox.IsEnabled = false;
        }

        private void SetFormToCreateAnswerStage()
        {
            SetAnswerPage();
            nameTextBox.Text = userName;
            nameTextBox.IsEnabled = false;
            answerTextBox.IsEnabled = true;
            addAnswerGrid.Visibility = Visibility.Visible;
            SetQuestionPage();
        }

        private void SetFormToCreateDecisionStage()
        {
            buttonDecisionGrid.Visibility = Visibility.Visible;
            TabItem thirdTab = (TabItem)tabControl.Items[2];
            thirdTab.Visibility = Visibility.Visible;
            SetCommentPage();
            SetAnswerPage();
            SetQuestionPage();
            commentTextBox.IsEnabled = true;
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (answerTextBox.Text == "")
            {
                MessageBox.Show("Напишите текст ответа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                databaseHelper.AddNewAnswer(requestData.QuestionId, databaseHelper.GetUserId(userName), DateTime.Now.ToString(), answerTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Обратитесь к администратору", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("Ответ успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            journalWindow.FillData();
            Close();
        }

        private void declineButton_Click(object sender, RoutedEventArgs e)
        {
            string ans_id = databaseHelper.GetAnswerId(requestData.Answer, requestData.AnswerTime);
            if (commentTextBox.Text != "")
            {
                string req_id = databaseHelper.GetRequestId(ans_id);
                databaseHelper.AddComment(req_id, commentTextBox.Text);
            }
            databaseHelper.ChangeStatusOfRequest(ans_id, false);
            MessageBox.Show("Статус вопроса успешно изменен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            journalWindow.FillData();
            Close();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            string ans_id = databaseHelper.GetAnswerId(requestData.Answer, requestData.AnswerTime);
            if (commentTextBox.Text != "")
            {
                string req_id = databaseHelper.GetRequestId(ans_id);
                databaseHelper.AddComment(req_id, commentTextBox.Text);
            }
            databaseHelper.ChangeStatusOfRequest(ans_id, true);
            MessageBox.Show("Статус вопроса успешно изменен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            journalWindow.FillData();
            Close();
        }
    }
}
