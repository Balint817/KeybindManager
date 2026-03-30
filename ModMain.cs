using System.IO;
using System.Linq;
using Il2CppAssets.Scripts.Database;
using MelonLoader;
using Tomlet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace KeybindManager
{
    public interface ISafeDisposable: IDisposable
    {
        bool IsDisposed { get; }
    }
    public class ModMain : MelonMod
    {
        public override void OnUpdate()
        {
            KeybindListener.UpdateAll();
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!MDKeybinds.LastKeybindReloadSuccess && sceneName == "UISystem_PC")
            {
                MDKeybinds.RefreshSettings();
            }
            
        }

        public override void OnEarlyInitializeMelon()
        {
            if ((Utils.MaxSpecialKey & Utils.MinExactSpecialKey) == 0)
            {
                return;
            }
        }
    }

}