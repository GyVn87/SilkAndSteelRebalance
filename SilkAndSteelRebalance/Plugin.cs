using BepInEx;
using HarmonyLib;
using ToolsRebalanced.Utils;
using System.Reflection;

namespace ToolsRebalanced;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.gyvn87.toolsrebalanced")]
public partial class Plugin : BaseUnityPlugin {
    private Harmony? _harmony = null;

    private void Awake() {
        Log.Init(Logger);
        _harmony = new Harmony(Id);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }
}
