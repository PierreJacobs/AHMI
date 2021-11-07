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
    protected override bool checkLeftHand() { return this.hLeftHand.PalmNormal.y < 0; }
    protected override bool checkRightHand() { return this.IsHandClosed(this.hRightHand); }

    protected override void processGestures() {

        if (!this.Robot.IsCurrentArm(RobotBehaviours.Arms.Hammer)) return;

        if (this.hRightHand.PalmVelocity.y < -yVelocity && !this.bIsRightHandDown) { 
                print("Hammer goes down");
                this.Animator.Play("Hammer Down");
                this.bIsRightHandDown = true;
        }
        else if (this.hRightHand.PalmVelocity.y > (yVelocity/2) && this.bIsRightHandDown) {
                print("Hammer goes up");
                this.Animator.Play("Hammer Up");
                this.bIsRightHandDown = false;
        }
        
    }

    protected override void processOthers() { return; }

}
