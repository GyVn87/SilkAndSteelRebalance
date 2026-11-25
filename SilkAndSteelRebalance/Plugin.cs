using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace SilkAndSteelRebalance;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.gyvn87.silkandsteelrebalance")]
public partial class Plugin : BaseUnityPlugin {
    private Harmony? _harmony = null;

    internal static ManualLogSource? ModLogger { get; private set; } = null;

    private void Awake() {
        ModLogger = Logger;
        _harmony = new Harmony(Id);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }
}
