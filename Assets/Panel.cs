using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Coulank
{
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
            /// <summary>10の位のときのオブジェクト</summary>
            public List<OnesPanel> TensPlaces;
        }

        public class Panel : Controller.Master
        {
            [SerializeField]
            public Grid m_Grids;
            [SerializeField]
            public NumberPanel m_Nums;
            private Tilemap m_BorderGridTile;
            private Tilemap m_NumberTile;

            public Vector3Int m_OriginPosition = new Vector3Int(0, 0, 0);
            public int m_Margin = 20;

            void SetTilemapMember()
            {
                GameObject borderObject = GameObject.FindGameObjectsWithTag("Border")[0];
                m_BorderGridTile = borderObject.GetComponentInChildren<Tilemap>();
                GameObject numberObject = GameObject.FindGameObjectsWithTag("Number")[0];
                m_NumberTile = numberObject.GetComponentInChildren<Tilemap>();
            }

            void SetGridsTile(int _x, int _y, int num_x, int num_y)
            {
                Tilemap tilemap = m_BorderGridTile;
                tilemap.origin = m_OriginPosition;
                tilemap.ClearAllTiles();
                // 余白
                for (int x = -m_Margin - num_x; x < _x + m_Margin; x++)
                {
                    for (int y = -m_Margin - num_y; y < _y + m_Margin; y++)
                    {
                        tilemap.SetTile(
                            new Vector3Int(m_OriginPosition.x + x, m_OriginPosition.y - y - 1, 0)
                            , m_Grids.WhitePanel);
                    }
                }

                // ユーザーが操作するエリアのグリッド
                for (int x = 0; x < _x; x++)
                {
                    for (int y = 0; y < _y; y++)
                    {
                        tilemap.SetTile(
                            new Vector3Int(m_OriginPosition.x + x, m_OriginPosition.y - y - 1, 0)
                            , m_Grids.AllBorderPanel);
                    }
                }
                // 左側のグリッド
                for (int x = -num_x; x < -1; x++)
                {
                    tilemap.SetTile(
                        new Vector3Int(m_OriginPosition.x + x, m_OriginPosition.y, 0)
                        , m_Grids.DownBorderPanel);
                    for (int y = 0; y < _y; y++)
                    {
                        tilemap.SetTile(
                            new Vector3Int(m_OriginPosition.x + x, m_OriginPosition.y - y - 1, 0)
                            , m_Grids.UpDownBorderPanel);
                    }
                }
                for (int y = 0; y < _y; y++)
                {
                    tilemap.SetTile(
                        new Vector3Int(m_OriginPosition.x - 1, m_OriginPosition.y - y - 1, 0)
                        , m_Grids.UpDownRightBorderPanel);
                }
                // 上側のグリッド
                for (int y = -num_y; y < -1; y++)
                {
                    tilemap.SetTile(
                        new Vector3Int(m_OriginPosition.x - 1, m_OriginPosition.y - y - 1, 0)
                        , m_Grids.RightBorderPanel);
                    for (int x = 0; x < _x; x++)
                    {
                        tilemap.SetTile(
                            new Vector3Int(m_OriginPosition.x + x, m_OriginPosition.y - y - 1, 0)
                            , m_Grids.LeftRightBorderPanel);
                    }
                }
                for (int x = 0; x < _x; x++)
                {
                    tilemap.SetTile(
                        new Vector3Int(m_OriginPosition.x + x, m_OriginPosition.y, 0)
                        , m_Grids.LeftRightDownBorderPanel);
                }
                tilemap.SetTile(
                    new Vector3Int(m_OriginPosition.x - 1, m_OriginPosition.y, 0)
                    , m_Grids.RightDownBorderPanel);
                // 下側のグリッド
                for (int x = -num_x; x < _x; x++)
                {
                    tilemap.SetTile(
                    new Vector3Int(m_OriginPosition.x + x, m_OriginPosition.y - _y - 1, 0)
                    , m_Grids.UpBorderPanel);
                }
                // 右側のグリッド
                for (int y = -num_y; y < _y; y++)
                {
                    tilemap.SetTile(
                        new Vector3Int(m_OriginPosition.x + _x, m_OriginPosition.y - y - 1, 0)
                        , m_Grids.LeftBorderPanel);
                }
                tilemap.SetTile(
                    new Vector3Int(m_OriginPosition.x + _x, m_OriginPosition.y - _y - 1, 0)
                    , m_Grids.LeftUpBorderPanel);
            }

            new void Start()
            {
                base.Start();
                SetTilemapMember();
                SetGridsTile(5, 5, 3, 3);
            }
            new void Update()
            {
                base.Update();
            }
        }
    }
}
