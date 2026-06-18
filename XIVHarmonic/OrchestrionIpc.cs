using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Serilog;

namespace XIVHarmonic;

public class OrchestrionIpc
{
    private static ICallGateSubscriber<int, bool>? OrchPlaySong;
    
    public static void Initialize(IDalamudPluginInterface pluginInterface)
    {
        OrchPlaySong = pluginInterface.GetIpcSubscriber<int, bool>("Orch.PlaySong");
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
}
