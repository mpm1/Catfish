﻿using Catfish.Core.Models;
using Catfish.Core.Models.Forms;
using Catfish.Tests.IntegrationTests.Helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Catfish.Tests.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;

namespace Catfish.Tests.IntegrationTests.Regions
{
    [TestFixture(typeof(ChromeDriver))]
    class AdvancedSearchPanelTest<TWebDriver> : BaseIntegrationTest<TWebDriver> where TWebDriver : IWebDriver, new()
    {
        public static string[] MetadataSetNames = 
        {
            "BasicMetadata"
        };

        public static FormField[] MetadataFields = {
            new TextField() { Name = "Name" },
            new NumberField() { Name = "Amount" },
            new NumberField() { Name = "Year" },
            new DropDownMenu() { Name = "Category", Options = new List<Option>(){
                new Option() { Value = { new TextValue("en", "English", "One") } },
                new Option() { Value = { new TextValue("en", "English", "Two") } },
                new Option() { Value = { new TextValue("en", "English", "Three") } },
            }.AsReadOnly() }
        };

        public static CFEntityType.eTarget[] TargetTypes =
        {
            CFEntityType.eTarget.Items
        };

        public void CreateSearchEntityType()
        {
            MetadataFields[3].Serialize();
            CreateMetadataSet(MetadataSetNames[0], "The base metadata set for advanced search", MetadataFields);

            CreateEntityType(EntityTypeName, "Entity type used for testing advanced search", MetadataSetNames, TargetTypes,
                new Tuple<string, FormField>[] {
                    new Tuple<string, FormField>(MetadataSetNames[0], MetadataFields[1]),
                    new Tuple<string, FormField>(MetadataSetNames[0], MetadataFields[2]),
                    new Tuple<string, FormField>(MetadataSetNames[0], MetadataFields[3])
                });
        }

        private void CreateItems(int count, Func<int, int> yearFunction, Func<int, float> amountFunction, Func<int, string> optionFunc, Func<int, string> nameFunction)
        {
            MetadataFields[0].Serialize();
            MetadataFields[1].Serialize();
            MetadataFields[2].Serialize();
            MetadataFields[3].Serialize();

            FormField[] fields = new FormField[4];

            for (int i = 0; i < count; ++i)
            {
                
                fields[0] = new TextField() { Content = MetadataFields[0].Content };
                fields[0].SetValues(new string[]{ nameFunction(i) });

                fields[1] = new NumberField() { Content = MetadataFields[1].Content };
                fields[1].SetValues(new string[] { amountFunction(i).ToString() });

                fields[2] = new NumberField() { Content = MetadataFields[2].Content };
                fields[2].SetValues(new string[] { yearFunction(i).ToString() });

                fields[3] = new DropDownMenu() { Content = MetadataFields[3].Content };
                fields[3].SetValues(new string[] { optionFunc(i) });
                ((DropDownMenu)fields[3]).UpdateValues(fields[3]);

                CreateItem(EntityTypeName, fields, false);
            }
        }

        [Test]
        public void CanSearch()
        {
            CreateSearchEntityType();

            CreateAndAddAddvancedSearchToMain(true, MetadataFields, 1);
            CreateAndAddEntityListToMain(2);

            Func<int, int> yearFunc = i => i + 2000;
            Func<int, float> amountFunc = i => (float)(Math.Sin((double)i) + 2.0);
            Func<int, string> nameFunc = i => "Item Entry " + i;
            Func<int, string> optionFunc = i => null;

            CreateItems(10, yearFunc, amountFunc, optionFunc, nameFunc);

            List<string> nameList = new List<string>();
            for(int i = 0; i < 10; ++i)
            {
                nameList.Add(nameFunc(i));
            }

            Driver.Navigate().GoToUrl(FrontEndUrl);
            AssertItemsNameShows(nameList);

            // Search on the year
            int minYear = 2001;
            int maxYear = 2005;
            for(int i = 9; i >= 0; --i)
            {
                int year = yearFunc(i);
                if (year < minYear || year > maxYear)
                {
                    nameList.RemoveAt(i);
                }
            }

            IWebElement field = Driver.FindElements(By.ClassName("search-entry"))[3];
            field.FindElement(By.ClassName("search-from")).SendKeys(minYear.ToString());
            field.FindElement(By.ClassName("search-to")).SendKeys(maxYear.ToString());
            
            Driver.FindElement(By.ClassName("search-button")).Click();

            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20.0));
            wait.Until(driver => driver.FindElements(By.ClassName("loading-panel")).Count == 0);
            AssertItemsNameShows(nameList);

