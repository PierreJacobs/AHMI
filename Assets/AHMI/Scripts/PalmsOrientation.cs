using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmsOrientation : MonoBehaviour
{

    bool bIsLeftPalmUp = false;
    
    public void LeftPalmUp() { this.bIsLeftPalmUp = true; }
    public void LeftPalmNotUp() { this.bIsLeftPalmUp = false; }

    public bool IsLeftPalmUp() { return this.bIsLeftPalmUp; }
}
