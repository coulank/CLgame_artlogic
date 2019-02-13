namespace Coulank.Controller
{
    public enum EButtonMode
    {
        Press = 1,
        Repeat = 2,
        Down = 4,
        Up = 8,
        Delay = 16,
        DelayRepeat = Delay | Repeat,
        DelayDown = Delay | Down,
        DelayUp = Delay | Up,
        Double = 32,
        Click = 64,
        DoubleClick = Double | Down,
        DoubleUp = Double | Up,
        None = 0
    }
    /// <summary>Set ConType</summary>
    public enum EConType
    {
        Default = 0,
        Xinput = 1,
        Direct = 2,
        Switch = 3,
        Android = 4,
        Other = 255
    }
    /// <summary>KeyInput or JoyAxis</summary>
    public enum EKeyType
    {
        Key = 1,
        JoyKey = 2,
        Axis = 4,
        JoyAxis = 8,
        Other = 0
    }
    /// <summary>
    /// 方向と動かすのに使うベクトルが入っている
    /// </summary>
    public enum EPosType
    {
        Left = 1,
        Right = 2,
        Center = 4,
        Move = 16,
        Rot = 32,
        None = 0
    }
    public enum EButtonNum
    {
        UP = 0x1,
        DOWN = 0x2,
        LEFT = 0x4,
        RIGHT = 0x8,
        A = 0x10,
        B = 0x20,
        X = 0x40,
        Y = 0x80,
        L = 0x100,
        R = 0x200,
        ZL = 0x400,
        ZR = 0x800,
        PLUS = 0x1000,
        MINUS = 0x2000,
        STARTMENU = 0x3000,
        PUSHSL = 0x4000,
        PUSHSR = 0x8000,
        PUSHSTICK = 0xC000,
        R_UP = 0x10000,
        R_DOWN = 0x20000,
        R_LEFT = 0x40000,
        R_RIGHT = 0x80000,
        CTRL = 0x100000,
        ALT = 0x200000,
        SHIFT = 0x400000,
        F1 = 0x1000000,
        F2 = 0x2000000,
        F3 = 0x3000000,
        F4 = 0x4000000,
        F5 = 0x5000000,
        F6 = 0x6000000,
        F7 = 0x7000000,
        F8 = 0x8000000,
        F9 = 0x9000000,
        F10 = 0xA000000,
        F11 = 0xB000000,
        F12 = 0xC000000,
        PRSCR = 0xD000000,
        HOME = 0xE000000,
        ESC = 0xF000000,
        ARROW = UP | DOWN | LEFT | RIGHT,
        R_ARROW = R_UP | R_DOWN | R_LEFT | R_RIGHT,
        ANY_ARROW = ARROW | R_ARROW,
        NONE = 0,
        ANY = ~0,
        BITJADGE = 0xFFFFFF
    }
    public enum EClickButton
    {
        left, right, middle, none
    }
    public enum EButtonSwitch
    {
        Normal = 1,
        Toggle = 2,
        Click = 4
    }
}
