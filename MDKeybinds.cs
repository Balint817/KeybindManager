using Il2CppAssets.Scripts.GameCore.Controller.Configs;
using MelonLoader;
using Il2CppPeroTools2.Resources;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace KeybindManager
{
    public static class MDKeybinds
    {
        public static bool LastKeybindReloadSuccess { get; private set; } = false;

        private static readonly List<KeybindListener> _airKeybinds = new();
        private static readonly List<KeybindListener> _groundKeybinds = new();
        private static readonly List<KeybindListener> _feverKeybinds = new();
        public static ReadOnlyCollection<KeybindListener> AirKeybinds => _airKeybinds.AsReadOnly();
        public static ReadOnlyCollection<KeybindListener> GroundKeybinds => _groundKeybinds.AsReadOnly();
        public static ReadOnlyCollection<KeybindListener> FeverKeybinds => _feverKeybinds.AsReadOnly();

        public static event Action? OnReload;
        private static void InvokeOnReload()
        {
            if (OnReload is null) return;
            foreach (Action item in OnReload.GetInvocationList())
            {
                try
                {
                    item?.Invoke();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex.ToString());
                }
            }
        }
        internal static void RefreshSettings(StandloneCtrlConfig? config = null)
        {
            try
            {
                config ??= ResourcesManager.instance.LoadFromName<StandloneCtrlConfig>("InputStandlone");
                LastKeybindReloadSuccess = false;
                var buttons = config.buttonKeyEnties[config.CurrentProposal];
                var air = buttons["BattleAir"];
                var ground = buttons["BattleGround"];
                var fever = buttons["Fever"];

                for (int i = 0; i < air.Count; i++)
                {
                    if (_airKeybinds.Count == i)
                    {
                        _airKeybinds.Add(new KeybindListener(new ParserDummyReadOnly(air[i])));
                        continue;
                    }
                    _airKeybinds[i].SetKeysInternal(air[i]);
                }

                for (int i = 0; i < ground.Count; i++)
                {
                    if (_groundKeybinds.Count == i)
                    {
                        _groundKeybinds.Add(new KeybindListener(new ParserDummyReadOnly(ground[i])));
                        continue;
                    }
                    _groundKeybinds[i].SetKeysInternal(ground[i]);
                }

                for (int i = 0; i < fever.Count; i++)
                {
                    if (_feverKeybinds.Count == i)
                    {
                        _feverKeybinds.Add(new KeybindListener(new ParserDummyReadOnly(fever[i])));
                        continue;
                    }
                    _feverKeybinds[i].SetKeysInternal(fever[i]);
                }

                LastKeybindReloadSuccess = true;
            }
            finally
            {
                InvokeOnReload();
            }
        }
    }

}
