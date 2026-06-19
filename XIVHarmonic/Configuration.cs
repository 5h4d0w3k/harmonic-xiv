using Dalamud.Configuration;
using System;
using System.Collections.Generic;
using Dalamud.Utility;

namespace XIVHarmonic;

public struct Condition
{
    public int weatherTest;
    public int areaTest;
    public int statusTest;
    public int combatTest;
    public int entityProximityTest;
    public string entityNameTest;
    public string chatLogTest;
    
    public int targetSong;
    public bool disableIfInactive;

    public override string ToString()
    {
        string str = "";
        if (weatherTest > 0)
        {
            str += "If weather is: " + GameData.StringifyId(
                       ref GameData.WeatherNames, ref GameData.WeatherIds, (uint)weatherTest) + "\n";
        }
        if (areaTest > 0)
        {
            str += "If current area is: " + GameData.StringifyId(
                       ref GameData.AreaNames, ref GameData.AreaIds, (uint)areaTest) + "\n";
        }
        if (statusTest > 0)
        {
            str += "If has status: " + GameData.StringifyId(
                       ref GameData.StatusNames, ref GameData.StatusIds, (uint)statusTest) + "\n";
        }
        if (combatTest == 1)
        {
            str += "If in combat\n";
        }
        if (combatTest == 2)
        {
            str += "If not in combat\n";
        }

        if (!entityNameTest.IsNullOrEmpty())
        {
            str += $"If entity with name [{entityNameTest}] is found";
            if (entityProximityTest > 0)
            {
                str += $" within {entityProximityTest} units";
            }
            str += "\n";
        }

        if (!chatLogTest.IsNullOrEmpty())
        {
            str += $"If chat log contains [{chatLogTest}]\n";
        }

        str += "Then play song: " + GameData.StringifyId(
                   ref GameData.SongNames, ref GameData.SongIds, (uint)targetSong);

        if (disableIfInactive)
        {
            str += "\nStop playing if condition is inactive";
        }

        return str;
    }
}

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public List<Condition> Conditions = [];

    // The below exists just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
