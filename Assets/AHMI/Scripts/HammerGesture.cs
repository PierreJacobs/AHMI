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
    protected override bool checkRightHand() { return this.checkExtendedFingers(this.getRightHand(), PointingState.NotExtended, PointingState.NotExtended, PointingState.NotExtended, PointingState.NotExtended, PointingState.NotExtended); }

    protected override void processGestures() {

        if (!this.GetRobot().IsCurrentArm(RobotBehaviours.Arms.Hammer)) return;

        if (this.getRightHand().PalmVelocity.y < -yVelocity && !this.bIsRightHandDown) { 
                //TODO: make hammer go down
                print("Hammer goes down");
                this.getAnimator().SetTrigger("Active Hammer");
                fElaspedTime = fRestTime;
                this.bIsRightHandDown = true;
        }
        else if (this.getRightHand().PalmVelocity.y > (yVelocity/2) && this.bIsRightHandDown) {
                //TODO: make hammer go up
                print("Hammer goes up");
                this.getAnimator().SetTrigger("Disable Hammer");
                fElaspedTime = fRestTime;
                this.bIsRightHandDown = false;
        }
        
    }

    protected override void processOthers() { return; }

}
