using System.Collections.Generic;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Serilog;

namespace XIVHarmonic;

public struct OrchSong
{
    public int Id;
    public bool DisableRestart;
    public byte SpecialMode;
    public string FilePath;
    public bool FileExists;
    public string Name;
    public string AlternateName;
    public string SpecialModeName;
    public string Locations;
    public string AdditionalInfo;
}

public class OrchestrionIpc
{
    private static ICallGateSubscriber<int, bool>? OrchPlaySong;
    private static ICallGateSubscriber<List<OrchSong>>? OrchAllSongInfo;
    private static ICallGateSubscriber<int>? OrchCurrentSong;
    
    public static void Initialize()
    {
        OrchPlaySong = Plugin.PluginInterface.GetIpcSubscriber<int, bool>("Orch.PlaySong");
        OrchAllSongInfo = Plugin.PluginInterface.GetIpcSubscriber<List<OrchSong>>("Orch.AllSongInfo");
        OrchCurrentSong = Plugin.PluginInterface.GetIpcSubscriber<int>("Orch.CurrentSong");
    }

    public static bool Play(int songId)
    {
        return OrchPlaySong != null && OrchPlaySong.InvokeFunc(songId);
    }

    public static bool Stop()
    {
        return Play(0);
    }

    public static bool IsAvailable()
    {
        return OrchPlaySong != null;
    }

    public static List<OrchSong>? AllSongs()
    {
        return OrchAllSongInfo?.InvokeFunc();
    }

    public static int CurrentSong()
    {
        return OrchCurrentSong?.InvokeFunc() ?? 0;
    }
}
