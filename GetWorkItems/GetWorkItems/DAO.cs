using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetWorkItems
{
    public class DAO
    {
        string sqlConnectionString;

        public DAO(string _sqlConnectionString)
        {
            sqlConnectionString = _sqlConnectionString;
        }

        //ToDo: Verificar viabilidade de implementar um controle de transação
                
        public void InsertWorkItems(List<WorkItem> workItems)
        {            
            Exception processException = null;

            using (var conn = new SqlConnection(sqlConnectionString))
            {
                conn.Open();
                    
                Parallel.ForEach(workItems,  (workItem, loopState) =>
                {
                    try
                    {
                        for (int i = 0; i < 3600; i++) //Numero de vezes que o registro será duplicado
                        {

                            string query = $@"INSERT INTO WorkItems Values
                                            (
                                               @Id, @Title, @WorkItemType, @IterationPath, @AreaPath, @State, @uID
                                            )";

                            using(SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Id", workItem.Id);
                                cmd.Parameters.AddWithValue("@Title", workItem.Fields["System.Title"]);
                                cmd.Parameters.AddWithValue("@WorkItemType", workItem.Fields["System.WorkItemType"]);
                                cmd.Parameters.AddWithValue("@IterationPath", workItem.Fields["System.IterationPath"]);
                                cmd.Parameters.AddWithValue("@AreaPath", workItem.Fields["System.AreaPath"]);
                                cmd.Parameters.AddWithValue("@State", workItem.Fields["System.State"]);
                                cmd.Parameters.AddWithValue("@uID", Guid.NewGuid().ToString());                                

                                cmd.ExecuteNonQuery();
                            }

                            Console.WriteLine("Inserting a new WorkItem...");
                        }
                    }
                    catch (Exception ex)
                    {
                        loopState.Break();
                        processException = ex;
                    }
                });

                conn.Close();
            }

            if (processException != null)
                throw processException;
            
        }
    }
}
