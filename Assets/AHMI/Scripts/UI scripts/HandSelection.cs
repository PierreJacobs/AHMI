using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class HandSelection : Gesture
{

    public GameObject LeftHandedText;
    public GameObject RightHandedText;

    protected override bool needLeftHand() { return true; }
    protected override bool needRightHand() { return true; }
    protected override bool checkLeftHand() { return true; }
    protected override bool checkRightHand() { return true; }

    protected override void processGestures() {
        return ;
    }

    protected override void processOthers() { return; }

}
