using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class HammerMovement : MonoBehaviour
{

    bool bIsRightHandClose = false;
    public int yVelocity;

    public float fRestTime;
    float fElaspedTime;
    Controller controller;
    public GameObject RigidRoundHand_L;
    PalmsOrientation PalmsOrientationScript;

    // Start is called before the first frame update
    void Start()
    {

        fElaspedTime = 0;

        controller = new Controller();
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

        if (hRightHand != null && !PalmsOrientationScript.IsLeftPalmUp() && fElaspedTime <= 0 && this.bIsRightHandClose) {
            if (hRightHand.PalmVelocity.y < -yVelocity) {
                //TODO: make hammer go down
                print("Hammer goes down");
                fElaspedTime = fRestTime;
            }
            else if (hRightHand.PalmVelocity.y > (yVelocity/2)) {
                //TODO: make hammer go up
                print("Hammer does up");
                fElaspedTime = fRestTime;
            }
        }

        fElaspedTime -= Time.deltaTime;
    }

    public void PalmClose() { this.bIsRightHandClose = true; }
    public void PalmOpen() { this.bIsRightHandClose = false; }
}
