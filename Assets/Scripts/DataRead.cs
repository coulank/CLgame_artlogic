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
            [SerializeField] QuestionListParam m_Question = null;
            [SerializeField] PanelParam m_DefaultPanel = null;
            PanelParam m_Panel;
            bool m_Touched;
            Tile m_Draw;
            Byte m_Byte;
            Vector2Int m_Pos;
            new void Start()
            {
                base.Start();
                if (m_DefaultPanel == null) m_DefaultPanel = Resources.Load<PanelParam>("GridPanel");
                //m_Panel = Instantiate(m_DefaultPanel);
                m_Panel = m_DefaultPanel;
                QuestionParam questionParam = m_Question.QuestionList[0];
                //byte[] question = Convert.Compress.ByteDecompress(questionParam.Data);
                //m_Panel.TestDataToDraw(question, questionParam.m_Threshold);
                m_Panel.QParamSetup(questionParam);
                m_Pos = new Vector2Int(-1, -1);
            }
            void doDraw()
            {
                Vector2 pos = m_controller.TouchesPosition[0];
                Vector3 wpos = Camera.main.ScreenToWorldPoint(pos);
                RaycastHit2D hit = Physics2D.Raycast(wpos, Vector3.forward, 100);
                if (hit.collider != null)
                {
                    var tilemap = hit.collider.GetComponent<Tilemap>();
                    if (tilemap != null)
                    {
                        var posInt = PanelParam.TilePosToDrawPos(
                            tilemap.WorldToCell(hit.point));
                        if (posInt == m_Pos) return;
                        m_Pos = posInt;
                        byte drawVal = 0;
                        if (m_button.Judge(
                            Controller.EButtonNum.A, Controller.EButtonMode.Press))
                        {
                            drawVal = 1;
                        }
                        if (m_button.Judge(
                            Controller.EButtonNum.X, Controller.EButtonMode.Press))
                        {
                            drawVal = 4;
                        }
                        byte data = m_Panel.GetUData(posInt);
                        if (!m_Touched)
                        {
                            if ((data & drawVal) > 0)
                            {
                                m_Draw = m_Panel.m_Draws.Blank;
                                m_Byte = 2;
                            }
                            else
                            {
                                if (drawVal == 1)
                                {
                                    m_Draw = m_Panel.m_Draws.Fill;
                                    m_Byte = drawVal;
                                }
                                else if (drawVal == 4) {
                                    m_Draw = m_Panel.m_Draws.Check;
                                    m_Byte = drawVal;
                                }
                                else
                                {
                                    m_Byte = 0;
                                }
                            }
                            m_Touched = true;
                        }
                        if (m_Byte != 0)
                        {
                            m_Panel.SetUData(posInt, (byte)
                                ((data & ~5) | (m_Byte & 5)));
                            if (m_Draw != null)
                            {
                                // まずはm_UNumsの更新
                                m_Panel.SetHorizon(m_Panel.m_UNums, 
                                    m_Panel.UData, posInt.y);
                                m_Panel.SetVertical(m_Panel.m_UNums,
                                    m_Panel.UData, posInt.x);
                                //m_Panel.SetNumTile(m_Panel.m_UNums);
                                // 色の変化をつけるためにm_QNumsの更新
                                m_Panel.SetNumHorizon(m_Panel.m_QNums.Horizon.m_list,
                                    posInt.y, m_Panel.m_UNums.Horizon.m_list);
                                m_Panel.SetNumVertical(m_Panel.m_QNums.Vertical.m_list,
                                    posInt.x, m_Panel.m_UNums.Vertical.m_list);
                                m_Panel.SetDrawTile(posInt, m_Draw);
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
                bool judge = (m_controller.TouchedTapMode)
                    ? ((m_controller.TouchedSwipeMode) ?
                        !m_controller.TouchesPress[1] : m_controller.TouchedUp
                    ) : m_controller.TouchedPress;
                if (judge)
                {
                    doDraw();
                } else
                {
                    if (m_Touched)
                    {
                        m_Pos = new Vector2Int(-1, -1);
                        m_Touched = false;
                    }
                }
            }
        }
    }
}
