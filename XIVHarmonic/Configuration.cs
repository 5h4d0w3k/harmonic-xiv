using Dalamud.Configuration;
using System;
using System.Collections.Generic;

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
