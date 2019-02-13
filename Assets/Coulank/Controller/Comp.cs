using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coulank.Position;

namespace Coulank.Controller
{
    public class CompValue
    {
        public float plus = 0f, minus = 0f;
        public void Set(float value)
        {
            if (value > 0f)
            {
                plus = value;
                if (plus < 0.0001) { plus = 0f; }
            }
            else if (value < 0f)
            {
                minus = -value;
                if (minus < 0.0001) { minus = 0f; }
            }
        }
        public float DoComp(float value)
        {
            if (value > 0f)
            {
                if (plus != 0f) value /= plus;
                if (value > 1f) { value = 1; }
            }
            else if (value < 0f)
            {
                if (minus != 0f) value /= minus;
                if (value < -1f) { value = -1; }
            }
            return value;
        }
    }
    public class CompPNT : Dictionary<EAxis, CompValue>
    {
        public void Set(float value, EAxis pointType)
        {
            CompValue cmp;
            if (ContainsKey(pointType))
            {
                cmp = this[pointType];
            }
            else
            {
                cmp = new CompValue();
                Add(pointType, cmp);
            }
            cmp.Set(value);
        }
        public float DoComp(float value, EAxis pointType)
        {
            if (ContainsKey(pointType))
            {
                value = this[pointType].DoComp(value);
            }
            return value;
        }
    }
    public class CompPOS : Dictionary<EPosType, CompPNT>
    {
        public void Set(float value, EPosType posType, EAxis pointType)
        {
            CompPNT pnt;
            if (ContainsKey(posType))
            {
                pnt = this[posType];
            }
            else
            {
                pnt = new CompPNT();
                Add(posType, pnt);
            }
            pnt.Set(value, pointType);
        }
        public float DoComp(float value, EPosType posType, EAxis pointType)
        {
            if (ContainsKey(posType))
            {
                value = this[posType].DoComp(value, pointType);
            }
            return value;
        }
    }
    public class CompPoint : Dictionary<EConType, CompPOS>
    {
        public void Set(float value, EConType conType, EPosType posType, EAxis pointType)
        {
            CompPOS pos;
            if (ContainsKey(conType))
            {
                pos = this[conType];
            }
            else
            {
                pos = new CompPOS();
                Add(conType, pos);
            }
            pos.Set(value, posType, pointType);
        }
        public CompPoint()
        {
            Set(0.75f, EConType.Switch, EPosType.Left, EAxis.X);
            Set(-0.55f, EConType.Switch, EPosType.Left, EAxis.X);
            Set(0.55f, EConType.Switch, EPosType.Left, EAxis.Y);
            Set(-0.70f, EConType.Switch, EPosType.Left, EAxis.Y);
            Set(0.62f, EConType.Switch, EPosType.Right, EAxis.X);
            Set(-0.70f, EConType.Switch, EPosType.Right, EAxis.X);
            Set(0.60f, EConType.Switch, EPosType.Right, EAxis.Y);
            Set(-0.70f, EConType.Switch, EPosType.Right, EAxis.Y);
        }
        public float DoComp(float value, EConType conType, EPosType posType, EAxis pointType)
        {
            if (ContainsKey(conType))
            {
                value = this[conType].DoComp(value, posType, pointType);
            }
            return value;
        }
    }
}