            // Reload the page
            Driver.Navigate().Refresh();
            wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20.0));
            wait.Until(driver => driver.FindElements(By.ClassName("loading-panel")).Count == 0);
            AssertItemsNameShows(nameList);
        }

        [Test]
        public void CanGenerateLineChart()
        {
            CreateSearchEntityType();

            CreateAndAddAddvancedSearchToMain(true, MetadataFields, 1);
            CreateAndAddGraphToMain("Year", MetadataSetNames[0], MetadataFields[2].Name, "Amount", MetadataSetNames[0], MetadataFields[1].Name, 1, 1, null, null, 2);

            Func<int, int> yearFunc = i => (i % 10) + 2000;
            Func<int, float> amountFunc = i => (float)((Math.Sin((double)i) + 1.0) * i * 0.5);
            Func<int, string> nameFunc = i => "Item Entry " + i;
            Func<int, string> optionFunc = i =>
            {
                switch(i % 3)
                {
                    case 1:
                        return "One";

                    case 2:
                        return "Two";
                }

                return "Three";
            };

            CreateItems(20, yearFunc, amountFunc, optionFunc, nameFunc);

            List<float> amountList = new List<float>();
            List<int> yearList = new List<int>();
            for (int i = 0; i < 20; ++i)
            {
                amountList.Add(amountFunc(i));
            }

            for(int i = 0; i < 10; ++i)
            {
                yearList.Add(i + 2000);
            }

            Driver.Navigate().GoToUrl(FrontEndUrl);
            AssertGraphIsCorrect(amountList, yearList);

            // Search on the year
            int minYear = 2001;
            int maxYear = 2005;
            for (int i = 9; i >= 0; --i)
            {
                int year = yearFunc(i);
                if (year < minYear || year > maxYear)
                {
                    amountList.RemoveAt(i);
                }
            }

            yearList.Clear();
            for (int i = 1; i < 6; ++i)
            {
                yearList.Add(i + 2000);
            }

            IWebElement field = Driver.FindElements(By.ClassName("search-entry"))[3];
            field.FindElement(By.ClassName("search-from")).SendKeys(minYear.ToString());
            field.FindElement(By.ClassName("search-to")).SendKeys(maxYear.ToString());

            Driver.FindElement(By.ClassName("search-button")).Click();
            
            AssertGraphIsCorrect(amountList, yearList);

            // Reload the page
            Driver.Navigate().Refresh();
            AssertGraphIsCorrect(amountList, yearList);
        }

        [Test]
        public void CanGenerateCatagorizedLineChart()
        {
            CreateSearchEntityType();

            CreateAndAddAddvancedSearchToMain(true, MetadataFields, 1);
            CreateAndAddGraphToMain("Year", MetadataSetNames[0], MetadataFields[2].Name, "Amount", MetadataSetNames[0], MetadataFields[1].Name, 1, 1, MetadataSetNames[0], MetadataFields[3].Name, 2);

            Func<int, int> yearFunc = i => (i % 10) + 2000;
            Func<int, float> amountFunc = i => (float)((Math.Sin((double)i) + 1.0) * i * 0.5);
            Func<int, string> nameFunc = i => "Item Entry " + i;
            Func<int, string> optionFunc = i =>
            {
                switch (i % 3)
                {
                    case 1:
                        return "One";

                    case 2:
                        return "Two";
                }

                return "Three";
            };

            CreateItems(20, yearFunc, amountFunc, optionFunc, nameFunc);

            List<float> amountList = new List<float>();
            List<int> yearList = new List<int>();
            for (int i = 0; i < 20; ++i)
            {
                amountList.Add(amountFunc(i));
            }

            for (int i = 0; i < 10; ++i)
            {
                yearList.Add(i + 2000);
            }

            Driver.Navigate().GoToUrl(FrontEndUrl);
            AssertGraphIsCorrect(amountList, yearList);

            // Search on the year
            int minYear = 2001;
            int maxYear = 2005;
            for (int i = 9; i >= 0; --i)
            {
                int year = yearFunc(i);
                if (year < minYear || year > maxYear)
                {
                    amountList.RemoveAt(i);
                }
            }

            yearList.Clear();
            for (int i = 1; i < 6; ++i)
            {
                yearList.Add(i + 2000);
            }

            IWebElement field = Driver.FindElements(By.ClassName("search-entry"))[3];
            field.FindElement(By.ClassName("search-from")).SendKeys(minYear.ToString());
            field.FindElement(By.ClassName("search-to")).SendKeys(maxYear.ToString());

            Driver.FindElement(By.ClassName("search-button")).Click();

            AssertGraphIsCorrect(amountList, yearList);

            // Reload the page
            Driver.Navigate().Refresh();
            AssertGraphIsCorrect(amountList, yearList);
        }

        private void AssertGraphIsCorrect(IEnumerable<float> amounts, IEnumerable<int> years)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20.0));
            wait.Until(driver => driver.FindElements(By.ClassName("loading-panel")).Count == 0);

            IEnumerable<IWebElement> dateElements = Driver.FindElements(By.CssSelector("g.axis-y > g.tick > text"));
            Assert.Greater(dateElements.Count(), 0);

            IEnumerable<IWebElement> amountElements = Driver.FindElements(By.CssSelector("g.axis-x > g.tick > text"));
            Assert.Greater(amountElements.Count(), 0);
        }

        private void AssertItemsNameShows(IEnumerable<string> itemNames)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20.0));
            wait.Until(driver => driver.FindElements(By.ClassName("loading-panel")).Count == 0);

            string tableRowsXpath = "//tbody[@id = 'ListEntitiesPanelTableBody']/tr";
            
            List<IWebElement> rows = Driver.FindElements(By.XPath(tableRowsXpath), 10).ToList();
            Assert.AreEqual(itemNames.Count(), rows.Count);

            int i = 0;
            foreach(string name in itemNames)
            {
                Assert.AreEqual(rows[i].FindElement(By.ClassName("column-1")).Text, name);
                ++i;
            }
        }
    }
}
