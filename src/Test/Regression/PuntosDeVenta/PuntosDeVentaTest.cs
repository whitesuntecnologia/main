using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test.Helpers.Regression;
using Xunit;

namespace Test.Regression.PuntosDeVenta
{
    public class PuntosDeVentaTest : IDisposable
    {
        public IWebDriver Driver { get; private set; }
        public IDictionary<string, object> Vars { get; private set; }
        public IJavaScriptExecutor JS { get; private set; }
        private readonly IUIActionHelper _uiActionHelper;

        public PuntosDeVentaTest(IUIActionHelper uiActionHelper)
        {
            //Driver = new ChromeDriver();
            //JS = (IJavaScriptExecutor)Driver;
            //Vars = new Dictionary<string, object>();
            _uiActionHelper = uiActionHelper;
        }

        public void Dispose()
        {
            Driver.Quit();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Deberia_Crear_Un_PuntoDeVenta()
        {
            _uiActionHelper.Maximize();
            _uiActionHelper.Login();
        }
    }
}
