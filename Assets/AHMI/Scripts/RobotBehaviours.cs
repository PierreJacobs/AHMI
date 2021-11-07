using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RobotBehaviours : MonoBehaviour
{

    public enum Arms { Clamp, Welder, Hammer, Unused }
    public int currentHandQuadrant = 0;
    public Text tTextElement;

    public GameObject circleObject;
    public float armRotationSpeed;

    public float robotRotationSpeed;
    public float movementSpeed;

    // Update is called once per frame
    void Update()
    {
        tTextElement.text = "Arm: " + Enum.GetName(typeof(Arms), getArm());
        circleObject.transform.localRotation  = Quaternion.Slerp(circleObject.transform.localRotation , Quaternion.Euler(0, this.currentHandQuadrant*90, 0), Time.deltaTime * armRotationSpeed); 
    }

    ///<summary>
    /// Rotates the robot according to given Euler.Quaternion
    ///</csummary>
    public void Rotate(Quaternion rotation) { transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rotation, Time.deltaTime * robotRotationSpeed); }

    ///<summary>
    /// Moves the robot according to given vector.
    /// The movement is relative to the current rotation
    ///</csummary>
    public void Move(Vector3 direction) { this.transform.Translate(Vector3.Slerp(Vector3.zero, direction, Time.deltaTime * movementSpeed), relativeTo:Space.Self); }

    ///<summary>
    /// Returns the currently used arm
    ///</csummary>
    public int getArm() { return this.currentHandQuadrant; }

    ///<summary>
    /// Selects the arm to the right
    ///</csummary>
    public void TurnRight() { this.currentHandQuadrant = (this.currentHandQuadrant+1) % 4; }

    ///<summary>
    /// Selects the arm to the left
    ///</csummary>
    public void TurnLeft() { this.currentHandQuadrant = (this.currentHandQuadrant+3) % 4; }

    ///<summary>
    /// Checks that given arm is current arm
    ///</csummary>
    public bool IsCurrentArm(Arms arm) { return (int) arm == this.getArm(); }
}
