using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coulank.Controller
{
    public class RigidPlayerController : Master
    {
        [Serializable]
        public struct Property
        {
            public EButtonNum jumpButton;
            public EButtonMode jumpMode;
            public Vector3 jumpVector;
            public int hoverMaxCount;
            public Vector3 hoverVector;
            public Vector3 moveToVector;
            public Vector3 forceVector;
            public Vector3 biasForceVector;
            public Vector3 maxVelocity;
            public Vector3 biasMaxVelocity;
        }
        public Property property = new Property();
        private Rigidbody rigid = null;
        private Rigidbody2D rigid2 = null;

        public void AddForceWithMove()
        {
            Vector3 forceVector;
            Property ep = property;
            forceVector = Vector3.Scale(ep.moveToVector, Controller.stick[EPosType.Move]);
            if (Button.Judge(property.jumpButton, property.jumpMode))
            {
                if (property.jumpVector == Vector3.zero) property.jumpVector = Vector3.up * 500;
                forceVector = property.jumpVector + forceVector;
            }
            forceVector = Vector3.Scale(ep.forceVector, forceVector);
            if (rigid != null)
            {
                forceVector = VecComp.LimitForce(forceVector, rigid.velocity, ep.maxVelocity, ep.biasMaxVelocity);
                rigid.AddForce(forceVector);
            }
            else if (rigid2 != null)
            {
                forceVector = VecComp.LimitForce(forceVector, rigid2.velocity, ep.maxVelocity, ep.biasMaxVelocity);
                rigid2.AddForce(forceVector);
            }
        }
        new void Start()
        {
            rigid = GetComponent<Rigidbody>();
            rigid2 = GetComponent<Rigidbody2D>();
            base.Start();
        }
        new void Update()
        {
            base.Update();
            AddForceWithMove();
        }
    }
}
