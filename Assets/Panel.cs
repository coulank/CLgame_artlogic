using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CLgames
{
    [Serializable]
    public struct Grid
    {
        /// <summary>無地パネル</summary>
        public TileBase WhitePanel;
        /// <summary>左上の境界に使う右ボーダー</summary>
        public TileBase RightBorderPanel;
        /// <summary>左上の境界に使う下ボーダー</summary>
        public TileBase DownBorderPanel;
        /// <summary>左上の境界に使う右下ボーダー</summary>
        public TileBase RightDownBorderPanel;
        /// <summary>右の境界に使う左ボーダー</summary>
        public TileBase LeftBorderPanel;
        /// <summary>下の境界に使う上ボーダー</summary>
        public TileBase UpBorderPanel;
        /// <summary>右下の境界に使う左上の点に見えるボーダー</summary>
        public TileBase LeftUpBorderPanel;

        /// <summary>上の数字入れるとこに使う左右ボーダー</summary>
        public TileBase LeftRightBorderPanel;
        /// <summary>上の数字入れるとこに使う左右下ボーダー</summary>
        public TileBase LeftRightDownBorderPanel;
        /// <summary>左の数字入れるとこに使う上下ボーダー</summary>
        public TileBase UpDownBorderPanel;
        /// <summary>左の数字入れるとこに使う右上下ボーダー</summary>
        public TileBase UpDownRightBorderPanel;
        /// <summary>プレイヤーが入力するパネルに使う全方位ボーダー</summary>
        public TileBase AllBorderPanel;
    }

    [Serializable]
    public struct OnesPanel
    {
        public TileBase LeftPlace;
        public TileBase RightPlace;
    }

    [Serializable]
    public struct NumberPanel
    {
        /// <summary>1の位のオブジェクト</summary>
        public List<OnesPanel> OnesPlaces;
        /// <summary>10の位のオブジェクト</summary>
        public List<TileBase> TensPlaces;
    }

    public class Panel : clController.ControllerMaster
    {
        [SerializeField]
        public Grid m_Grids;
        [SerializeField]
        public NumberPanel m_Nums;

        new void Start()
        {
            base.Start();
            Tilemap tilemap = GetComponentInChildren<Tilemap>();
            var position = new Vector3Int(0, 0, 0);
            tilemap.BoxFill(new Vector3Int(0, 0, 0), m_Grids.AllBorderPanel, -5, -5, 5, 5);
            tilemap.BoxFill(new Vector3Int(1, 0, 0), m_Grids.AllBorderPanel, -5, -5, 5, 5);
            tilemap.BoxFill(new Vector3Int(2, 0, 0), m_Grids.AllBorderPanel, -5, -5, 5, 5);
        }
        new void Update()
        {
            base.Update();
        }
    }
}
