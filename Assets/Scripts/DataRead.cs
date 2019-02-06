using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Coulank;

namespace Coulank
{
    namespace CLgames
    {
        public class DataRead : Controller.Master
        {
            [SerializeField] QuestionListParam m_Question;
            [SerializeField] PanelParam m_DefaultPanel;
            PanelParam m_Panel;
            new void Start()
            {
                base.Start();
                if (m_DefaultPanel == null) m_DefaultPanel = Resources.Load<PanelParam>("GridPanel");
                m_Panel = Instantiate(m_DefaultPanel);
                m_Panel.SetTilemapMember();
                QuestionParam questionParam = m_Question.QuestionList[0];
                byte[] question = Compress.ByteDecompress(questionParam.Data);
                m_Panel.TestDataToDraw(question, questionParam.m_Threshold);
            }
            new void Update()
            {
                base.Update();
            }
        }
    }
}
