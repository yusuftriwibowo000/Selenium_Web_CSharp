using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using SeleniumNew;
using LibraryPDF;
using GlobalLibrary;
namespace SeleniumNew
{
    class Test_Login
    {
        public static IWebDriver driver = SingletonDriver.GetDriver();
        Actions action = new Actions(driver);
        public static string excelFilePath = LibPDF.projectDir + "/Excel/TC0001_LoginTravelio.xlsx";
        public static string excelSheetName = "TC0001";

        [OneTimeSetUp]
        public void SetUp()
        {
            LibPDF.InitializeDocument(excelFilePath, excelSheetName); // Initialize document before tests
            LibPDF.CreateCover();
        }

        [Test]
        public void RunningTestCase()
        {
            LibGlobal.openBrowser();
            LibGlobal.Login(excelFilePath,excelSheetName);
            LibGlobal.Logout();
        }

        [TearDown]
        public void Close()
        {
            LibPDF.GeneratePDF(excelFilePath, excelSheetName);
            driver.Quit();
        }            
    }
}