using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace KeybindManager.Patches
{

    [HarmonyPatch(typeof(PnlInputPc), nameof(PnlInputPc.OnDisablePnl))]
    class PnlInputDisablePatch
    {
        static void Postfix(PnlInputPc __instance)
        {
            try
            {
                MDKeybinds.RefreshSettings(__instance.m_Config);
            }
            catch (Exception ex)
            {
                MelonLogger.Msg(ConsoleColor.Red, ex.ToString());
            }
        }
    }
}
