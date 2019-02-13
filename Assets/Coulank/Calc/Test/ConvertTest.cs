using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coulank.Convert
{
    public class ConvertTest : MonoBehaviour
    {
        [SerializeField] int m_Shifted = 0;
        [SerializeField] int m_Frequency = 1;
        [ContextMenuItem("Byte配列に変換する", "Exec_Base642Byte")]
        [SerializeField] string m_Base64 = null;
        [ContextMenuItem("Byte配列に変換する", "Exec_Hex2Byte")]
        [SerializeField] string m_Hex = null;
        [ContextMenuItem("Bool配列に変換する", "Exec_Byte2Bool")]
        [ContextMenuItem("Base64に変換する", "Exec_Byte2Base64")]
        [ContextMenuItem("16進数に変換する", "Exec_Byte2Hex")]
        [SerializeField] byte[] m_Bytes = null;
        [ContextMenuItem("Byte配列に変換する", "Exec_Bool2Byte")]
        [SerializeField] bool[] m_Bools = null;

        void Exec_Byte2Bool()
        {
            m_Bools = Cast.Byte2Bool(m_Bytes, m_Shifted, m_Frequency);
        }
        void Exec_Bool2Byte()
        {
            m_Bytes = Cast.Bool2Byte(m_Bools, m_Shifted, m_Frequency);
        }
        void Exec_Hex2Byte()
        {
            m_Bytes = Compress.ByteDecompress(Cast.Str2Byte(m_Hex));
        }
        void Exec_Byte2Hex()
        {
            m_Hex = Cast.Byte2Str(Compress.ByteCompress(m_Bytes));
        }
        void Exec_Byte2Base64()
        {
            m_Base64 = Cast.Byte2Str(Compress.ByteCompress(m_Bytes), EStringByte.Base64);
        }
        void Exec_Base642Byte()
        {
            m_Bytes = Compress.ByteDecompress(Cast.Str2Byte(m_Base64, EStringByte.Base64));
        }

        [ContextMenu("test")]
        void test()
        {
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
