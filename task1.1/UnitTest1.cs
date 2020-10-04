using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RA;
using System.Threading.Tasks;
using RestSharp;

namespace task1._1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAllLaunches()
        {
            //1 all launches
            var client = new RestClient("https://rp.epam.com");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("ui/#vadim_gagarinskiy_personal/launches/", Method.GET);
            var queryResult = client.Execute<List<string>>(request).Data;
        }

        [TestMethod]
        public void TestAllLaunchesWhereTTLMoreThanNumber()
        {
            //2 launches with TTL >20
            const int totalExecutions = 20;
            var client = new RestClient("https://rp.epam.com");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("ui/#vadim_gagarinskiy_personal/launches/{total_executions}", Method.GET);
            request.AddParameter("total_executions", ">20");
            var timer = new Stopwatch();
            timer.Start();
            var response = client.Execute<List<string>>(request);
            timer.Stop();
            
            var queryResult = response.Data;
            Console.WriteLine(queryResult);

            //check StatusCode
            Assert.AreEqual(response.StatusCode,200);

            //3 check launches with status FAILED
            int valueFailedStatus = 0;
            foreach (var q in queryResult)
            {
                {
                    //bool FAILED = check q.status == failed
                }
                //if(FAILED)
                {
                    valueFailedStatus += 1;
                }
            }

            //7 assert that server's response < 1 second
            Assert.IsTrue(timer.ElapsedMilliseconds<1000);
        }

        [TestMethod]
        public void TestCreateDashboard()
        {
            //4 create Dashboard
            var client = new RestClient("https://rp.epam.com");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var requestBefore = new RestRequest("ui/#vadim_gagarinskiy_personal/dashboards/", Method.GET);
            var queryResultBefore = client.Execute<List<string>>(requestBefore).Data;

            var request = new RestRequest("ui/#vadim_gagarinskiy_personal/dashboards/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new Item { });
            client.Execute(request);

            var requestAfter = new RestRequest("ui/#vadim_gagarinskiy_personal/dashboards/", Method.GET);
            var queryResultAfter = client.Execute<List<string>>(requestAfter).Data;
            //Assert that dashboard list BEFORE POST method is not equal dashboard list AFTER POST method
            Assert.AreNotEqual(queryResultBefore.Count,queryResultAfter.Count);
        }

        [TestMethod]
        public void TestEditDashboard()
        {
            //5 edit Dashboard
            int idDashboard = 5;
            var client = new RestClient("https://rp.epam.com");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var requestBefore = new RestRequest("ui/#vadim_gagarinskiy_personal/dashboards/{id}", Method.GET);
            requestBefore.AddParameter("id", idDashboard);
            var queryResultBefore = client.Execute<List<Item>>(requestBefore).Data;

            var request = new RestRequest("ui/#vadim_gagarinskiy_personal/dashboards/", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new Item { });
            client.Execute(request);

            var requestAfter = new RestRequest("ui/#vadim_gagarinskiy_personal/dashboards/{id}", Method.GET);
            requestAfter.AddParameter("id", idDashboard);
            var queryResultAfter = client.Execute<List<Item>>(requestAfter).Data;

            //Assert that dashboard's description with idDashboard ID BEFORE PUT method
            //is not equal dashboard's description with idDashboard ID AFTER PUT method
            Assert.AreNotEqual(queryResultBefore.Description, queryResultAfter.Description );
        }

        [TestMethod]
        public void TestDeleteDashboard()
        {
            //6 delete Dashboard
            int idDashboard = 5;
            var client = new RestClient("https://rp.epam.com");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var requestBefore = new RestRequest("ui/#vadim_gagarinskiy_personal/dashboards/", Method.GET);
            var queryResultBefore = client.Execute<List<string>>(requestBefore).Data;

            var request = new RestRequest("ui/#vadim_gagarinskiy_personal/dashboards/{id}", Method.DELETE);
            request.AddParameter("id", idDashboard);
            client.Execute(request);

            var requestAfter = new RestRequest("ui/#vadim_gagarinskiy_personal/dashboards/", Method.GET);
            var queryResultAfter = client.Execute<List<string>>(requestAfter).Data;
            //Assert that dashboard list BEFORE DELETE method is not equal dashboard list AFTER DELETE method
            Assert.AreNotEqual(queryResultBefore.Count, queryResultAfter.Count);
        }
    }
}
