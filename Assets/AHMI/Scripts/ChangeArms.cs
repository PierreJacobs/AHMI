using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class ChangeArms : MonoBehaviour
{

    bool bIsLeftPalmUp;
    public int update_rate;
    public int xVelocity;
    int tick;

    Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        this.bIsLeftPalmUp = false;
        controller = new Controller();
        tick = update_rate;
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

        if (hRightHand != null && this.bIsLeftPalmUp && tick <= 0 && hRightHand.PalmNormal.x < -0.5) {
            
            if (hRightHand.PalmVelocity.x < -xVelocity) { 
                print("Move Done!!! To the left"); 
                tick = update_rate; 
                //TODO: Interact with robot
                }

            else if (hRightHand.PalmVelocity.x > xVelocity) {
                 print("Move Done!!! To the right"); 
                 tick = update_rate; 
                //TODO: Interact with robot
            }
        }

        tick--;

    }

    public void LeftPalmUp() { this.bIsLeftPalmUp = true; }
    public void LeftPalmNotUp() { this.bIsLeftPalmUp = false; }

}