﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GetWorkItems
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new WorkItemsHandler();
            a.GetWorkItems();
        }

        //static int[] GetWorkItemsIds()
        //{

        //}

        

      
    }
}
