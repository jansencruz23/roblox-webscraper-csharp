using HtmlAgilityPack;

namespace RobloxWebScraper;

public sealed class Scraper
{
    private HtmlDocument _html;

    public Scraper()
    {
        _html = new HtmlDocument();
    }

    public void InitializeHtml(HtmlDocument html)
    {
        _html = html;
    }

    public string GetGameTitle()
    {
        var gameTitleElement = _html.DocumentNode.SelectSingleNode("//*[@id=\"game-detail-page\"]/div[3]/div[2]/div[1]/h1");
        var gameTitle = gameTitleElement.InnerText.Replace('|', '\0');

        return gameTitle;
    }

    public string GetGameCreator()
    {
        var gameCreatorElement = _html.DocumentNode.SelectSingleNode("//*[@id=\"game-detail-page\"]/div[3]/div[2]/div[1]/div[1]/a");
        var gameCreator = gameCreatorElement.InnerText;

        return gameCreator;
    }

    public string GetAgeRecommendation()
    {
        var ageRecommendationElement = _html.DocumentNode.SelectSingleNode("//*[@id=\"game-age-recommendation-container\"]/a");
        var ageRecommendation = ageRecommendationElement == null ? "All Ages" : ageRecommendationElement.InnerText;

        return ageRecommendation;
    }
    
    public string GetVoteUp()
    {
        var voteUpElement = _html.DocumentNode.SelectSingleNode("//*[@id=\"vote-up-text\"]");
        var voteUp = voteUpElement.InnerText.Replace(',', '\0'); ;

        return voteUp;
    }

    public string GetVoteDown()
    {
        var voteDownElement = _html.DocumentNode.SelectSingleNode("//*[@id=\"vote-down-text\"]");
        var voteDown = voteDownElement.InnerText.Replace(',', '\0'); ;

        return voteDown;
    }

    public List<string> GetAttributes()
    {
        var attributes = new List<string>();

        for (int i = 1; i <= 9; i++)
        {
            var attributeElement = _html.DocumentNode.SelectSingleNode($"/html/body/div[3]/main/div[2]/div[1]/div[4]/div/div[1]/div/div/div[1]/ul/li[{i}]/p[2]");
            attributeElement ??= _html.DocumentNode.SelectSingleNode($"/html/body/div[3]/main/div[2]/div[1]/div[4]/div/div[1]/div/div/div[2]/ul/li[{i}]/p[2]");

            var attribute = attributeElement == null ? "" : attributeElement.InnerText.Replace(',', '\0');
            if (string.IsNullOrWhiteSpace(attribute)) break;
            attributes.Add(attribute);
        }

        return attributes;
    }
}

