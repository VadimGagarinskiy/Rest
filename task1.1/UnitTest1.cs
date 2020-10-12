﻿using System;
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
        private readonly string baseUrl = "https://rp.epam.com";
        private readonly string resource = "/api/v1/vadim_gagarinskiy_personal";
        private readonly string tokenAuthenticator = "a0e00448-cf1d-44d0-b944-32b16933c275";
        private readonly string contentType = "application/json; charset=utf-8";

        [TestMethod]
        public void TestAllLaunches()
        {
            var expectedValueOfLaunches = 10;
            var totalElements = "totalElements";

            var client = new RestClient(baseUrl);

            var request = new RestRequest(resource+"/launch", Method.GET);
            request.AddHeader("Content-Type", contentType);
            request.RequestFormat = DataFormat.Json;
            client.Authenticator = new JwtAuthenticator(tokenAuthenticator);

            var timer = new Stopwatch();
            timer.Start();
            client.Get(request);
            client.Get(request);
            timer.Stop();

            Assert.IsTrue(timer.ElapsedMilliseconds < 2000);

            var response = client.Get(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedValueOfLaunches, Int32.Parse(GetValueAttribute(response.Content, totalElements)));
        }

        [TestMethod]
        public void TestAllLaunchesWhereTTLMoreThanNumber()
        {
            var totalExecutions = 20;
            var expectedValueOfLaunches = 8;
            var TTL = "filter.gt.statistics$executions$total";
            var totalElements = "totalElements";
            var attributeTotal = "executions\":{\"total";

            var client = new RestClient(baseUrl);

            var request = new RestRequest(resource+"/launch", Method.GET);
            request.AddHeader("Content-Type", contentType);
            request.AddParameter(TTL, $"{totalExecutions}");
            request.RequestFormat = DataFormat.Json;
            client.Authenticator = new JwtAuthenticator(tokenAuthenticator);

            var response = client.Get(request);
            var amountFailedLaunches = new Regex("FAILED").Matches(response.Content).Count;

            foreach (var value in GetValuesOfAttributes(response.Content, attributeTotal))
            {
                Assert.IsTrue(Int32.Parse(value)>20);
            }
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedValueOfLaunches, Int32.Parse(GetValueAttribute(response.Content, totalElements)));
        }

        public string GetValueAttribute(string context, string attribute)
        {
            context = context.Substring(context.IndexOf(attribute));
            var endAtr = context.IndexOf(',');
            var endObj = context.IndexOf('}');
            if (endAtr > endObj||endAtr==null)
            {
                return context.Substring(attribute.Length + 2, endObj - attribute.Length - 2);
            }
            else
            {
                return context.Substring(attribute.Length + 2, endAtr - attribute.Length - 2);
            }
        }

        public List<string> GetValuesOfAttributes(string context, string attribute)
        {
            List<string> values = new List<string>();
            while (new Regex(attribute).Matches(context).Count>0)
            {
                context = context.Remove(0,context.IndexOf(attribute));
                var endAtr = context.IndexOf(',');
                var endObj = context.IndexOf('}');
                if (endAtr > endObj || endAtr == null)
                {
                    values.Add(context.Substring(attribute.Length + 2, endObj - attribute.Length - 2));
                }
                else
                {
                    values.Add(context.Substring(attribute.Length + 2, endAtr - attribute.Length - 2));
                }
                context = context.Remove(0, attribute.Length);
            }
            return values;
        }
    }
}
