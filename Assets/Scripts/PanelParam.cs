using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Coulank.Graphics;
// いじってわかった、GUID変更されたら設定しなおしになる

namespace Coulank
{
    namespace CLgames
    {
        [Serializable]
        public struct Grid
        {
            /// <summary>無地パネル</summary>
            public Tile WhitePanel;
            /// <summary>左上の境界に使う右ボーダー</summary>
            public Tile RightBorderPanel;
            /// <summary>左上の境界に使う下ボーダー</summary>
            public Tile DownBorderPanel;
            /// <summary>左上の境界に使う右下ボーダー</summary>
            public Tile RightDownBorderPanel;
            /// <summary>右の境界に使う左ボーダー</summary>
            public Tile LeftBorderPanel;
            /// <summary>下の境界に使う上ボーダー</summary>
            public Tile UpBorderPanel;
            /// <summary>右下の境界に使う左上の点に見えるボーダー</summary>
            public Tile LeftUpBorderPanel;

            /// <summary>上の数字入れるとこに使う左右ボーダー</summary>
            public Tile LeftRightBorderPanel;
            /// <summary>上の数字入れるとこに使う左右下ボーダー</summary>
            public Tile LeftRightDownBorderPanel;
            /// <summary>左の数字入れるとこに使う上下ボーダー</summary>
            public Tile UpDownBorderPanel;
            /// <summary>左の数字入れるとこに使う右上下ボーダー</summary>
            public Tile UpDownRightBorderPanel;
            /// <summary>プレイヤーが入力するパネルに使う全方位ボーダー</summary>
            public Tile AllBorderPanel;
        }

        [Serializable]
        public struct OnesPanel
        {
            public Tile LeftPlace;
            public Tile RightPlace;
        }

        [Serializable]
        public struct NumberPanel
        {
            /// <summary>1の位のオブジェクト</summary>
            public List<OnesPanel> OnesPlaces;
            /// <summary>10の位のときのオブジェクト</summary>
            public List<OnesPanel> TensPlaces;
            public NumberPanel Clone(EColor eColor = EColor.None)
            {
                var clonePanel = new NumberPanel();
                clonePanel.OnesPlaces = new List<OnesPanel>();
                for (int i = 0; i < OnesPlaces.Count; i++)
                {
                    clonePanel.OnesPlaces.Add(new OnesPanel());
                    var place = clonePanel.OnesPlaces[i];
                    place.LeftPlace = Tiles.TileClone(
                        OnesPlaces[i].LeftPlace, eColor);
                    place.RightPlace = Tiles.TileClone(
                        OnesPlaces[i].RightPlace, eColor);
                    clonePanel.OnesPlaces[i] = place;
                }
                clonePanel.TensPlaces = new List<OnesPanel>();
                for (int i = 0; i < TensPlaces.Count; i++)
                {
                    clonePanel.TensPlaces.Add(new OnesPanel());
                    var place = clonePanel.TensPlaces[i];
                    place.LeftPlace = Tiles.TileClone(
                        TensPlaces[i].LeftPlace, eColor);
                    place.RightPlace = Tiles.TileClone(
                        TensPlaces[i].RightPlace, eColor);
                    clonePanel.TensPlaces[i] = place;
                }
                return clonePanel;
            }
        }

        [Serializable]
        public struct DrawPanel
        {
            public Tile Blank;
            public Tile Check;
            public Tile Fill;
        }

