using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetWorkItems
{
    public class DAO
    {        
        //ToDo: Verificar viabilidade de implementar um controle de transação
        //ToDo : Quantificar tempo de insert normal e paralelo
        void InsertWorkItems(List<WorkItem> workItems)
        {
            string sqlConnectionString = Properties.Settings.Default.sqlConnectionString;

            using (var conn = new SqlConnection(sqlConnectionString))
            {
                foreach (var workItem in workItems)
                {
                    for (int i = 0; i < 3600; i++) //Numero de vezes que o registro será duplicado
                    {
                        string query = $@"INSERT INTO WorkItems Values
                                         (
                                             { workItem.Id },
                                             { workItem.Fields["System.Title"] },
                                             { workItem.Fields["System.WorkItemType"] },
                                             { workItem.Fields["System.IterationPath"] },
                                             { workItem.Fields["System.AreaPath"] },
                                             { workItem.Fields["System.State"] }
                                          )";

                        SqlCommand cmd = new SqlCommand(query, conn);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Inserting a new WorkItem...");
                        conn.Close();

                    }                    
                }
            }
        }
    }
}
