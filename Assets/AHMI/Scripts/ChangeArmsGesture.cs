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
    protected override bool checkLeftHand() { return this.getLeftHand().PalmNormal.y > 0; }
    protected override bool checkRightHand() { return fElaspedTime <= 0 && this.getRightHand().PalmNormal.x < -0.5; }

    protected override void processGestures() {

        if (this.getRightHand().PalmVelocity.x < -xVelocity) { 
                fElaspedTime = fRestTime;
                this.GetRobot().TurnLeft();
        }
        else if (this.getRightHand().PalmVelocity.x > xVelocity) {
                fElaspedTime = fRestTime;
                this.GetRobot().TurnRight();
        }
        
    }

    protected override void processOthers() { fElaspedTime -= Time.deltaTime; }

}