        [CreateAssetMenu(menuName = "Data/CreateGridMenu")]
        public class PanelParam : ScriptableObject
        {
            [SerializeField]
            public Grid Grids;
            [SerializeField]
            public NumberPanel Nums;
            [SerializeField]
            public DrawPanel Draws;
            public Tilemap BorderGridTile;
            public Tilemap NumberTile;
            public Dictionary<EColor, NumberPanel> ColorNumbers;
            public Tilemap DrawTile;
            public int TopHeight;
            public int LeftWidth;
            public QuestionParam QParam;
            const int DATA_HEADER = 4;
            public DicNums QNums;
            public DicNums UNums;
            public class CNums
            {
                public struct Property
                {
                    public int Num;
                    public EColor Ecolor;
                    public static Property New(int num)
                    {
                        var newPro = new Property();
                        newPro.Num = num;
                        newPro.Ecolor = EColor.Black;
                        return newPro;
                    }
                }
                public List<List<Property>> list;
                public CNums()
                {
                    list = new List<List<Property>>();
                }
                public List<Property> GetProperties(int li_i)
                {
                    int count = list.Count;
                    if (count <= li_i)
                    {
                        for (int j = count; j < (li_i + 1); j++)
                        {
                            list.Add(new List<Property>());
                        }
                    }
                    return list[li_i];
                }
                public Property GetProperty(int li_i, int pr_i)
                {
                    var properties = GetProperties(li_i);
                    int count = properties.Count;
                    if (count <= pr_i)
                    {
                        for (int j = count; j < (pr_i + 1); j++)
                        {
                            properties.Add(new Property());
                        }

                    }
                    return properties[pr_i];
                }
            }
            public class DicNums : Dictionary<Position.EAxis, CNums>
            {
                public DicNums()
                {
                    // 縦の数
                    Add(Position.EAxis.X, new CNums());
                    // 横の数
                    Add(Position.EAxis.Y, new CNums());
                }
                public CNums Vertical {
                    get { return this[Position.EAxis.X]; }
                    set { this[Position.EAxis.X] = value; }
                }
                public CNums Horizon {
                    get { return this[Position.EAxis.Y]; }
                    set { this[Position.EAxis.Y] = value; }
                }

            }

            /// <summary>
            /// 設問データ、QuestionParamからDefault圧縮の復元したものを置く
            /// </summary>
            public byte[] QData { get; private set; }
            public byte[] SetQData(byte[] qCompressData)
            {
                QData = Convert.Compress.ByteDecompress(qCompressData);
                return QData;
            }
            byte[] CreateAUData(byte[] auData)
            {
                if (QData == null) return auData;
                if (QData.Length < 5) return auData;
                auData = new byte[QData.Length];
                auData[2] = QData[2];
                auData[3] = QData[3];
                return auData;
            }
            /// <summary>
            /// 正解データ、設問データから答え合わせを効率化するために用意
            /// </summary>
            public byte[] AData;
            public byte[] SetAData()
            {
                AData = CreateAUData(AData);
                return CreateAUData(AData);
            }

            /// <summary>
            /// ユーザの塗られている情報、ヘッダー4バイト後に塗り配置
            /// ADataと突合するのでADataと同じ形式をとる
            /// セーブデータから復元する先もこっち
            /// </summary>
            public byte[] UData;
            public byte GetUData(Vector2Int Pos)
            {
                if (UData.Length < 5) return 0;
                int i = DATA_HEADER + Pos.x + Pos.y * UData[2];
                return UData[i];
            }
            public byte[] SetUData(Vector2Int Pos, byte u)
            {
                if (UData.Length < 5) return UData;
                int i = DATA_HEADER + Pos.x + Pos.y * UData[2];
                UData[i] = u;
                return UData;
            }
            public byte[] SetUData()
            {
                UData = CreateAUData(UData);
                return UData;
            }
            public byte[] SetUData(string aCompressData,
                Convert.EStringByte eStringType = Convert.EStringByte.Base64)
            {
                var uData = Convert.Compress.ByteDecompress(
                    Convert.Cast.Str2Byte(aCompressData, eStringType));
                if (UData.Length < 5) return SetUData();
                if (QData == null) return UData;
                if (QData.Length < 5) return UData;
                if (UData[2] == QData[2] && UData[3] == QData[3]) return UData;
                byte[] newData = new byte[QData.Length];
                newData[2] = QData[2]; newData[3] = QData[3];
                int width = (UData[2] < QData[2]) ? UData[2] : QData[2];
                int height = (UData[3] < QData[3]) ? UData[3] : QData[3];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int i = DATA_HEADER + x * UData[2] + y;
                        newData[i] = UData[i];
                    }
                }
                return newData;
            }

