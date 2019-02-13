using UnityEngine;

namespace Coulank.Graphics
{
    class Colors
    {
        public static Color EColor2Color(EColor eColor)
        {
            Color color;
            switch (eColor)
            {
                case EColor.Black:
                    color = Color.black; break;
                case EColor.White:
                    color = Color.white; break;
                case EColor.Gray:
                    color = Color.gray; break;
                case EColor.Red:
                    color = Color.red; break;
                case EColor.Green:
                    color = Color.green; break;
                case EColor.Blue:
                    color = Color.blue; break;
                case EColor.Cyan:
                    color = Color.cyan; break;
                case EColor.Magenta:
                    color = Color.magenta; break;
                case EColor.Yellow:
                    color = Color.yellow; break;
                case EColor.Transparence:
                    color = Color.clear; break;
                default:
                    color = Color.black; break;
            }
            return color;
        }
    }
}
