using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class MovementGesture : Gesture
{

    public float fMaxPalmVelocity = 50.0f;

    protected override bool needLeftHand() { return true; }
    protected override bool needRightHand() { return false; }
    protected override bool checkLeftHand() { 
        return this.isSlower3D(this.hLeftHand.PalmVelocity, this.fMaxPalmVelocity)
        && this.hLeftHand.PalmNormal.y < -0.4f && this.hLeftHand.PalmNormal.y > -0.9f
        && this.checkExtendedFingers(this.hLeftHand, PointingState.Extended, PointingState.Extended, PointingState.Extended, PointingState.Extended, PointingState.Extended);
    }

    protected override void processGestures() {
        Vector3 movementVector = new Vector3(-this.hLeftHand.PalmNormal.z , 0.0f, -this.hLeftHand.PalmNormal.x);
        this.Robot.Move(movementVector);   
    }

    protected override void processOthers() { return; }

}