            public Vector3Int OriginPosition = new Vector3Int(0, 0, 0);
            public int Margin = 1;

            public void OnEnable()
            {
                var bd = GameObject.FindGameObjectsWithTag("Border");
                if (bd.Length == 0) return;
                GameObject borderObject = bd[0];
                BorderGridTile = borderObject.GetComponentInChildren<Tilemap>();
                GameObject numberObject = GameObject.FindGameObjectsWithTag("Number")[0];
                NumberTile = numberObject.GetComponentInChildren<Tilemap>();
                GameObject drawObject = GameObject.FindGameObjectsWithTag("Draw")[0];
                DrawTile = drawObject.GetComponentInChildren<Tilemap>();
                ColorNumbers = new Dictionary<EColor, NumberPanel>();
                ColorNumbers.Add(EColor.Black, Nums);
                ColorNumbers.Add(EColor.Gray, Nums.Clone(EColor.Gray));

            }
            [ContextMenu("Json形式で書きだす")]
            public void WriteOut()
            {
                Debug.Log(JsonUtility.ToJson(this));
            }

            /// <summary>
            /// 垂直方向に数値を配置
            /// </summary>
            /// <param name="verticals">xを列挙とする垂直方向の数字パラメータ</param>
            /// <param name="x">水平座標</param>
            public void SetNumVertical(List<List<CNums.Property>> verticals, int x)
            {
                if (verticals[x].Count > 0) {
                    for (int y = 0; y < verticals[x].Count; y++)
                    {
                        SetNumTile(verticals[x][y].Num,
                            new Vector2Int(x, -verticals[x].Count + y));
                    }
                    Calc.ValueSet.Max(TopHeight, verticals[x].Count);
                }
                else
                {
                    SetNumTile(0, new Vector2Int(x, 0));
                }
            }
            /// <summary>
            /// 水平方向に数値を配置、比較による色分けに対応
            /// </summary>
            /// <param name="verticals">yを列挙とする垂直方向の数字パラメータ</param>
            /// <param name="x">垂直座標</param>
            /// <param name="comp">比較、verticalsと同じ形式をとる、ユーザ入力</param>
            public void SetNumVertical(List<List<CNums.Property>> verticals,
                int x, List<List<CNums.Property>> comp)
            {
                if (verticals[x].Count > 0)
                {
                    int ci = 0;
                    for (int y = 0; y < verticals[x].Count; y++)
                    {
                        EColor eColor = verticals[x][y].Ecolor;
                        int num = verticals[x][y].Num;
                        bool bCompCount = (comp.Count > x);
                        if (bCompCount) bCompCount = (verticals[x].Count >= comp[x].Count);
                        if (bCompCount)
                        {
                            for (int i = ci; i < comp[x].Count; i++)
                            {
                                ci = i;
                                if (num == comp[x][ci].Num
                                    && eColor == comp[x][ci].Ecolor) break;
                            }
                            if (comp[x].Count > ci)
                            {
                                if (num == comp[x][ci].Num)
                                {
                                    eColor = EColor.Gray;
                                }
                            }
                        }
                        SetNumTile(num, new Vector2Int(x, -verticals[x].Count + y), eColor);
                    }
                    Calc.ValueSet.Max(TopHeight, verticals[x].Count);
                }
                else
                {
                    SetNumTile(0, new Vector2Int(x, 0));
                }
            }
            /// <summary>
            /// 水平方向に数値を配置、比較による色分けに対応
            /// </summary>
            /// <param name="horizons">yを列挙とする水平方向の数字パラメータ</param>
            /// <param name="y">垂直座標</param>
            /// <param name="comp">比較、verticalsと同じ形式をとる、ユーザ入力</param>
            public void SetNumHorizon(List<List<CNums.Property>> horizons, int y)
            {
                if (horizons[y].Count > 0)
                {
                    for (int x = 0; x < horizons[y].Count; x++)
                    {
                        SetNumTile(horizons[y][x].Num,
                            new Vector2Int(-horizons[y].Count + x, y));
                        Calc.ValueSet.Max(LeftWidth, horizons[y].Count);
                    }
                } else
                {
                    SetNumTile(0, new Vector2Int(0, y));
                }
            }
            /// <summary>
            /// 水平方向に数値を配置
            /// </summary>
            /// <param name="horizons">yを列挙とする水平方向の数字パラメータ</param>
            /// <param name="y">垂直座標</param>
            public void SetNumHorizon(List<List<CNums.Property>> horizons,
                int y, List<List<CNums.Property>> comp)
            {
                if (horizons[y].Count > 0)
                {
                    int ci = 0;
                    for (int x = 0; x < horizons[y].Count; x++)
                    {
                        EColor eColor = horizons[y][x].Ecolor;
                        int num = horizons[y][x].Num;
                        bool bCompCount = (comp.Count > y);
                        if (bCompCount) bCompCount = (horizons[x].Count >= comp[x].Count);
                        if (bCompCount)
                        {
                            for (int i = ci; i < comp[y].Count; i++)
                            {
                                ci = i;
                                if (num == comp[y][ci].Num
                                    && eColor == comp[y][ci].Ecolor) break;
                            }
                            if (comp[y].Count > ci)
                                if (num == comp[y][ci].Num)
                                {
                                    eColor = EColor.Gray;
                                }
                        }
                        SetNumTile(num, new Vector2Int(-horizons[y].Count + x, y), eColor);
                        Calc.ValueSet.Max(LeftWidth, horizons[y].Count);
                    }
                } else
                {
                    SetNumTile(0, new Vector2Int(0, y));
                }
            }
            /// <summary>
            /// 一括で反映させる
            /// </summary>
            /// <param name="dnums">番号ステータス</param>
            public void SetNumTile(DicNums dnums)
            {
                TopHeight = 4; LeftWidth = 4;
                List<List<CNums.Property>> verticals = dnums.Vertical.list;
                for (int x = 0; x < verticals.Count; x++)   // ||の並び
                {
                    SetNumVertical(verticals, x);
                }
                List<List<CNums.Property>> horizons = dnums.Horizon.list;
                for (int y = 0; y < horizons.Count; y++)    // =の並び
                {
                    SetNumHorizon(horizons, y);
                }
            }
            /// <summary>
            /// 一括で反映させる、比較による色分けに対応
            /// </summary>
            /// <param name="dnums">番号ステータス</param>
            /// <param name="compDnums">比較ステータス、ユーザーによる入力</param>
            public void SetNumTile(DicNums dnums, DicNums compDnums)
            {
                TopHeight = 4; LeftWidth = 4;
                if (dnums == null)
                {
                    Debug.Log("設問の生成に失敗しました");
                    return;
                }
                List<List<CNums.Property>> verticals = dnums.Vertical.list;
                for (int x = 0; x < verticals.Count; x++)   // ||の並び
                {
                    SetNumVertical(verticals, x, compDnums.Vertical.list);
                }
                List<List<CNums.Property>> horizons = dnums.Horizon.list;
                for (int y = 0; y < horizons.Count; y++)    // =の並び
                {
                    SetNumHorizon(horizons, y, compDnums.Horizon.list);
                }
            }
            /// <summary>
            /// 一括で反映させる
            /// </summary>
            /// <param name="dnums">番号ステータス</param>
            public void SetNumTile(DicNums dnums, int x, int y)
            {
                SetNumVertical(dnums.Vertical.list, x);
                SetNumHorizon(dnums.Horizon.list, y);
            }
            /// <summary>
            /// 数字タイルの配置
            /// </summary>
            /// <param name="num">配置したい数字、0～99</param>
            /// <param name="position">配置したい座標、Gridに沿って配置</param>
            /// <param name="eColor">指定したい色、ColorNumbersの定義必要</param>
            public void SetNumTile(int num, Vector2Int position,
                EColor eColor = EColor.Black)
            {
                Tile leftTile, rightTile;
                NumberPanel nums;
                if (ColorNumbers.ContainsKey(eColor))
                    nums = ColorNumbers[eColor];
                else
                    nums = Nums;
                if (num < 10)
                {
                    leftTile = nums.OnesPlaces[num].LeftPlace;
                    rightTile = nums.OnesPlaces[num].RightPlace;
                }
                else
                {
                    num = num % 100;
                    int one = num % 10;
                    int ten = (num - one) / 10;
                    leftTile = nums.TensPlaces[ten].LeftPlace;
                    rightTile = nums.TensPlaces[one].RightPlace;
                }
                Vector3Int vl = new Vector3Int((OriginPosition.x + position.x) * 2, OriginPosition.y - position.y - 1, 0);
                Vector3Int vr = new Vector3Int(vl.x + 1, vl.y, 0);
                //Debug.Log(string.Format("left:{0}, right:{1}", vl, vr));
                NumberTile.SetTile(vl, leftTile);
                NumberTile.SetTile(vr, rightTile);
            }
            /// <summary>
            /// タイルから描画用の座標を取得する
            /// </summary>
            public static Vector2Int TilePosToDrawPos(Vector3Int tilePos)
            {
                return new Vector2Int(tilePos.x, -tilePos.y - 1);
            }
            /// <summary>
            /// 描画用の座標で塗る
            /// </summary>
            public void SetDrawTile(Vector2Int pos)
            {
                SetDrawTile(pos.x, pos.y, Draws.Fill);
            }
            /// <summary>
            /// 描画用の座標でタイルを指定して塗る
            /// </summary>
            public void SetDrawTile(Vector2Int pos, Tile tile)
            {
                SetDrawTile(pos.x, pos.y, tile);
            }
            /// <summary>
            /// 描画用の座標でタイルを指定して塗る
            /// </summary>
            public void SetDrawTile(int x, int y, Tile tile)
            {
                Vector3Int v = new Vector3Int(OriginPosition.x + x,
                    OriginPosition.y - y - 1, 0);
                DrawTile.SetTile(v, tile);
            }
            /// 描画配列から自動的に塗る
            public void SetDrawTile(byte[] adata)
            {
                if (adata.Length < 1)
                {
                    Debug.Log("ADataがないため塗ることができませんでした");
                    return;
                }
                DrawTile.origin = OriginPosition;
                DrawTile.ClearAllTiles();
                Tile tile;
                int width = adata[2];
                for (int i = 4; i < adata.Length; i++)
                {
                    int x = Mathf.FloorToInt((i - 4) % width);
                    int y = Mathf.FloorToInt((i - 4) / width);
                    if ((adata[i] & 1) > 0)
                    {
                        tile = Draws.Fill;
                    } else if ((adata[i] & 4) > 0)
                    {
                        tile = Draws.Check;
                    } else
                    {
                        tile = Draws.Blank;
                    }
                    SetDrawTile(x, y, tile);
                }

            }
            public void QDataLoad()
            {
                if (QParam == null) return;
                QData = Convert.Compress.ByteDecompress(QParam.Data);
            }
            public static bool AUDataJudge(byte[] auData, int x, int y)
            {
                if (auData.Length < 4) return false;
                int width = auData[2];
                int i = DATA_HEADER + x + y * width;
                return (auData[i] & 3) > 0;
            }
            public static bool DataJudge(byte[] data, int i, int threshold)
            {
                if (data.Length <= i) return false;
                return (data[i] & 15) >= threshold;
            }
            public static bool DataJudge(byte[] data, int x, int y,
                int threshold, byte[] aData = null)
            {
                if (data.Length < 4) return false;
                int width = data[2];
                int i = DATA_HEADER + y + x * width;
                bool judge = DataJudge(data, i, threshold);
                if (aData != null)
                {
                    aData[i] = (byte)((aData[i] & ~1) | (judge ? 1 : 0));
                }
                return judge;
            }
            /// <summary>
            /// 垂直方向に配置
            /// </summary>
            /// <param name="nums">番号リストオブジェクト</param>
            /// <param name="data">データ配列</param>
            /// <param name="x">水平方向の座標</param>
            /// <param name="threshold">塗りつぶす閾値</param>
            /// <param name="setAData">
            /// 正解用に反映するかどうか、ユーザが塗るときは反映しない
            /// </param>
            protected void SetVertical(DicNums nums, byte[] data, int x,
                int threshold, bool setAData = false)
            {
                if (data.Length < 4) return;
                byte[] aData = setAData ? AData : null;
                int height = data[3];
                CNums cnum = nums.Vertical;
                var verticals = cnum.GetProperties(x);
                verticals.Clear();
                int count = 0;
                for (int y = 0; y < height; y++)
                {
                    bool judge = DataJudge(data, x, y, threshold, aData);
                    if (judge)
                    {
                        count++;
                        if ((y + 1) == height)
                        {
                            verticals.Add(CNums.Property.New(count));
                            count = 0;
                        }
                    }
                    else
                    {
                        if (count > 0)
                        {
                            verticals.Add(CNums.Property.New(count));
                            count = 0;
                        }
                    }
                }
            }
            /// <summary>
            /// 垂直方向に配置
            /// </summary>
            /// <param name="nums">番号リストオブジェクト</param>
            /// <param name="auData">解答データ配列</param>
            /// <param name="x">水平方向の座標</param>
            public void SetVertical(DicNums nums, byte[] auData, int x)
            {
                if (auData.Length < 4) return;
                int height = auData[3];
                CNums cnum = nums.Vertical;
                var verticals = cnum.GetProperties(x);
                verticals.Clear();
                int count = 0;
                for (int y = 0; y < height; y++)
                {
                    bool judge = AUDataJudge(auData, x, y);
                    //Debug.Log(string.Format("vertical x:{0} y:{1}, count:{2}, judge:{3}", x, y, count, judge));
                    if (judge)
                    {
                        count++;
                        if ((y + 1) == height)
                        {
                            verticals.Add(CNums.Property.New(count));
                            count = 0;
                        }
                    }
                    else
                    {
                        if (count > 0)
                        {
                            verticals.Add(CNums.Property.New(count));
                            count = 0;
                        }
                    }
                }
            }
            /// <summary>
            /// 水平方向に配置
            /// </summary>
            /// <param name="nums">番号リストオブジェクト</param>
            /// <param name="data">データ配列</param>
            /// <param name="y">垂直方向の座標</param>
            /// <param name="threshold">塗りつぶす閾値</param>
            /// <param name="setAData">
            /// 正解用に反映するかどうか、ユーザが塗るときは反映しない
            /// </param>
            protected void SetHorizon(DicNums nums, byte[] data, int y,
                int threshold, bool setAData = false)
            {
                if (data.Length < 4) return;
                byte[] aData = setAData ? AData : null;
                int width = data[2];
                CNums cnum = nums.Horizon;
                var horizons = cnum.GetProperties(y);
                horizons.Clear();
                int count = 0;
                for (int x = 0; x < width; x++)
                {
                    bool judge = DataJudge(data, x, y, threshold, aData);
                    if (judge)
                    {
                        count++;
                        if ((x + 1) == width)
                        {
                            horizons.Add(CNums.Property.New(count));
                            count = 0;
                        }
                    }
                    else
                    {
                        if (count > 0)
                        {
                            horizons.Add(CNums.Property.New(count));
                            count = 0;
                        }
                    }
                }
            }
            /// <summary>
            /// 水平方向に配置
            /// </summary>
            /// <param name="nums">番号リストオブジェクト</param>
            /// <param name="auData">解答データ配列</param>
            /// <param name="y">垂直方向の座標</param>
            public void SetHorizon(DicNums nums, byte[] auData, int y)
            {
                if (auData.Length < 4) return;
                int width = auData[2];
                CNums cnum = nums.Horizon;
                var horizons = cnum.GetProperties(y);
                horizons.Clear();
                int count = 0;
                for (int x = 0; x < width; x++)
                {
                    bool judge = AUDataJudge(auData, x, y);
                    //Debug.Log(string.Format("horizon x:{0} y:{1}, count:{2}, judge:{3}", x, y, count, judge));
                    if (judge)
                    {
                        count++;
                        if ((x + 1) == width)
                        {
                            horizons.Add(CNums.Property.New(count));
                            count = 0;
                        }
                    }
                    else
                    {
                        if (count > 0)
                        {
                            horizons.Add(CNums.Property.New(count));
                            count = 0;
                        }
                    }
                }
            }
            public void QDataMapping()
            {
                if (QData == null)
                {
                    Debug.Log("設問データが設定されておりません");
                    return;
                } else if (QData.Length < 1)
                {
                    Debug.Log("QDataの生成に失敗しました");
                    return;
                }
                NumberTile.origin = OriginPosition;
                NumberTile.ClearAllTiles();
                SetAData();
                int width = QData[2];
                int height = QData[3];
                for (int i = 4; i < AData.Length; i++)
                {
                    bool judge = DataJudge(QData, i, QParam.Threshold);
                    AData[i] = (byte)(judge ? 1 : 0);
                }
                QNums = new DicNums();
                for (int i = 0; i < width; i++)
                {
                    SetHorizon(QNums, AData, i);
                }
                for (int i = 0; i < height; i++)
                {
                    SetVertical(QNums, AData, i);
                }
            }
            public void UDataMapping()
            {
                UDataMapping(null);
            }
            public void UDataMapping(byte[] uData)
            {
                DrawTile.origin = OriginPosition;
                DrawTile.ClearAllTiles();
                if (uData == null)
                {
                    SetUData();
                }
                else
                    UData = uData;
                if (UData.Length < 1)
                {
                    Debug.Log("UDataの生成に失敗しました");
                    return;
                }

                int width = UData[2];
                int height = UData[3];

                UNums = new DicNums();
                for (int i = 0; i < width; i++)
                {
                    SetHorizon(UNums, UData, i);
                }
                for (int i = 0; i < height; i++)
                {
                    SetVertical(UNums, UData, i);
                }
            }
            public void QParamSetup(QuestionParam qParam)
            {
                QParam = qParam;
                QDataLoad();
                QDataMapping();
                UDataMapping();
                SetNumTile(QNums, UNums);
                SetDrawTile(AData);
            }

