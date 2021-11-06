using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Attributes;
using Leap.Unity;

public class HammerGesture : Gesture
{
    bool bIsRightHandDown;

    public int yVelocity;

    protected override bool needLeftHand() { return true; }
    protected override bool needRightHand() { return true; }
    protected override bool checkLeftHand() { return this.getLeftHand().PalmNormal.y < 0; }
    protected override bool checkRightHand() { return this.IsHandClosed(this.getRightHand()); }

    protected override void processGestures() {

        if (!this.GetRobot().IsCurrentArm(RobotBehaviours.Arms.Hammer)) return;

        if (this.getRightHand().PalmVelocity.y < -yVelocity && !this.bIsRightHandDown) { 
                print("Hammer goes down");
                this.getAnimator().Play("Hammer Down");
                this.bIsRightHandDown = true;
        }
        else if (this.getRightHand().PalmVelocity.y > (yVelocity/2) && this.bIsRightHandDown) {
                print("Hammer goes up");
                this.getAnimator().Play("Hammer Up");
                this.bIsRightHandDown = false;
        }
        
    }

    protected override void processOthers() { return; }

}
