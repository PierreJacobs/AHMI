using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class RotationGesture : Gesture
{

    public GameObject point;
    public float minRadius;
    public float maxRadius;
    protected override bool needLeftHand() { return false; }
    protected override bool needRightHand() { return true; }
    protected override bool checkRightHand() { 
        float distance = Vector3.Distance(unityVector(this.getRightHand().PalmPosition), point.transform.position);
        return distance >= minRadius && distance <= maxRadius;
    }

    protected override void processGestures() {
        Vector3 rotationVector = unityVector(this.getRightHand().PalmPosition) - point.transform.position;
        this.GetRobot().Rotate(Quaternion.Euler(rotationVector));   
    }

    protected override void processOthers() { return; }


    private Vector3 unityVector(Leap.Vector vector) {
        return new Vector3(vector.x, vector.y, vector.z);
    }
}
