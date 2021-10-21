using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeArms : MonoBehaviour
{

    bool bIsLeftPalmUp;
    bool bHasRightPalmBeenNorthWest;
    bool bIsRightPalmNotNorthWest;

    // Start is called before the first frame update
    void Start()
    {
        
        // init variables
        this.bIsLeftPalmUp = false;
        this.bHasRightPalmBeenNorthWest = false;
        this.bIsRightPalmNotNorthWest = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LeftPalmUp() { this.bIsLeftPalmUp = true; }
    public void LeftPalmNotUp() { this.bIsLeftPalmUp = false; }

    public void RightPalmNorthWest() { 
        if (this.bIsLeftPalmUp) { 
            this.bHasRightPalmBeenNorthWest = true; 
            this.bIsRightPalmNotNorthWest = false;
        }
    }

    public void RightPalmNotNorthWest() { 
        this.bIsRightPalmNotNorthWest = true; 
    }

    public void RightPalmSouthWest() {
        if (this.bIsLeftPalmUp && this.bHasRightPalmBeenNorthWest && this.bIsRightPalmNotNorthWest) {
            this.bHasRightPalmBeenNorthWest = false;

            //TODO: Make the robot change arm
            print("MOVE DONE");
        }
    }

}
