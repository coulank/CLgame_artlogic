using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coulank.Convert
{
    public class ConvertTest : MonoBehaviour
    {
        [SerializeField] int Shifted = 0;
        [SerializeField] int Frequency = 1;
        [ContextMenuItem("Byte配列に変換する", "Exec_Base642Byte")]
        [SerializeField] string Base64 = null;
        [ContextMenuItem("Byte配列に変換する", "Exec_Hex2Byte")]
        [SerializeField] string Hex = null;
        [ContextMenuItem("Bool配列に変換する", "Exec_Byte2Bool")]
        [ContextMenuItem("Base64に変換する", "Exec_Byte2Base64")]
        [ContextMenuItem("16進数に変換する", "Exec_Byte2Hex")]
        [SerializeField] byte[] Bytes = null;
        [ContextMenuItem("Byte配列に変換する", "Exec_Bool2Byte")]
        [SerializeField] bool[] Bools = null;

        void Exec_Byte2Bool()
        {
            Bools = Cast.Byte2Bool(Bytes, Shifted, Frequency);
        }
        void Exec_Bool2Byte()
        {
            Bytes = Cast.Bool2Byte(Bools, Shifted, Frequency);
        }
        void Exec_Hex2Byte()
        {
            Bytes = Compress.ByteDecompress(Cast.Str2Byte(Hex));
        }
        void Exec_Byte2Hex()
        {
            Hex = Cast.Byte2Str(Compress.ByteCompress(Bytes));
        }
        void Exec_Byte2Base64()
        {
            Base64 = Cast.Byte2Str(Compress.ByteCompress(Bytes), EStringByte.Base64);
        }
        void Exec_Base642Byte()
        {
            Bytes = Compress.ByteDecompress(Cast.Str2Byte(Base64, EStringByte.Base64));
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
