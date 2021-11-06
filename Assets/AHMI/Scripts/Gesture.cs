using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;

public abstract class Gesture : MonoBehaviour
{
    private GameObject RigidRoundHand_L; // left hand gameobject

    private GameObject RigidRoundHand_R; // right hand gameobject
    private Controller controller;
    private Hand hLeftHand; // left hand (detection)
    private Hand hRightHand; // right hand (detection)
    private RobotBehaviours RobotScript;
    private Animator anim;
    
    public void startSetup(GameObject left, GameObject right, Controller controller, RobotBehaviours robotScript, Animator anim) {
        this.RigidRoundHand_L = left;
        this.RigidRoundHand_R = right;
        this.controller = controller;
        this.RobotScript = robotScript;
        this.anim = anim;
    }

    protected RobotBehaviours GetRobot() { return this.RobotScript; }
    protected Hand getLeftHand() { return this.hLeftHand; }
    protected Hand getRightHand() { return this.hRightHand; }

    protected Animator getAnimator() { return this.anim; }

    /**
    Test the gesture and process the result
    */
    public void testGesture() {
        this.updateHands();
        
        if ((!this.needLeftHand() || (this.getLeftHand() != null && this.checkLeftHand()))
         && (!this.needRightHand() || (this.getRightHand() != null && this.checkRightHand()))) {
            this.processGestures();
        }
        this.processOthers();
    }

    protected abstract void processGestures();
    protected abstract void processOthers();

    private void updateHands() {
        Frame fFrame = controller.Frame ();
        hRightHand = null;
        hLeftHand = null;
        if (fFrame.Hands.Count > 0){
            List<Hand> hands = fFrame.Hands;
            foreach (Hand hand in hands) if (hand.IsRight) hRightHand = hand; else if (hand.IsLeft) hLeftHand = hand;
        }
    }

    protected virtual bool needLeftHand() { return false; }
    protected virtual bool needRightHand() { return false; }
    protected virtual bool checkLeftHand() { return true; }
    protected virtual bool checkRightHand() { return true; }

    
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

    protected Vector3 unityVector(Leap.Vector vector) { return new Vector3(vector.x, vector.y, vector.z); } 

    // Checks that the palm is not moving to fast (to diffentiate this gesture with other gestures)
    protected bool isSlower3D(Vector vector, float fMaxVelocity) { return (Math.Abs(vector.x) < fMaxVelocity) && (Math.Abs(vector.y) < fMaxVelocity) && (Math.Abs(vector.z) < fMaxVelocity); }

    protected bool IsHandClosed(Hand hand) { return this.checkExtendedFingers(this.getRightHand(), PointingState.NotExtended, PointingState.NotExtended, PointingState.NotExtended, PointingState.NotExtended, PointingState.NotExtended); }
    
}
