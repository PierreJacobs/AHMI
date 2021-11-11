using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class HandSelection : Gesture
{

    public GameObject LeftHandedText;
    public GameObject RightHandedText;

    public GameObject LevelChanger;
    private LevelChanger levelChangerScript;

    public float fElevationDiffenrenceFactor = 1.75f;
    public float fWavingTime = 1.0f;
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

        if (this.isHigher(fLeftHandElevation, this.fElevationDiffenrenceFactor, fRightHandElevation)) {
            if (this.bIsRightHand || this.fElapsedTime < -this.fWavingTime) this.LeftHandMinMaxVelocity = new float[2];
            if (this.fElapsedTime < -this.fWavingTime) this.fElapsedTime = 0;

            this.bIsRightHand = false;

            xVelocity = this.hLeftHand.PalmVelocity.x;
            minmaxVelocity = this.LeftHandMinMaxVelocity;
        }
        else if (this.isHigher(fRightHandElevation, this.fElevationDiffenrenceFactor, fLeftHandElevation)) {
            if (!this.bIsRightHand || this.fElapsedTime < -this.fWavingTime) this.RightHandMinMaxVelocity = new float[2];
            if (this.fElapsedTime < -this.fWavingTime) this.fElapsedTime = 0;

            this.bIsRightHand = true;

            xVelocity = this.hRightHand.PalmVelocity.x;
            minmaxVelocity = this.RightHandMinMaxVelocity;
        }

        FIllMinMaxVelocity(xVelocity, minmaxVelocity);
        if (this.isWaving(this.MinMaxVelocity, minmaxVelocity)) { 
            ChosenHand.righthand = this.bIsRightHand;
            levelChangerScript.fadeToScene(1);
        }

    }

    protected override void processOthers() { fElapsedTime -= Time.deltaTime; }

    private bool isHigher(float fFirstElevation, float factor, float fSecondElevation) { return fFirstElevation > fSecondElevation * factor; }
    private void FIllMinMaxVelocity(float xVelocity, float [] lMinMaxVelocity) {
        if (xVelocity < 0 && xVelocity < lMinMaxVelocity[0]) lMinMaxVelocity[0] = xVelocity;
        else if (xVelocity > 0 && xVelocity > lMinMaxVelocity[1]) lMinMaxVelocity[1] = xVelocity;
    }
    private bool isWaving(float [] gMinMaxVelocity, float [] lMinMaxVelocity) { return (gMinMaxVelocity[0] > lMinMaxVelocity[0]) && (gMinMaxVelocity[1] < lMinMaxVelocity[1]); }

}
