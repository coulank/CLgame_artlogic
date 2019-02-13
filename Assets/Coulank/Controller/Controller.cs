/// <summary>
/// コントローラーなどのデバイスを管理するクラス
/// 必ずStart関数以降の実行にすること
/// 依存関係はComp.csとVecComp.csは必須
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using Coulank.Position;

namespace Coulank.Controller
{
    /// <summary>
    /// ボタンクラス、ビット形式にしたものをDictionary配列に格納する
    /// </summary>
    public class ButtonObj : Dictionary<EButtonMode, EButtonNum>
    {
        // Key Controler
        public ButtonObj()
        {
            foreach (EButtonMode Type in Enum.GetValues(typeof(EButtonMode))) Add(Type, 0);
        }

        /// <summary>調べたいボタンを調べる</summary>
        /// <param name="judge_button">調べたいボタンの値</param>
        /// <param name="press_button">現在のボタンの値</param>
        /// <param name="AndMode">Trueなら調べたいボタンが全て押されていればTrueを返し、
        /// Falseならどれか一つだけでも押されていればTrueを返す</param>
        static public bool Judge(EButtonNum judge_button, EButtonNum press_button, bool AndMode = false)
        {
            if (judge_button == EButtonNum.NONE) return false;
            // まずBit判定とEqual判定に分離する
            EButtonNum judge_high = judge_button & ~EButtonNum.BITJADGE;
            judge_button &= EButtonNum.BITJADGE;
            EButtonNum press_high = press_button & ~EButtonNum.BITJADGE;
            press_button &= EButtonNum.BITJADGE;
            // Equal判定
            bool judge = AndMode;
            if (judge_high != EButtonNum.NONE) judge = (judge_high == press_high);

            // Bit判定を行って、Equalと結合する
            if (AndMode)
            {
                return judge && ((press_button & judge_button) == judge_button);
            }
            else
            {
                return judge || ((press_button & judge_button) != EButtonNum.NONE);
            }
        }
        /// <summary>調べたいボタンを調べる</summary>
        /// <param name="judge_button">調べたいボタンの値</param>
        /// <param name="btype">調べたいボタンのモード</param>
        /// <param name="AndMode">Trueなら調べたいボタンが全て押されていればTrueを返し、
        /// Falseならどれか一つだけでも押されていればTrueを返す</param>
        public bool Judge(EButtonNum judge_button, 
            EButtonMode btype = EButtonMode.Press, bool AndMode = false)
        {
            return Judge(judge_button, this[btype], AndMode);
        }
        /// <summary>押しっぱなしのときに発生</summary>
        public EButtonNum Press { get { return this[EButtonMode.Press]; } }
        /// <summary>押しっぱなしのときに断続的に発生</summary>
        public EButtonNum Repeat { get { return this[EButtonMode.Repeat]; } }
        /// <summary>押した瞬間だけ発生</summary>
        public EButtonNum Down { get { return this[EButtonMode.Down]; } }
        /// <summary>離した瞬間だけ発生</summary>
        public EButtonNum Up { get { return this[EButtonMode.Up]; } }
        /// <summary>長押し + 押しっぱなしのときに発生</summary>
        public EButtonNum Delay { get { return this[EButtonMode.Press]; } }
        /// <summary>長押し + 押しっぱなしのときに断続的に発生</summary>
        public EButtonNum DelayRepeat { get { return this[EButtonMode.Repeat]; } }
        /// <summary>長押し + 押した瞬間だけ発生</summary>
        public EButtonNum DelayDown { get { return this[EButtonMode.Down]; } }
        /// <summary>長押し + 離した瞬間だけ発生</summary>
        public EButtonNum DelayUp { get { return this[EButtonMode.Up]; } }
        /// <summary>ダブルクリック + 押し続けているときに発生</summary>
        public EButtonNum Double { get { return this[EButtonMode.Double]; } }
        /// <summary>ダブルクリック + 押した瞬間に発生</summary>
        public EButtonNum DoubleClick { get { return this[EButtonMode.DoubleClick]; } }
        /// <summary>ダブルクリック + 離した瞬間に発生</summary>
        public EButtonNum DoubleDown { get { return this[EButtonMode.DoubleUp]; } }

        /// <summary>押されたボタンの一覧を文字列で出力する</summary>
        static public string ResultButton(EButtonNum btn)
        {
            bool boolOutput;
            EButtonNum highbtn = btn & ~EButtonNum.BITJADGE;
            btn &= EButtonNum.BITJADGE;
            List<string> OutputStrList = new List<string>();
            foreach (EButtonNum ibtn in Controller.BTNNUMLIST)
            {
                if ((ibtn & EButtonNum.BITJADGE) != EButtonNum.NONE)
                {
                    boolOutput = ((btn & ibtn) != EButtonNum.NONE);
                }
                else
                {
                    boolOutput = (highbtn == ibtn);
                }
                if (boolOutput) OutputStrList.Add(Controller.BTNSTRDIC[ibtn]);
            }
            return string.Join(",", OutputStrList);
        }

        public string ToString(EButtonMode buttonMode = EButtonMode.Press)
        {
            return ResultButton(this[buttonMode]);
        }
    }
    /// <summary>スティック入力クラス</summary>
    public class StickObj : Dictionary<EPosType, Vector2>
    {
        public void SetMost(EPosType pos, Vector2 addvc2, float magnitude = 1f)
        {
            if (ContainsKey(pos))
            {
                this[pos] = VecComp.AbsMax(this[pos], addvc2) * magnitude;
            }
        }
        public void PosAdd(EPosType pos, Vector2 vc2 = new Vector2())
        {
            Add(pos, new Vector2());
        }
        /// <summary>左スティック</summary>
        public Vector2 Left { get { return this[EPosType.Left]; } }
        /// <summary>右スティック</summary>
        public Vector2 Right { get { return this[EPosType.Right]; } }
        /// <summary>ホイールボタンスクロール</summary>
        public Vector2 Center { get { return this[EPosType.Center]; } }
        /// <summary>左スティックなどから移動に定義されたベクトル</summary>
        public Vector2 Move { get { return this[EPosType.Move]; } }
        /// <summary>右スティックなどからカメラに定義されたベクトル</summary>
        public Vector2 Rot { get { return this[EPosType.Rot]; } }
        public void PosClear()
        {
            Vector2 vc2;
            foreach (EPosType key in new List<EPosType>(Keys))
            {
                vc2 = this[key]; vc2.Set(0f, 0f); this[key] = vc2;
            }
        }

        public StickObj()
        {
            PosAdd(EPosType.Left);
            PosAdd(EPosType.Right);
            PosAdd(EPosType.Center);
            PosAdd(EPosType.Move);
            PosAdd(EPosType.Rot);
        }
    }

    [Serializable]
    public class ConTemplate
    {
        public EKeyType keytype;
        public string keyname;
        public EButtonNum button;
        public bool reverse;
        public float dead;
        public EPosType posType;
        public EAxis pntType;
        public float num = 0;
        public KeyCode key = KeyCode.Space;

        public ConTemplate(EKeyType _keytype = EKeyType.Key, EButtonNum _button = 0, float _num = 1, string _keyname = "", bool _reverse = false, float _dead = 0.1f,
            EPosType _pos = EPosType.Left, EAxis _pnt = EAxis.X)
        {
            keytype = _keytype;
            if (_keyname == "")
            {
                switch (keytype)
                {
                    case EKeyType.JoyKey:
                        _keyname = "button " + _num.ToString();
                        break;
                    case EKeyType.Axis:
                    case EKeyType.JoyAxis:
                        _keyname = "axis " + _num.ToString();
                        break;
                    case EKeyType.Key:
                        key = (KeyCode)_num;
                        break;
                    default:
                        break;
                }
            }
            num = _num;
            keyname = _keyname;
            button = _button;
            reverse = _reverse;
            dead = _dead;
            posType = _pos;
            pntType = _pnt;
        }
    }


