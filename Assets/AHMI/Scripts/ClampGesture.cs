using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;
using UnityEngine.Serialization;
using System.Linq;

public class ClampGesture : Gesture
{
    // Uses Unity Game Unit
    [Units("Unity Game Unit")]
    public float fActivateDistance;

    [Units("Unity Game Unit")]
    public float fDeactivateDistance;

    [Units("Unity Game Unit")]
    public float fIMRPDistance;

    bool bIsPinched;

    protected override bool needLeftHand() { return true; }
    protected override bool needRightHand() { return true; }
    protected override bool checkLeftHand() { return this.getLeftHand().PalmNormal.y < 0; }
    protected override bool checkRightHand() { return true; }


    private float GetThumbIndexDistance(Hand hand) {
      Vector3 thumbTipPosition = hand.GetThumb().TipPosition.ToVector3();
      Vector3 indexTipPosition = hand.GetIndex().TipPosition.ToVector3();
      return Vector3.Distance(indexTipPosition, thumbTipPosition);
    }

    private List<float> GetIMRPDistance(Hand hand) {
        Vector3 indexTipPosition = hand.GetIndex().TipPosition.ToVector3();
        Vector3 middleTipPosition = hand.GetMiddle().TipPosition.ToVector3();
        Vector3 ringTipPosition = hand.GetRing().TipPosition.ToVector3();
        Vector3 pinkyTipPosition = hand.GetPinky().TipPosition.ToVector3();

        return new List<float>() { Vector3.Distance(indexTipPosition, middleTipPosition), Vector3.Distance(middleTipPosition, ringTipPosition), Vector3.Distance(ringTipPosition, pinkyTipPosition) };
    }

    protected override void processGestures() {
        if (!this.GetRobot().IsCurrentArm(RobotBehaviours.Arms.Clamp)) return;
        
        if (!bIsPinched && this.GetThumbIndexDistance(this.getRightHand()) < fActivateDistance && GetIMRPDistance(this.getRightHand()).All(x => x < this.fIMRPDistance)) {
            // TODO: Close the claw
            print("Claw closes");
            bIsPinched = true;
        }
        else if (bIsPinched && this.GetThumbIndexDistance(this.getRightHand()) > fDeactivateDistance) {
            // TODO: Open the claw
            print("Clawn opens");
            bIsPinched = false;
        }
    }

    protected override void processOthers() { return; }

}
