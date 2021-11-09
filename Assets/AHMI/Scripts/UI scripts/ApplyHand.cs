using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyHand : MonoBehaviour
{
    void Start()
    {
        if(!ChosenHand.righthand) {
            GameObject robot = GameObject.Find("robot");
            robot.transform.localScale = new Vector3(-robot.transform.localScale.x, robot.transform.localScale.y, robot.transform.localScale.z);
            robot.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
