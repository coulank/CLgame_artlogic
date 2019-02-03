using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Monry.XsvUtility;
using System.IO;

namespace Coulank
{
    namespace test
    {
        public enum testenum
        {
            a, b, c
        }
        public struct TestColumns
        {
            [XsvColumn(0), XsvKey] public testenum id;
            [XsvColumn(1)] public string name;
        }

    }
    struct Data<T>
    {
        [XsvRow] public IEnumerable<T> Rows { get; set; }
    }

    public class grid : MonoBehaviour
    {
        [SerializeField]
        public string m_CsvPathInResources;
        [SerializeField]
        public bool m_HeaderEnable;
        [SerializeField]
        public XsvReader CsvAsset;
        // Start is called before the first frame update
        void Start()
        {
            var csvdict = CsvAsset.ToDictionary<test.testenum, test.TestColumns>();
            return;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
