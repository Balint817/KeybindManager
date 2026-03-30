using Harmony;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KeybindManager
{
    public sealed class AnyNoMousePressInfo : CollectionAnyPressInfo
    {
        internal AnyNoMousePressInfo() : base() { }
        public override IList<KeyCode> TargetCollection => KeybindListener._allKeysExceptMouse;
    }
    public sealed class AnyMousePressInfo : CollectionAnyPressInfo
    {
        internal AnyMousePressInfo() : base() { }
        public override IList<KeyCode> TargetCollection => KeybindListener._mouseKeys;
    }
    public sealed class AnyControllerPressInfo : CollectionAnyPressInfo
    {
        internal AnyControllerPressInfo() : base() { }
        public override IList<KeyCode> TargetCollection => KeybindListener._controllerKeys;
    }
    public sealed class AnyKeyboardPressInfo : CollectionAnyPressInfo
    {
        internal AnyKeyboardPressInfo() : base() { }
        public override IList<KeyCode> TargetCollection => KeybindListener._keyboardKeys;
    }
    public sealed class AnyInputPressInfo : CollectionAnyPressInfo
    {
        internal AnyInputPressInfo() : base() { }
        public override IList<KeyCode> TargetCollection => KeybindListener._allKeysExceptNone;
    }
    public abstract class CollectionAnyPressInfo : BaseCodeTargetedPressInfo
    {
        internal CollectionAnyPressInfo() : base() { }
        protected override void UpdatePressedKeys()
        {
            _pressedKeys.Clear();
            var pressInfoSource = PressInfoSource;
            foreach (var key in TargetCollection)
            {
                if (pressInfoSource[key] is PressInfo normalInfo && normalInfo.IsPressed)
                {
                    _pressedKeys.Add(normalInfo);
                }
            }
        }
    }
    public sealed class FeverBoundPressInfo : BaseIndexedFilteredPressInfo
    {
        public override IList<KeybindListener> UnfilteredTarget => MDKeybinds.FeverKeybinds;
        internal FeverBoundPressInfo(byte idx) : base(idx)
        {

        }
    }
    public sealed class FeverPressInfo : BaseIndexedPressInfo
    {
        public override IList<KeybindListener> TargetCollection => MDKeybinds.FeverKeybinds;
        internal FeverPressInfo(byte idx) : base(idx) { }
    }
    public sealed class GroundBoundPressInfo : BaseIndexedFilteredPressInfo
    {
        public override IList<KeybindListener> UnfilteredTarget => MDKeybinds.GroundKeybinds;
        internal GroundBoundPressInfo(byte idx) : base(idx)
        {

        }
    }
    public sealed class GroundPressInfo : BaseIndexedPressInfo
    {
        public override IList<KeybindListener> TargetCollection => MDKeybinds.GroundKeybinds;
        internal GroundPressInfo(byte idx) : base(idx) { }
    }
    public sealed class AirBoundPressInfo : BaseIndexedFilteredPressInfo
    {
        public override IList<KeybindListener> UnfilteredTarget => MDKeybinds.AirKeybinds;
        internal AirBoundPressInfo(byte idx) : base(idx) { }
    }
    public sealed class AirPressInfo : BaseIndexedPressInfo
    {
        public override IList<KeybindListener> TargetCollection => MDKeybinds.AirKeybinds;
        internal AirPressInfo(byte idx) : base(idx) { }
    }
    public abstract class BaseIndexedFilteredPressInfo : BaseIndexedPressInfo
    {
        public abstract IList<KeybindListener> UnfilteredTarget { get; }
        public override IList<KeybindListener> TargetCollection => UnfilteredTarget
                    .Where(x => x.KeysInternal[0] != KeyCode.None)
                    .ToArray();
        internal BaseIndexedFilteredPressInfo(byte idx) : base(idx) { }
    }
    public abstract class BaseKeyRefPressInfo : BaseSourcedPressInfo
    {
        internal BaseKeyRefPressInfo() : base() { }
        protected override void UpdatePressedKeys()
        {
            _pressedKeys.Clear();
            if (Target.PressInfos[0] is PressInfo normalInfo && normalInfo.IsPressed)
            {
                _pressedKeys.Add(normalInfo);
            }
        }
        public abstract KeybindListener Target { get; }
    }
    public abstract class BaseIndexedPressInfo : BaseListenerTargetedPressInfo
    {
        public byte KeyIdx { get; protected set; }
        internal BaseIndexedPressInfo(byte idx) : base()
        {
            KeyIdx = idx;
        }
        protected override void UpdatePressedKeys()
        {
            _pressedKeys.Clear();
            var targetCollection = TargetCollection;
            if (KeyIdx != 0)
            {
                var idx = KeyIdx - 1;

                if (idx < targetCollection.Count)
                {
                    if (targetCollection[idx].PressInfos[0] is PressInfo normalInfo && normalInfo.IsPressed)
                    {
                        _pressedKeys.Add(normalInfo);
                    }
                }
                return;
            }
            foreach (var item in TargetCollection)
            {
                if (item.PressInfos[0] is PressInfo normalInfo && normalInfo.IsPressed)
                {
                    _pressedKeys.Add(normalInfo);
                }
            }
            return;
        }
    }
    public abstract class BaseCodeTargetedPressInfo : BaseSourcedPressInfo
    {
        internal BaseCodeTargetedPressInfo() : base() { }
        public abstract IList<KeyCode> TargetCollection { get; }
    }
    public abstract class BaseListenerTargetedPressInfo : BaseSourcedPressInfo
    {
        internal BaseListenerTargetedPressInfo() : base() { }
        public abstract IList<KeybindListener> TargetCollection { get; }
    }
    public abstract class BaseSourcedPressInfo : SpecialPressInfo
    {
        internal BaseSourcedPressInfo() : base() { }
        public virtual IDictionary<KeyCode, BasePressInfo> PressInfoSource => KeybindListener._pressInfoDict;
    }
    public abstract class SpecialPressInfo : BasePressInfo
    {
        public override string ToString(bool forceOrder)
        {
            return Utils.GetKeybindString(forceOrder, _pressedKeys.OrderBy(x => x.PressStartTime).Select(x => x.Key));
        }
        protected readonly List<PressInfo> _pressedKeys;
        public ReadOnlyCollection<PressInfo> PressedKeys { get; }
        internal SpecialPressInfo() : base()
        {
            _pressedKeys = new();
            PressedKeys = _pressedKeys.AsReadOnly();
        }
        protected abstract void UpdatePressedKeys();
        protected sealed override bool PressCondition()
        {
            UpdatePressedKeys();
            return _pressedKeys.Count != 0;
        }
        internal static SpecialPressInfo Create(SpecialKeyType specialKey, byte idx)
        {
            return specialKey switch
            {
                SpecialKeyType.Air => new AirPressInfo(idx),
                SpecialKeyType.AirBound => new AirBoundPressInfo(idx),
                SpecialKeyType.Ground => new GroundPressInfo(idx),
                SpecialKeyType.GroundBound => new GroundBoundPressInfo(idx),
                SpecialKeyType.Fever => new FeverPressInfo(idx),
                SpecialKeyType.FeverBound => new FeverPressInfo(idx),
                SpecialKeyType.AnyInput => new AnyInputPressInfo(),
                SpecialKeyType.AnyKeyboard => new AnyKeyboardPressInfo(),
                SpecialKeyType.AnyController => new AnyControllerPressInfo(),
                SpecialKeyType.AnyMouse => new AnyMousePressInfo(),
                SpecialKeyType.AnyNoMouse => new AnyNoMousePressInfo(),
                _ => throw new ArgumentException($"unhandled enum value ({(int)specialKey})", nameof(specialKey)),
            };
        }
    }
    public enum KeyState
    {
        None,
        Press,
        Hold,
        Release
    }
    public abstract class BasePressInfo
    {
        public abstract string ToString(bool forceOrder);
        public sealed override string ToString()
        {
            return ToString(false);
        }
        public KeyState State { get; protected set; }
        public bool IsPressed => State == KeyState.Press || State == KeyState.Hold;
        public float PressStartTime { get; protected set; }
        internal BasePressInfo()
        {
            State = KeyState.None;
            PressStartTime = -1;
        }
        protected abstract bool PressCondition();
        internal void Update()
        {
            if (PressCondition())
            {
                switch (State)
                {
                    case KeyState.None:
                    case KeyState.Release:
                        State = KeyState.Press;
                        PressStartTime = Time.realtimeSinceStartup;
                        break;
                    case KeyState.Press:
                        State = KeyState.Hold;
                        break;
                    case KeyState.Hold:
                    default:
                        break;
                }
                return;
            }

            if (State == KeyState.Release)
            {
                PressStartTime = -1;
                State = KeyState.None;
            }
            else if (State != KeyState.None)
            {
                State = KeyState.Release;
            }

        }
    }
    public class PressInfo : BasePressInfo
    {
        public override string ToString(bool forceOrder)
        {
            return Utils.GetKeybindString(forceOrder, Key);
        }
        public virtual KeyCode Key { get; }
        internal PressInfo(KeyCode key) : base()
        {
            Key = key;
        }
        protected override bool PressCondition()
        {
            return Input.GetKey(Key);
        }
    }
    public class NullPressInfo : PressInfo
    {
        protected override bool PressCondition()
        {
            return false;
        }
        internal NullPressInfo() : base(KeyCode.None)
        {

        }
    }
}