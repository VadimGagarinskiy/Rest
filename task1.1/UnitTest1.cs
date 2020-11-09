using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RA;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web.ModelBinding;

namespace task1._1
{
    [TestClass]
    public class UnitTest1
    {
        private const int DashboardID = 14529;
        private readonly string _baseUrl = "https://rp.epam.com";
        private readonly string _resource = "/api/v1/vadim_gagarinskiy_personal";
        private readonly string _total = "filter.gt.statistics$executions$total";
        private readonly string _tokenAuthenticator = "a0e00448-cf1d-44d0-b944-32b16933c275";
        private readonly string _contentTypeValue = "application/json; charset=utf-8";
        private readonly string _launch = "/launch";
        private readonly string _contentType = "Content-Type";
        private readonly string _dashboard = "/dashboard";
        private readonly string _dashboardWithID = $"/dashboard/{DashboardID}";

        [TestMethod]
        public void TestAllLaunches()
        {
            var expectedValueOfLaunches = 10;

            var client = new RestClient(_baseUrl);

            var request = new RestRequest(_resource+_launch, Method.GET);
            request.AddHeader(_contentType, _contentTypeValue);
            request.RequestFormat = DataFormat.Json;
            client.Authenticator = new JwtAuthenticator(_tokenAuthenticator);

            var timer = new Stopwatch();
            timer.Start();
            var response = client.Get(request);
            timer.Stop();

            Assert.IsTrue(timer.ElapsedMilliseconds < 1000);

            var launches = SimpleJson.DeserializeObject<Launches>(response.Content);
            var actualValueOfLaunches = launches.page.totalElements;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedValueOfLaunches, actualValueOfLaunches);
        }

        [TestMethod]
        public void TestAllLaunchesWhereTTLMoreThanNumber()
        {
            var totalExecutions = 20;
            
            var client = new RestClient(_baseUrl);

            var request = new RestRequest(_resource+_launch, Method.GET);
            request.AddHeader(_contentType, _contentTypeValue);
            request.AddParameter(_total, $"{totalExecutions}");
            request.RequestFormat = DataFormat.Json;
            client.Authenticator = new JwtAuthenticator(_tokenAuthenticator);

            var response = client.Get(request);
            var launches = SimpleJson.DeserializeObject<Launches>(response.Content);
            var amountFailedLaunches = launches.content.Count(x => x.status=="FAILED");
            Assert.IsTrue(launches.content.Count(x => x.statistics.executions.total>20)==launches.content.Length);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void TestGetAllDashboards()
        {
            var client = new RestClient(_baseUrl);

            var request = new RestRequest(_resource + _dashboard, Method.GET);
            request.AddHeader(_contentType, _contentTypeValue);
            request.RequestFormat = DataFormat.Json;
            client.Authenticator = new JwtAuthenticator(_tokenAuthenticator);

            var response = client.Execute(request);
            var dashboards = SimpleJson.DeserializeObject<Dashboards>(response.Content);
            var valueDashboardsWithDescription = dashboards.content.Count(x => !string.IsNullOrEmpty(x.description));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void TestCreateDashboard()
        {
            var dashboardDescription = "test";
            var dashboardName = "CreateTestDashboard2";

            var client = new RestClient(_baseUrl);

            var request = new RestRequest(_resource+_dashboard, Method.POST);
            request.AddHeader(_contentType, _contentTypeValue);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new
            {
                description = dashboardDescription,
                name = dashboardName,
                share = true
            });
            client.Authenticator = new JwtAuthenticator(_tokenAuthenticator);

            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public void TestUpdateDashboard()
        {
            var newDescription = "test2";
            var newDashboardName = "CreateTestDashboard2";

            var client = new RestClient(_baseUrl);

            var request = new RestRequest(_resource + _dashboardWithID, Method.PUT);
            request.AddHeader(_contentType, _contentTypeValue);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new
            {
                description = newDescription,
                name = newDashboardName
            });
            client.Authenticator = new JwtAuthenticator(_tokenAuthenticator);

            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void TestDeleteDashboard()
        {
            var client = new RestClient(_baseUrl);

            var request = new RestRequest(_resource + _dashboardWithID, Method.DELETE);
            request.AddHeader(_contentType, _contentTypeValue);
            request.RequestFormat = DataFormat.Json;
            client.Authenticator = new JwtAuthenticator(_tokenAuthenticator);

            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
