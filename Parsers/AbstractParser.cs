using MelonLoader;
using System.Collections.Specialized;
using UnityEngine;

namespace KeybindManager
{
    public abstract class AbstractParser
    {
        public override string ToString()
        {
            return Utils.GetKeybindString(ForceOrder, KeysInternal);
        }
        protected bool _forceOrder;
        public virtual bool ForceOrder => _forceOrder;

        protected readonly static KeyCode[] EmptyKeys = Array.Empty<KeyCode>();
        protected void InvokeOnReload(KeyCode[]? oldKeys, bool? oldForceOrder)
        {
            if (OnReload is null) return;
            foreach (ParserReloadHandler item in OnReload.GetInvocationList())
            {
                try
                {
                    item?.Invoke(this, oldKeys, oldForceOrder);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error(ex.ToString());
                }
            }
        }

        /// <summary>
        /// The arguments are null only if they were never updated (e.g. even if they were set to the same thing, they still won't be null)
        /// </summary>
        public delegate void ParserReloadHandler(AbstractParser instance, KeyCode[]? oldKeys, bool? oldForceOrder);

        public event ParserReloadHandler? OnReload;
        public KeyCode[] Keys => KeysInternal.ToArray();

        private KeyCode[]? _keys;
        protected internal KeyCode[] KeysInternal {
            get
            {
                return _keys ??= EmptyKeys;
            }
            protected set
            {
                var oldKeys = _keys;
                ArgumentNullException.ThrowIfNull(value, nameof(value));
                var newKeys = new List<KeyCode>();
                foreach (var key in value)
                {
                    if (!newKeys.Contains(key))
                    {
                        newKeys.Add(key);
                    }
                }
                _keys = newKeys.ToArray();
            }
        }
        public bool LastReloadSuccess { get; protected set; }
        public abstract void Reload();
    }
}
