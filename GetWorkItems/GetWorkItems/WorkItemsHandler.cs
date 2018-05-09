using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GetWorkItems
{
    public class WorkItemsHandler
    {
        
        public WorkItemsHandler()
        {

        }

        public List<WorkItem> GetWorkItems()
        {
            Uri uri = new Uri(Properties.Settings.Default.baseAddress);
            string token = Properties.Settings.Default.token;            

            VssBasicCredential credentials = new VssBasicCredential("", token);

            Wiql wiql = new Wiql()
            {
                Query = @" Select [Id] From WorkItems"
            };

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(uri, credentials))
            {
                WorkItemQueryResult workItemQueryResult = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).Result;

                if (workItemQueryResult.WorkItems.Count() != 0)
                {
                    List<int> ids = new List<int>();

                    foreach (var item in workItemQueryResult.WorkItems)
                        ids.Add(item.Id);


                    string[] fields = new string[6]
                    {
                        "System.Id",
                        "System.Title",
                        "System.WorkItemType",
                        "System.IterationPath",
                        "System.AreaPath",
                        "System.State"
                    };                    

                    return workItemTrackingHttpClient.GetWorkItemsAsync(ids,
                                                                        fields,
                                                                        workItemQueryResult.AsOf)
                                                     .Result;

                    
                }

                return null;
            }
        }
    }
}
