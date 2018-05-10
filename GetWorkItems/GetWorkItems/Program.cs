using System;
using System.Data.SqlClient;
using System.IO;

namespace GetWorkItems
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                WorkItemsHandler handler = new WorkItemsHandler();
                DAO dao = new DAO(Properties.Settings.Default.sqlConnectionString);

                var workItems = handler.GetWorkItems();

                dao.InsertWorkItems(workItems);
                
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        static void LogError(Exception ex)
        {
            string error = $"Message: { ex.Message },  StackTrace: {ex.StackTrace}";
            try
            {
                error += ex.InnerException != null ?
                                        ($"Inner Exception: {ex.InnerException.Message + " " + ex.InnerException.InnerException?.Message } ")
                                        : "";

                using (var conn = new SqlConnection(Properties.Settings.Default.sqlConnectionString))
                {
                    string query = $@"INSERT INTO dbo.ErrorLogs ( Error ) VALUES (@error)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@error", error);                    
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception exp)
            {
                string logError = error + exp.Message;
                string logFileName = $"log{ DateTime.Now.ToString("ddMMyyyyHHmmss") }.txt";
                string path = Directory.GetCurrentDirectory() + "\\" + logFileName;

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(logError);
                    sw.Close();
                }
            }
        }
    }
}
