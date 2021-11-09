using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Attributes;
using Leap.Unity;

public class ChangeArmsGesture : Gesture
{

    float fElaspedTime;

    public int xVelocity;
    public float fRestTime;


    protected override bool needLeftHand() { return true; }
    protected override bool needRightHand() { return true; }
    protected override bool checkLeftHand() { return this.hLeftHand.PalmNormal.y > 0; }
    protected override bool checkRightHand() { return fElaspedTime <= 0 && ((this.bIsRightHanded && this.hRightHand.PalmNormal.x < -0.5) || (!this.bIsRightHanded && this.hRightHand.PalmNormal.x > 0.5)); }

    protected override void processGestures() {

        if (this.hRightHand.PalmVelocity.x < -xVelocity) {
            fElaspedTime = fRestTime;
            if (this.bIsRightHanded) this.Robot.TurnLeft();
            else this.Robot.TurnRight();
        }
        else if (this.hRightHand.PalmVelocity.x > xVelocity) {
            fElaspedTime = fRestTime;
            if (this.bIsRightHanded) this.Robot.TurnRight();
            else this.Robot.TurnLeft();
        }
        
    }

    protected override void processOthers() { fElaspedTime -= Time.deltaTime; }

}
