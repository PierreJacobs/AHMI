using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;
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
    protected override bool checkLeftHand() { return this.hLeftHand.PalmNormal.y < 0; }
    protected override bool checkRightHand() { return true; }

    ///<summary>
    /// Computes the distance between the thumb and the index of the given hand
    ///</summary>
    private float GetThumbIndexDistance(Hand hand) {
      Vector3 thumbTipPosition = hand.GetThumb().TipPosition.ToVector3();
      Vector3 indexTipPosition = hand.GetIndex().TipPosition.ToVector3();
      return Vector3.Distance(indexTipPosition, thumbTipPosition);
    }

    ///<summary>
    /// Computes the distance between the index and the middle finger, between the middle finger and the ring finger, between the ring finger and the pinky if the given hand
    ///</summary>
    private List<float> GetIMRPDistance(Hand hand) {
        Vector3 indexTipPosition = hand.GetIndex().TipPosition.ToVector3();
        Vector3 middleTipPosition = hand.GetMiddle().TipPosition.ToVector3();
        Vector3 ringTipPosition = hand.GetRing().TipPosition.ToVector3();
        Vector3 pinkyTipPosition = hand.GetPinky().TipPosition.ToVector3();

        return new List<float>() { Vector3.Distance(indexTipPosition, middleTipPosition), Vector3.Distance(middleTipPosition, ringTipPosition), Vector3.Distance(ringTipPosition, pinkyTipPosition) };
    }

    protected override void processGestures() {
        if (!this.Robot.IsCurrentArm(RobotBehaviours.Arms.Clamp)) return;
        
        if (!bIsPinched && this.GetThumbIndexDistance(this.hRightHand) < fActivateDistance && GetIMRPDistance(this.hRightHand).All(x => x < this.fIMRPDistance)) {
            print("Claw closes");
            this.Animator.Play("Close Clamp");
            bIsPinched = true;
        }
        else if (bIsPinched && this.GetThumbIndexDistance(this.hRightHand) > fDeactivateDistance) {
            print("Clawn opens");
            this.Animator.Play("Open Clamp");
            bIsPinched = false;
        }
    }

    protected override void processOthers() { return; }

}
