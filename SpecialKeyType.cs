namespace KeybindManager
{
    public enum SpecialKeyType
    {
        /// <summary>
        /// Represents the N-th air keybind, regardless of whether it is bound or not.
        /// 0 represents any air keybind.
        /// </summary>
        Air = 1000,
        /// <summary>
        /// Represents the N-th air keybind that is actually bound.
        /// 0 represents any air keybind.
        /// </summary>
        AirBound = 1000 + (byte.MaxValue + 1),
        /// <summary>
        /// Represents the N-th ground keybind, regardless of whether it is bound or not.
        /// 0 represents any ground keybind.
        /// </summary>
        Ground = 1000 + (byte.MaxValue + 1) * 2,
        /// <summary>
        /// Represents the N-th ground keybind that is actually bound.
        /// 0 represents any ground keybind.
        /// </summary>
        GroundBound = 1000 + (byte.MaxValue + 1) * 3,
        /// <summary>
        /// Represents the N-th fever keybind, regardless of whether it is bound or not.
        /// 0 represents any fever keybind.
        /// </summary>
        Fever = 1000 + (byte.MaxValue + 1) * 4,
        /// <summary>
        /// Represents the N-th fever keybind that is actually bound.
        /// 0 represents any fever keybind.
        /// </summary>
        FeverBound = 1000 + (byte.MaxValue + 1) * 5,
        /// <summary>
        /// Represents any input.
        /// </summary>
        AnyInput = 1000000,
        /// <summary>
        /// Represents any keyboard input.
        /// </summary>
        AnyKeyboard = 1000001,
        /// <summary>
        /// Represents any controller input.
        /// </summary>
        AnyController = 1000002,
        /// <summary>
        /// Represents any mouse input.
        /// </summary>
        AnyMouse = 1000003,
        /// <summary>
        /// Represents any input, except mouse inputs.
        /// </summary>
        AnyNoMouse = 1000004,
    }

}
