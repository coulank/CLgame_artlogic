using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coulank.Controller
{
    public class ControllerPlayer : Master
    {
        [SerializeField] private GameObject m_defaultGameController = null;
        new void Start()
        {
            if (m_defaultGameController != null)
                if (m_gameController == null) m_gameController = m_defaultGameController;
            base.Start();
        }
        new void Update()
        {
            base.Update();
            if (m_button.Judge(EButtonNum.A)) Debug.Log("test");
        }
    }
}
