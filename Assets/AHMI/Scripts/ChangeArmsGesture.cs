using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Attributes;

public class ChangeArmsGesture : Gesture
{
    protected new bool needLeftHand() {
        return true;
    }

    protected new bool needRightHand() {
        return true;
    }

    protected new bool checkLeftHand() {
        return this.checkExtendedFingers(this.getLeftHand(), PointingState.Extended, PointingState.Extended, PointingState.Extended, PointingState.Extended, PointingState.Extended) && this.getLeftHand().PalmNormal.y > 0;
    }

    protected new bool checkRightHand() {
        if (fElaspedTime <= 0 && this.getRightHand().PalmNormal.x < -0.5) {
            
            
        }

        fElaspedTime -= Time.deltaTime;
    }

    protected override void processGestures() {
        if (this.getRightHand().PalmVelocity.x < -xVelocity) { 
                fElaspedTime = fRestTime;
                RobotScript.TurnLeft();
        }
        else if (this.getRightHand().PalmVelocity.x > xVelocity) {
                fElaspedTime = fRestTime;
                RobotScript.TurnRight();
        }
    }
}
