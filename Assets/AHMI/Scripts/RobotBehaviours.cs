using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RobotBehaviours : MonoBehaviour
{

    public enum Arms { Hammer, Unused, Clamp, Welder }
    public int currentHandQuadrant = 0;

    public Text tTextElement;

    public int getArm() { return this.currentHandQuadrant; }

    public void TurnRight() { this.currentHandQuadrant = (this.currentHandQuadrant+1) % 4; }

    public void TurnLeft() { this.currentHandQuadrant = (this.currentHandQuadrant+3) % 4; }

    public bool IsCurrentArm(Arms arm) { return (int) arm == this.getArm(); }

    public GameObject circleObject;
    public float RotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tTextElement.text = "Arm: " + Enum.GetName(typeof(Arms), getArm());
        circleObject.transform.rotation = Quaternion.Slerp(circleObject.transform.rotation, Quaternion.Euler(0, this.currentHandQuadrant*90, 0), Time.deltaTime * RotationSpeed);
    }
}
