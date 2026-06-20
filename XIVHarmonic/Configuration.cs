using Dalamud.Configuration;
using System;
using System.Collections.Generic;
using Dalamud.Utility;

namespace XIVHarmonic;

public struct Condition
{
    public int weatherTest = 0;
    public int areaTest = 0;
    public int areaLeaveTest = 0;
    public int statusTest = 0;
    public int combatTest = 0;
    public int entityProximityTest = 0;
    public string entityNameTest = "";
    public string chatLogTest = "";
    
    public int targetSong = 0;
    public bool disableIfInactive = false;

    public Condition()
    {
    }

    public string ToIfString()
    {
        string str = "";
        if (weatherTest > 0)
        {
            str += "\nIf weather is: " + GameData.StringifyId(
                       ref GameData.WeatherNames, ref GameData.WeatherIds, (uint)weatherTest);
        }
        if (areaTest > 0)
        {
            str += "\nIf current area is: " + GameData.StringifyId(
                       ref GameData.AreaNames, ref GameData.AreaIds, (uint)areaTest);
        }
        if (areaLeaveTest > 0)
        {
            str += "\nWhen leaving area: " + GameData.StringifyId(
                       ref GameData.AreaNames, ref GameData.AreaIds, (uint)areaLeaveTest);
        }
        if (statusTest > 0)
        {
            str += "\nIf has status: " + GameData.StringifyId(
                       ref GameData.StatusNames, ref GameData.StatusIds, (uint)statusTest);
        }
        if (combatTest == 1)
        {
            str += "\nIf in combat";
        }
        if (combatTest == 2)
        {
            str += "\nIf not in combat";
        }

        if (!entityNameTest.IsNullOrEmpty())
        {
            str += $"\nIf entity with name [{entityNameTest}] is found";
            if (entityProximityTest > 0)
            {
                str += $" within {entityProximityTest} units";
            }
        }

        if (!chatLogTest.IsNullOrEmpty())
        {
            str += $"\nTrigger on chat log [{chatLogTest}]";
        }

        if (str.IsNullOrEmpty())
        {
            str += "Always";
        }

        return str.Substring(1);
    }

    public string ToThenString()
    {
        string str;

        if (targetSong == 0)
        {
            str = "Stop all custom tracks";
        } else {
            str = "Then play song: " + GameData.StringifyId(
                      ref GameData.SongNames, ref GameData.SongIds, (uint)targetSong);
        }
        
        if (disableIfInactive)
        {
            str += "\nStop playing if condition is inactive";
        }

        return str;
    }

    public override string ToString()
    {
        return ToIfString() + "\n" + ToThenString();
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
