using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;

public abstract class Gesture
{
    private GameObject RigidRoundHand_L; // left hand gameobject

    private GameObject RigidRoundHand_R; // right hand gameobject
    private Controller controller;
    private Hand hLeftHand; // left hand (detection)
    private Hand hRightHand; // right hand (detection)
    
    public void startSetup(GameObject left, GameObject right, Controller controller) {
        this.RigidRoundHand_L = left;
        this.RigidRoundHand_R = right;
        this.controller = controller;
    }

    protected Hand getLeftHand() {
        return this.hLeftHand;
    }

    protected Hand getRightHand() {
        return this.hRightHand;
    }

    /**
    Test the gesture and process the result
    */
    public void testGesture() {
        this.updateHands();

        if((!this.needLeftHand() || (this.getLeftHand() != null && this.checkLeftHand()))
         && (!this.needRightHand() || (this.getRightHand() != null && this.checkRightHand()))) {
             this.processGestures();
         }

    }

    protected abstract void processGestures();

    private void updateHands() {
        Frame fFrame = controller.Frame ();
        hRightHand = null;
        hLeftHand = null;
        if (fFrame.Hands.Count > 0){
            List<Hand> hands = fFrame.Hands;
            foreach (Hand hand in hands) if (hand.IsRight) hRightHand = hand; else if (hand.IsLeft) hLeftHand = hand;
        }
    }

    protected bool needLeftHand() {
        return false;
    }

    protected bool needRightHand() {
        return false;
    }

    protected bool checkLeftHand() {
        return true;
    }

    protected bool checkRightHand() {
        return true;
    }

    
    ///<summary>States
    ///PointingState.Either | PointingState.Extended | PointingState.NotExtended
    ///</summary>
    public bool checkExtendedFingers(Hand hand, PointingState Thumb, PointingState Index, PointingState Middle, PointingState Ring, PointingState Pinky) {
        return matchFingerState(hand.Fingers[0], Thumb)
            && matchFingerState(hand.Fingers[1], Index)
            && matchFingerState(hand.Fingers[2], Middle)
            && matchFingerState(hand.Fingers[3], Ring)
            && matchFingerState(hand.Fingers[4], Pinky);
    }

    
    ///<summary>Copied from ExtendedFingerDetector
    ///PointingState.Either | PointingState.Extended | PointingState.NotExtended
    ///</summary>
    private bool matchFingerState (Finger finger, PointingState requiredState) {
      return (requiredState == PointingState.Either) ||
             (requiredState == PointingState.Extended && finger.IsExtended) ||
             (requiredState == PointingState.NotExtended && !finger.IsExtended);
    }

}
