using System.Diagnostics.CodeAnalysis;

namespace KeybindManager
{
    public sealed class StringParser : AbstractStringParser
    {
        private string _value;
        private string _default;

        public override string Value
        {
            get
            {
                return _value;
            }
            [MemberNotNull(nameof(_value))]
            set
            {
                _value = value ?? throw new ArgumentNullException(nameof(value));
                Reload();
            }
        }
        public override string DefaultValue
        {
            get
            {
                return _default;
            }
            [MemberNotNull(nameof(_default))]
            set
            {
                _default = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
        public StringParser(string defaultValue)
        {
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        public StringParser(string value, string defaultValue)
        {
            DefaultValue = defaultValue;
            Value = value;
        }
    }

}
