using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class RequestData
    {
        public string Number { get; set; }
        public string QuestionId { get; set; }
        public string DocId { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }
        public string Question { get; set; }
        public string Technologist { get; set; }
        public string QuestionTime { get; set; }
        public string Constructor { get; set; }
        public string Answer { get; set; }
        public string AnswerTime { get; set; }
        public string Comment { get; set; }

        public Brush RowBackground { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для JournalWindow.xaml
    /// </summary>
    public partial class JournalWindow : Window
    {

        public ObservableCollection<RequestData> RequestData { get; set; }

        private string userName;
        private List<int> markedIndex;
        public List<string> getMenuItems(string userType)
        {
            List<string> result = new List<string>();
            if (userType == "admin")
            {
                result.Add("Изменить статус вопроса");
            }
            if (userType == "technolog")
            {
                result.Add("Добавить запрос");
                result.Add("Принять ответ");
            }
            if (userType == "constructor")
            {
                result.Add("Добавить ответ");
            }
            return result;
        }


        public JournalWindow(string userType, string userName)
        {
            InitializeComponent();
            markedIndex = new List<int>();
            this.userName = userName;
            DatabaseHelper databaseHelper = new DatabaseHelper();
            List<List<string>> data = databaseHelper.GetFullData();

            List<string> menuItems = getMenuItems(userType);
            MenuItem exitItem = new MenuItem();
            exitItem.Header = "Выйти";
            exitItem.Click += Relogin;
            MenuItem firstMenuItem = (MenuItem)menu.Items[0];
            firstMenuItem.Items.Add(exitItem);

            MenuItem newItem = new MenuItem();
            newItem.Header = menuItems[0];
            MenuItem secondMenuItem = (MenuItem)menu.Items[1];
            if (userType == "technolog")
            {
                newItem.Click += ShowCreateQuestionPage;
                secondMenuItem.Items.Add(newItem);
                MenuItem secondNewItem = new MenuItem();
                secondNewItem.Header = menuItems[1];
                secondNewItem.Click += ShowCheckAnswerPage;
                secondMenuItem.Items.Add(secondNewItem);
            }
            if (userType == "constructor")
            {
                newItem.Click += ShowAddAnswerPage;
                secondMenuItem.Items.Add(newItem);
            }
            if (userType == "admin")
            {
                newItem.Click += ShowChangeStatusPage;
                secondMenuItem.Items.Add(newItem);
            }


            ObservableCollection<RequestData> requestDataCollection = new ObservableCollection<RequestData>();
            int index = 1;
            foreach (var row in data)
            {
                if (row.Count == 11)
                {
                    RequestData requestData = new RequestData
                    {
                        Number = index.ToString(),
                        QuestionId = row[0],
                        Status = row[1],
                        DocId = row[2],
                        Department = row[3],
                        Question = row[4],
                        Technologist = row[5],
                        QuestionTime = row[6],
                        Constructor = row[7],
                        Answer = row[8],
                        AnswerTime = row[9],
                        Comment = row[10]
                    };
                    if (row[5] == userName || row[7] == userName)
                    {
                        requestData.RowBackground = Brushes.Yellow;
                    }
                    else
                    {
                        requestData.RowBackground = Brushes.White;
                    }

                    requestDataCollection.Add(requestData);
                }
                else
                {
                    Console.WriteLine($"Строка с неверным количеством элементов: {string.Join(", ", row)}");
                }
                index++;
            }

            RequestData = requestDataCollection;
            this.DataContext = this;
            dataGrid.ItemsSource = RequestData;
        }

        private void ShowCreateQuestionPage(object sender, RoutedEventArgs e)
        {
            AddQuestionForm addQuestionForm = new AddQuestionForm(userName);
            addQuestionForm.Topmost = false;
        }

        private void ShowAddAnswerPage(object sender, RoutedEventArgs e)
        {
            AddAnswerForm addAnswerForm = new AddAnswerForm(userName);
            addAnswerForm.Topmost = false;
        }

        private void ShowCheckAnswerPage(object sender, RoutedEventArgs e)
        {
            CheckAnswerForm checkAnswerForm = new CheckAnswerForm(userName);
            checkAnswerForm.Topmost = false;
        }

        private void ShowChangeStatusPage(object sender, RoutedEventArgs e)
        {
            ChangeStatusForm changeStatusForm = new ChangeStatusForm();
            changeStatusForm.Topmost = false;
        }

        private void Relogin(object sender, RoutedEventArgs e)
        {
            MainWindow w = new MainWindow();
            Close();
            return;
        }
    }
}
