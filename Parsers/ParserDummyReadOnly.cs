using UnityEngine;

namespace KeybindManager
{
    public class ParserDummyReadOnly : AbstractParser
    {
        internal void SetKeysInternal(params KeyCode[] keys)
        {
            var oldKeys = KeysInternal;
            KeysInternal = keys.ToArray();
            InvokeOnReload(oldKeys, null);
        }
        internal void SetKeysInternal(bool forceOrder, params KeyCode[] keys)
        {
            var oldForceOrder = _forceOrder;
            _forceOrder = forceOrder;
            if (keys is null)
            {
                InvokeOnReload(null, oldForceOrder);
                return;
            }
            var oldKeys = KeysInternal;
            KeysInternal = keys.ToArray();
            InvokeOnReload(oldKeys, oldForceOrder);
        }
        public ParserDummyReadOnly(params KeyCode[] keys)
        {
            LastReloadSuccess = true;
            KeysInternal = keys.ToArray();
        }
        public ParserDummyReadOnly(bool forceOrder, params KeyCode[] keys) : this(keys)
        {
            _forceOrder = forceOrder;
        }
        public override void Reload()
        {
            return;
        }
    }

}
