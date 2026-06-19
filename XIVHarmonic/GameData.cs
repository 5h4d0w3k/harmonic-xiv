using System;
using Dalamud.Utility;
using Lumina.Excel;

namespace XIVHarmonic;
using System.Collections.Generic;

public class GameData
{
    public static List<string> WeatherNames = [];
    public static List<uint> WeatherIds = [];
    public static List<string> AreaNames = [];
    public static List<uint> AreaIds = [];
    public static List<string> StatusNames = [];
    public static List<uint> StatusIds = [];
    public static List<int> SongIds = [];
    public static List<string> SongNames = [];

    private static void PopulateListFromExcel<TFrom>(ref List<uint> ids, ref List<string> names,
                                                     Func<TFrom, string> nameSelector)
        where TFrom : struct, IExcelRow<TFrom>
    {
        var sheet = Plugin.DataManager.GetExcelSheet<TFrom>(Plugin.ClientState.ClientLanguage);
        foreach (var row in sheet)
        {
            var weatherName = nameSelector(row);
            if (weatherName.IsNullOrWhitespace())
            {
                weatherName = "Unknown";
            }
            names.Add($"[{row.RowId}] {weatherName}");
            ids.Add(row.RowId);
        }
    }

    static void Populate()
    {
        PopulateListFromExcel<Lumina.Excel.Sheets.Weather>(
            ref WeatherIds, ref WeatherNames, x => x.Name.ToString()
        );
        PopulateListFromExcel<Lumina.Excel.Sheets.TerritoryType>(
            ref AreaIds, ref AreaNames, x => x.PlaceName.Value.Name.ToString()
        );
        PopulateListFromExcel<Lumina.Excel.Sheets.Status>(
            ref StatusIds, ref StatusNames, x => x.Name.ToString()
        );

        var orchSongs = OrchestrionIpc.AllSongs();
        if (orchSongs != null)
        {
            foreach (var song in orchSongs)
            {
                if (song.Id <= 0) continue;
                SongIds.Add(song.Id);
                // End credits songs have absurdly long names which don't fit in UI
                // We fix it by omitting their full names
                SongNames.Add(song.FilePath.Contains("_EndCredit")
                                  ? $"[{song.Id}] {song.FilePath}"
                                  : $"[{song.Id}] {song.Name} ({song.FilePath})");
            }
        }
    }

    static GameData()
    {
        Populate();
    }
}
