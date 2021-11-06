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

    public int getArm() { return this.currentHandQuadrant; }

    public void TurnRight() { this.currentHandQuadrant = (this.currentHandQuadrant+1) % 4; }

    public void TurnLeft() { this.currentHandQuadrant = (this.currentHandQuadrant+3) % 4; }

    public bool IsCurrentArm(Arms arm) { return (int) arm == this.getArm(); }

    public GameObject circleObject;
    public float RotationSpeed;

    public float robotRotationSpeed;

    public float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        tTextElement.text = "Arm: " + Enum.GetName(typeof(Arms), getArm());
        circleObject.transform.localRotation  = Quaternion.Slerp(circleObject.transform.localRotation , Quaternion.Euler(0, this.currentHandQuadrant*90, 0), Time.deltaTime * RotationSpeed);
        
    }

    public void Rotate(Quaternion rotation) {
        transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rotation, Time.deltaTime * robotRotationSpeed);
    }

    public void Move(Vector3 direction) {
        this.transform.Translate(Vector3.Slerp(Vector3.zero, direction, Time.deltaTime * movementSpeed), relativeTo:Space.Self);
    }
}
