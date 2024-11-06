using HtmlAgilityPack; 
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RobloxWebScraper;

var csvHelper = new RobloxWebScraper.CsvHelper();
var gameNames = csvHelper.ReadCsv("roblox_games_data");

var options = new ChromeOptions();
options.AddArgument("--headless");
options.AddArgument("--no-sandbox");
options.AddArgument("--disable-dev-shm-usage");

using (var driver = new ChromeDriver(options))
{
    LoginToRoblox(driver);
    Thread.Sleep(2000);

    var scraper = new Scraper();
    var index = 1;

    foreach (var game in gameNames)
    {
        var url = $"https://www.roblox.com/discover/?Keyword={game}";

        driver.Navigate().GoToUrl(url);
        Thread.Sleep(1500);

        var pageSource = driver.PageSource;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(pageSource);

        var gameCardElement = htmlDocument.DocumentNode.SelectSingleNode("/html/body/div[3]/main/div[2]/div/div/div[1]/a");
        if (gameCardElement != null)
        {
            var gameLink = gameCardElement.GetAttributeValue("href", string.Empty);
            Console.WriteLine(gameLink);

            driver.Navigate().GoToUrl(gameLink);
            Thread.Sleep(1000);

            try
            {
                var gamePageSource = driver.PageSource;
                var gameHtmlDocument = new HtmlDocument();
                gameHtmlDocument.LoadHtml(gamePageSource);

                scraper.InitializeHtml(gameHtmlDocument);

                var gameTitle = scraper.GetGameTitle();
                var gameCreator = scraper.GetGameCreator();
                var ageRecommendation = scraper.GetAgeRecommendation();
                var voteUp = scraper.GetVoteUp();
                var voteDown = scraper.GetVoteDown();
                var attributes = scraper.GetAttributes();


                var gameData = new GameData(
                    Title: gameTitle,
                    Creator: gameCreator,
                    AgeRecommendation: ageRecommendation,
                    Active: attributes[0],
                    Favorites: attributes[1],
                    Visits: attributes[2],
                    VoiceChat: attributes[3],
                    Camera: attributes[4],
                    Created: attributes[5],
                    Updated: attributes[6],
                    ServerSize: attributes[7],
                    Genre: attributes[8],
                    Likes: voteUp,
                    Dislikes: voteDown,
                    GameLink: gameLink,
                    DateFetched: DateTime.Now
                );

                csvHelper.WriteCsv(gameData, "roblox_games_data(day2)");
                Console.WriteLine("Game #:" + index);
                index++;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        else
        {
            Console.WriteLine("Game card not found.");
        }
    }
}

static void LoginToRoblox(ChromeDriver driver)
{
    var username = Environment.GetEnvironmentVariable("ROBLOX_USERNAME");
    var password = Environment.GetEnvironmentVariable("ROBLOX_PASSWORD");

    Console.WriteLine($"Logging in as {username}");
    Console.WriteLine($"Password: {password}");

    driver.Navigate().GoToUrl("https://www.roblox.com/login");
    Thread.Sleep(2000);

    var usernameField = driver.FindElement(By.Id("login-username"));
    var passwordField = driver.FindElement(By.Id("login-password"));
    var loginButton = driver.FindElement(By.Id("login-button"));

    usernameField.SendKeys(username);
    passwordField.SendKeys(password);
    loginButton.Click();
}