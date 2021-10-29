using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class HammerMovement : MonoBehaviour
{

    bool bIsRightHandClose = false;
    bool bIsRightHandDown = false;
    public int yVelocity;

    public float fRestTime;
    float fElaspedTime;
    Controller controller;

    public GameObject RigidRoundHand_L;
    PalmsOrientation PalmsOrientationScript;

    public GameObject Robot;
    RobotBehaviours RobotScript;

    // Start is called before the first frame update
    void Start()
    {

        fElaspedTime = 0;

        controller = new Controller();
        this.PalmsOrientationScript = RigidRoundHand_L.GetComponent<PalmsOrientation>();
        this.RobotScript = Robot.GetComponent<RobotBehaviours>();
    }

    // Update is called once per frame
    void Update()
    {
        Frame fFrame = controller.Frame ();
        Hand hRightHand = null;

        if (fFrame.Hands.Count > 0){
            List<Hand> hands = fFrame.Hands;
            foreach (Hand hand in hands) if (hand.IsRight) hRightHand = hand;
        }

        if (RobotScript.IsCurrentArm(RobotBehaviours.Arms.Hammer) && hRightHand != null && !PalmsOrientationScript.IsLeftPalmUp() && fElaspedTime <= 0 && this.bIsRightHandClose) {
            if (hRightHand.PalmVelocity.y < -yVelocity && !this.bIsRightHandDown) {
                //TODO: make hammer go down
                print("Hammer goes down");
                fElaspedTime = fRestTime;
                this.bIsRightHandDown = true;
            }
            else if (hRightHand.PalmVelocity.y > (yVelocity/2) && this.bIsRightHandDown) {
                //TODO: make hammer go up
                print("Hammer goes up");
                fElaspedTime = fRestTime;
                this.bIsRightHandDown = false;
            }
        }

        fElaspedTime -= Time.deltaTime;
    }

    public void PalmClose() { this.bIsRightHandClose = true; }
    public void PalmOpen() { this.bIsRightHandClose = false; }
}
