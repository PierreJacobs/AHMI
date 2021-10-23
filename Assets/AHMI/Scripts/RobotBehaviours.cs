using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBehaviours : MonoBehaviour
{
    public int current_hand = 0;

    public int getHand() {
        return this.current_hand;
    }

    public void TurnRight() {
        this.current_hand = (this.current_hand+1) % 4;
    }

    public void TurnLeft() {
        this.current_hand = (this.current_hand-1) % 4;
    }

    public GameObject circleObject;
    public float RotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        circleObject.transform.rotation = Quaternion.Slerp(circleObject.transform.rotation, Quaternion.Euler(0, this.current_hand*90, 0), Time.deltaTime * RotationSpeed);
    }
}
