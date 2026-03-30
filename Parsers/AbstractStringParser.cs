using KeybindManager.Parsers;
using MelonLoader;
using UnityEngine;

namespace KeybindManager
{
    public abstract class AbstractStringParser : AbstractParser, IOrderToggleableParser
    {
        private static readonly Dictionary<string, KeyCode> _d = Utils.GetNameValuePairs<KeyCode>().ToDictionary(x => x.Key.ToLowerInvariant(), x => x.Value);
        private static readonly Dictionary<string, SpecialKeyType> _sd = Utils.GetNameValuePairs<SpecialKeyType>().ToDictionary(x => x.Key.ToLowerInvariant(), x => x.Value);

        private bool _ignoreForceOrder;
        public bool IgnoreForceOrder
        {
            get
            {
                
                return _ignoreForceOrder;
            }
            set
            {
                if (value != _ignoreForceOrder)
                {
                    _ignoreForceOrder = value;
                    InvokeOnReload(null, _forceOrder);
                }
            }
        }
        public override bool ForceOrder => !_ignoreForceOrder && _forceOrder;
        public static bool TryParse(string keyName, out KeyCode result)
        {
            keyName = keyName.ToLowerInvariant();
            if (_d.TryGetValue(keyName, out result))
            {
                return true;
            }
            if (keyName == Utils.UnknownKeyLower)
            {
                result = KeyCode.None;
                return true;
            }
            foreach (var kv in _sd)
            {
                var specialKeyName = kv.Key;
                if (!keyName.StartsWith(specialKeyName))
                {
                    continue;
                }
                var specialKeyValue = kv.Value;
                if (keyName.Length == specialKeyName.Length)
                {
                    if (specialKeyValue > Utils.MinExactSpecialKey)
                    {
                        try
                        {
                            result = Utils.GetKeyCode(specialKeyValue, 0);
                        }
                        catch (Exception ex)
                        {
                            MelonLogger.Error(ex);
                            return false;
                        }
                        return true;
                    }
                    continue;
                }
                keyName = keyName[specialKeyName.Length..];
                if (!byte.TryParse(keyName, out var b))
                {
                    continue;
                }
                try
                {
                    result = Utils.GetKeyCode(specialKeyValue, b);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex);
                    return false;
                }
                return true;
            }
            return false;
        }
        public abstract string Value { get; set; }
        public abstract string DefaultValue { get; set; }
        public override void Reload()
        {
            var oldKeys = KeysInternal;
            var oldForceOrder = _forceOrder;
            var trimmed = Value?.Trim();
            if (trimmed is null || trimmed.Length == 0)
            {
                LastReloadSuccess = true;
                KeysInternal = EmptyKeys;
                _forceOrder = false;
                InvokeOnReload(oldKeys, oldForceOrder);
                return;
            }
            LastReloadSuccess = false;

            bool isForceOrder = false;
            if (trimmed[0] == '$')
            {
                if (trimmed.Length == 1)
                {
                    if (Value == DefaultValue)
                    {
                        MelonLogger.Error($"Failed to parse default keys \"{DefaultValue}\", go cry to the mod's creator");
                        InvokeOnReload(null, null);
                        return;
                    }
                    MelonLogger.Error($"Failed to parse key \"{Value}\", attempting fallback to default keys '{DefaultValue}'");
                    Value = DefaultValue;
                    return;
                }
                trimmed = trimmed[1..];
                isForceOrder = true;
            }
            trimmed = trimmed.ToLowerInvariant();
            var result = new List<KeyCode>();
            foreach (var key in trimmed.Split(' '))
            {
                if (key.Length == 0) continue;

                if (TryParse(key, out KeyCode parsed))
                {
                    result.Add(parsed);
                    continue;
                }

                if (Value == DefaultValue)
                {
                    MelonLogger.Error($"Failed to parse default keys \"{DefaultValue}\", go cry to the mod's creator");
                    InvokeOnReload(null, null);
                    return;
                }
                MelonLogger.Error($"Failed to parse key \"{key}\" in \"{Value}\", attempting fallback to default keys '{DefaultValue}'");
                Value = DefaultValue;
            }
            KeysInternal = result.ToArray();
            _forceOrder = isForceOrder;
            LastReloadSuccess = true;
            InvokeOnReload(oldKeys, oldForceOrder);
        }
    }

}
