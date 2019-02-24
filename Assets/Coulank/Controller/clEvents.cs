using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Coulank.Controller
{
    /// <summary>
    /// コントローラーのイベントシステム
    /// コントローラー入力とキー入力の挙動を割り振る
    /// デフォルトでコントローラーは修飾キーを無効化する
    /// キー入力の修飾キーはデフォルトでCtrlキーとする
    /// </summary>
    [Serializable]
    public class ControllerEvents
    {
        [NonSerialized]
        protected GameObject parentObject = null;
        public GameObject ParentObject
        {
            get { return parentObject; }
            set
            {
                parentObject = value;
            }
        }
        public ControllerEvents(GameObject gameObject = null)
        {
            parentObject = gameObject;
        }
        public enum presetType
        {
            MoveForce,
        }
        [Serializable]
        public class ButtonEvent
        {
            [Serializable]
            public struct Property
            {
                [Tooltip("入力されたボタン")]
                public EButtonNum InputButton;
                [Tooltip("入力されていたら除外するボタン")]
                public EButtonNum ExclusionButton;
                [Tooltip("入力されたキーボード入力、修飾キーを必須とする")]
                public KeyCode InputKey;
                [Tooltip("キーボード入力を動作させるときの修飾キー")]
                public FModifierKeys ModifierKeys;
                [Tooltip("ボタン入力状態、リピート入力の指定もできる")]
                public EButtonMode ButtonMode;
                public static Property CreateDefault()
                {
                    var newObj = new Property
                    {
                        InputKey = KeyCode.None,
                        ExclusionButton = EButtonNum.MODIFIER,
                        ModifierKeys = new FModifierKeys
                        {
                            Ctrl = true
                        }
                    };
                    return newObj;
                }

                public EButtonNum GetModifierKeys()
                {
                    var modifier = ModifierKeys.Ctrl ? EButtonNum.CTRL : EButtonNum.NONE;
                    if (ModifierKeys.Alt) modifier |= EButtonNum.ALT;
                    if (ModifierKeys.Shift) modifier |= EButtonNum.SHIFT;
                    return modifier;
                }
                public bool InKeyJudge()
                {

                    return true;
                }
            }
            public Property property = Property.CreateDefault();
            public UnityEvent OnEvent = new UnityEvent();
        }

        public List<ButtonEvent> buttonEvents = new List<ButtonEvent>();

        public void Update(Controller con)
        {
            for (int i = 0; i < buttonEvents.Count; i++)
            {
                ButtonEvent e = buttonEvents[i];
                ButtonEvent.Property ep = e.property;
                // ジャッジ判定はボタンが押されている→除外ボタン判定と
                // キー入力の修飾キー判定→キーが押されているの順で行う
                bool judge = ((con.Button.Judge(ep.InputButton, ep.ButtonMode))
                    && (!con.Button.Judge(ep.ExclusionButton, EButtonMode.Press)))
                    || (Input.GetKey(ep.InputKey)
                    && ((con.Button.Judge(EButtonNum.KEYBOARD, ep.ButtonMode)))
                    && ((con.Button.Judge(ep.GetModifierKeys(), EButtonMode.Press))));
            if (judge)
                {
                    e.OnEvent.Invoke();
                }
            }
        }
    }
}
