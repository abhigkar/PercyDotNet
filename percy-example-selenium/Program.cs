using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using percy_sdk_selenium;
using System.Threading;

namespace percy_example_selenium
{
    class Program
    {
  
        static void Main(string[] args)
        {
            Thread.Sleep(20000);


            IWebDriver driver = new ChromeDriver();
            try {
                var percy = new Percy(driver);

                driver.Navigate().GoToUrl("https://www.google.com");

                percy.SnapShot("google", null, null, false, null);
            }
            catch { }
            finally {
                driver.Close();
                driver.Dispose();
            }
            

           
        }
    }
}
