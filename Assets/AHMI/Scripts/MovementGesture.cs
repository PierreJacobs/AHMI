using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class MovementGesture : Gesture
{
    protected override bool needLeftHand() { return true; }
    protected override bool needRightHand() { return false; }
    protected override bool checkLeftHand() { 
        return this.getLeftHand().PalmNormal.y < -0.5f && this.getLeftHand().PalmNormal.y > -0.9f
        && this.checkExtendedFingers(this.getLeftHand(), PointingState.Extended, PointingState.Extended, PointingState.Extended, PointingState.Extended, PointingState.Extended);
    }

    protected override void processGestures() {
        Vector3 movementVector = new Vector3(-this.getLeftHand().PalmNormal.z , 0.0f, -this.getLeftHand().PalmNormal.x);
        this.GetRobot().Move(movementVector);   
    }

    protected override void processOthers() { return; }

}
