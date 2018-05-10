using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public void InsertWorkItems(List<JObject> workItems)
        {
            Exception processException = null;

            using (var conn = new SqlConnection(sqlConnectionString))
            {
               conn.Open();

               Parallel.ForEach(workItems, (workItem, loopState) =>
               {
                   try
                   {
                        for (int i = 0; i < 3600; i++) //Numero de vezes que o registro será duplicado
                        {

                            string query = $@"  Insert into WorkItems 
                                                SELECT Id, AreaPath, IterationPath,
                                                     WorkItemType, State, Title, NEWID()
                                                FROM
                                                OpenJson(' { workItem.ToString() }')
                                                  WITH
                                                    (Id bigint '$.""System.Id""', 
                                                    AreaPath VARCHAR(200) '$.""System.AreaPath""',
                                                    IterationPath VARCHAR(200) '$.""System.IterationPath""', 
                                                    WorkItemType VARCHAR(200) '$.""System.WorkItemType""',
                                                    State VARCHAR(200) '$.""System.State""', 
                                                    Title VARCHAR(200) '$.""System.Title""'
                                                    ) ";


                            using (SqlCommand cmd = new SqlCommand(query, conn))
                                cmd.ExecuteNonQuery();

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
