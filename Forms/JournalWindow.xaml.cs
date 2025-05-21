using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using WorkJournal.Classses;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using Window = System.Windows.Window;

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
        public string Technolog { get; set; }
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
        DatabaseHelper databaseHelper;
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
            return result;
        }


        public JournalWindow(string userType, string userName)
        {
            InitializeComponent();
            markedIndex = new List<int>();
            this.userName = userName;
            databaseHelper = new DatabaseHelper();

            List<string> menuItems = getMenuItems(userType);
            MenuItem saveItem = new MenuItem();
            saveItem.Header = "Сохранить";
            saveItem.Click += Save;
            MenuItem exitItem = new MenuItem();
            exitItem.Header = "Выйти";
            exitItem.Click += Relogin;
            MenuItem firstMenuItem = (MenuItem)menu.Items[0];
            firstMenuItem.Items.Add(saveItem);
            firstMenuItem.Items.Add(exitItem);
            FillData();
            this.DataContext = this;

            if (userType == "constructor")
            {
                menu.Items.Remove(menu.Items[1]);
            }
            else {
                MenuItem newItem = new MenuItem();
                newItem.Header = menuItems[0];
                MenuItem secondMenuItem = (MenuItem)menu.Items[1];
                if (userType == "technolog")
                {
                    newItem.Click += ShowCreateQuestionPage;
                    secondMenuItem.Items.Add(newItem);
                }
                if (userType == "admin")
                {
                    newItem.Click += ShowChangeStatusPage;
                    secondMenuItem.Items.Add(newItem);
                }
            }
        }

        public void FillData()
        {
            List<List<string>> data = databaseHelper.GetFullData();
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
                        Technolog = row[5],
                        QuestionTime = row[6],
                        Constructor = row[7],
                        Answer = row[8],
                        AnswerTime = row[9],
                        Comment = row[10]
                    };
                    if (databaseHelper.GetUserType(userName) == "admin")
                    {
                        requestData.RowBackground = Brushes.White;
                    }
                    else if (row[5] == userName || row[7] == userName || row[3] == databaseHelper.GetDepName(databaseHelper.GetDep_idFromUserName(userName)))
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
            AddQuestionForm addQuestionForm = new AddQuestionForm(userName, this);
            addQuestionForm.Topmost = false;
        }

        private void ShowChangeStatusPage(object sender, RoutedEventArgs e)
        {
            ChangeStatusForm changeStatusForm = new ChangeStatusForm(this);
            changeStatusForm.Topmost = false;
        }

        private void Relogin(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow w = new AuthorizationWindow();
            Close();
            return;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "WorkJournal";
            saveFileDialog.DefaultExt = ".xlsx";
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog.Title = "Выберите место для сохранения файла";

            DialogResult result = saveFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = excelApp.Workbooks.Add();
                Worksheet worksheet = (Worksheet)workbook.Sheets[1];

               
                for (int j = 0; j < dataGrid.Columns.Count; j++)
                {
                    Range myRange = (Range)worksheet.Cells[1, j + 1];
                    myRange.Value2 = dataGrid.Columns[j].Header;
                }

               
                for (int i = 0; i < dataGrid.Items.Count; i++)
                {
                    for (int j = 0; j < dataGrid.Columns.Count; j++)
                    {
                        TextBlock b = dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock;
                        if (b != null)
                        {
                            Range myRange = (Range)worksheet.Cells[i + 2, j + 1];
                            myRange.Value2 = b.Text;
                        }
                    }
                }

                Range columnRange = worksheet.UsedRange.Columns;
                columnRange.AutoFit();

               
                try
                {
                    workbook.SaveAs(filePath);
                    excelApp.Quit();
                    MessageBox.Show("Сохранено успешно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                   
                    ReleaseObject(worksheet);
                    ReleaseObject(workbook);
                    ReleaseObject(excelApp);
                }
            }
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                System.Windows.MessageBox.Show("Exception occurred while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }


        private void MyDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true; // Отменяем начало редактирования
            DataGridRow row = e.Row;
            RequestData requestData = (RequestData)row.Item;
            dataGrid.SelectedItem = null;
            // Получаем объект данных из DataContext строки
            QuestionChangeForm questionChangeForm = new QuestionChangeForm(requestData, userName, this);
        }
    }
}
