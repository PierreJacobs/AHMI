using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;

/// <summary>
/// Class <c>Gesture</c> provides a blueprint for creating gesture recognition
/// Functions that can be overriden:
///     + protected void <c>processGestures()</c>
///     + protected void <c>processOthers()</c>
///     + protected bool <c>needLeftHand()</c>
///     + protected bool <c>needRightHand()</c>
///     + protected bool <c>checkLeftHand()</c>
///     + protected bool <c>checkRightHand()</c>
/// </summary>
public abstract class Gesture : MonoBehaviour
{
    private GameObject RigidRoundHand_L;                    // left hand gameobject
    private GameObject RigidRoundHand_R;                    // right hand gameobject
    private Controller controller;                          // leapmotion controller
    protected Hand hLeftHand { get; private set; }          // left hand (on controller detection)
    protected Hand hRightHand { get; private set; }         // right hand (on controller detection)
    protected RobotBehaviours Robot { get; private set; }   // robot script to control movement and rotation
    protected Animator Animator { get; private set; }       // animator to play animations on screen

    protected bool bIsRightHanded;                          // Left-Handed or Right-Handed
    
    ///<summary>
    /// Setup function, similar to a constructor
    ///</summary>
    public void startSetup(GameObject left, GameObject right, Controller controller, RobotBehaviours robotScript, Animator anim) {
        this.RigidRoundHand_L = left;
        this.RigidRoundHand_R = right;
        this.controller = controller;
        this.Robot = robotScript;
        this.Animator = anim;
        this.bIsRightHanded = ChosenHand.righthand;
    }

    ///<summary>
    /// Checks if the requirements for a gesture to be recognised are met. If so, process the gesture using <c>processGestures()</c>.
    /// After the check, performs other actions (that are not linked to the gesture processing) using <c>processOthers()</c>
    ///</summary>
    public void testGesture() {
        this.updateHands();
        
        if ((!this.needLeftHand() || (this.hLeftHand != null && this.checkLeftHand()))
         && (!this.needRightHand() || (this.hRightHand != null && this.checkRightHand()))) this.processGestures();
        this.processOthers();
    }

    ///<summary>
    /// Abstract method than can be implemented to process a gesture
    ///</summary>
    protected abstract void processGestures();
    
    ///<summary>
    /// Abstract method than can be implemented to process other things not related to the gesture
    /// Example: the time elasped between two gestures
    ///</summary>
    protected abstract void processOthers();

    ///<summary>
    /// Checks if the left hand and the right hand are visible on the controller
    ///</summary>
    private void updateHands() {
        Frame fFrame = controller.Frame();
        hRightHand = null;
        hLeftHand = null;

        if (fFrame.Hands.Count > 0){
            List<Hand> hands = fFrame.Hands;
            foreach (Hand hand in hands) 
                if ((hand.IsRight && this.bIsRightHanded) || (hand.IsLeft && !this.bIsRightHanded)) hRightHand = hand; 
                else hLeftHand = hand; 
        }
    }

    ///<summary>
    /// Virtual method that can be overriden that tells whether the left hand is needed to perfrom the gesture
    ///</summary>
    protected virtual bool needLeftHand() { return false; }

    ///<summary>
    /// Virtual method that can be overriden that tells whether the right hand is needed to perfrom the gesture
    ///</summary>
    protected virtual bool needRightHand() { return false; }

    ///<summary>
    /// Virtual method that can be overriden that tells whether the conditions on the left hand are met to perform the gesture
    /// Override this method only if the left hand is necessary
    ///</summary>
    protected virtual bool checkLeftHand() { return true; }

    ///<summary>
    /// Virtual method that can be overriden that tells whether the conditions on the right hand are met to perform the gesture
    /// Override this method only if the right hand is necessary
    ///</summary>
    protected virtual bool checkRightHand() { return true; }

    
    ///<summary>
    /// Checks whether all the fingers on a given hand match the desired states.
    /// Available states: <c>PointingState.Either</c> | <c>PointingState.Extended</c> | <c>PointingState.NotExtended</c>
    ///</summary>
    protected bool checkMatchFingers(Hand hand, PointingState Thumb, PointingState Index, PointingState Middle, PointingState Ring, PointingState Pinky) {
        return matchFingerState(hand.Fingers[0], Thumb)
            && matchFingerState(hand.Fingers[1], Index)
            && matchFingerState(hand.Fingers[2], Middle)
            && matchFingerState(hand.Fingers[3], Ring)
            && matchFingerState(hand.Fingers[4], Pinky);
    }

    
    ///<summary>
    /// Copied from ExtendedFingerDetector
    /// <c>PointingState.Either</c> | <c>PointingState.Extended</c> | <c>PointingState.NotExtended</c>
    ///</summary>
    private bool matchFingerState (Finger finger, PointingState requiredState) {
      return (requiredState == PointingState.Either) ||
             (requiredState == PointingState.Extended && finger.IsExtended) ||
             (requiredState == PointingState.NotExtended && !finger.IsExtended);
    }

    ///<summary>
    /// Converts a leapmotion vector to a unity vector
    ///</summary>
    protected Vector3 unityVector(Leap.Vector vector) { return new Vector3(vector.x, vector.y, vector.z); } 

    ///<summary>
    /// Checks that the components of a given vector are all inferior to a given number
    ///</summary>
    protected bool isSlower3D(Vector vector, float fMax) { return (Math.Abs(vector.x) < fMax) && (Math.Abs(vector.y) < fMax) && (Math.Abs(vector.z) < fMax); }

    ///<summary>
    /// Checks if the given hand is closed
    ///</summary>
    protected bool IsHandClosed(Hand hand) { return this.checkMatchFingers(hand, PointingState.NotExtended, PointingState.NotExtended, PointingState.NotExtended, PointingState.NotExtended, PointingState.NotExtended); }
    
}
