using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class MovementGesture : Gesture
{
    protected override bool needLeftHand() { return true; }
    protected override bool needRightHand() { return false; }
    protected override bool checkLeftHand() { 
        return this.getLeftHand().PalmNormal.y < -0.5f && this.getLeftHand().PalmNormal.y > -0.8f;
    }

    protected override void processGestures() {
        Vector3 movementVector = new Vector3(-this.getLeftHand().PalmNormal.x , 0, -this.getLeftHand().PalmNormal.z);
        this.GetRobot().Move(movementVector);   
    }

    protected override void processOthers() { return; }


}
