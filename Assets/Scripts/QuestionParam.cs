using UnityEngine;
using Coulank.Convert;

[CreateAssetMenu(menuName = "Data/CreateQuestion")]
public class QuestionParam : ScriptableObject
{
    [SerializeField]
    [ContextMenuItem("設定したテクスチャの反映", "SetDataFromInspector")]
    public Sprite m_Sprite;
    public string m_Title;
    public string m_Answer;
    [TextArea]
    public string m_Comment;
    [Range(0, 15)]
    public int m_Threshold = 7;
    [SerializeField]
    byte[] m_Data;
    public byte[] Data
    {
        get { return m_Data; }
    }
    [ContextMenu("Json形式で書きだす")]
    public void WriteOut()
    {
        Debug.Log(JsonUtility.ToJson(this));
    }

    [ContextMenu("設定したテクスチャの反映")]
    public void SetDataFromInspector()
    {
        if (m_Sprite == null)
        {
            Debug.Log("Sprite is null");
            return;
        }
        if (m_Sprite.texture.isReadable)
        {
            SetData();
            Debug.Log(string.Format("{0} out of Binary data!", m_Sprite.name));
        }
        else
        {
            Debug.Log("Not Readable");
        }
    }
    public void SetData()
    {
        if (m_Sprite == null) return;
        Texture2D image = m_Sprite.texture;
        if (image.isReadable)
        {
            Color[] pixels = image.GetPixels(0, 0, image.width, image.height);
            byte[] data = new byte[4 + pixels.Length];
            // data[0]とdata[1]は一意にしても良いように予約領域とする
            data[2] = (byte)image.width;
            data[3] = (byte)image.height;
            int width = data[2];
            int height = data[3] - 1;
            for (int i = 0; i < pixels.Length; i++)
            {
                Color color = pixels[i];
                float gray = color.grayscale;
                int pos = (i % width) + (height - Mathf.FloorToInt(i / width)) * width;
                byte ag = (byte)(Mathf.FloorToInt(Mathf.Abs(((1 - color.grayscale) * color.a * 16) - 0.00001f)));
                data[4 + pos] = (byte)( ag
                    | ((color.r > 0.5) ? (1 << 4) : 0)
                    | ((color.g > 0.5) ? (1 << 5) : 0)
                    | ((color.b > 0.5) ? (1 << 6) : 0)
                    );
            }
            //m_Data = data;
            m_Data = Compress.ByteCompress(data);
        }
    }
}

