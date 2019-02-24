using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Coulank.Controller
{
    public class VirtualButton : Master
    {
        [System.NonSerialized]
        bool pressFlag = false, clickFlag = false;
        public bool PressFlag
        {
            get { return pressFlag; }
            protected set { pressFlag = value; UpdateDisplay(); }
        }
        [SerializeField]
        protected EButtonNum PressButton = EButtonNum.NONE;
        [SerializeField]
        protected EButtonSwitch SwitchMode = EButtonSwitch.Normal;
        [SerializeField]
        protected Sprite PressImage = null;
        [SerializeField]
        public Color PressColor = Color.white;
        private Image ImageObj = null;
        private Sprite beforeImage = null;
        private Color beforeColor = Color.white;
        [SerializeField]
        public OnEventClass OnEvent = new OnEventClass();
        [System.Serializable]
        public class OnEventClass
        {
            public UnityEvent OnDown = new UnityEvent();
            public UnityEvent OnUp = new UnityEvent();
            public UnityEvent OnEnter = new UnityEvent();
            public UnityEvent OnExit = new UnityEvent();
            public UnityEvent OnClick = new UnityEvent();
        }

        public void SetUp(object vbn = null)
        {
            if (vbn == null) vbn = this;
            VirtualButton _vbn = (VirtualButton)vbn;
            // まず押しっぱなしと離したときの定義を行う
            EventTrigger trigger = GetComponent<EventTrigger>();
            if (trigger == null) trigger = gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry;

            // Canvasから生成されるEventSystemで取得
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => _vbn._onDown((PointerEventData)data));
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onUp((PointerEventData)data));
            entry.eventID = EventTriggerType.PointerUp;
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onEnter((PointerEventData)data));
            entry.eventID = EventTriggerType.PointerEnter;
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onExit((PointerEventData)data));
            entry.eventID = EventTriggerType.PointerExit;

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onClick((PointerEventData)data));
            entry.eventID = EventTriggerType.PointerClick;
            trigger.triggers.Add(entry);
        }

        new void Start()
        {
            base.Start();
            SetUp();
        }
        protected void Start(object vbn = null)
        {
            base.Start();
            SetUp(vbn);
        }
        public static int GetFingerFromData(Vector2 position)
        {
            if (Input.touchSupported)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.touches[i];
                    if (position == t.position) return t.fingerId;
                }
            }
            return -1;
        }
        public void OnDown() { }
        private void _onDown(PointerEventData data)
        {
            Controller.SetFingerLock(GetFingerFromData(data.position));
            PressFlag ^= true;
            switch (SwitchMode)
            {
                case EButtonSwitch.Click:
                    clickFlag = true;
                    break;
            }
            OnEvent.OnDown.Invoke();
            OnDown();
        }
        public void OnUp() { }
        private void _onUp(PointerEventData data)
        {
            Controller.SetFingerUnLock(GetFingerFromData(data.position));
            switch (SwitchMode)
            {
                case EButtonSwitch.Toggle:
                    break;
                default:
                    PressFlag = false;
                    clickFlag = false;
                    break;
            }
            OnEvent.OnUp.Invoke();
            OnUp();
        }
        public void OnEnter() { }
        private void _onEnter(PointerEventData data)
        {
            switch (SwitchMode)
            {
                case EButtonSwitch.Click:
                    if (clickFlag) PressFlag = true;
                    break;
            }
            OnEvent.OnEnter.Invoke();
            OnEnter();
        }
        public void OnExit() { }
        private void _onExit(PointerEventData data)
        {
            switch (SwitchMode)
            {
                case EButtonSwitch.Click:
                    if (clickFlag) PressFlag = false;
                    break;
            }
            OnEvent.OnExit.Invoke();
            OnExit();
        }
        public void OnClick() { }
        private void _onClick(PointerEventData data)
        {
            switch (SwitchMode)
            {
                case EButtonSwitch.Click:
                    Controller.SetVirtualButton(PressButton);
                    PressFlag = false;
                    break;
            }
            OnEvent.OnClick.Invoke();
            OnClick();
        }
        public void UpdateDisplay()
        {
            ImageObj = GetComponent<Image>();
            if (ImageObj != null)
            {
                if (pressFlag)
                {
                    beforeImage = ImageObj.sprite;
                    beforeColor = ImageObj.color;
                    if (PressImage != null) ImageObj.sprite = PressImage;
                    ImageObj.color = PressColor;
                }
                else
                {
                    ImageObj.sprite = beforeImage;
                    ImageObj.color = beforeColor;

                }
            }
        }
        new void Update()
        {
            base.Update();
            if (PressFlag)
            {
                if (!clickFlag) Controller.SetVirtualButton(PressButton);
            }
        }
    }
}
