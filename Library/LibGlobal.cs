using OpenQA.Selenium;
using iText.Kernel.Pdf;
using iText.IO.Image;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using SeleniumNew;
using LibraryPDF;
using LibraryExcel;
using DocumentFormat.OpenXml.Bibliography;
using iText.StyledXmlParser.Jsoup.Nodes;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Interactions;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;

namespace GlobalLibrary
{
    public class LibGlobal
    {
        public static void openBrowser()
        {
            IWebDriver driver = SingletonDriver.GetDriver();
            driver.Navigate().GoToUrl(LibExcel.GetDataExcel(LibPDF.globalExcelFilePath, "URL", "Global"));
            Thread.Sleep(1000);
            driver.Manage().Window.Maximize();
            Thread.Sleep(2000);
        }

        public static void Login(string excelPath, string excelSheet)
        {
            IWebDriver driver = SingletonDriver.GetDriver();
            // Objek yang ada di Home Page Travelio
            //IWebElement modalIklan = driver.FindElement(By.XPath("//div[@id='tpmModal']"));
            IWebElement modalClose = driver.FindElement(By.XPath("//i[@class='fa fa-close fa-lg close padding15']"));
            IWebElement logoTravelio = driver.FindElement(By.XPath("//div[@id='menu-wrapper']/div/a[@class='navbar-brand']"));
            IWebElement menuTravelio = driver.FindElement(By.XPath("//*[@id='menu-wrapper']"));
            IWebElement btnMasuk = driver.FindElement(By.XPath("//*[@id='loginBtn']"));

            string username = LibExcel.GetDataExcel(excelPath, "USERNAME", excelSheet);
            string password = LibExcel.GetDataExcel(excelPath, "PASSWORD", excelSheet);
            string greet = LibExcel.GetDataExcel(excelPath, "NAME", excelSheet);

            // Jika muncul popup iklan
            if (modalClose.Displayed)
            {
                LibPDF.CaptureScreen("Tutup Popup Iklan Travelio", "Done");
                modalClose.Click();
                Thread.Sleep(2000);
            }
            else
            {
                Console.WriteLine("Popup Iklan tidak ada di page");
            }

            if (logoTravelio.Displayed && menuTravelio.Displayed && btnMasuk.Displayed)
            {
                LibPDF.CaptureScreen("Berhasil Masuk Beranda Travelio", "Passed");

                // 'Btn Login' di home page
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btnMasuk);
                Thread.Sleep(5000);
                // Objek yang ada di Modal Login Page
                IWebElement tabLoginEmail = driver.FindElement(By.XPath("//*[@id='auth-modal-sign-in-with-email-icon']"));
                IWebElement inputUsername = driver.FindElement(By.XPath("//input[@id='login-email']"));
                IWebElement inputPassword = driver.FindElement(By.XPath("//input[@id='login-password']"));
                IWebElement btnLogin = driver.FindElement(By.XPath("//button[@id='auth-modal-btn']"));

                if (tabLoginEmail.Displayed)
                {
                    tabLoginEmail.Click();
                    if (inputUsername.Displayed && inputPassword.Displayed)
                    {
                        LibPDF.CaptureScreen("Masuk Halaman Login", "Passed");
                        inputUsername.SendKeys(username);
                        Thread.Sleep(1000);
                        inputPassword.SendKeys(password);
                        Thread.Sleep(1000);
                        LibPDF.CaptureScreen("Isi Field Username dan Password", "Done");
                        if (btnLogin.Displayed)
                        {
                            btnLogin.Click();
                            Thread.Sleep(5000);

                            // Jika Berhasil Login
                            IWebElement dropdownUser = null;
                            try
                            {
                                dropdownUser = driver.FindElement(By.XPath($"//div[@id='user-dropdown']/div[@id='user-option']/span[text()='{greet}']"));
                            }
                            catch (Exception) { }

                            try
                            {
                                IWebElement usernamePasswordSalah = driver.FindElement(By.XPath("//div[@id='modal-error-message' and text()='Email atau password salah']"));
                                IWebElement btnModalOK = driver.FindElement(By.XPath("//button[@class='col-xs-12 btn btn-tosca']"));
                                if (usernamePasswordSalah.Displayed)
                                {
                                    LibPDF.CaptureScreen("Username atau Password Salah", "Failed");
                                    btnModalOK.Click();
                                    Environment.Exit(1);
                                    driver.Quit();
                                }
                            }
                            catch (Exception) { }

                            if (dropdownUser != null)
                            {
                                LibPDF.CaptureScreen("Berhasil Login sebagai : " + greet, "Passed");
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                LibPDF.CaptureScreen("Gagal Login", "Failed");
                                Environment.Exit(1);
                                driver.Quit();
                            }
                        }
                        else
                        {
                            LibPDF.CaptureScreen("Button Submit Login tidak ada di page", "Failed");
                            Environment.Exit(1);
                            driver.Quit();
                        }
                    }
                    else
                    {
                        LibPDF.CaptureScreen("Field Username dan Password tidak ada di page", "Failed");
                        Environment.Exit(1);
                        driver.Quit();
                    }
                }
                else
                {
                    LibPDF.CaptureScreen("Tab Masuk tidak tampil di page", "Failed");
                    Environment.Exit(1);
                    driver.Quit();
                }
            }
            else
            {
                LibPDF.CaptureScreen("Gagal Masuk Travelio Page", "Failed");
                Environment.Exit(1);
                driver.Quit();
            }
        }

        public static void Logout()
        {
            IWebDriver driver = SingletonDriver.GetDriver();
            try
            {
                IWebElement dropdown = driver.FindElement(By.Id("user-dropdown"));
                dropdown.Click();
                Thread.Sleep(2000);
                IWebElement btnLogout = driver.FindElement(By.XPath("//a[@onclick='userLogout()']"));
                if (btnLogout.Displayed)
                {
                    LibPDF.CaptureScreen("Klik Button Keluar Akun", "Done");
                    btnLogout.Click();
                    Thread.Sleep(5000);
                    IWebElement btnMasuk = driver.FindElement(By.XPath("//*[@id='loginBtn']"));
                    if (btnMasuk.Displayed)
                    {
                        LibPDF.CaptureScreen("Berhasil Logout", "Passed");
                        driver.Quit();
                    }
                    else
                    {
                        LibPDF.CaptureScreen("Gagal Logout", "Failed");
                        Environment.Exit(1);
                        driver.Quit();
                    }
                }
                else
                {
                    LibPDF.CaptureScreen("Button Logout Tidak Muncul", "Failed");
                    Environment.Exit(1);
                    driver.Quit();
                }
            }
            catch (Exception)
            {
                LibPDF.CaptureScreen("Dropdown User Tidak Muncul", "Failed");
                Environment.Exit(1);
                driver.Quit();
            }
        }
    }
}