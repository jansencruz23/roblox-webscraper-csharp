﻿using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RobloxWebScraper;
using System.Globalization;

var username = "congratsuhackoor"; 
var password = "12 qwaszxC";

var game = "bloxfruits";
var url = $"https://www.roblox.com/discover/?Keyword=bloxfruits";

var options = new ChromeOptions();
options.AddArgument("--headless");
options.AddArgument("--no-sandbox");
options.AddArgument("--disable-dev-shm-usage");

using (var driver = new ChromeDriver(options))
{
    driver.Navigate().GoToUrl("https://www.roblox.com/login");
    Thread.Sleep(2000);

    var usernameField = driver.FindElement(By.Id("login-username"));
    var passwordField = driver.FindElement(By.Id("login-password"));
    var loginButton = driver.FindElement(By.Id("login-button"));

    usernameField.SendKeys(username);
    passwordField.SendKeys(password);
    loginButton.Click();

    Thread.Sleep(2000);

    driver.Navigate().GoToUrl(url);
    Thread.Sleep(2000);

    var pageSource = driver.PageSource;
    var htmlDocument = new HtmlDocument();
    htmlDocument.LoadHtml(pageSource);  

    var gameCardElement = htmlDocument.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div/div/div[1]/a");
    if (gameCardElement != null)
    {
        var gameLink = gameCardElement.GetAttributeValue("href", string.Empty);
        Console.WriteLine(gameLink);

        driver.Navigate().GoToUrl(gameLink);
        Thread.Sleep(2000);

        var gamePageSource = driver.PageSource;
        var gameHtmlDocument = new HtmlDocument();
        gameHtmlDocument.LoadHtml(gamePageSource);

        var gameTitleElement = gameHtmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"game-detail-page\"]/div[3]/div[2]/div[1]/h1");
        var gameTitle = gameTitleElement.InnerText;

        var gameCreatorElement = gameHtmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"game-detail-page\"]/div[3]/div[2]/div[1]/div[1]/a");
        var gameCreator = gameCreatorElement.InnerText;

        var ageRecommendationElement = gameHtmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"game-age-recommendation-container\"]/a");
        var ageRecommendation = ageRecommendationElement.InnerText;

        var attributes = new List<string>();

        for (int i = 1; i <= 9; i++)
        {
            var attributeElement = gameHtmlDocument.DocumentNode.SelectSingleNode($"/html/body/div[3]/main/div[2]/div[1]/div[4]/div/div[1]/div/div/div[1]/ul/li[{i}]/p[2]");
            var attribute = attributeElement.InnerText.Replace(',','\0');

            attributes.Add(attribute);
        }

        var gameData = new GameData
        {
            Title = gameTitle,
            Creator = gameCreator,
            AgeRecommendation = ageRecommendation,
            Active = attributes[0],
            Favorites = attributes[1],
            Visits = attributes[2],
            VoiceChat = attributes[3],
            Camera = attributes[4],
            Created = attributes[5],
            Updated = attributes[6],
            ServerSize = attributes[7],
            Genre = attributes[8],
        };

        var projectRootPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..");
        var dataFolderPath = Path.Combine(projectRootPath, "data");
        Directory.CreateDirectory(dataFolderPath);
        var filePath = Path.Combine(dataFolderPath, "roblox_games_data.csv");

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "|",
            HasHeaderRecord = !File.Exists(filePath) || new FileInfo(filePath).Length == 0
        };

        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, csvConfig))
        {
            csv.WriteRecords(new List<GameData> { gameData });
        }
        
        Console.WriteLine("Game data written to game_data.csv");
    }
    else
    {
        Console.WriteLine("Game card not found.");
    }
}