    /// <summary>
    /// コントローラーのボタンやスティックなどの単体オブジェクト
    /// JoyAxisはjoystick axis 1 ～ joystick axis 10 を取得するように固定しています
    /// よってJoystickAxis.prisetで動作します。
    /// コントローラー親を第一引数にする
    /// </summary>
    public class ConObj : ConTemplate
    {
        public static List<string> joysticks = new List<string>
{
    "joystick ", "joystick1 ", "joystick2 ", "joystick3 ", "joystick4 "
};
        public Controller parent;

        public ConObj(Controller _controller, EKeyType _keytype = EKeyType.Key, EButtonNum _button = 0, float _num = 1, string _keyname = "", bool _reverse = false, float _dead = 0.1f,
            EPosType _pos = EPosType.Left, EAxis _pnt = EAxis.X)
            : base(_keytype, _button, _num, _keyname, _reverse, _dead, _pos, _pnt)
        {
            parent = _controller;
        }
        public ConObj(Controller _controller, ConTemplate _contemp)
            : this(_controller, _contemp.keytype, _contemp.button, 0, _contemp.keyname, _contemp.reverse, _contemp.dead, _contemp.posType, _contemp.pntType) { }

        public EButtonNum UpdateButton(bool KeyToCon)
        {
            EButtonNum retval = 0;
            float axisval = 0f;
            bool keypush = false;
            Vector3 vc3;
            switch (keytype)
            {
                case EKeyType.Axis:
                case EKeyType.JoyAxis:
                    if (parent.JoystickID < 0) break;
                    axisval = Input.GetAxisRaw(((keytype == EKeyType.JoyAxis) ? joysticks[parent.JoystickID] : "") + keyname);
                    if (reverse) axisval *= -1;
                    EPosType LocalPosType;
                    if ((posType & EPosType.Left) == EPosType.Left)
                    {
                        LocalPosType = EPosType.Left;
                    }
                    else if ((posType & EPosType.Right) == EPosType.Right)
                    {
                        LocalPosType = EPosType.Right;
                    }
                    else
                    {
                        LocalPosType = EPosType.Center;
                    }
                    // 座標が最大じゃないコントローラー向けに補正
                    if (parent.Property.CompFlag) axisval = Controller.CompObj.DoComp(axisval, parent.Sys_ConType, LocalPosType, pntType);
                    if (axisval > dead) retval = button;
                    vc3 = parent.Stick[LocalPosType];
                    switch (pntType)
                    {
                        case EAxis.X:
                            vc3.x = axisval;
                            break;
                        case EAxis.Y:
                            vc3.y = axisval;
                            break;
                        case EAxis.Z:
                            vc3.z = axisval;
                            break;
                    }

                    parent.Stick.SetMost(posType, vc3); ;
                    if ((posType & EPosType.Move) == EPosType.Move) parent.Stick.SetMost(EPosType.Move, vc3);
                    if ((posType & EPosType.Rot) == EPosType.Rot) parent.Stick.SetMost(EPosType.Rot, vc3);
                    break;
                case EKeyType.Key:
                    if (KeyToCon)
                    {
                        if ((parent.JoystickID > 0) && !parent.Property.MultPlayKeyboard) break;
                        if (keyname != "")
                            keypush = Input.GetKey(keyname);
                        else
                            keypush = Input.GetKey(key);
                        if (keypush) retval = button;
                    }
                    break;
                case EKeyType.JoyKey:
                    if (parent.JoystickID < 0) break;
                    keypush = Input.GetKey(joysticks[parent.JoystickID] + keyname);
                    if (keypush) retval = button;
                    break;
                default:
                    if (keyname != "")
                        keypush = Input.GetKey(keyname);
                    else
                        keypush = Input.GetKey(key);
                    if (keypush) retval = button;
                    break;
            }
            return retval;
        }
    }

    [Serializable]
    /// <summary>リピートクラス、単位は秒、小数点以下で指定すること</summary>
    public class KeyRepeatClass
    {
        public bool lock_enable = false;
        public bool enable = false;
        public bool started = false;
        public bool first = false;
        public bool last = false;
        public float pushing;
        public float lock_start;
        public float start;
        public float interval;
        public bool double_enable = false;
        public bool double_press = false;
        public float double_time;
        public float double_latency;
        public KeyRepeatClass(float _lock_start = 0f, float _start = 0.4f,
            float _interval = 0.2f, float _double_latency = 1f)
        {
            lock_start = _lock_start; start = _start; interval = _interval; double_latency = _double_latency;
            double_time = double_latency + 1;
        }
        public bool Check(bool press)
        {
            bool RetBool = false;
            first = false; last = false;
            if (double_time < double_latency) double_time += Time.deltaTime;
            if (press)
            {
                if (enable)
                {
                    pushing += Time.deltaTime;
                    if (started)
                    {
                        if ((interval > 0f) && (pushing > interval))
                        {
                            RetBool = true; // ButtonMode.Repeat
                            pushing = 0f;
                        }
                    }
                    else
                    {
                        if ((start > 0f) && (pushing > start))
                        {
                            RetBool = true;
                            started = true;
                            pushing = 0f;
                        }
                    }
                }
                else
                {
                    if (lock_enable)
                    {
                        pushing += Time.deltaTime;
                    }
                    else
                    {
                        lock_enable = true;
                        pushing = 0f;
                    }
                    if (pushing >= lock_start)
                    {
                        first = true;       // ButtonMode.Down
                        RetBool = true;
                        enable = true;
                        started = false;
                        double_press = (double_time <= double_latency);
                        if (double_press)
                            double_time = double_latency + 1f;
                        else
                            double_time = 0f;
                    }
                }

            }
            else
            {
                if (enable)
                {
                    if (double_time <= double_latency)
                        double_time = 0f;
                    else
                        double_time = double_latency + 1f;
                    last = true;    // ButtonMode.Up
                    started = false;
                    enable = false;
                    lock_enable = false;
                }
                else
                {
                    double_press = false;
                }
            }
            return RetBool;
        }
    }

    /// <summary>キーリピート配列を予め生成する</summary>
    public class KeyRepeatDict : Dictionary<EButtonNum, KeyRepeatClass>
    {
        public KeyRepeatDict(float lock_start = 0f)
        {
            foreach (EButtonNum con in Controller.BTNNUMLIST)
            {
                Add(con, new KeyRepeatClass(lock_start));
            }
        }
    }

    public class TouchPhaseCount : Dictionary<TouchPhase, int>
    {
        public static int PhaseCount(Touch[] touches, TouchPhase touchPhase, bool notequal = false)
        {
            int count = 0;
            foreach (Touch touch in touches)
            {
                if (notequal ^ (touch.phase == touchPhase)) count++;
            }
            return count;
        }
        public static int GetNotEndedCount(Touch[] touches)
        {
            int notEnded = PhaseCount(touches, TouchPhase.Ended, true);
            notEnded -= PhaseCount(touches, TouchPhase.Canceled);
            return notEnded;
        }
        public TouchPhaseCount() { }
        public TouchPhaseCount(Touch[] touches)
        {
            foreach (TouchPhase touchPhase in Enum.GetValues(typeof(TouchPhase)))
            {
                Add(touchPhase, PhaseCount(touches, touchPhase));
            }
        }
    }

    /// <summary>コントローラー設定のテンプレートクラス</summary>
    public class ConTempSet : List<ConTemplate>
    {
        /// <summary>テンプレート配列から生成する、システム用</summary>
        public static List<ConObj> OutUseList(Controller parentCon, ConTempSet templates)
        {
            List<ConObj> conList = new List<ConObj>();
            foreach (ConTemplate template in templates)
            {
                conList.Add(new ConObj(parentCon, template));
            }
            return conList;
        }
        /// <summary>コピペ用</summary>
        public static ConTempSet Template = new ConTempSet()
{
    new ConTemplate(),
};

