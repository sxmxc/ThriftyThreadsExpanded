using HarmonyLib;
using MelonLoader;
using ThriftyThreadsExpanded.Utils;

[assembly: MelonInfo(typeof(ThriftyThreadsExpanded.Core), Constants.ModName, Constants.ModVersion, Constants.ModAuthor)]
[assembly: MelonGame(Constants.Game.Studio, Constants.Game.Name)]

namespace ThriftyThreadsExpanded;

public sealed class Core : MelonMod
{
    public static Core? Instance { get; private set; }

    public override void OnInitializeMelon()
    {
        Instance = this;
        ModPreferences.Initialize();
        HarmonyInstance.PatchAll(typeof(Core).Assembly);
        LoggerInstance.Msg($"{Constants.ModName} {Constants.ModVersion} initialized.");
    }

    public override void OnLateInitializeMelon()
    {
        ModHubIntegration.TryRegister();
    }

    public override void OnApplicationQuit()
    {
        Instance = null;
    }
}
