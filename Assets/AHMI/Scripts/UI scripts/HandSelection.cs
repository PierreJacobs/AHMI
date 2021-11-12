using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class HandSelection : Gesture
{

    public GameObject LevelChanger;                     // Game object containing the scene changer script
    private LevelChanger levelChangerScript;

    public float fElevationDiffenrenceFactor = 1.75f;   // One hand must be fElevationDiffenrenceFactor times higher than the other
    public float fWavingTime = 1.0f;                    // Waving time in seconds => the hand must perform the waving gesture within 1 sec or it will reset
    private float fElapsedTime;

    private float [] LeftHandMinMaxVelocity = new float[2];
    private float [] RightHandMinMaxVelocity = new float[2];

    public float [] MinMaxVelocity = new float[] {-800, 800};

    private bool bIsRightHand;

    protected override bool needLeftHand() { return true; }
    protected override bool needRightHand() { return true; }
    protected override bool checkLeftHand() { return true; }
    protected override bool checkRightHand() { return true; }

    protected override void processGestures() {

        if (levelChangerScript == null) levelChangerScript = LevelChanger.GetComponent<LevelChanger>();

        float fLeftHandElevation = hLeftHand.PalmPosition.y;
        float fRightHandElevation = hRightHand.PalmPosition.y;

        float xVelocity = 0;
        float [] minmaxVelocity = new float[2];

        // Finds which hand is higher and uses its characteristics for later
        // Left hand is higher
        if (this.isHigher(fLeftHandElevation, this.fElevationDiffenrenceFactor, fRightHandElevation)) {
            if (this.bIsRightHand || this.fElapsedTime < -this.fWavingTime) this.LeftHandMinMaxVelocity = new float[2];
            if (this.fElapsedTime < -this.fWavingTime) this.fElapsedTime = 0;

            this.bIsRightHand = false;

            xVelocity = this.hLeftHand.PalmVelocity.x;
            minmaxVelocity = this.LeftHandMinMaxVelocity;
        }
        // Right hand is higher
        else if (this.isHigher(fRightHandElevation, this.fElevationDiffenrenceFactor, fLeftHandElevation)) {
            if (!this.bIsRightHand || this.fElapsedTime < -this.fWavingTime) this.RightHandMinMaxVelocity = new float[2];
            if (this.fElapsedTime < -this.fWavingTime) this.fElapsedTime = 0;

            this.bIsRightHand = true;

            xVelocity = this.hRightHand.PalmVelocity.x;
            minmaxVelocity = this.RightHandMinMaxVelocity;
        }

        FIllMinMaxVelocity(xVelocity, minmaxVelocity);

        // When the hand is considered waving, changes the scene => goes to TheExpanse.scene
        if (this.isWaving(this.MinMaxVelocity, minmaxVelocity)) { 
            ChosenHand.righthand = this.bIsRightHand;
            levelChangerScript.fadeToScene(1);
        }

    }

    protected override void processOthers() { fElapsedTime -= Time.deltaTime; }

    ///<summary>
    /// Checks first > second * factor
    ///</summary>
    private bool isHigher(float fFirstElevation, float factor, float fSecondElevation) { return fFirstElevation > fSecondElevation * factor; }
    
    
    ///<summary>
    /// Completes the array if the given xVelocity is higher than the highest contained value or lower than the lowest contained value
    ///</summary>
    private void FIllMinMaxVelocity(float xVelocity, float [] lMinMaxVelocity) {
        if (xVelocity < 0 && xVelocity < lMinMaxVelocity[0]) lMinMaxVelocity[0] = xVelocity;
        else if (xVelocity > 0 && xVelocity > lMinMaxVelocity[1]) lMinMaxVelocity[1] = xVelocity;
    }

    ///<summary>
    /// Checks that the lMinMaxVelocity array contains lower and higher values than gMinMaxVelocity
    ///</summary>
    private bool isWaving(float [] gMinMaxVelocity, float [] lMinMaxVelocity) { return (gMinMaxVelocity[0] > lMinMaxVelocity[0]) && (gMinMaxVelocity[1] < lMinMaxVelocity[1]); }

}