        /// <summary>最初に読み込むユーザ設定</summary>
        public static ConTempSet UserFirstTemplate = new ConTempSet();
        /// <summary>最後に読み込むユーザ設定、こちらは優先度高い</summary>
        public static ConTempSet UserLastTemplate = new ConTempSet();

        /// <summary>コントローラの共通登録</summary>
        public static ConTempSet CommonTemplate = new ConTempSet()
{
    new ConTemplate(EKeyType.JoyKey, EButtonNum.L, 4),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.R, 5),
};
        /// <summary>キーボードのシステム部分登録</summary>
        public static ConTempSet SysTemplate = new ConTempSet()
{
    new ConTemplate(EKeyType.Key, EButtonNum.UP, 0, "up"),
    new ConTemplate(EKeyType.Key, EButtonNum.DOWN, 0, "down"),
    new ConTemplate(EKeyType.Key, EButtonNum.LEFT, 0, "left"),
    new ConTemplate(EKeyType.Key, EButtonNum.RIGHT, 0, "right"),

    new ConTemplate(EKeyType.Key, EButtonNum.A, 0, "return"),
    new ConTemplate(EKeyType.Key, EButtonNum.A, 0, "enter"),
    new ConTemplate(EKeyType.Key, EButtonNum.A, 0, "space"),
    new ConTemplate(EKeyType.Key, EButtonNum.B, 0, "backspace"),
    new ConTemplate(EKeyType.Key, EButtonNum.ESC, 0, "escape"),
    new ConTemplate(EKeyType.Key, EButtonNum.PRSCR, (float)KeyCode.Print),

    new ConTemplate(EKeyType.Axis, EButtonNum.CTRL, 0, "ctrl"),
    new ConTemplate(EKeyType.Axis, EButtonNum.ALT, 0, "alt"),
    new ConTemplate(EKeyType.Axis, EButtonNum.SHIFT, 0, "shift"),

    new ConTemplate(EKeyType.Key, EButtonNum.F1, 0, "f1"),
    new ConTemplate(EKeyType.Key, EButtonNum.F2, 0, "f2"),
    new ConTemplate(EKeyType.Key, EButtonNum.F3, 0, "f3"),
    new ConTemplate(EKeyType.Key, EButtonNum.F4, 0, "f4"),
    new ConTemplate(EKeyType.Key, EButtonNum.F5, 0, "f5"),
    new ConTemplate(EKeyType.Key, EButtonNum.F6, 0, "f6"),
    new ConTemplate(EKeyType.Key, EButtonNum.F7, 0, "f7"),
    new ConTemplate(EKeyType.Key, EButtonNum.F8, 0, "f8"),
    new ConTemplate(EKeyType.Key, EButtonNum.F9, 0, "f9"),
    new ConTemplate(EKeyType.Key, EButtonNum.F10, 0, "f10"),
    new ConTemplate(EKeyType.Key, EButtonNum.F11, 0, "f11"),
    new ConTemplate(EKeyType.Key, EButtonNum.F12, 0, "f12"),
};

        /// <summary>キーボード登録</summary>
        public static ConTempSet KeyboardTemplate = new ConTempSet()
{
    new ConTemplate(EKeyType.Key, EButtonNum.UP, 0, "w"),
    new ConTemplate(EKeyType.Key, EButtonNum.DOWN, 0, "s"),
    new ConTemplate(EKeyType.Key, EButtonNum.LEFT, 0, "a"),
    new ConTemplate(EKeyType.Key, EButtonNum.RIGHT, 0, "d"),

    new ConTemplate(EKeyType.Key, EButtonNum.R_UP, 0, "i"),
    new ConTemplate(EKeyType.Key, EButtonNum.R_DOWN, 0, "k"),
    new ConTemplate(EKeyType.Key, EButtonNum.R_LEFT, 0, "j"),
    new ConTemplate(EKeyType.Key, EButtonNum.R_RIGHT, 0, "l"),

    new ConTemplate(EKeyType.Key, EButtonNum.A, 0, "z"),
    new ConTemplate(EKeyType.Key, EButtonNum.B, 0, "x"),
    new ConTemplate(EKeyType.Key, EButtonNum.X, 0, "c"),
    new ConTemplate(EKeyType.Key, EButtonNum.Y, 0, "v"),
    new ConTemplate(EKeyType.Key, EButtonNum.L, 0, "q"),
    new ConTemplate(EKeyType.Key, EButtonNum.R, 0, "e"),
    new ConTemplate(EKeyType.Key, EButtonNum.ZL, 0, "1"),
    new ConTemplate(EKeyType.Key, EButtonNum.ZR, 0, "4"),
    new ConTemplate(EKeyType.Key, EButtonNum.PLUS, 0, "2"),
    new ConTemplate(EKeyType.Key, EButtonNum.MINUS, 0, "3"),
    new ConTemplate(EKeyType.Key, EButtonNum.PUSHSL, 0, "b"),
    new ConTemplate(EKeyType.Key, EButtonNum.PUSHSR, 0, "n"),
};

