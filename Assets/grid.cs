using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class grid : MonoBehaviour
{
    /// <summary>無地パネル</summary>
    public TileBase m_WhitePanel;
    /// <summary>左上の境界に使う右ボーダー</summary>
    public TileBase m_RightBorderPanel;
    /// <summary>左上の境界に使う下ボーダー</summary>
    public TileBase m_DownBorderPanel;
    /// <summary>左上の境界に使う右下ボーダー</summary>
    public TileBase m_RightDownBorderPanel;
    /// <summary>右の境界に使う左ボーダー</summary>
    public TileBase m_LeftBorderPanel;
    /// <summary>下の境界に使う上ボーダー</summary>
    public TileBase m_UpBorderPanel;
    /// <summary>右下の境界に使う左上の点に見えるボーダー</summary>
    public TileBase m_LeftUpBorderPanel;

    /// <summary>上の数字入れるとこに使う左右ボーダー</summary>
    public TileBase m_LeftRightBorderPanel;
    /// <summary>上の数字入れるとこに使う左右下ボーダー</summary>
    public TileBase m_LeftRightDownBorderPanel;
    /// <summary>左の数字入れるとこに使う上下ボーダー</summary>
    public TileBase m_UpDownBorderPanel;
    /// <summary>左の数字入れるとこに使う右上下ボーダー</summary>
    public TileBase m_UpDownRightBorderPanel;
    /// <summary>プレイヤーが入力するパネルに使う全方位ボーダー</summary>
    public TileBase m_AllBorderPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
