using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coulank.Controller
{
    public class ControllerPlayer : Master
    {
        [SerializeField] private GameObject defaultGameController = null;
        new void Start()
        {
            if (defaultGameController != null)
                if (GameController == null) GameController = defaultGameController;
            base.Start();
        }
        new void Update()
        {
            base.Update();
            if (Button.Judge(EButtonNum.A)) Debug.Log("test");
        }
    }
}
