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
            this.getAnimator().Play("Fire Welder");
            this.getAnimator().Play("SetFire");
            
        }

        else if (bIsHandClosed && !this.IsHandClosed(this.getRightHand())) {
            bIsHandClosed = false;
            print("NOT FIRING");
            this.getAnimator().Play("UnsetFire");
            this.getAnimator().Play("Unfire Welder");
        }


    }

    protected override void processOthers() { return; }

}
