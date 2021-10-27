using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class ChangeArms : MonoBehaviour
{

    public int xVelocity;
    
    public float fRestTime;
    float fElaspedTime;

    Controller controller;

    public GameObject Robot;
    RobotBehaviours RobotScript;

    public GameObject RigidRoundHand_L;
    PalmsOrientation PalmsOrientationScript;

    // Start is called before the first frame update
    void Start()
    {
        fElaspedTime = 0;
        controller = new Controller();

        this.RobotScript = Robot.GetComponent<RobotBehaviours>();
        this.PalmsOrientationScript = RigidRoundHand_L.GetComponent<PalmsOrientation>();
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

        if (hRightHand != null && this.PalmsOrientationScript.IsLeftPalmUp() && fElaspedTime <= 0 && hRightHand.PalmNormal.x < -0.5) {
            
            if (hRightHand.PalmVelocity.x < -xVelocity) { 
                fElaspedTime = fRestTime;
                RobotScript.TurnLeft();
            }

            else if (hRightHand.PalmVelocity.x > xVelocity) {
                fElaspedTime = fRestTime;
                RobotScript.TurnRight();
            }
        }

        fElaspedTime -= Time.deltaTime;
    }

}