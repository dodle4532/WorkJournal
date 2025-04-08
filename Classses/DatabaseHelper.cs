using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkJournal.Classses
{
    public class DatabaseHelper : IDisposable
    {
        private static readonly string connectionString = "Host=localhost;Port=5432;Database=WorkJournal;Username=postgres;Password=admin";
        private NpgsqlConnection connection;
        private bool disposed = false;

        public DatabaseHelper()
        {
            connection = new NpgsqlConnection(connectionString);
            try
            {
                connection.Open();
            }
            catch (System.InvalidOperationException) { throw; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (connection != null)
                    {
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            connection.Close();
                        }
                        connection.Dispose();
                        connection = null;
                    }
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private List<List<string>> GetDataFromDatabase(string query, int paramCount)
        {
            List<List<string>> res = new List<List<string>>();
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new List<string>());
                        for (int i = 0; i < paramCount; ++i)
                        {
                            res.Last().Add(reader.GetValue(i).ToString());
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Произошла ошибка при подключении к базе данных: " + ex.Message); // Хочу кинуть его дальше, как
                //throw;
            }
            return res;
        }

        public (string userName, string userType) GetUserTypeAndUserName(string login, string password)
        {
            List<List<string>> response = GetDataFromDatabase("select name, type from \"Users\" where login = '"
                                                              + login + "' and password = '" + password + "'", 2);
            string userName = "";
            string userType = "";

            if (response.Count() == 1)
            {
                userName = response[0][0];
                userType = response[0][1];
            }
            return (userName, userType);
        }

        public List<string> GetAllConstuctorDepartmentsName()
        {
            List<string> res = new List<string>();
            List<List<string>> response = GetDataFromDatabase("select d.name from \"Departments\" d left join \"Users\" u on d.id = u.dep_id " +
                                                              "where u.type = 'constructor'", 1);
            foreach (List<string> department in response)
            {
                res.Add(department[0]);
            }
            return res;
        }

        public string GetUserId(string userName)
        {
            List<List<string>> response = GetDataFromDatabase("select id from \"Users\" where name = '" + userName + "'", 1);
            return response[0][0];
        }

        public string GetUserType(string userName)
        {
            List<List<string>> response = GetDataFromDatabase("select type from \"Users\" where name = '" + userName + "'", 1);
            return response[0][0];
        }

        public List<(string text, string request_id)> GetAllStartRequests(string dep_id)
        {
            List<List<string>> tmp = GetDataFromDatabase("select question, id from \"Requests\" where status = 'created' and " +
                                                         "dep_id = " + dep_id, 2);
            List<(string text, string request_id)> res = new List<(string text, string request_id)>();
            foreach (List<string> req in tmp)
            {
                res.Add((req[0], req[1]));
            }
            return res;
        }

        public List<string> GetAllRequests_id()
        {
            List<List<string>> tmp = GetDataFromDatabase("select id from \"Requests\"", 1);
            List<string> res = new List<string>();
            foreach (List<string> req in tmp)
            {
                res.Add(req[0]);
            }
            return res;
        }

        public string GetDep_idFromUserName(string userName)
        {
            List<List<string>> tmp = GetDataFromDatabase("select dep_id from \"Users\" where name ='" + userName + "'", 1);
            return tmp[0][0];
        }

        public string GetDep_idFromDepName(string depName)
        {
            List<List<string>> tmp = GetDataFromDatabase("select id from \"Departments\" where name ='" + depName + "'", 1);
            return tmp[0][0];
        }

        public string GetDepName(string dep_id)
        {
            List<List<string>> tmp = GetDataFromDatabase("select name from \"Departments\" where id ='" + dep_id + "'", 1);
            return tmp[0][0];
        }



        public string GetRequestId(string answer_id)
        {
            List<List<string>> tmp = GetDataFromDatabase("select req.id from \"Requests\" req, \"Answers\" ans " +
                "                                         where ans.request_id = req.id and ans.id=" + answer_id, 1);
            return tmp[0][0];
        }

        public string GetAnswerId(string answer, string time)
        {
            List<List<string>> tmp = GetDataFromDatabase("select id from \"Answers\" " +
                "                                         where answer = '" + answer + "' and time = '" + time + "'", 1);
            return tmp[0][0];
        }

        public string GetComment(string req_id)
        {
            List<List<string>> tmp = GetDataFromDatabase("select comment from \"Requests\" where id = " + req_id, 1);
            if (tmp.Count == 0)
            {
                return "";
            }
            return tmp[0][0];
        }

        public List<List<string>> GetFullData()
        {
            return GetDataFromDatabase("select r.id, r.status, r.doc_id, d.name, r.question, ut.name, r.time, uc.name, ans.answer," +
                                       " ans.time, r.comment " +
                                        "from \"Requests\" r " +
                                        "left join \"Answers\" ans on ans.request_id = r.id " +
                                        "inner join \"Departments\" d on d.id = r.dep_id " +
                                        "inner join \"Users\" ut on ut.id = r.technolog_id " +
                                        "left join \"Users\" uc on ans.constructor_id = uc.id " +
                                        "order by r.id", 11);
        }

        private void SentRequestToDatabase(string query)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Произошла ошибка при подключении к базе данных: " + ex.Message);
                throw;
            }
            return;
        }

        public void AddNewQuestion(string doc_id, string questionText, string dep_id, string technolog_id, string time)
        {
            string query = "insert into \"Requests\"(doc_id, question, dep_id, status, technolog_id, time) values(" +
                            doc_id + ",'" + questionText + "'," + dep_id + ",'created'," + technolog_id + ",'" + time + "')";
            SentRequestToDatabase(query);
            return;
        }

        public void AddNewAnswer(string request_id, string constructor_id, string time, string answerText)
        {
            string query = "insert into \"Answers\" (constructor_id, answer, time, request_id) values(" +
                            constructor_id + ",'" + answerText + "','" + time + "',"
                            + request_id + "); update \"Requests\" set status = 'solved' where id = " + request_id;
            SentRequestToDatabase(query);
            return;
        }

        public void ChangeStatusOfRequest(string ans_id, bool isAccept)
        {
            string status = isAccept ? "closed" : "created";
            SentRequestToDatabase("update\"Requests\" set status='" + status + "' where id=" + GetRequestId(ans_id));
            return;
        }

        public void AddComment(string req_id, string text)
        {
            SentRequestToDatabase("update \"Requests\" set comment='" + text + "' where id=" + req_id);
            return;
        }

        public void SetNewStatus(string request_id, string status)
        {
            SentRequestToDatabase("update \"Requests\" set status = '" + status + "' where id=" + request_id);
            return;
        }
    }
}
