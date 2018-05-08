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


        HttpClient ConfiguredHttpClient()
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(Properties.Settings.Default.baseAddress)
            };

            string token = $"{ string.Empty }:{ Properties.Settings.Default.token}";
            string encodedToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(token));

            client.DefaultRequestHeaders
                  .Authorization = new AuthenticationHeaderValue("Basic", encodedToken);

            return client;
        }

        public List<int> GetWorkItemsIds()
        {
            List<int> ids = new List<int>();

            try
            {
                JObject workItemsResponse;

                using (var client = ConfiguredHttpClient())
                {
                    var query = "Select [System.Id] From WorkItems";
                    var content = new StringContent("{ \"query\": \"" + query + "\" }",
                                                    Encoding.UTF8,
                                                    "application/json");

                    using (HttpResponseMessage response = client.PostAsync("_apis/wit/wiql?api-version=4.1", content).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        workItemsResponse = JObject.Parse(responseBody);
                    }
                }

                JArray workingItems = (JArray)workItemsResponse["workItems"];

                foreach (dynamic workItem in workingItems)                
                    ids.Add((int)workItem.id);

                return ids;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public void Get()
        {
            using (var client = ConfiguredHttpClient())
            {
                string fields = "fields=System.Id,System.Title,System.WorkItemType,System.IterationPath,System.AreaPath,System.State";

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=1&" + fields + "&api-version=4.1").Result;

                //check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    //set the viewmodel from the content in the response
                    //var viewModel = response.Content.ReadAsAsync<object>().Result;

                    var value = response.Content.ReadAsStringAsync().Result;
                }
            }
        }
    }
}
