using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace kf2server_telegrambot {
    class TestSelentium {

        public TestSelentium() {

            FirefoxProfile profile = new FirefoxProfile();
            profile.SetPreference("permissions.default.stylesheet", 2);
            profile.SetPreference("permissions.default.image", 2);
            

            IWebDriver driver;
            FirefoxOptions options = new FirefoxOptions();
            //options.AddArguments("--headless");
            options.Profile = profile;

            driver = new FirefoxDriver(options);
            driver.Url = "http://kf2server.rhome.net:8080/ServerAdmin/";

            driver.FindElement(By.Id("username")).SendKeys("admin");
            driver.FindElement(By.Id("password")).SendKeys("WelcomeToBrampton69");
            SelectElement rememberDOM = new SelectElement(driver.FindElement(By.Name("remember")));
            rememberDOM.SelectByValue("2678400");

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            
            List<string> tabs = new List<string>(driver.WindowHandles);

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.open('http://kf2server.rhome.net:8080/ServerAdmin/current/change', '_blank');");

            tabs.Add(driver.CurrentWindowHandle);
            

            // Launch a window for necessary actions at init
            // all commands from all users that require that action, port through that window, one at a time.

            //driver.SwitchTo().Window()
            //driver.Quit();


        }

    }
}
