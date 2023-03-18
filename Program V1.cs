using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Interactions;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Collections;
using OpenQA.Selenium.DevTools;
using System.Linq.Expressions;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace ConsoleApp1
{
    internal class Program
    {
        string connectionString;
        SqlConnection cn;
        StreamWriter writetext;
        IWebDriver webDriver;
        WebDriverWait wait;
        int minMaxListCounter = 48;

        public Program()
        {
            try {
                connectionString = new string("Data Source=Dell;Initial Catalog = Innoscripta_Sweden_Data;Integrated Security = true");
                writetext = new StreamWriter("log.txt", true);
                webDriver = new ChromeDriver();
                cn = new SqlConnection(connectionString);
                wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
            }
            catch(Exception ex) { log(ex, " constructor"); }
            }

        static void Main(string[] args )
        {
            if (args[0] == "-1")
            {
                Environment.Exit(0);
            }

            /*800 000 data should be divided into 80 to make it 10 000 by 10 000. 
             So list is not very necessary, filtering only by employee number may not be sufficent
            List<Tuple<int,int>> minMaxList = new List<Tuple<int, int>>(); 
            // This must be filled spesifically for the country
            // From high to lower should be better
            minMaxList.Add(new Tuple<int, int>(50, 50000));
            minMaxList.Add(new Tuple<int, int>(45, 50));
            minMaxList.Add(new Tuple<int, int>(40, 45));
            minMaxList.Add(new Tuple<int, int>(35, 40));
            minMaxList.Add(new Tuple<int, int>(30, 35));
            minMaxList.Add(new Tuple<int, int>(25, 30));
            minMaxList.Add(new Tuple<int, int>(21, 25));
            minMaxList.Add(new Tuple<int, int>(19, 23));
            minMaxList.Add(new Tuple<int, int>(16, 19));
            minMaxList.Add(new Tuple<int, int>(14, 16));
            minMaxList.Add(new Tuple<int, int>(12, 14));
            minMaxList.Add(new Tuple<int, int>(11, 12));
            minMaxList.Add(new Tuple<int, int>(10, 11));
            minMaxList.Add(new Tuple<int, int>(9, 10));
            minMaxList.Add(new Tuple<int, int>(8, 8));
            minMaxList.Add(new Tuple<int, int>(7, 7));
            minMaxList.Add(new Tuple<int, int>(6, 6));
            minMaxList.Add(new Tuple<int, int>(5, 5));
            minMaxList.Add(new Tuple<int, int>(4, 4));
            minMaxList.Add(new Tuple<int, int>(3, 3));
            minMaxList.Add(new Tuple<int, int>(2, 2));
            minMaxList.Add(new Tuple<int, int>(1, 1));
            minMaxList.Add(new Tuple<int, int>(0, 0));
            */
            Program app = new Program();
            //string[] argsComing = Environment.GetCommandLineArgs();

            app.webDriver.Navigate().GoToUrl("https://platform.globaldatabase.com/");
            app.webDriver.Manage().Window.Size = new System.Drawing.Size(1950, 1050);
            app.login();
       
            app.findCountry(app.webDriver, "Sweden");
        
            int numberOfRegions = app.selectRegion(args[2]);
  
            app.setNumberOfEmployeeInterval(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
           
            app.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("tbody[class='rc-table-tbody']")));
            IWebElement DataTable = app.webDriver.FindElement(By.CssSelector("tbody[class='rc-table-tbody']"));

            app.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("tr[class='rc-table-row rc-table-row-level-0']")));

            Thread.Sleep(4000);

            int i = 1; 
            IWebElement row = app.webDriver.FindElement(By.XPath("//*[@id=\"scrollable-table\"]/div[2]/div[2]/table/tbody/tr[" + i + "]"));

            while (true)
            {
                List<string> companyData = new();
                try
                {   
                    app.wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[1]/div/div[4]/div/div[2]/div/div[2]/div[2]/table/tbody/tr[" + i + "]")));
                    row = app.webDriver.FindElement(By.XPath("/html/body/div[1]/div/div[4]/div/div[2]/div/div[2]/div[2]/table/tbody/tr[" + i + "]"));
                    i++;
                }
                catch (Exception ex)
                {
                    app.log(ex, " After find next row");
                    //When code comes here, it means there is no more rows loaded. According to my personal observations, default limit is around 10 000 rows.
                    //So now we should change filter to load new data.
                    //For the filter, my first idea was to go region by region 
                    //Apperently better idea is to used 'Corporation Data interval'
                    //However when filter needs to be changed, restarting the process WITH NEW FILTERS would be wise idea for releasing unnecessary ram usage.
                    //Or we need to find another way to free the ram
                    string arguments = "";
                    if (args[2] == numberOfRegions.ToString()) // if all regions are iterated, reduce empoloye number by one
                    {
                        arguments = (Convert.ToInt32(args[0]) - 1).ToString();
                        arguments = arguments + " " + arguments + " " + "1" ;
                    }
                    else // iterated all regions for same employee number
                    {
                        arguments = args[0] + " " + args[0] + " " + (Convert.ToInt32(args[2]) + 1).ToString(); ;
                    }
                    app.writetext.Close();
                    // Restart current application, with different filters
                    System.Diagnostics.Process.Start("InnoscriptaDataScraping.exe", arguments);
                    app.webDriver.Quit();
                    Environment.Exit(0);
                    /*if (app.minMaxListCounter < 0)
                    {
                        app.setNumberOfEmployeeInterval(app.minMaxListCounter, app.minMaxListCounter);
                        app.minMaxListCounter--;
                    }
                    Thread.Sleep(5000);
                    i = 2;
                    app.wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[1]/div/div[4]/div/div[2]/div/div[2]/div[2]/table/tbody/tr[" + i + "]")));
                    row = app.webDriver.FindElement(By.XPath("/html/body/div[1]/div/div[4]/div/div[2]/div/div[2]/div[2]/table/tbody/tr[" + i + "]"));
                    i++;
                    app.wait.Until(ExpectedConditions.ElementToBeClickable(row));
                    app.log(ex, " Rows with minMax condition are started to be called");*/
                }

                Thread.Sleep(5);
                try
                {
                    app.wait.Until(ExpectedConditions.ElementToBeClickable(row.FindElement(By.XPath("/html/body/div[1]/div/div[4]/div/div[2]/div/div[2]/div[2]/table/tbody/tr[" + i + "]/td[3]/div"))));
                    //IWebElement email = row.FindElement(By.XPath("//div[text()='Show email']")); // thats probably slower
                    IWebElement email = row.FindElement(By.XPath("/ html / body / div[1] / div / div[4] / div / div[2] / div / div[2] / div[2]/table/tbody/tr["+i+"]/td[3]/div"));
                    app.wait.Until(ExpectedConditions.ElementToBeClickable(email));
                    email.Click();

                    app.wait.Until(ExpectedConditions.ElementToBeClickable(row.FindElement(By.XPath("/ html / body / div[1] / div / div[4] / div / div[2] / div / div[2] / div[2] / table / tbody / tr[" + i + "] / td[4] / div"))));
                    //IWebElement phone = row.FindElement(By.XPath("//div[text()='Show phone']")); // thats probably slower
                    IWebElement phone = row.FindElement(By.XPath("/ html / body / div[1] / div / div[4] / div / div[2] / div / div[2] / div[2] / table / tbody / tr[" + i + "] / td[4] / div"));
                    app.wait.Until(ExpectedConditions.ElementToBeClickable(phone));
                    phone.Click();

                    app.waitForPhoneAndMailToBeLoaded(email, phone);
                }
                catch (Exception ex) { app.log(ex , " After show email phone click"); }

                Thread.Sleep(5);
                try 
                {
                    app.wait.Until(ExpectedConditions.ElementExists(By.TagName("td")));
                    // maybe some condition to ensure that mail and phone appears
                    foreach (var cell in row.FindElements(By.TagName("td")))
                    {
                        try
                        {
                            Thread.Sleep(2);
                            companyData.Add(cell.Text);
                        }
                        catch (Exception ex){ app.log(ex, " add cellText to list"); }
                    }
                }
                catch (Exception ex){ app.log(ex, " row.FindElements(By.TagName('td')"); }
                try
                {
                    //Once row is processed, scroll to next row, that is a must to avoid delays and consequent interruptions when loading next data 
                    app.webDriver.ExecuteJavaScript("arguments[0].scrollIntoView();", row);
                }
                catch(Exception ex) { app.log(ex , " scroll down");}
    
                string query = "INSERT INTO CompanyInfoAll VALUES (@CompanyName, @CompanyEmail, @Phone, @Website, @SICCode, @VatNumber, @SICDescription, @Industry, @LegalForm, @RegistrationNo, @Address, @AlexaRank, @MonthlyVisits, @EmployeeNumber, @TurnOver, @Age)";
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, app.cn))
                    {
                        cmd.Parameters.Add("@CompanyName", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(companyData[1]) ? "0" : companyData[1];
                        cmd.Parameters.Add("@CompanyEmail", SqlDbType.NVarChar, 100).Value = companyData[2];
                        cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 30).Value = companyData[3];
                        cmd.Parameters.Add("@Website", SqlDbType.NVarChar, 200).Value = companyData[4];
                        cmd.Parameters.Add("@SICCode", SqlDbType.NVarChar, 50).Value = companyData[5];
                        cmd.Parameters.Add("@VatNumber", SqlDbType.NVarChar, 20).Value = string.IsNullOrEmpty(companyData[6]) ? "0" : companyData[6];
                        cmd.Parameters.Add("@SICDescription", SqlDbType.NVarChar, 500).Value = companyData[7];
                        cmd.Parameters.Add("@Industry", SqlDbType.NVarChar, 300).Value = companyData[8];
                        cmd.Parameters.Add("@LegalForm", SqlDbType.NVarChar, 100).Value = companyData[9];
                        cmd.Parameters.Add("@RegistrationNo", SqlDbType.NVarChar, 15).Value = string.IsNullOrEmpty(companyData[10]) ? "0" : companyData[10];
                        cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 100).Value = companyData[11];
                        cmd.Parameters.Add("@AlexaRank", SqlDbType.NVarChar, 50).Value = companyData[12];
                        cmd.Parameters.Add("@MonthlyVisits", SqlDbType.NVarChar, 12).Value = companyData[13];
                        cmd.Parameters.Add("@EmployeeNumber", SqlDbType.NVarChar, 10).Value = companyData[14];
                        cmd.Parameters.Add("@TurnOver", SqlDbType.NVarChar, 20).Value = companyData[15];
                        cmd.Parameters.Add("@Age", SqlDbType.NVarChar, 10).Value = companyData[16];

                        app.cn.Open();
                        if (!string.IsNullOrEmpty(companyData[2]) || !string.IsNullOrEmpty(companyData[3]) || !string.IsNullOrEmpty(companyData[4]) ||
                            !string.IsNullOrEmpty(companyData[5]) || !string.IsNullOrEmpty(companyData[7]) || !string.IsNullOrEmpty(companyData[8]) ||
                            !string.IsNullOrEmpty(companyData[9]) || !string.IsNullOrEmpty(companyData[10]) || !string.IsNullOrEmpty(companyData[11]) )
                        {
                            cmd.ExecuteNonQuery();
                        }
                        app.cn.Close();
                    }

                }
                catch (Exception ex)
                {
                    app.cn.Close();
                    app.log(ex, " sql db insert");
                }

                try
                {
                    //in UI we always need some data to be able to load next data
                    if(i<100)
                    {
                        continue;
                    }
                    //Avoids memory-use to become too high too soon.
                    //It reduces ram usage drastically, But still not definite solution
                    app.webDriver.ExecuteJavaScript("var ele=arguments[0]; ele.innerHTML = '';", row);
                    //webDriver.ExecuteJavaScript("var ele=arguments[0]; ele.remove();", row);
                    //row.Clear();  
                }
                catch (Exception ex) { app.log(ex , " remove processed row"); }
            }

            Console.WriteLine("End");
        }

        void login()
        {
            IWebElement userNameInput = webDriver.FindElement(By.XPath("//input[@placeholder='Email address']"));
            userNameInput.Clear();
            userNameInput.SendKeys("hohenester@innoscripta.com");
            IWebElement passwordInput = webDriver.FindElement(By.XPath("//input[@placeholder='Password']"));
            passwordInput.Clear();
            passwordInput.SendKeys("wahyuthebest");
            passwordInput.Submit();
        }

        void findCountry(IWebDriver webDriver, string countryFullName)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(@class, 'item-title') and text()='Location']")));

            Thread.Sleep(3000);
            IWebElement Location = webDriver.FindElement(By.XPath("//span[contains(@class, 'item-title') and text()='Location']"));
            Location.Click();

            wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[text()='Countries']")));
            IWebElement Countries = webDriver.FindElement(By.XPath("//div[text()='Countries']"));
            Countries.Click();

            wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@placeholder='Search country']")));
            IWebElement CountrySearchForm = webDriver.FindElement(By.XPath("//input[@placeholder='Search country']"));
            CountrySearchForm.Clear();
            CountrySearchForm.SendKeys(countryFullName);

            Thread.Sleep(1000);
            /*wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[contains(@class, 'ant-tree-checkbox-inner')]")));
            Thread.Sleep(1000);
            IWebElement SelectCountry = webDriver.FindElement(By.XPath("//span[contains(@class, 'ant-tree-checkbox-inner')]"));
            SelectCountry.Click();*/
        }

        int selectRegion(string regionIndex)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/div[3]/div[7]/div[2]/div/div[1]/div[2]/div/ul/li/span[1]")));

            Thread.Sleep(1000);
            IWebElement DropDownRegions = webDriver.FindElement(By.XPath("/html/body/div[1]/div/div[3]/div[7]/div[2]/div/div[1]/div[2]/div/ul/li/span[1]"));
            DropDownRegions.Click();
            Thread.Sleep(1000);

            IWebElement regionTable = webDriver.FindElement(By.XPath("/html/body/div[1]/div/div[3]/div[7]/div[2]/div/div[1]/div[2]/div/ul/li/ul"));
            int count = regionTable.FindElements(By.TagName("li")).Count;

            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[1]/div/div[3]/div[7]/div[2]/div/div[1]/div[2]/div/ul/li/ul/li[" + regionIndex + "]/span[2]/span")));
            IWebElement region = webDriver.FindElement(By.XPath(        "/html/body/div[1]/div/div[3]/div[7]/div[2]/div/div[1]/div[2]/div/ul/li/ul/li[" + regionIndex + "]/span[2]/span"));
            region.Click();

            Thread.Sleep(2000);
            return count;
        }

        void waitForPhoneAndMailToBeLoaded(IWebElement email, IWebElement phone)
        {
            while (true)
            {
                if (email.Text.Contains("@") || email.Text.Contains("Not"))
                {
                    Thread.Sleep(80);
                    break;
                }
                else if (email.Text == "Show email")
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(email));
                    email.Click();
                }
                Thread.Sleep(100);
            }
            while (true)
            {
                if(phone.Text.Contains("+46") || phone.Text.Contains("Not"))
                {
                    Thread.Sleep(80);
                    break;
                }
                else if(phone.Text == "Show phone")
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(phone));
                    phone.Click();
                }
                Thread.Sleep(100);
            }
            /* couldnt make this work, so i wrote my own waiter above
            WebDriverWait waiter = new WebDriverWait(webDriver, TimeSpan.FromSeconds(1));
            bool a = waiter.Until(ExpectedConditions.TextToBePresentInElement(phone, "+46"));
            bool b = waiter.Until(ExpectedConditions.TextToBePresentInElement(email, "@"));*/
        }

        void setNumberOfEmployeeInterval(int min, int max)
        {
            Thread.Sleep(1000);
            IWebElement Companies = webDriver.FindElement(By.XPath("/html/body/div[1]/div/div[3]/div[4]/div[1]/div"));
            wait.Until(ExpectedConditions.ElementToBeClickable(Companies));
            Companies.Click();

            IWebElement NoOfCompanies = webDriver.FindElement(By.XPath("/html/body/div[1]/div/div[3]/div[4]/div[2]/div/div[2]"));
            wait.Until(ExpectedConditions.ElementToBeClickable(NoOfCompanies));
            NoOfCompanies.Click();
            Thread.Sleep(100);
    
            IWebElement minNoForm = webDriver.FindElement(By.XPath("/html/body/div[1]/div/div[3]/div[4]/div[2]/div/div[2]/div[2]/div/div/div[1]/div[2]/input"));
            wait.Until(ExpectedConditions.ElementToBeClickable(minNoForm));
            minNoForm.Clear();
            minNoForm.SendKeys(min.ToString());
            Thread.Sleep(100);
    
            IWebElement maxNoForm = webDriver.FindElement(By.XPath("/html/body/div[1]/div/div[3]/div[4]/div[2]/div/div[2]/div[2]/div/div/div[2]/div[2]/input"));
            wait.Until(ExpectedConditions.ElementToBeClickable(maxNoForm));
            maxNoForm.Clear();
            maxNoForm.SendKeys(max.ToString());
        }
        
        void log(Exception ex,  string explicitLog)
        {
            writetext.WriteLine(DateTime.Now.ToString() + explicitLog);
            writetext.WriteLine(ex.TargetSite);
            writetext.WriteLine(ex.Message);
            writetext.Flush();
        }
    }
}






