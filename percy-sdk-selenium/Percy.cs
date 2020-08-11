using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace percy_sdk_selenium
{
    public class Percy
    {
        private const string AGENTJS_FILE = "percy-agent.js";
        private IWebDriver driver;
        private String percyAgentJs;
        private bool percyIsRunning = true;

        public Percy(IWebDriver driver)
        {
            this.driver = driver;
            //this.env = new Environment(driver);
            this.percyAgentJs = loadPercyAgentJs();
        }

        //TODO handle async 
        private string loadPercyAgentJs()
        {
            string agentjs = String.Empty;

            using (var client = new HttpClient())
            {
                var response = client.GetAsync("http://localhost:5338/percy-agent.js").Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    // by calling .Result you are synchronously reading the result
                    agentjs = Task.Run(() => responseContent.ReadAsStringAsync()).Result;
                }
            }
            return agentjs;

        }

        public void SnapShot(string name, List<int> widths, int? minHeight, bool enableJavaScript, string percyCSS ) {
            string domSnapShot = String.Empty;
            if (String.IsNullOrEmpty(percyAgentJs))
            {
                // This would happen if we couldn't load percy-agent.js in the constructor.
                return;
            }

            try {
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript(percyAgentJs);
                domSnapShot = (string)jse.ExecuteScript(buildSnapshotJS());
            }
            catch (Exception ex) {
            }
            postSnapshot(domSnapShot, name, widths, minHeight, driver.Url, enableJavaScript, percyCSS);
        }

        private void postSnapshot(string domSnapShot, string name, List<int> widths, int? minHeight, string url, bool enableJavaScript, string percyCSS)
        {
            if (percyIsRunning == false)
            {
                return;
            }
            JObject json = new JObject();
            json.Add("url", url);
            json.Add("name", name);
            json.Add("percyCSS", percyCSS);
            json.Add("minHeight", minHeight);
            json.Add("domSnapshot", domSnapShot);
            //json.Add("clientInfo", env.getClientInfo());
            json.Add("enableJavaScript", enableJavaScript);
            //json.Add("environmentInfo", env.getEnvironmentInfo());
            if (widths != null && widths.Count() != 0)
            {
                json["widths"] = JToken.FromObject(widths);
            }
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
           

            using (var client = new HttpClient())
            {
                var result = client.PostAsync("http://localhost:5338/percy/snapshot", content).Result;
            }
        }

        private String getAgentOptions()
        {
            StringBuilder info = new StringBuilder();
            info.Append("{ ");
            info.Append(String.Format("handleAgentCommunication: false"));
            info.Append(" }");
            return info.ToString();
        }

        private string buildSnapshotJS()
        {
            StringBuilder jsBuilder = new StringBuilder();
            jsBuilder.Append(String.Format("var percyAgentClient = new PercyAgent({0})\n", getAgentOptions()));
            jsBuilder.Append(String.Format("return percyAgentClient.snapshot('not used')"));

            return jsBuilder.ToString();
        }
    }
}
