using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RA;
using System.Threading.Tasks;

namespace task1._1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestLaunch()
        {
            new RestAssured()
                .Given()
                .Param("page.page","1")
                .Param("page.size", "1")
                .Param("page.sort", "start_time,ASC")
                .When()
                .Get("https://rp.epam.com/ui/#vadim_gagarinskiy_personal/launches")
                .Then()
                .Debug()
                .TestStatus("codeResponse", x => x == 200)
                .AssertAll();
        }

        [TestMethod]
        public void TestDashboardCreate()
        {
            new RestAssured()
                .Given()
                .Param("page.sort", "DASHBOARD NAME,ASC")
                .When()
                .Post("https://reportportal.epam.com/ui/#vadim_gagarinskiy_personal/dashboard")
                .Then()
                .Debug()
                .TestStatus("codeResponse", x => x == 200)
                .AssertAll();
        }

        [TestMethod]
        public void TestDashboardEdit()
        {
            new RestAssured()
                .Given()
                .When()
                .Put("https://reportportal.epam.com/ui/#vadim_gagarinskiy_personal/dashboard/14072")
                .Then()
                .Debug()
                .TestStatus("codeResponse", x => x == 200)
                .AssertAll();
        }

        [TestMethod]
        public void TestDashboardDelete()
        {
            new RestAssured()
                .Given()
                .When()
                .Delete("https://reportportal.epam.com/ui/#vadim_gagarinskiy_personal/dashboard/14072")
                .Then()
                .Debug()
                .TestStatus("codeResponse", x => x == 200)
                .AssertAll();
        }
    }
}
