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

        [Serializable]
        public struct DrawPanel
        {
            public TileBase Check;
            public TileBase Fill;
        }

        [CreateAssetMenu(menuName = "Data/CreateGridMenu")]
        public class PanelParam : ScriptableObject
        {
            [SerializeField]
            List<List<int>> m_Verticals;
            [SerializeField]
            List<List<int>> m_Horizons;

            [SerializeField]
            public Grid m_Grids;
            [SerializeField]
            public NumberPanel m_Nums;
            [SerializeField]
            public DrawPanel m_Draws;
            public Tilemap m_BorderGridTile;
            public Tilemap m_NumberTile;
            public Tilemap m_DrawTile;

            public Vector3Int m_OriginPosition = new Vector3Int(0, 0, 0);
            public int m_Margin = 20;

            public void SetTilemapMember()
            {
                GameObject borderObject = GameObject.FindGameObjectsWithTag("Border")[0];
                m_BorderGridTile = borderObject.GetComponentInChildren<Tilemap>();
                GameObject numberObject = GameObject.FindGameObjectsWithTag("Number")[0];
                m_NumberTile = numberObject.GetComponentInChildren<Tilemap>();
                GameObject drawObject = GameObject.FindGameObjectsWithTag("Draw")[0];
                m_DrawTile = drawObject.GetComponentInChildren<Tilemap>();
            }
            public void SetNumTile(int num, Vector3Int position)
            {
                TileBase leftTile, rightTile;
                if (num < 10)
                {
                    leftTile = m_Nums.OnesPlaces[num].LeftPlace;
                    rightTile = m_Nums.OnesPlaces[num].RightPlace;
                }
                else
                {
                    num = num % 100;
                    int one = num % 10;
                    int ten = (num - one) / 10;
                    leftTile = m_Nums.TensPlaces[ten].LeftPlace;
                    rightTile = m_Nums.TensPlaces[one].RightPlace;
                }
                Vector3Int vl = new Vector3Int((m_OriginPosition.x + position.x) * 2, m_OriginPosition.y - position.y - 1, 0);
                Vector3Int vr = new Vector3Int(vl.x + 1, vl.y, 0);
                Debug.Log(string.Format("left:{0}, right:{1}", vl, vr));
                m_NumberTile.SetTile(vl, leftTile);
                m_NumberTile.SetTile(vr, rightTile);
            }
            public void TestDataToDraw(byte[] data, int threshold = 7)
            {
                m_NumberTile.origin = m_OriginPosition;
                m_NumberTile.ClearAllTiles();
                List<bool> pixelJudges = new List<bool>();
                int width = data[2];
                int height = data[3];
                int leftWidth = 4;
                int topHeight = 4;
                Tilemap tilemap = m_DrawTile;
                tilemap.origin = m_OriginPosition;
                tilemap.ClearAllTiles();
                for (int j = 4; j < data.Length; j++)
                {
                    int i = j - 4;
                    int x = Mathf.FloorToInt((i) % width);
                    int y = Mathf.FloorToInt((i) / width);
                    pixelJudges.Add((data[j] & 15) >= threshold);
                    Vector3Int v = new Vector3Int(m_OriginPosition.x + x, m_OriginPosition.y - y - 1, 0);
                    if (pixelJudges[i]) tilemap.SetTile(v, m_Draws.Fill);
                }
                List<List<int>> verticals = new List<List<int>>();
                List<List<int>> horizons = new List<List<int>>();
                int count = 0;
                for (int x= 0; x < width; x++)  // ||の並び
                {
                    verticals.Add(new List<int>());
                    for (int y = 0; y < height; y++)
                    {
                        int i = y + x * width;
                        if (pixelJudges[i])
                        {
                            count++;
                            if ((y + 1) == height)
                            {
                                verticals[x].Add(count);
                                count = 0;
                            }
                        }
                        else
                        {
                            if (count > 0)
                            {
                                verticals[x].Add(count);
                                count = 0;
                            }
                        }
                    }
                    if (verticals[x].Count == 0) verticals[x].Add(0);
                    for (int y = 0; y < verticals[x].Count; y++)
                    {
                        SetNumTile(verticals[x][y], new Vector3Int(-verticals[x].Count + y, x, 0));
                    }
                    if (topHeight < verticals[x].Count) topHeight = verticals[x].Count;
                }
                for (int y = 0; y < height; y++)    // =の並び
                {
                    horizons.Add(new List<int>());
                    for (int x = 0; x < width; x++)
                    {
                        int i = y + x * height;
                        if (pixelJudges[i])
                        {
                            count++;
                            if ((x + 1) == width)
                            {
                                horizons[y].Add(count);
                                count = 0;
                            }
                        }
                        else
                        {
                            if (count > 0)
                            {
                                horizons[y].Add(count);
                                count = 0;
                            }
                        }
                    }
                    if (horizons[y].Count == 0) horizons[y].Add(0);
                    for (int x = 0; x < horizons[y].Count; x++)
                    {
                        SetNumTile(horizons[y][x], new Vector3Int(y, -horizons[y].Count + x, 0));
                    }
                    if (leftWidth < horizons[y].Count) leftWidth = horizons[y].Count;
                }
                SetGridsTile(width, height, leftWidth, topHeight);
            }
            public void SetGridsTile(int _x, int _y, int num_x, int num_y)
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
        }
    }
}
