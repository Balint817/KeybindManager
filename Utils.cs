using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace KeybindManager
{
    public class KeyAction : IEnumerable
    {
        internal unsafe static void SetInputValue(float inputValue)
        {
            var keyboard = Keyboard.current;
            var nativeArray = StateEvent.From(keyboard, out InputEventPtr eventPtr, Allocator.Persistent);

            float currentInputValue = (keyboard.qKey).ReadValue();
            if (currentInputValue != inputValue && eventPtr.valid)
            {
                //Didn't use "InputControlExtensions.WriteValueIntoEvent" here due to generic ValueType limitations (ExecutionEngineException AOT code not generated)
                //Also couldn't use pointer-based alternatives because IL2CPP tries to... construct... a pointer..... what?
                InputControlExtensions.WriteValueFromObjectIntoEvent(keyboard.qKey, eventPtr, inputValue.BoxToCppInt().BoxIl2CppObject());
                InputSystem.QueueEvent(eventPtr);
            }
        }
        public IEnumerator GetEnumerator()
        {
            yield break;
        }
    }
    public static class Utils
    {
        internal static Il2CppSystem.Int32 BoxToCppInt(this int n)
        {
            return new Il2CppSystem.Int32()
            {
                m_value = n
            };
        }
        internal unsafe static Il2CppSystem.Int32 BoxToCppInt(this float n)
        {
            return (*(int*)&n).BoxToCppInt();
        }

        //static readonly Dictionary<KeyCode, int> legacyToNew;
        //static readonly Dictionary<int, KeyCode> newToLegacy;

        public static Dictionary<string, T> GetNameValuePairs<T>() where T : struct, Enum
        {
            return Enum.GetNames<T>().ToDictionary(x => x, x => Enum.Parse<T>(x, false));
        }
        public static Dictionary<T, string[]> GetValuesAndNames<T>() where T : struct, Enum
        {
            return Enum.GetNames<T>().GroupBy(x => Enum.Parse<T>(x, false)).ToDictionary(x => x.Key, x => x.ToArray());
        }
        /// <summary>
        /// Mouse0 = 323,
        /// Mouse1 = 324,
        /// Mouse2 = 325,
        /// Mouse3 = 326,
        /// Mouse4 = 327,
        /// Mouse5 = 328,
        /// Mouse6 = 329,
        /// JoystickButton0 = 330,
        /// JoystickButton1 = 331,
        /// JoystickButton2 = 332,
        /// JoystickButton3 = 333,
        /// JoystickButton4 = 334,
        /// JoystickButton5 = 335,
        /// JoystickButton6 = 336,
        /// JoystickButton7 = 337,
        /// JoystickButton8 = 338,
        /// JoystickButton9 = 339,
        /// JoystickButton10 = 340,
        /// JoystickButton11 = 341,
        /// JoystickButton12 = 342,
        /// JoystickButton13 = 343,
        /// JoystickButton14 = 344,
        /// JoystickButton15 = 345,
        /// JoystickButton16 = 346,
        /// JoystickButton17 = 347,
        /// JoystickButton18 = 348,
        /// JoystickButton19 = 349,
        /// </summary>
        /// 
        //static bool IsIMESelected
        //{
        //    get
        //    {
        //        return false;
        //    }
        //    set
        //    {
        //        //...
        //    }
        //}
        //static Utils()
        //{
        //    legacyToNew = new();

        //    var legacyKeyDict = GetNameValuePairs<KeyCode>().Where(x => x.Value < KeyCode.Mouse0).ToDictionary(x => x.Key, x => x.Value);
        //    var newKeyDict = GetNameValuePairs<Key>();

        //    var legacyNameDict = GetValuesAndNames<KeyCode>().Where(x => x.Key < KeyCode.Mouse0).ToDictionary(x => x.Key, x=> x.Value);
        //    var newNameDict = GetValuesAndNames<Key>();

        //    newKeyDict = new(newKeyDict, StringComparer.OrdinalIgnoreCase);
        //    legacyKeyDict = new(legacyKeyDict, StringComparer.OrdinalIgnoreCase);

        //    /// Local function definitions start here
        //    void RemoveLegacyKey(KeyCode oldKey)
        //    {
        //        var oldNames = legacyNameDict![oldKey];
        //        foreach (var oldName in oldNames)
        //        {
        //            legacyKeyDict!.Remove(oldName);
        //        }
        //        legacyNameDict.Remove(oldKey);
        //    }
        //    void RemoveNewKey(Key newKey)
        //    {
        //        var newNames = newNameDict![newKey];
        //        foreach (var newName in newNames)
        //        {
        //            newKeyDict!.Remove(newName);
        //        }
        //        newNameDict.Remove(newKey);
        //    }
        //    void RemoveKeys(KeyCode oldKey, Key newKey)
        //    {
        //        RemoveLegacyKey(oldKey);
        //        RemoveNewKey(newKey);
        //    }
        //    void AddKey(Key newKey, KeyCode oldKey, int offset = 0)
        //    {
        //        legacyToNew.Add(oldKey, (int)newKey);
        //        var newNames = newNameDict[newKey];
        //        RemoveKeys(oldKey, newKey);
        //    }
        //    /// Local function definitions end here
            
        //    foreach (var kv in legacyKeyDict.ToArray())
        //    {
        //        var legacyKeyName = kv.Key;
        //        if (newKeyDict.TryGetValue(legacyKeyName, out var newKey))
        //        {
        //            AddKey(newKey, kv.Value);
        //        }
        //    }

        //    /// Local function definitions start here
        //    void SkipKey(KeyCode oldKey)
        //    {
        //        RemoveLegacyKey(oldKey);
        //    }
        //    void AddKeyRange(Key newKeyStart, Key newKeyEnd, KeyCode oldKeyStart, int offset=0)
        //    {
        //        var start = (int)newKeyStart;
        //        var end = (int)newKeyEnd;
        //        var difference = (int)oldKeyStart - start;
        //        for (int i = (int)newKeyStart; i <= (int)newKeyEnd; i++)
        //        {
        //            var legacyKey = (KeyCode)(i+difference);
        //            AddKey((Key)i, legacyKey, offset);
        //        }
        //    }
        //    /// Local function definitions end here

        //    AddKey(Key.Digit0, KeyCode.Alpha0);
        //    AddKeyRange(Key.Digit1, Key.Digit9, KeyCode.Alpha1);

        //    AddKeyRange(Key.Numpad0, Key.Numpad9, KeyCode.Keypad0);
        //    AddKeyRange(Key.NumpadDivide, Key.NumpadMultiply, KeyCode.KeypadDivide);
        //    AddKey(Key.NumpadEnter, KeyCode.KeypadEnter);
        //    AddKey(Key.NumpadPlus, KeyCode.KeypadPlus);
        //    AddKey(Key.NumpadMinus, KeyCode.KeypadMinus);
        //    AddKey(Key.NumpadPeriod, KeyCode.KeypadPeriod);
        //    AddKey(Key.NumpadEquals, KeyCode.KeypadEquals);
        //    SkipKey(KeyCode.F13);
        //    SkipKey(KeyCode.F14);
        //    SkipKey(KeyCode.F15);

        //    AddKey(Key.Enter, KeyCode.Return);
        //    AddKey(Key.LeftCtrl, KeyCode.LeftControl);
        //    AddKey(Key.RightCtrl, KeyCode.RightControl);
        //    AddKey(Key.PrintScreen, KeyCode.Print);
        //    AddKey(Key.PrintScreen, KeyCode.Print);


        //    bool error = false;

        //    if (legacyKeyDict.Count != 0)
        //    {
        //        error |= true;
        //        MelonLogger.Msg(ConsoleColor.DarkRed, "The following 'KeyCode' values have not been accounted for:");
        //        foreach (var item in legacyKeyDict)
        //        {
        //            Console.WriteLine($"- {item.Key}");
        //        }
        //    }
        //    if (newKeyDict.Count != 0)
        //    {
        //        error |= true;
        //        MelonLogger.Msg(ConsoleColor.DarkRed, "The following 'Key' values have not been accounted for:");
        //        foreach (var item in newKeyDict)
        //        {
        //            Console.WriteLine($"- {item.Key}");
        //        }
        //    }
        //    //if (newMouseDict.Count != 0)
        //    //{
        //    //    error |= true;
        //    //    MelonLogger.Msg(ConsoleColor.DarkRed, "The following 'MouseButton' values have not been accounted for:");
        //    //    foreach (var item in newMouseDict)
        //    //    {
        //    //        Console.WriteLine($"- {item.Key}");
        //    //    }
        //    //}

        //    /// TODO: Mouse and Joystick inputs

        //    if (error)
        //    {
        //        Thread.Sleep(int.MaxValue);
        //    }


        //    newToLegacy = legacyToNew.ToDictionary(x => x.Value, x => x.Key);
        //}
        ///// <summary>
        ///// Presses, then releases the given key on the next update.
        ///// </summary>
        //public static bool TapKey(KeyCode key)
        //{
        //    return false;
        //}
        ///// <summary>
        ///// Starts pressing (holding down) the given key.
        ///// </summary>
        //public static bool PressKey(KeyCode key)
        //{
        //    return false;
        //}
        ///// <summary>
        ///// Releases the key if it is currently held.
        ///// </summary>
        //public static bool ReleaseKey(KeyCode key)
        //{
        //    return false;
        //}
        public static IEnumerable<int> Range(int start, int end, bool inclusive=true)
        {
            if (end < start)
            {
                (start, end) = (end, start);
            }
            if (inclusive)
                end++;
            return Enumerable.Range(start, end-start);
        }
        public static readonly KeyCode MaxKeyCode = Enum.GetValues<KeyCode>().Max();
        public static readonly SpecialKeyType MinIndexedSpecialKey = Enum.GetValues<SpecialKeyType>().Min();
        public static readonly SpecialKeyType MinExactSpecialKey = Enum.GetValues<SpecialKeyType>().Where(x => (int)x >= 1000000).Min();
        public static readonly SpecialKeyType MaxSpecialKey = Enum.GetValues<SpecialKeyType>().Max();
        public static readonly string UnknownKey = "Unknown";
        public static readonly string UnknownKeyLower = UnknownKey.ToLowerInvariant();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KeyCode GetKeyCode(SpecialKeyType type, byte idx)
        {
            return (KeyCode)(type > MinExactSpecialKey ? type : type + idx);
        }
        public static string GetKeyName(this KeyCode keyCode)
        {
            if (Enum.IsDefined(keyCode))
            {
                return keyCode.ToString();
            }
            else if (keyCode < (KeyCode)MinIndexedSpecialKey || (KeyCode)MaxSpecialKey < keyCode)
            {
                return Utils.UnknownKey;
            }
            if (keyCode > (KeyCode)MinExactSpecialKey)
            {
                return ((SpecialKeyType)keyCode).ToString();
            }
            var offsetKey = (int)(keyCode - (int)MinIndexedSpecialKey);
            var asEnum = (SpecialKeyType)offsetKey;
            if (!Enum.IsDefined(asEnum))
            {
                return Utils.UnknownKey;
            }
            return $"{asEnum}{offsetKey % 256}";
        }

        public static string[] GetKeyNames(params KeyCode[] keyCodes)
        {
            var result = new string[keyCodes.Length];
            GetKeyNames(keyCodes).CopyTo(result, 0);
            return result;
        }

        public static IEnumerable<string> GetKeyNames(this IEnumerable<KeyCode> keyCodes)
        {
            foreach (var keyCode in keyCodes)
            {
                yield return GetKeyName(keyCode);
            }
        }
        public static string GetKeybindString(bool forceOrder, params KeyCode[] keyCodes)
        {
            return GetKeybindString(forceOrder, (IEnumerable<KeyCode>)keyCodes);
        }
        public static string GetKeybindString(bool forceOrder, IEnumerable<KeyCode> keyCodes)
        {
            var result = string.Join(' ', GetKeyNames(keyCodes));
            if (result.Length == 0)
            {
                result = GetKeyName(KeyCode.None);
            }
            if (forceOrder)
            {
                result = "$" + result;
            }
            return result;
        }
    }
}