        /// <summary>Xinput用のテンプレ、読み込み専用</summary>
        public static ConTempSet XinputTemplate
        {
            get
            {
                return new ConTempSet()
        {
            new ConTemplate(EKeyType.JoyKey, EButtonNum.B, 0),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.A, 1),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.Y, 2),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.X, 3),
            new ConTemplate(EKeyType.JoyAxis, EButtonNum.UP, 7, _reverse: false, _pos: EPosType.Move, _pnt: EAxis.Y),
            new ConTemplate(EKeyType.JoyAxis, EButtonNum.DOWN, 7, _reverse: true, _pos: EPosType.Move, _pnt: EAxis.Y),
            new ConTemplate(EKeyType.JoyAxis, EButtonNum.RIGHT, 6, _reverse: false, _pos: EPosType.Move, _pnt: EAxis.X),
            new ConTemplate(EKeyType.JoyAxis, EButtonNum.LEFT, 6, _reverse: true, _pos: EPosType.Move, _pnt: EAxis.X),
            new ConTemplate(EKeyType.JoyAxis, EButtonNum.ZL, 9, _reverse: false),
            new ConTemplate(EKeyType.JoyAxis, EButtonNum.ZR, 10, _reverse: false),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.MINUS, 6),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.PLUS, 7),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.PUSHSL, 8),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.PUSHSR, 9),
            new ConTemplate(EKeyType.JoyAxis, _num: 1, _pos: EPosType.Left | EPosType.Move, _pnt: EAxis.X),
            new ConTemplate(EKeyType.JoyAxis, _num: 2, _pos: EPosType.Left | EPosType.Move, _pnt: EAxis.Y, _reverse: Controller.Yreverse),
            new ConTemplate(EKeyType.JoyAxis, _num: 4, _pos: EPosType.Right | EPosType.Rot, _pnt: EAxis.X),
            new ConTemplate(EKeyType.JoyAxis, _num: 5, _pos: EPosType.Right | EPosType.Rot, _pnt: EAxis.Y, _reverse: Controller.Yreverse),
        };
            }
        }

        /// <summary>DirectInput用テンプレ、読み込み専用</summary>
        public static ConTempSet DirectTemplate
        {
            get
            {
                return new ConTempSet()
        {
    new ConTemplate(EKeyType.JoyKey, EButtonNum.Y, 0),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.B, 1),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.A, 2),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.X, 3),
    new ConTemplate(EKeyType.JoyAxis, EButtonNum.UP, 6, _reverse: false, _pos: EPosType.Move, _pnt: EAxis.Y),
    new ConTemplate(EKeyType.JoyAxis, EButtonNum.DOWN, 6, _reverse: true, _pos: EPosType.Move, _pnt: EAxis.Y),
    new ConTemplate(EKeyType.JoyAxis, EButtonNum.RIGHT, 5, _reverse: false, _pos: EPosType.Move, _pnt: EAxis.X),
    new ConTemplate(EKeyType.JoyAxis, EButtonNum.LEFT, 5, _reverse: true, _pos: EPosType.Move, _pnt: EAxis.X),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.ZL, 6),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.ZR, 7),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.MINUS, 8),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.PLUS, 9),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.PUSHSL, 10),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.PUSHSR, 11),
    new ConTemplate(EKeyType.JoyAxis, _num: 3, _pos: EPosType.Left | EPosType.Move, _pnt: EAxis.X),
    new ConTemplate(EKeyType.JoyAxis, _num: 4, _pos: EPosType.Left | EPosType.Move, _pnt: EAxis.Y, _reverse: Controller.Yreverse),
    new ConTemplate(EKeyType.JoyAxis, _num: 1, _pos: EPosType.Right | EPosType.Rot, _pnt: EAxis.X),
    new ConTemplate(EKeyType.JoyAxis, _num: 2, _pos: EPosType.Right | EPosType.Rot, _pnt: EAxis.Y, _reverse: Controller.Yreverse),
        };
            }
        }

        /// <summary>Switchのコントローラ用テンプレ、読み込み専用</summary>
        public static ConTempSet SwitchTemplate
        {
            get
            {
                return new ConTempSet()
        {
            new ConTemplate(EKeyType.JoyKey, EButtonNum.B, 0),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.A, 1),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.Y, 2),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.X, 3),
            new ConTemplate(EKeyType.JoyAxis, EButtonNum.UP, 10, _reverse: false, _pos: EPosType.Move, _pnt: EAxis.Y),
            new ConTemplate(EKeyType.JoyAxis, EButtonNum.DOWN, 10, _reverse: true, _pos: EPosType.Move, _pnt: EAxis.Y),
            new ConTemplate(EKeyType.JoyAxis, EButtonNum.RIGHT, 9, _reverse: false, _pos: EPosType.Move, _pnt: EAxis.X),
            new ConTemplate(EKeyType.JoyAxis, EButtonNum.LEFT, 9, _reverse: true, _pos: EPosType.Move, _pnt: EAxis.X),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.ZL, 6),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.ZR, 7),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.ZL, 14),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.ZR, 15),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.MINUS, 8),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.PLUS, 9),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.PUSHSL, 10),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.PUSHSR, 11),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.HOME, 12),
            new ConTemplate(EKeyType.JoyKey, EButtonNum.PRSCR, 13),
            new ConTemplate(EKeyType.JoyAxis, _num: 2, _pos: EPosType.Left | EPosType.Move, _pnt: EAxis.X),
            new ConTemplate(EKeyType.JoyAxis, _num: 4, _pos: EPosType.Left | EPosType.Move, _pnt: EAxis.Y, _reverse: Controller.Yreverse),
            new ConTemplate(EKeyType.JoyAxis, _num: 7, _pos: EPosType.Right | EPosType.Rot, _pnt: EAxis.X),
            new ConTemplate(EKeyType.JoyAxis, _num: 8, _pos: EPosType.Right | EPosType.Rot, _pnt: EAxis.Y, _reverse: Controller.Yreverse),
        };
            }
        }
        public static ConTempSet AndroidTemplate
        {
            get
            {
                return new ConTempSet()
            {
    new ConTemplate(EKeyType.JoyKey, EButtonNum.B, 0),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.A, 1),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.Y, 2),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.X, 3),
    new ConTemplate(EKeyType.JoyAxis, EButtonNum.UP, 6, _reverse: true, _pos: EPosType.Move, _pnt: EAxis.Y),
    new ConTemplate(EKeyType.JoyAxis, EButtonNum.DOWN, 6, _reverse: false, _pos: EPosType.Move, _pnt: EAxis.Y),
    new ConTemplate(EKeyType.JoyAxis, EButtonNum.RIGHT, 5, _reverse: false, _pos: EPosType.Move, _pnt: EAxis.X),
    new ConTemplate(EKeyType.JoyAxis, EButtonNum.LEFT, 5, _reverse: true, _pos: EPosType.Move, _pnt: EAxis.X),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.PLUS, 10),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.PUSHSL, 8),
    new ConTemplate(EKeyType.JoyKey, EButtonNum.PUSHSR, 9),
    new ConTemplate(EKeyType.JoyAxis, _num: 1, _pos: EPosType.Left | EPosType.Move, _pnt: EAxis.X),
    new ConTemplate(EKeyType.JoyAxis, _num: 2, _pos: EPosType.Left | EPosType.Move, _pnt: EAxis.Y, _reverse: Controller.Yreverse),
    new ConTemplate(EKeyType.JoyAxis, _num: 3, _pos: EPosType.Right | EPosType.Rot, _pnt: EAxis.X),
    new ConTemplate(EKeyType.JoyAxis, _num: 4, _pos: EPosType.Right | EPosType.Rot, _pnt: EAxis.Y, _reverse: Controller.Yreverse),
            };
            }
        }
    }

    public class Controller : MonoBehaviour
    {
        public List<ConObj> m_controller = new List<ConObj>();
        public ControllerEvents m_globalControllerEvents = new ControllerEvents();

        static protected List<EButtonNum> GetListButtonType()
        {
            var list = new List<EButtonNum>();
            int bitjadge = (int)EButtonNum.BITJADGE;
            foreach (EButtonNum value in Enum.GetValues(typeof(EButtonNum)))
            {
                switch (value)
                {
                    case EButtonNum.BITJADGE:
                    case EButtonNum.ANY:
                        continue;
                }
                if ((value & ~EButtonNum.BITJADGE) == 0)
                {

                    if ((bitjadge & (int)value) != 0)
                    {
                        list.Add(value);
                        bitjadge &= ~(int)value;
                    }
                }
                else
                {
                    list.Add(value);
                }
            }
            return list;
        }
        /// <summary>
        /// Enumを文字列化、中身が重複してるケースを一意にするために生成
        /// </summary>
        static protected Dictionary<EButtonNum, string> GetDicButtonType()
        {
            var dic = new Dictionary<EButtonNum, string>
        {
            { EButtonNum.A, "A" }, { EButtonNum.B, "B" },
            { EButtonNum.X, "X" }, { EButtonNum.Y, "Y" },
            { EButtonNum.PLUS, "PLUS" }, { EButtonNum.MINUS, "MINUS" },
        };
            foreach (EButtonNum value in Enum.GetValues(typeof(EButtonNum)))
            {
                if (!dic.ContainsKey(value))
                {
                    dic.Add(value, value.ToString());
                }
            }
            return dic;
        }
        /// <summary>定数配列、登録してるボタンは以下の通り、Switchのコントローラーが基準になります</summary>
        static public List<EButtonNum> BTNNUMLIST = GetListButtonType();
        static public Dictionary<EButtonNum, string> BTNSTRDIC = GetDicButtonType();

        /// <summary>コントローラーが現在アクティブなのかどうか、これがFalseならUpdateは初期化だけになる</summary>
        [NonSerialized] public bool Active = true;
        /// <summary>スティックのYの正負を入れ替えるかどうか、デフォで入れ替える</summary>
        static public bool Yreverse = true;

        /// <summary>タッチされているか</summary>
        public bool TouchedPress { get; private set; }
        /// <summary>全体的にクリックされたか</summary>
        public bool TouchedDown { get; private set; }
        /// <summary>全体的にクリック終了したかどうか</summary>
        public bool TouchedUp { get; private set; }
        /// <summary>指で押されているかどうかのフラグ</summary>
        public bool TouchedTapMode { get; private set; }
        /// <summary>全体的なスワイプ判定</summary>
        public bool TouchedSwipeMode { get; private set; } = false;

        protected const int USE_TOUCHESCOUNT = 3;

        /// <summary>クリックした座標</summary>
        [SerializeField]
        public Vector2[] TouchesPosition { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>前回クリックした座標</summary>
        public Vector2[] TouchesBeforePosition { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>TouchesCursorの最初と現在の間のベクトル</summary>
        public Vector2[] TouchesVector { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>TouchesVectorの差分ベクトル</summary>
        public Vector2[] TouchesDeltaVector { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>前回のTouchesVector、確認用とかベクトル決めるときとかに使う</summary>
        public Vector2[] TouchesBeforeVector { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>指の本数ごとの入力値、とりあえず3つまで</summary>
        public bool[] TouchesPress { get; private set; } = new bool[USE_TOUCHESCOUNT];
        public bool[] TouchesDown { get; private set; } = new bool[USE_TOUCHESCOUNT];
        public bool[] TouchesUp { get; private set; } = new bool[USE_TOUCHESCOUNT];
        public bool[] TouchesDouble { get; private set; } = new bool[USE_TOUCHESCOUNT];
        public Vector2[] TouchesDownPosition { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        public Vector2[] TouchesUpPosition { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>マルチタッチのときの座標の基準</summary>
        public List<int> TouchesFocusID { get; private set; } = new List<int>(3);
        /// <summary>スワイプ判定</summary>
        public bool[] TouchesSwipeMode { get; private set; } = new bool[USE_TOUCHESCOUNT];
        /// <summary>スクリーンをタップしていた時間</summary>
        public float[] TouchesTime { get; private set; } = new float[USE_TOUCHESCOUNT];
        public Dictionary<int, bool> FingerLock { get; private set; } = new Dictionary<int, bool>();

        /// <summary>前回のスクロール量、Scrollで差分になる調整に使う</summary>
        public float BeforeScroll { get; private set; } = 0f;
        /// <summary>スクロール量、ズームとかに使う</summary>
        public float Scroll { get; private set; } = 0f;

        [Serializable]
        public struct InputInspector
        {
            public int PressButtonInt;
            public string PressButton;
            public Vector2 TouchPosition, TouchVector;
            public Vector2 MoveStick, RotStick;
            public Vector2 LeftStick, RightStick, neko;
        }
        [SerializeField] public InputInspector m_inputInspector = new InputInspector();

        /// <summary>
        /// ここで指定するプロパティは動的に複数管理できることを想定
        /// </summary>
        public PropertyClass Property { get { return m_property; } set { m_property = value; } }
        [SerializeField] private PropertyClass m_property = new PropertyClass();
        /// <summary>
        /// プロパティ、まとめて設定を管理できる
        /// </summary>
        [Serializable]
        public class PropertyClass
        {
            /// <summary>名前、付けなくても良い</summary>
            public string Name;
            /// <summary>ZLZRとLRを入れ替えるかどうか</summary>
            public bool ButtonZLR_Reverse = false;
            /// <summary>キーボードをコントローラーにするかどうか</summary>
            public bool ConKeyboard = true;
            /// <summary>joystickIDが0よりも大きいときにキーボードコントローラ有効にするか</summary>
            public bool MultPlayKeyboard = false;
            /// <summary>スティックの補正を行うかのフラグ</summary>
            public bool CompFlag = true;
            /// <summary>TouchDeltaVectorの倍率</summary>
            public float TouchDeltaMagnitude = 5f;
            /// <summary>タッチの際に取得したRotの反転、倍率の補正</summary>
            public Vector2 TouchRotReverseComp = new Vector2(1, 1);
            public bool TouchRotReverseX { get { return TouchRotReverseComp.x < 0; } set { TouchRotReverseComp.x = value ? -1 : 1; } }
            public bool TouchRotReverseY { get { return TouchRotReverseComp.y < 0; } set { TouchRotReverseComp.y = value ? -1 : 1; } }
            /// <summary>タッチの際にRotを取る対象で値を反転させるかどうか</summary>
            public Vector2 RotReverseComp = new Vector2(1, -1);
            public bool RotReverseX { get { return RotReverseComp.x < 0; } set { RotReverseComp.x = value ? -1 : 1; } }
            public bool RotReverseY { get { return RotReverseComp.y < 0; } set { RotReverseComp.y = value ? -1 : 1; } }

            /// <summary>スクリーン全体をタップしたときのボタン発生を有効にするか</summary>
            public bool TouchButtonFlag = true;
            /// <summary>タップしたときのボタンアクション</summary>
            [Tooltip("タップしたときのボタンアクション")]
            public EButtonNum[] TouchesButtonName = new EButtonNum[USE_TOUCHESCOUNT]
                { EButtonNum.A, EButtonNum.NONE, EButtonNum.NONE };
            /// <summary>マウスクリックのボタンアクション</summary>
            [Tooltip("マウスクリックのボタンアクション")]
            public EButtonNum[] MouseButtonName = new EButtonNum[3]
                { EButtonNum.A, EButtonNum.X, EButtonNum.NONE };
            /// <summary>
            /// タップしたときのボタンの挙動、Noneは動作無し
            /// スワイプモードになったとき、Clickは無効になる仕様です（一般的なタップになる）
            /// </summary>
            [Tooltip("タップしたときのボタンの挙動、Noneは動作無し")]
            public EButtonMode[] TouchesButtonMode = new EButtonMode[USE_TOUCHESCOUNT]
                { EButtonMode.Click, EButtonMode.Press, EButtonMode.Press };
            /// <summary>
            /// マウスクリックしたときのボタンの挙動、Noneは動作無し
            /// TouchesButtonModeと同じ、左右中央の順になります
            /// </summary>
            [Tooltip("マウスボタンの挙動、左右中の順")]
            public EButtonMode[] MouseButtonMode = new EButtonMode[3]
                { EButtonMode.Click, EButtonMode.Click, EButtonMode.Press };
            /// <summary>指に対しての動かす対象</summary>
            [Tooltip("指に対しての動かす対象")]
            public EPosType[] TouchVectorMode = new EPosType[USE_TOUCHESCOUNT]
            { EPosType.Move, EPosType.Rot, EPosType.Center};
            /// <summary>マウスに対しての動かす対象</summary>
            [Tooltip("マウスに対しての動かす対象")]
            public EPosType[] MouseVectorMode = new EPosType[3]
            { EPosType.Move, EPosType.Rot, EPosType.Center};

            /// <summary>矢印キーを移動ベクトルにするか</summary>
            [Tooltip("矢印キーを移動ベクトルにするか")]
            public bool ArrowToMove = true;
            /// <summary>矢印キーの移動ベクトルにおける倍率</summary>
            [Tooltip("矢印キーの移動ベクトルにおける倍率")]
            public float ArrowKeyStrange = 1f;
            /// <summary>移動ベクトルを矢印キーにするか</summary>
            [Tooltip("移動ベクトルを矢印キーにするか")]
            public bool MoveToArrow = true;
            /// <summary>移動ベクトルを矢印キーとみなす下限</summary>
            [Tooltip("移動ベクトルを矢印キーとみなす下限")]
            public float MoveArrowDead = 0.3f;

            /// <summary>タッチパネルを移動ベクトルにするか</summary>
            public bool SwipeToMove = true;
            /// <summary>スワイプしたとみなす距離、超えたらスワイプモードになる</summary>
            public float SwipeDead = 20f;
            /// <summary>スワイプの最大半径、この範囲内を0～1で表現</summary>
            public float SwipeMaxRadius = 40f;
        }

        /// <summary>ボタンをBoolディクショナリ配列形式にしたもの、これにまず格納する</summary>
        public Dictionary<EButtonNum, bool> btnlist { get; private set; }
        /// <summary>リピートクラス</summary>
        public KeyRepeatDict Repeat = new KeyRepeatDict();
        static float StartLongRepeat = 1f;
        /// <summary>長押しリピートクラス</summary>
        public KeyRepeatDict LongRepeat = new KeyRepeatDict(StartLongRepeat);
        /// <summary>
        /// ボタンオブジェクト
        /// .Natural 押しっぱなしのときに発生
        /// .Repeat 押しっぱなしのときに断続的に発生
        /// .Down 押した瞬間だけ発生
        /// .Up 離した瞬間だけ発生
        /// </summary>
        public ButtonObj Button { get { return m_button; } set { m_button = value; } }
        protected ButtonObj m_button = new ButtonObj();
        [NonSerialized] public EButtonNum VirtualButton = 0;
        /// <summary>スティックオブジェクト</summary>
        public StickObj Stick { get { return m_stick; } set { m_stick = value; } }
        public StickObj m_stick = new StickObj();
        /// <summary>補正クラス</summary>
        static public CompPoint CompObj { get; private set; } = new CompPoint();

        private int controllerCount;
        public string[] ControllerNames;

        public static EConType DefaultConType = EConType.Default;
        // 現在のコントローラ
        [SerializeField] EConType currentConType = DefaultConType;
        // コントローラの切り替えは再生成にする、複数体作るときのメモリ軽減
        public EConType CurrentConType
        {
            get { return currentConType; }
            set
            {
                bool doChange;
                int joystickID = 0;
                joystickID = JoystickID;
                doChange = ConType != currentConType;
                if (doChange)
                {
                    currentConType = value;
                    ConType = currentConType;
                }
            }
        }
        // コントローラの数が変わったときに実行
        private int activeJoystickCount = 0;
        public int ActiveJoystickCount
        {
            get { return activeJoystickCount; }
            private set
            {
                if (activeJoystickCount != value)
                {
                    if (CurrentConType == EConType.Default) BuildOfType(CurrentConType);
                    activeJoystickCount = value;
                }
            }
        }
        static public Dictionary<EConType, string> ConAutoReg = new Dictionary<EConType, string> {
        { EConType.Android, @".*::.*Android.*" },
        { EConType.Switch, @".*Wireless Gamepad.*::.*" },
    };
        public EConType Sys_ConType { get; private set; }
        /// <summary>コントローラーの種類</summary>
        private EConType m_conType;
        public EConType ConType
        {
            set { BuildOfType(value); }
            get { return m_conType; }
        }
        /// <summary>現在のコントローラ名やコントローラID</summary>
        public string JoystickName = "All";
        private int joystickID = 0;
        public int JoystickID
        {
            set
            {
                JoystickName = "All";
                string[] jsns = Input.GetJoystickNames();
                if ((value < ConObj.joysticks.Count) && (value <= jsns.Length))
                {
                    joystickID = value;
                    if (joystickID == 0)
                        JoystickName = "All";
                    else if (joystickID < 0)
                        JoystickName = "Keyboard";
                    else
                        JoystickName = jsns[joystickID - 1];
                }
            }
            get { return joystickID; }
        }

        public void Add(EKeyType _keytype = EKeyType.Key, EButtonNum _button = 0, int _num = 1, string _keyname = "", bool _reverse = false, float _dead = 0.1f,
            EPosType _pos = EPosType.Left, EAxis _pnt = EAxis.X)
        {
            m_controller.Add(new ConObj(this, _keytype, _button, _num, _keyname, _reverse, _dead, _pos, _pnt));
        }
        /// <summary>通常のコンストラクタ、コントローラーオブジェクトの初期化</summary>
        public static Controller Create(Controller _controller = null, EConType contype = EConType.Default,
            int _joystickID = 0)
        {
            if (_controller == null) _controller = new Controller();
            _controller.JoystickID = _joystickID;
            _controller.BuildOfType(contype);
            _controller.btnlist = new Dictionary<EButtonNum, bool>();
            foreach (EButtonNum con in BTNNUMLIST) _controller.btnlist.Add(con, false);
            return _controller;
        }
        public static Controller Create(EConType contype = EConType.Default, int _joystickID = 0)
        {
            return Create(null, contype, _joystickID);
        }

        /// <summary>有効なスティックの数を出力する</summary>
        public static int JoystickCount()
        {
            int cnt = 0;
            foreach (string stickname in Input.GetJoystickNames())
                if (stickname != "") cnt++;
            return cnt;
        }
        /// <summary>最も番号が小さいスティックを取得する</summary>
        public static string GetJoystickVeteran()
        {
            foreach (string stickname in Input.GetJoystickNames())
                if (stickname != "") return stickname;
            return "";
        }

        /// <summary>キーの交換、何番目のビットなのかで指定</summary>
        static public EButtonNum KeySwap(EButtonNum btn, int num1, int num2, bool oneway = false)
        {
            EButtonNum b1 = (EButtonNum)(1 << num1), b2 = (EButtonNum)(1 << num2);
            EButtonNum _b1 = (EButtonNum)((((btn & b1) == b1) ? 1 : 0) << num2);
            EButtonNum _b2 = (EButtonNum)((((btn & b2) == b2) ? 1 : 0) << num1);
            btn = (btn & ~b1) | _b2;
            if (!oneway) btn = (btn & ~b2) | _b1;
            return btn;
        }

        /// <summary>テンプレ設置、後で追加することもできる</summary>
        public void SetTemp(ConTempSet templates)
        {
            m_controller.AddRange(ConTempSet.OutUseList(this, templates));
        }

        public void KeySwapLocal(EButtonNum b1, EButtonNum b2, bool oneway = false)
        {
            int num1, num2;
            num1 = (int)Math.Truncate(Math.Log((int)b1, 2));
            num2 = (int)Math.Truncate(Math.Log((int)b2, 2));
            Button[EButtonMode.Press] = KeySwap(Button[EButtonMode.Press], num1, num2, oneway);
            Button[EButtonMode.Repeat] = KeySwap(Button[EButtonMode.Repeat], num1, num2, oneway);
            Button[EButtonMode.Down] = KeySwap(Button[EButtonMode.Down], num1, num2, oneway);
            Button[EButtonMode.Up] = KeySwap(Button[EButtonMode.Up], num1, num2, oneway);
            Button[EButtonMode.Delay] = KeySwap(Button[EButtonMode.Press], num1, num2, oneway);
            Button[EButtonMode.DelayRepeat] = KeySwap(Button[EButtonMode.Repeat], num1, num2, oneway);
            Button[EButtonMode.DelayDown] = KeySwap(Button[EButtonMode.Down], num1, num2, oneway);
            Button[EButtonMode.DelayUp] = KeySwap(Button[EButtonMode.Up], num1, num2, oneway);
        }

        public void SetFingerLock(int fingerID)
        {
            if (!FingerLock.ContainsKey(fingerID)) FingerLock.Add(fingerID, true);
        }
        public void SetFingerUnLock(int fingerID)
        {
            FingerLock.Remove(fingerID);
        }

        /// <summary>仮想ボタン発火、PointerDownを使うのがおすすめ</summary>
        public void SetVirtualButton(EButtonNum btn = EButtonNum.NONE)
        {
            VirtualButton |= btn;
        }

        public void ControllerUpdate()
        {
            if (btnlist == null) return;
            // 初期化
            foreach (EButtonNum btnkey in BTNNUMLIST) btnlist[btnkey] = false;
            foreach (EButtonMode Type in Enum.GetValues(typeof(EButtonMode))) Button[Type] = 0;
            Stick.PosClear();
            TouchesPress = Array.ConvertAll(TouchesPress, x => false);
            TouchesDown = Array.ConvertAll(TouchesDown, x => false);
            TouchesUp = Array.ConvertAll(TouchesUp, x => false);
            TouchesDouble = Array.ConvertAll(TouchesDouble, x => false);
            TouchedPress = false;
            TouchedDown = false;
            TouchedUp = false;
            if (!Active) return;

            TouchesBeforePosition = (Vector2[])TouchesPosition.Clone();
            TouchesBeforeVector = (Vector2[])TouchesVector.Clone();
            TouchedTapMode = Input.touchSupported && (Input.touchCount > 0);

            // タッチ周りのリニューアル
            if (TouchedTapMode)
            {
                int count = 0;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (count >= USE_TOUCHESCOUNT) break;
                    Touch t = Input.touches[i];
                    if (!FingerLock.ContainsKey(t.fingerId))
                    {
                        TouchesPosition[count] = t.position;
                        switch (t.phase)
                        {
                            case TouchPhase.Began:
                                TouchesDown[count] = true;
                                TouchesPress[count] = true;
                                break;
                            case TouchPhase.Moved:
                            case TouchPhase.Stationary:
                                TouchesPress[count] = true;
                                break;
                            case TouchPhase.Canceled:
                            case TouchPhase.Ended:
                                TouchesPress[count] = true;
                                TouchesUp[count] = true;
                                break;
                        }
                        count++;
                    }
                }
                if (TouchesPress[1] && !TouchesSwipeMode[0])
                {
                    BeforeScroll = Scroll;
                    Scroll = Vector3.Distance(TouchesPosition[0], TouchesPosition[0]) / 100f;
                }
            }
            else
            {
                if (!FingerLock.ContainsKey(-1))
                {
                    for (int i = 0; i < USE_TOUCHESCOUNT; i++)
                    {
                        TouchesPosition[i] = Input.mousePosition;
                        TouchesDown[i] = Input.GetMouseButtonDown(i);
                        TouchesUp[i] = Input.GetMouseButtonUp(i);
                        TouchesPress[i] = Input.GetMouseButton(i) || TouchesUp[i];
                    }
                    BeforeScroll = 0f;
                    Scroll = Input.GetAxis("Mouse ScrollWheel");
                }
            }
            for (int i = 0; i < USE_TOUCHESCOUNT; i++)
            {
                TouchedPress |= TouchesPress[i];
                TouchedUp |= TouchesUp[i];
                TouchedDown |= TouchesDown[i];
            }
            float mag;
            EPosType controllVector;

            EPosType[] touchVectorModes;
            EButtonNum[] touchButtonNames;
            EButtonMode[] touchesButtonModes;
            if (TouchedTapMode) {
                touchVectorModes = m_property.TouchVectorMode;
                touchButtonNames = m_property.TouchesButtonName;
                touchesButtonModes = m_property.TouchesButtonMode;
            }
            else
            {
                touchVectorModes = m_property.MouseVectorMode;
                touchButtonNames = m_property.MouseButtonName;
                touchesButtonModes = m_property.MouseButtonMode;
            }
            if (TouchedPress)
            {
                for (int i = 0; i < USE_TOUCHESCOUNT; i++)
                {
                    controllVector = touchVectorModes[i];
                    if (TouchesDown[i])
                    {
                        TouchesTime[i] = 0;
                        TouchesSwipeMode[i] = false;
                        TouchesDownPosition[i] = TouchesPosition[i];
                        if (m_property.TouchButtonFlag)
                        {
                            if ((touchesButtonModes[i]
                                & (EButtonMode.Down)) > 0)
                            {
                                VirtualButton |= touchButtonNames[i];
                            }
                        }
                    }

                    if (TouchesPress[i])
                    {
                        TouchesTime[i] += Time.deltaTime;
                        if (m_property.TouchButtonFlag)
                        {
                            if ((m_property.TouchesButtonMode[i]
                                & (EButtonMode.Press | EButtonMode.Repeat | EButtonMode.Double)) > 0)
                            {
                                VirtualButton |= touchButtonNames[i];
                            }
                        }

                        TouchesVector[i] = TouchesPosition[i] - TouchesDownPosition[i];
                        TouchesDeltaVector[i] = TouchesPosition[i] - TouchesBeforePosition[i];


                        mag = TouchesVector[i].magnitude;
                        if (!TouchesSwipeMode[i])
                            if (mag > m_property.SwipeDead)
                                TouchesSwipeMode[i] = true;
                        Vector2 vector2 = TouchesVector[i];
                        float delta_mag = 1f;

                        switch (controllVector)
                        {
                            case EPosType.Rot:
                                vector2 = TouchesDeltaVector[i] * m_property.TouchRotReverseComp;
                                delta_mag = m_property.TouchDeltaMagnitude;
                                break;
                        }
                        if (controllVector != EPosType.None)
                            if (TouchesSwipeMode[i])
                            {
                                Stick.SetMost(controllVector,
                                    VecComp.ConvMag(vector2, m_property.SwipeMaxRadius), delta_mag);
                            }
                    }
                    if (TouchesUp[i])
                    {
                        if (m_property.TouchButtonFlag)
                        {
                            if ((m_property.TouchesButtonMode[i] & (EButtonMode.Up)) > 0)
                            {
                                VirtualButton |= touchButtonNames[i];
                            }
                            else if ((m_property.TouchesButtonMode[i] & (EButtonMode.Click)) > 0)
                            {
                                if (!TouchesSwipeMode[i]) VirtualButton |= touchButtonNames[i];
                            }
                        }
                        TouchesUpPosition[i] = TouchesPosition[i];
                        TouchesBeforeVector[i] = TouchesUpPosition[i] - TouchesDownPosition[i];
                        TouchesVector[i].Set(0, 0);
                        TouchesSwipeMode[i] = false;
                        TouchesTime[i] = 0f;
                    }
                }
            }

            if (!TouchedPress) TouchedSwipeMode = false;
            foreach (bool tds in TouchesSwipeMode) TouchedSwipeMode |= tds;

            // ボタン周り取得する
            foreach (ConObj con in m_controller)
                VirtualButton |= con.UpdateButton(m_property.ConKeyboard);
            if ((VirtualButton & EButtonNum.ANY_ARROW) != 0)
                if (m_property.ArrowToMove)
                {
                    Vector2 vc2 = Vector2.zero;
                    float srg = m_property.ArrowKeyStrange;
                    // キー入力をMoveに落とし込む
                    if (ButtonObj.Judge(EButtonNum.UP | EButtonNum.DOWN, VirtualButton))
                        vc2.y = ((ButtonObj.Judge(EButtonNum.UP, VirtualButton)) ? (Yreverse ? srg : -srg) : 0)
                            + ((ButtonObj.Judge(EButtonNum.DOWN, VirtualButton)) ? (Yreverse ? -srg : srg) : 0);
                    if (ButtonObj.Judge(EButtonNum.LEFT | EButtonNum.RIGHT, VirtualButton))
                        vc2.x = ((ButtonObj.Judge(EButtonNum.LEFT, VirtualButton)) ? -srg : 0)
                            + ((ButtonObj.Judge(EButtonNum.RIGHT, VirtualButton)) ? srg : 0);
                    Stick[EPosType.Move] = VecComp.SetCurcler(Stick[EPosType.Move], vc2);
                    // キー入力をRotに落とし込む
                    vc2 = Vector2.zero;
                    if (ButtonObj.Judge(EButtonNum.R_UP | EButtonNum.R_DOWN, VirtualButton))
                        vc2.y = ((ButtonObj.Judge(EButtonNum.R_UP, VirtualButton)) ? (Controller.Yreverse ? srg : -srg) : 0)
                            + ((ButtonObj.Judge(EButtonNum.R_DOWN, VirtualButton)) ? (Controller.Yreverse ? -srg : srg) : 0);
                    if (ButtonObj.Judge(EButtonNum.R_LEFT | EButtonNum.R_RIGHT, VirtualButton))
                        vc2.x = ((ButtonObj.Judge(EButtonNum.R_LEFT, VirtualButton)) ? -srg : 0)
                            + ((ButtonObj.Judge(EButtonNum.R_RIGHT, VirtualButton)) ? srg : 0);
                    Stick[EPosType.Rot] = VecComp.SetCurcler(Stick[EPosType.Rot], vc2);
                }
            if (m_property.MoveToArrow)
            {
                // Moveから方向キーを取得
                Vector2 smov = Stick[EPosType.Move];
                float rot = VecComp.ToAbsDeg(smov);
                if (smov.magnitude > m_property.MoveArrowDead)
                {
                    if (160f > rot && rot > 20f) VirtualButton |= Yreverse ? EButtonNum.UP : EButtonNum.DOWN;
                    if (250f > rot && rot > 110f) VirtualButton |= EButtonNum.LEFT;
                    if (340f > rot && rot > 200f) VirtualButton |= Yreverse ? EButtonNum.DOWN : EButtonNum.UP;
                    if (rot > 290f || 70f > rot) VirtualButton |= EButtonNum.RIGHT;
                }
                // Rotから方向キーを取得
                smov = Stick[EPosType.Rot];
                rot = VecComp.ToAbsDeg(smov);
                if (smov.magnitude > m_property.MoveArrowDead)
                {
                    if (160f > rot && rot > 20f) VirtualButton |= Yreverse ? EButtonNum.R_UP : EButtonNum.R_DOWN;
                    if (250f > rot && rot > 110f) VirtualButton |= EButtonNum.R_LEFT;
                    if (340f > rot && rot > 200f) VirtualButton |= Yreverse ? EButtonNum.R_DOWN : EButtonNum.R_UP;
                    if (rot > 290f || 70f > rot) VirtualButton |= EButtonNum.R_RIGHT;
                }
            }

            // 各々のボタンステータスの反映
            foreach (EButtonNum btnkey in BTNNUMLIST)
            {
                if (ButtonObj.Judge(btnkey, VirtualButton, true))
                {
                    btnlist[btnkey] = true;
                }
            }

            // リピートなどの付与
            foreach (EButtonNum btnkey in BTNNUMLIST)
            {
                bool press = btnlist[btnkey];
                KeyRepeatClass rep = Repeat[btnkey];
                if (press)
                {
                    Button[EButtonMode.Press] |= btnkey;
                    if (rep.double_press) Button[EButtonMode.Double] |= btnkey;
                }
                if (rep.Check(press))
                {
                    Button[EButtonMode.Repeat] |= btnkey;
                }
                if (rep.first)
                {
                    Button[EButtonMode.Down] |= btnkey;
                    if (rep.double_press) Button[EButtonMode.DoubleClick] |= btnkey;
                }
                if (rep.last)
                {
                    Button[EButtonMode.Up] |= btnkey;
                    Button[EButtonMode.Click] |= btnkey;
                    if (rep.double_press) Button[EButtonMode.DoubleUp] |= btnkey;
                }
                rep = LongRepeat[btnkey];
                if (rep.enable)
                {
                    Button[EButtonMode.Delay] |= btnkey;
                }
                if (rep.Check(press))
                {
                    Button[EButtonMode.DelayRepeat] |= btnkey;
                }
                if (rep.first)
                {
                    Button[EButtonMode.DelayDown] |= btnkey;
                }
                if (rep.last)
                {
                    Button[EButtonMode.DelayUp] |= btnkey;
                }
            }

            if (m_property.ButtonZLR_Reverse)
            {
                KeySwapLocal(EButtonNum.L, EButtonNum.ZL);
                KeySwapLocal(EButtonNum.R, EButtonNum.ZR);
            }
            Stick[EPosType.Rot] *= m_property.RotReverseComp;
            VirtualButton = 0;
            m_globalControllerEvents.Update(this);
        }

        // デフォルトで生成されるキーコンフィグデータ
        public void BuildOfType(EConType contype)
        {
            // Defaultは条件によって変化
            if (contype == EConType.Default)
            {
                string joylocal = JoystickName;
                if (joystickID == 0) joylocal = GetJoystickVeteran();
                joylocal += "::" + SystemInfo.operatingSystem;
                foreach (EConType key in new List<EConType>(ConAutoReg.Keys))
                {
                    if (Regex.IsMatch(joylocal, ConAutoReg[key])) contype = key;
                }
                if (contype == EConType.Default) contype = EConType.Xinput;
                BuildOfType(contype);
                m_conType = EConType.Default;
                return;
            }
            m_conType = contype;
            Sys_ConType = m_conType;

            m_controller.Clear();
            if (contype == EConType.Other) return;   // Otherは各自でAddすることを想定
            SetTemp(ConTempSet.UserFirstTemplate);
            SetTemp(ConTempSet.CommonTemplate);
            SetTemp(ConTempSet.SysTemplate);
            SetTemp(ConTempSet.KeyboardTemplate);
            switch (contype)
            {
                case EConType.Xinput:
                    SetTemp(ConTempSet.XinputTemplate);
                    break;
                case EConType.Direct:
                    SetTemp(ConTempSet.DirectTemplate);
                    break;
                case EConType.Switch:
                    SetTemp(ConTempSet.SwitchTemplate);
                    break;
                case EConType.Android:
                    SetTemp(ConTempSet.AndroidTemplate);
                    break;
            }
            SetTemp(ConTempSet.UserLastTemplate);
        }
        // Unityの終了命令
        static public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }
        const string CONTROLLER_NAME = "MainController";
        const string CONTROLLER_TAG = "GameController";
        static public GameObject GetGameMain(GameObject gameMain = null)
        {
            // 操作親を設定する、デフォルトでgameMainを取得、なければ自分で作る
            if (gameMain == null)
            {
                gameMain = GameObject.FindGameObjectWithTag(CONTROLLER_TAG);
                if (gameMain == null)
                {
                    gameMain = new GameObject(CONTROLLER_NAME);
                    gameMain.tag = CONTROLLER_TAG;
                }
                var _controller = gameMain.GetComponent<Controller>();
                if (_controller == null)
                {
                    gameMain.AddComponent<Controller>();
                }
            }
            return gameMain;
        }
        void Start()
        {
            m_globalControllerEvents.ParentObject = gameObject;
            Create(this, currentConType);
            activeJoystickCount = JoystickCount();
            Update();
        }

        private void OnValidate()
        {
            CurrentConType = currentConType;
        }
        void Update()
        {
            ActiveJoystickCount = JoystickCount();
            ControllerNames = Input.GetJoystickNames();
            ControllerUpdate();

            m_inputInspector.TouchPosition = TouchesPosition[0];
            m_inputInspector.TouchVector = TouchesVector[0];
            m_inputInspector.LeftStick = Stick[EPosType.Left];
            m_inputInspector.RightStick = Stick[EPosType.Right];
            m_inputInspector.MoveStick = Stick[EPosType.Move];
            m_inputInspector.RotStick = Stick[EPosType.Rot];

            m_button = Button;
            m_inputInspector.PressButtonInt = (int)m_button[EButtonMode.Press];
            m_inputInspector.PressButton = ButtonObj.ResultButton((EButtonNum)m_inputInspector.PressButtonInt);

            if (m_button.Judge((EButtonNum)13056, EButtonMode.Delay, true)) { Quit(); }
            if (m_button.Judge(EButtonNum.ESC, EButtonMode.DelayDown)) { Quit(); }
        }
    }
}
