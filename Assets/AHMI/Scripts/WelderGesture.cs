using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class WelderGesture : Gesture
{

    private bool bIsHandClosed;

    protected override bool needLeftHand() { return true; }
    protected override bool needRightHand() { return true; }
    protected override bool checkLeftHand() { return this.hLeftHand.PalmNormal.y < 0; }
    protected override bool checkRightHand() { return true; }

    protected override void processGestures() {

        if (!this.Robot.IsCurrentArm(RobotBehaviours.Arms.Welder)) return;

        if (!bIsHandClosed && this.IsHandClosed(this.hRightHand)) {
            bIsHandClosed = true;
            print("FIRING");
            this.Animator.Play("Fire Welder");
            //this.Animator.Play("SetFire");
            
        }

        else if (bIsHandClosed && !this.IsHandClosed(this.hRightHand)) {
            bIsHandClosed = false;
            print("NOT FIRING");
            //this.Animator.Play("UnsetFire");
            this.Animator.Play("Unfire Welder");
        }

    }

    protected override void processOthers() { return; }

}