            public void TestDataToDraw(byte[] data, int threshold = 7)
            {
                NumberTile.origin = OriginPosition;
                NumberTile.ClearAllTiles();
                List<bool> pixelJudges = new List<bool>();
                int width = data[2];
                int height = data[3];
                int leftWidth = 4;
                int topHeight = 4;
                Tilemap tilemap = DrawTile;
                tilemap.origin = OriginPosition;
                tilemap.ClearAllTiles();
                for (int j = 4; j < data.Length; j++)
                {
                    int i = j - 4;
                    int x = Mathf.FloorToInt((i) % width);
                    int y = Mathf.FloorToInt((i) / width);
                    pixelJudges.Add((data[j] & 15) >= threshold);
                    Vector3Int v = new Vector3Int(OriginPosition.x + x, OriginPosition.y - y - 1, 0);
                    if (pixelJudges[i]) tilemap.SetTile(v, Draws.Fill);
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
                        SetNumTile(verticals[x][y], new Vector2Int(-verticals[x].Count + y, x));
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
                        SetNumTile(horizons[y][x], 
                            new Vector2Int(y, -horizons[y].Count + x));
                    }
                    if (leftWidth < horizons[y].Count) leftWidth = horizons[y].Count;
                }
                SetGridsTile(width, height, leftWidth, topHeight);
            }
            /// <summary>
            /// グリッド線自動生成
            /// </summary>
            /// <param name="user_width">ユーザが塗るとこの横幅</param>
            /// <param name="user_height">ユーザが塗るとこの縦幅</param>
            /// <param name="num_width">数字部分の横幅</param>
            /// <param name="num_height">数字部分の縦幅</param>
            /// <param name="num_min">数字部分の最小幅</param>
            public void SetGridsTile(int user_width, int user_height, int num_width, int num_height, int num_min = 4)
            {
                num_width = (num_width < num_min) ? num_min : num_width;
                num_height = (num_height < num_min) ? num_min : num_height;
                Tilemap tilemap = BorderGridTile;
                tilemap.origin = OriginPosition;
                tilemap.ClearAllTiles();
                // 余白
                for (int x = -Margin - num_width; x < user_width + Margin; x++)
                {
                    for (int y = -Margin - num_height; y < user_height + Margin; y++)
                    {
                        tilemap.SetTile(
                            new Vector3Int(OriginPosition.x + x, OriginPosition.y - y - 1, 0)
                            , Grids.WhitePanel);
                    }
                }

                // ユーザーが操作するエリアのグリッド
                for (int x = 0; x < user_width; x++)
                {
                    for (int y = 0; y < user_height; y++)
                    {
                        tilemap.SetTile(
                            new Vector3Int(OriginPosition.x + x, OriginPosition.y - y - 1, 0)
                            , Grids.AllBorderPanel);
                    }
                }
                // 左側のグリッド
                for (int x = -num_width; x < -1; x++)
                {
                    tilemap.SetTile(
                        new Vector3Int(OriginPosition.x + x, OriginPosition.y, 0)
                        , Grids.DownBorderPanel);
                    for (int y = 0; y < user_height; y++)
                    {
                        tilemap.SetTile(
                            new Vector3Int(OriginPosition.x + x, OriginPosition.y - y - 1, 0)
                            , Grids.UpDownBorderPanel);
                    }
                }
                for (int y = 0; y < user_height; y++)
                {
                    tilemap.SetTile(
                        new Vector3Int(OriginPosition.x - 1, OriginPosition.y - y - 1, 0)
                        , Grids.UpDownRightBorderPanel);
                }
                // 上側のグリッド
                for (int y = -num_height; y < -1; y++)
                {
                    tilemap.SetTile(
                        new Vector3Int(OriginPosition.x - 1, OriginPosition.y - y - 1, 0)
                        , Grids.RightBorderPanel);
                    for (int x = 0; x < user_width; x++)
                    {
                        tilemap.SetTile(
                            new Vector3Int(OriginPosition.x + x, OriginPosition.y - y - 1, 0)
                            , Grids.LeftRightBorderPanel);
                    }
                }
                for (int x = 0; x < user_width; x++)
                {
                    tilemap.SetTile(
                        new Vector3Int(OriginPosition.x + x, OriginPosition.y, 0)
                        , Grids.LeftRightDownBorderPanel);
                }
                tilemap.SetTile(
                    new Vector3Int(OriginPosition.x - 1, OriginPosition.y, 0)
                    , Grids.RightDownBorderPanel);
                // 下側のグリッド
                for (int x = -num_width; x < user_width; x++)
                {
                    tilemap.SetTile(
                    new Vector3Int(OriginPosition.x + x, OriginPosition.y - user_height - 1, 0)
                    , Grids.UpBorderPanel);
                }
                // 右側のグリッド
                for (int y = -num_height; y < user_height; y++)
                {
                    tilemap.SetTile(
                        new Vector3Int(OriginPosition.x + user_width, OriginPosition.y - y - 1, 0)
                        , Grids.LeftBorderPanel);
                }
                tilemap.SetTile(
                    new Vector3Int(OriginPosition.x + user_width, OriginPosition.y - user_height - 1, 0)
                    , Grids.LeftUpBorderPanel);
            }
        }
    }
}
