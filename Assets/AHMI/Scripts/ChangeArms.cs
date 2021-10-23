using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class ChangeArms : MonoBehaviour
{

    bool bIsLeftPalmUp;
    public int xVelocity;
    
    public float fRestTime;
    float fElaspedTime;

    Controller controller;

    public GameObject Robot;
    RobotBehaviours RobotScript;

    // Start is called before the first frame update
    void Start()
    {
        this.bIsLeftPalmUp = false;
        fElaspedTime = 0;
        controller = new Controller();
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

        if (hRightHand != null && this.bIsLeftPalmUp && fElaspedTime <= 0 && hRightHand.PalmNormal.x < -0.5) {
            
            if (hRightHand.PalmVelocity.x < -xVelocity) { 
                print("Move Done!!! To the left"); 
                fElaspedTime = fRestTime;
                //TODO: Interact with robot
                RobotScript.TurnLeft();
                }

            else if (hRightHand.PalmVelocity.x > xVelocity) {
                 print("Move Done!!! To the right"); 
                 fElaspedTime = fRestTime;
                //TODO: Interact with robot
                RobotScript.TurnRight();
            }
        }

        fElaspedTime -= Time.deltaTime;
    }

    public void LeftPalmUp() { this.bIsLeftPalmUp = true; }
    public void LeftPalmNotUp() { this.bIsLeftPalmUp = false; }

}