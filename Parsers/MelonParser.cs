using MelonLoader;
using System.Diagnostics.CodeAnalysis;

namespace KeybindManager
{
    public sealed class MelonParser : AbstractStringParser, ISafeDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            _entry.OnEntryValueChanged.Unsubscribe(ReloadEvent);
            _entry = null!;
            IsDisposed = true;
        }

        private MelonPreferences_Entry<string> _entry;

        public MelonPreferences_Entry<string> Entry
        {
            get
            {
                return _entry;
            }
            [MemberNotNull(nameof(_entry))]
            set
            {
                _entry = value ?? throw new ArgumentNullException(nameof(value));
                Reload();
            }
        }

        public override string Value
        {
            get
            {
                return _entry.Value;
            }
            set
            {
                _entry.Value = value;
                Reload();
            }
        }
        public override string DefaultValue
        {
            get
            {
                return _entry.DefaultValue;
            }
            set
            {
                _entry.DefaultValue = value;
            }
        }

        public MelonParser(MelonPreferences_Entry<string> entry)
        {
            Entry = entry;
            entry.OnEntryValueChanged.Subscribe(ReloadEvent);
        }
        private void ReloadEvent(string oldValue, string newValue)
        {
            Reload();
        }

    }
}
