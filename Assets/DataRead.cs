using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monry.XsvUtility;
namespace Coulank
{
    public class DataRead : Controller.Master
    {
        public struct DataColumns
        {
            [XsvColumn(0), XsvKey] public int id;
            [XsvColumn(1)] public string path;
            [XsvColumn(2)] public string title;
            [XsvColumn(3)] public string description;
        }
        [SerializeField]
        public XsvReader m_CsvData;
        public Dictionary<int, DataColumns> m_DicData;
        public SpriteRenderer m_SpriteRenderer;
        new void Start()
        {
            base.Start();
            m_DicData = m_CsvData.ToDictionary<int, DataColumns>();
            if (m_SpriteRenderer == null) {
                m_SpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }
        new void Update()
        {
            base.Update();
            if (m_button.JudgeButton(Controller.ButtonType.A))
            {
                var image = Resources.Load<Sprite>(m_DicData[1].path);
                m_SpriteRenderer.sprite = image;
                Debug.Log(image.name);
            }
        }
    }
}
