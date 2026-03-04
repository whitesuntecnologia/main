using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Test.Helpers.Regression
{
    public interface IUIActionHelper : IDisposable
    {
        IWebDriver Driver { get; }
        IWait<IWebDriver> Wait { get; }

        void Login();
        void Maximize();
    }
}