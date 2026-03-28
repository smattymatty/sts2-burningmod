using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;

namespace BurningMod;

[ModInitializer("ModLoaded")]
public static class BurningMod
{
    public const string ModId = "BurningMod";

    public static void ModLoaded()
    {
        var harmony = new Harmony(ModId);
        harmony.PatchAll();
        Log.Warn("MOD FINISHED LOADING");
    }
}