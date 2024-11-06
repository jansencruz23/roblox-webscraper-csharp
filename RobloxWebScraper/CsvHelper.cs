using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace RobloxWebScraper;

public sealed class CsvHelper
{
    public List<string> ReadCsv(string fileName)
    {
        var gamesPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..") + $"/data/{fileName}.csv";
        var gameNames = new List<string>();

        using (var reader = new StreamReader(gamesPath))
        using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        }))
        {
            while (csvReader.Read())
            {
                var gameName = csvReader.GetField<string>(0);
                gameNames.Add(gameName ?? string.Empty);
            }
        }

        return gameNames;
    }

    public void WriteCsv(GameData gameData, string fileName)
    {
        var projectRootPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..");
        var dataFolderPath = Path.Combine(projectRootPath, "data");
        Directory.CreateDirectory(dataFolderPath);
        var filePath = Path.Combine(dataFolderPath, $"{fileName}.csv");

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "|",
            HasHeaderRecord = !File.Exists(filePath) || new FileInfo(filePath).Length == 0
        };

        using (var writer = new StreamWriter(filePath, true))
        using (var csv = new CsvWriter(writer, csvConfig))
        {
            csv.WriteRecords(new List<GameData> { gameData });
        }

        Console.WriteLine($"Game data written to {fileName}.csv");
    }
} 
