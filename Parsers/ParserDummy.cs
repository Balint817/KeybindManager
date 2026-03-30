using UnityEngine;

namespace KeybindManager
{
    public sealed class ParserDummy : ParserDummyReadOnly
    {
        public void SetKeys(params KeyCode[] keys)
        {
            SetKeysInternal(keys);
        }
        public void SetKeys(bool forceOrder, params KeyCode[] keys)
        {
            SetKeysInternal(forceOrder, keys);
        }

        public ParserDummy(params KeyCode[] keys) : base(keys)
        {

        }
        public ParserDummy(bool forceOrder, params KeyCode[] keys) : base(forceOrder, keys)
        {

        }
    }
}
