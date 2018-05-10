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

        List<int> GetWorkItemsIds()
        {
            List<int> ids = new List<int>();

            try
            {
                JObject apiResponse;

                using (var client = ConfiguredHttpClient())
                {
                    var query = "Select [System.Id] From WorkItems";
                    var requestBody = new StringContent("{ \"query\": \"" + query + "\" }",
                                                    Encoding.UTF8,
                                                    "application/json");

                    using (HttpResponseMessage response = client.PostAsync("_apis/wit/wiql?api-version=4.1", requestBody).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        apiResponse = JObject.Parse(responseBody);
                    }
                }

                JArray workItems = (JArray)apiResponse["workItems"];

                foreach (dynamic workItem in workItems)                
                    ids.Add((int)workItem.id);

                return ids;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<JObject> GetWorkItems()
        {
            try
            {
                List<JObject> workItems = new List<JObject>();

                List<int> ids = GetWorkItemsIds();
                JObject apiResponse = null;

                using (var client = ConfiguredHttpClient())
                {
                    string[] fields = new string[]
                    {
                        "System.Id",
                        "System.Title",
                        "System.WorkItemType",
                        "System.IterationPath",
                        "System.AreaPath",
                        "System.State"
                    };

                    string requestURI = "_apis/wit/workitems?ids=" + string.Join(",", ids) +
                                        "&fields=" + string.Join(",", fields) +
                                        "&api-version=4.1";

                    using (HttpResponseMessage response = client.GetAsync(requestURI).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        apiResponse = JObject.Parse(responseBody);
                    }
                }

                foreach (dynamic workItem in (JArray)apiResponse["value"])                
                    workItems.Add((JObject)workItem.fields);

                return workItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }                
        }
    }
}
