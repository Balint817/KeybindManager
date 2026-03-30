using System.Reflection;

namespace KeybindManager.Properties
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, Feature = "all")]
    internal static class MelonModInfo
    {
        public const string Name = "KeybindManager";

        public const string Description = "A library to parse and manage keybinds";

        public const string Author = "PBalint817";

        public const string Version = "3.3.0";

        public const string DownloadLink = "";

        public const int Priority = -1;
    }
}
