using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Helpers.Regression
{
    public class UIActionHelper : IUIActionHelper, IDisposable
    {
        public IWebDriver Driver { get; }
        public IWait<IWebDriver> Wait { get; }

        public UIActionHelper(IWebDriver driver, IWait<IWebDriver> wait)
        {
            Driver = driver;
            Wait = wait;
        }

        public void Login()
        {
            Driver.Navigate().GoToUrl("http://localhost:5000");

            Wait.Until(x => x.FindElement(By.CssSelector("#Input_UserName"))).SendKeys("blazor");
            Wait.Until(x => x.FindElement(By.CssSelector("#Input_Password"))).SendKeys("blazor");

            Wait.Until(x => x.FindElement(By.CssSelector("#account > div:nth-child(7) > button"))).Click();
        }

        public void Maximize()
        {
            Wait.Until(x => x.Manage()).Window.Maximize();
        }

        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
