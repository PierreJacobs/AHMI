using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class ProcessGestures : MonoBehaviour
{

    public GameObject RigidRoundHand_L; // left hand gameobject
    public GameObject RigidRoundHand_R; // right hand gameobject
    
    public Gesture[] gestures;
    // Start is called before the first frame update

    private Controller controller;
    public GameObject Robot;

    void Start()
    {
        this.controller = new Controller();
        foreach(Gesture gesture in gestures) {
            gesture.startSetup(this.RigidRoundHand_L, this.RigidRoundHand_R, this.controller, Robot.GetComponent<RobotBehaviours>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Gesture gesture in gestures) {
            gesture.testGesture();
        }
    }
}
