using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class WelderGesture : Gesture
{

    private bool bIsHandClosed;

    protected override bool needLeftHand() { return true; }
    protected override bool needRightHand() { return true; }
    protected override bool checkLeftHand() { return this.getLeftHand().PalmNormal.y < 0; }
    protected override bool checkRightHand() { return true; }

    protected override void processGestures() {

        if (!this.GetRobot().IsCurrentArm(RobotBehaviours.Arms.Welder)) return;

        if (!bIsHandClosed && this.IsHandClosed(this.getRightHand())) {
            bIsHandClosed = true;
            print("FIRING");
        }

        else if (bIsHandClosed && !this.IsHandClosed(this.getRightHand())) {
            bIsHandClosed = false;
            print("NOT FIRING");
        }


    }

    protected override void processOthers() { return; }

}
