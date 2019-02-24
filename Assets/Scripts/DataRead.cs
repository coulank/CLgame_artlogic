using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Coulank;
using UnityEngine.Tilemaps;

namespace Coulank
{
    namespace CLgames
    {
        public class DataRead : Controller.Master
        {
            [SerializeField] QuestionListParam question = null;
            [SerializeField] PanelParam defaultPanel = null;
            PanelParam Panel;
            bool Touched;
            Tile Draw;
            Byte Byte;
            Vector2Int Pos;
            [SerializeField]
            bool assistMode;
            public bool AssistMode
            {
                get { return assistMode; }
                set {
                    assistMode = value;
                    if (Panel != null)
                    {
                        if (AssistMode)
                            Panel.SetNumTile(Panel.QNums, Panel.UNums);
                        else
                            Panel.SetNumTile(Panel.QNums);
                    }
                }
            }
            new void Start()
            {
                base.Start();
                if (defaultPanel == null) defaultPanel = Resources.Load<PanelParam>("GridPanel");
                Panel = Instantiate(defaultPanel);
                //Panel = defaultPanel;
                QuestionParam questionParam = question.QuestionList[0];
                //byte[] question = Convert.Compress.ByteDecompress(questionParam.Data);
                //Panel.TestDataToDraw(question, questionParam.Threshold);
                Panel.QParamSetup(questionParam);
                Pos = new Vector2Int(-1, -1);
            }
            public void test()
            {
                Debug.Log("キーが押されたよ");
            }

            void doDraw()
            {
                Vector2 pos = Controller.TouchesPosition[0];
                Vector3 wpos = Camera.main.ScreenToWorldPoint(pos);
                RaycastHit2D hit = Physics2D.Raycast(wpos, Vector3.forward, 100);
                if (hit.collider != null)
                {
                    var tilemap = hit.collider.GetComponent<Tilemap>();
                    if (tilemap != null)
                    {
                        var posInt = PanelParam.TilePosToDrawPos(
                            tilemap.WorldToCell(hit.point));
                        if (posInt == Pos) return;
                        Pos = posInt;
                        byte drawVal = 0;
                        if (Button.Judge(
                            Coulank.Controller.EButtonNum.A, Coulank.Controller.EButtonMode.Press))
                        {
                            drawVal = 1;
                        }
                        if (Button.Judge(
                            Coulank.Controller.EButtonNum.X, Coulank.Controller.EButtonMode.Press))
                        {
                            drawVal = 4;
                        }
                        byte data = Panel.GetUData(posInt);
                        if (!Touched)
                        {
                            if ((data & drawVal) > 0)
                            {
                                Draw = Panel.Draws.Blank;
                                Byte = 2;
                            }
                            else
                            {
                                if (drawVal == 1)
                                {
                                    Draw = Panel.Draws.Fill;
                                    Byte = drawVal;
                                }
                                else if (drawVal == 4) {
                                    Draw = Panel.Draws.Check;
                                    Byte = drawVal;
                                }
                                else
                                {
                                    Byte = 0;
                                }
                            }
                            Touched = true;
                        }
                        if (Byte != 0)
                        {
                            Panel.SetUData(posInt, (byte)
                                ((data & ~5) | (Byte & 5)));
                            if (Draw != null)
                            {
                                // まずはUNumsの更新
                                Panel.SetHorizon(Panel.UNums, 
                                    Panel.UData, posInt.y);
                                Panel.SetVertical(Panel.UNums,
                                    Panel.UData, posInt.x);
                                //Panel.SetNumTile(Panel.UNums);
                                // 色の変化をつけるためにQNumsの更新
                                if (assistMode)
                                {
                                    Panel.SetNumHorizon(Panel.QNums.Horizon.list,
                                        posInt.y, Panel.UNums.Horizon.list);
                                    Panel.SetNumVertical(Panel.QNums.Vertical.list,
                                        posInt.x, Panel.UNums.Vertical.list);
                                } else
                                {
                                    Panel.SetNumHorizon(Panel.QNums.Horizon.list,
                                        posInt.y);
                                    Panel.SetNumVertical(Panel.QNums.Vertical.list,
                                        posInt.x);
                                }
                                Panel.SetDrawTile(posInt, Draw);
                            }
                        }
                    }
                }
            }
            new void Update()
            {
                base.Update();
                // クリックした瞬間はとるが、タップした瞬間は取得しない
                // スワイプモードに突入したときは条件次第で変化
                // 二本指にしなかった場合は塗りつぶす、した場合は移動
                // 指を離したとき、スワイプモードではなかったときは判定を入れる
                bool judge = (Controller.TouchedTapMode)
                    ? ((Controller.TouchedSwipeMode) ?
                        !Controller.TouchesPress[1] : Controller.TouchedUp
                    ) : Controller.TouchedPress;
                if (judge)
                {
                    doDraw();
                } else
                {
                    if (Touched)
                    {
                        Pos = new Vector2Int(-1, -1);
                        Touched = false;
                    }
                }
                if (Button.Judge(Coulank.Controller.EButtonNum.B, Coulank.Controller.EButtonMode.Down))
                {
                    AssistMode = AssistMode ^ true;
                }

            }
        }
    }
}
