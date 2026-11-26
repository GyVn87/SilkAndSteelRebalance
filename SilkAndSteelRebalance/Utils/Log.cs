using BepInEx.Logging;
using System.IO;
using System.Runtime.CompilerServices;

namespace SilkAndSteelRebalance.Utils {
    internal static class Log {
        private static ManualLogSource? _logger;

        public static void Init(ManualLogSource logger) => _logger = logger;

        public static void Error(string message,
                                 [CallerMemberName] string member = "",
                                 [CallerFilePath] string file = "",
                                 [CallerLineNumber] int line = 0) {
                                 string fileName = Path.GetFileNameWithoutExtension(file);
            _logger?.LogError($"[{fileName}.{member}:{line}] {message}");
        }
    }
}
