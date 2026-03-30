using Harmony;
using Il2CppAssets.Scripts.UI.Specials;
using MelonLoader;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KeybindManager
{
    public sealed class KeybindListener : ISafeDisposable
    {
        //Mouse0 = 323,
        //Mouse1 = 324,
        //Mouse2 = 325,
        //Mouse3 = 326,
        //Mouse4 = 327,
        //Mouse5 = 328,
        //Mouse6 = 329,
        static KeybindListener()
        {
            static void AddSpecialKey(SpecialKeyType keyType, byte idx)
            {
                var key = Utils.GetKeyCode(keyType, idx);
                _specialKeyDict[key] = SpecialPressInfo.Create(keyType, idx);
            }
            _allKeysExceptNone = Enum.GetValues<KeyCode>().DistinctBy(x => (int)x).Where(x => x != 0).ToArray();

            _mouseKeys = Utils.Range(323, 329).Cast<KeyCode>().ToArray();
            _controllerKeys = _allKeysExceptNone.Where(x => (int)x >= 330).ToArray();
            _allKeysExceptMouse = _allKeysExceptNone.Except(_mouseKeys).ToArray();

            _pressInfoDict = _allKeysExceptNone.ToDictionary(x => x, x => (BasePressInfo)new PressInfo(x));
            _pressInfoDict[KeyCode.None] = new NullPressInfo();
            PressInfoDict = new(_pressInfoDict);

            _keyboardKeys = _pressInfoDict.Keys.Except(_controllerKeys).ToArray();

            _specialKeyDict = new();

            foreach (var keyType in Enum.GetValues<SpecialKeyType>())
            {
                if (keyType >= Utils.MinExactSpecialKey)
                {
                    AddSpecialKey(keyType, 0);
                    continue;
                }

                for (byte idx = 0; idx < byte.MaxValue; idx++)
                {
                    AddSpecialKey(keyType, idx);
                }
                AddSpecialKey(keyType, byte.MaxValue);
            }
            SpecialKeyDict = new(_specialKeyDict);
        }

        internal static readonly KeyCode[] _keyboardKeys;
        /// <summary>
        /// from 323 to 329
        /// </summary>
        internal static readonly KeyCode[] _mouseKeys;
        /// <summary>
        /// greater than 330 (ends at 509)
        /// </summary>
        internal static readonly KeyCode[] _controllerKeys;
        internal static readonly KeyCode[] _allKeysExceptNone;
        internal static readonly KeyCode[] _allKeysExceptMouse;
        internal static readonly Dictionary<KeyCode, BasePressInfo> _pressInfoDict;
        internal static readonly Dictionary<KeyCode, SpecialPressInfo> _specialKeyDict;
        public static readonly ReadOnlyDictionary<KeyCode, BasePressInfo> PressInfoDict;
        public static readonly ReadOnlyDictionary<KeyCode, SpecialPressInfo> SpecialKeyDict;
        public bool IsDisposed { get; private set; }
        public KeyCode[] Keys => _parser.Keys;
        public bool ForceOrder => _parser.ForceOrder;
        internal KeyCode[] KeysInternal => _parser.KeysInternal;
        public void SetKeys(params KeyCode[] keys)
        {
            if (KeybindParser.GetType() == typeof(ParserDummyReadOnly))
            {
                throw new MethodAccessException($"{nameof(ParserDummyReadOnly)} does not have a public setter");
            }
            SetKeysInternal(keys);
        }
        public void SetKeys(bool forceOrder, params KeyCode[] keys)
        {
            if (KeybindParser.GetType() == typeof(ParserDummyReadOnly))
            {
                throw new MethodAccessException($"{nameof(ParserDummyReadOnly)} does not have a public setter");
            }
            SetKeysInternal(forceOrder, keys);
        }
        internal void SetKeysInternal(params KeyCode[] keys)
        {
            if (_parser is ParserDummyReadOnly parser)
            {
                parser.SetKeysInternal(keys);
                return;
            }
            throw new InvalidOperationException($"the assigned parser doesn't inherit from {nameof(ParserDummyReadOnly)}");
        }
        internal void SetKeysInternal(bool forceOrder, params KeyCode[] keys)
        {
            if (_parser is ParserDummyReadOnly parser)
            {
                parser.SetKeysInternal(forceOrder, keys);
                return;
            }
            throw new InvalidOperationException($"the assigned parser doesn't inherit from {nameof(ParserDummyReadOnly)}");
        }

        public bool LastReloadSuccess => _parser.LastReloadSuccess;
        public MelonPreferences_Entry<string> Entry
        {
            get
            {
                return ((MelonParser)_parser).Entry;
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            OnPress = null!;
            OnRelease = null!;
            OnTick = null!;
            _parser.OnReload -= ReloadEvent;
            _parser = null!;

            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        ~KeybindListener()
        {
            if (!IsDisposed) Dispose();
        }

        private void AddThis()
        {
            _instances.Add(new WeakReference<KeybindListener>(this));
        }

        private static readonly List<WeakReference<KeybindListener>> _instances = new List<WeakReference<KeybindListener>>();
        internal static void UpdateAll()
        {
            foreach (var pressInfo in _pressInfoDict.Values)
            {
                pressInfo.Update();
            }
            foreach (var specialPressInfo in _specialKeyDict.Values)
            {
                specialPressInfo.Update();
            }
            for (int i = 0; i < _instances.Count; i++)
            {

                var reference = _instances[i];
                if (reference.TryGetTarget(out var bind))
                {
                    if (!bind.IsDisposed)
                    {
                        bind.Update();
                        continue;
                    }
                };
                _instances.RemoveAt(i);
                i--;
            }
        }
        List<BasePressInfo> _pressInfos;
        public ReadOnlyCollection<BasePressInfo> PressInfos;
        private void Checks_Ordered()
        {
            var count = _pressInfos.Count;
            if (count == 0)
            {
                IsHeld = false;
                return;
            }
            else if (count == 1)
            {
                IsHeld = _pressInfos[0].IsPressed;
                return;
            }

            if (IsHeld)
            {
                if (_pressInfos.Any(x => !x.IsPressed))
                {
                    IsHeld = false;
                    return;
                }
                return;
            }
            var previousKey = _pressInfos[0];
            if (!previousKey.IsPressed)
            {
                return;
            }
            for (int i = 1; i < count; i++)
            {
                var currentKey = _pressInfos[i];
                if (!currentKey.IsPressed)
                {
                    return;
                }
                if (currentKey.PressStartTime < previousKey.PressStartTime)
                {
                    return;
                }
                previousKey = currentKey;
            }
            IsHeld = true;
        }

        private void Checks_Unordered()
        {
            IsHeld = _pressInfos.Count != 0
                && _pressInfos.All(x => x.IsPressed);
        }
        private void Update()
        {
            if (ForceOrder)
            {
                Checks_Ordered();
            }
            else
            {
                Checks_Unordered();
            }
            InvokeOnTick();
        }

        private void InternalEventInvoke(Action<KeybindListener>? pressEvent)
        {
            if (pressEvent is null) return;
            foreach (Action<KeybindListener> item in pressEvent.GetInvocationList())
            {
                try
                {
                    item?.Invoke(this);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex.ToString());
                }
            }
        }
        private void InvokeOnPress()
        {
            InternalEventInvoke(OnPress);
        }
        private void InvokeOnRelease()
        {
            InternalEventInvoke(OnRelease);
        }
        private void InvokeOnTick()
        {
            InternalEventInvoke(OnTick);
        }

        public event Action<KeybindListener>? OnPress;
        public event Action<KeybindListener>? OnRelease;
        public event Action<KeybindListener>? OnTick;

        private AbstractParser _parser;
        public AbstractParser KeybindParser
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(nameof(KeybindListener));
                return _parser;
            }
            [MemberNotNull(nameof(_parser))]
            set
            {
                if (IsDisposed) throw new ObjectDisposedException(nameof(KeybindListener));
                if (_parser is null)
                {
                    _parser = value ?? throw new ArgumentNullException(nameof(value));
                    _parser.OnReload += ReloadEvent;
                    Reload();
                    return;
                }
                var old = _parser;
                if (_parser != old)
                {
                    _parser = value ?? throw new ArgumentNullException(nameof(value));
                    old.OnReload -= ReloadEvent;
                    _parser.OnReload += ReloadEvent;
                    if (old.ForceOrder != _parser.ForceOrder)
                    {
                        if (!Enumerable.SequenceEqual(old.KeysInternal, _parser.KeysInternal))
                        {
                            ReloadEvent(value, old.KeysInternal, value.ForceOrder);
                        }
                    }
                    else if (!Enumerable.SequenceEqual(old.KeysInternal, _parser.KeysInternal))
                    {
                        ReloadEvent(value, old.KeysInternal, null);
                    }
                }
            }
        }

        private void ReloadEvent(AbstractParser parser, KeyCode[]? oldKeys, bool? oldForceOrder)
        {
            if (parser is not null && !LastReloadSuccess) return;
            if (oldForceOrder is null && oldKeys is null) return;
            Reload();
        }
        private void Reload()
        {
            IsHeld = false;
            _pressInfos.Clear();
            foreach (var key in KeysInternal)
            {
                if (_pressInfoDict.TryGetValue(key, out BasePressInfo? pressInfo))
                {
                    _pressInfos.Add(pressInfo);
                }
                else if (_specialKeyDict.TryGetValue(key, out var specialPressInfo))
                {
                    _pressInfos.Add(specialPressInfo);
                }
                else
                {
                    _pressInfos.Clear();
                    _pressInfos.Add(_pressInfoDict[KeyCode.None]);
                    break;
                }
            }
        }

        private bool _isHeld;

        public bool IsHeld
        {
            get
            {
                return _isHeld;
            }
            set
            {
                if (_isHeld == value)
                {
                    return;   
                }
                if (_isHeld = value)
                {
                    InvokeOnPress();
                    return;
                }
                InvokeOnRelease();
            }
        }
        public KeybindListener(AbstractParser keybindParser)
        {
            _pressInfos = new();
            PressInfos = _pressInfos.AsReadOnly();
            KeybindParser = keybindParser;
            AddThis();
        }
        public KeybindListener(MelonPreferences_Entry<string> entry) : this(new MelonParser(entry)) { }
        public KeybindListener(string defaultValue) : this(new StringParser(defaultValue)) { }
        public KeybindListener(string value, string defaultValue) : this(new StringParser(value, defaultValue)) { }
        public KeybindListener(params KeyCode[] keys) : this(new ParserDummy(keys)) { }
        public KeybindListener(bool forceOrder, params KeyCode[] keys) : this(new ParserDummy(forceOrder, keys)) { }
        public override string ToString()
        {
            return Utils.GetKeybindString(ForceOrder, KeysInternal);
        }
    }

}
