using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Coulank.Controller
{
    [DefaultExecutionOrder(0xF)]
    public class Master : MonoBehaviour
    {
        [NonSerialized] public GameObject GameController = null;
        // 内部以外は読み取り専用にする
        public Controller Controller { get; private set; }
        public FollowController Follow { get; private set; }
        public ButtonObj Button { get; private set; }
        public StickObj Stick { get; private set; }
        // コントローラーイベントはクラス共通
        public ControllerEvents ControllerEvents = new ControllerEvents();

        public void Start()
        {
            ControllerEvents.ParentObject = gameObject;

            GameController = Controller.GetGameMain(GameController);
            Controller = GameController.GetComponent<Controller>();
            Follow = GameController.GetComponent<FollowController>();
            Update();
        }
        public void Update()
        {
            if (Controller != null)
            {
                Button = Controller.Button;
                Stick = Controller.Stick;
                ControllerEvents.Update(Controller);

            }
        }
    }
}
