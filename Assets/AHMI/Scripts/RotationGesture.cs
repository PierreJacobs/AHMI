using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class RotationGesture : Gesture
{ 
    public float fErrorAngle = 10.0f;
    public float fMaxPalmVelocity = 200.0f;
    public Vector vIdlingArea = new Vector(0.3f, 0.3f, 0);
    public float fYIdlingOffset = -0.2f; // Finger is naturally pointing down

    protected override bool needLeftHand() { return false; }
    protected override bool needRightHand() { return true; }
    protected override bool checkRightHand() { 
        return this.checkMatchFingers(this.hRightHand, PointingState.Extended, PointingState.Extended, PointingState.NotExtended, PointingState.NotExtended, PointingState.NotExtended) 
        && this.isSlower3D(this.hRightHand.PalmVelocity, this.fMaxPalmVelocity); 
    }

    protected override void processGestures() {
        
        Vector vIndexDirection = this.hRightHand.Fingers[1].Direction;

        if (!this.isIdlingArea(vIndexDirection)) {
            float xAngle = computeXAngle(vIndexDirection);
            float yAngle = computeYAngle(vIndexDirection);

            // Comment the if-else statement if you want to move diagonally (but is uncontrollable)
            if (Math.Abs(xAngle) > Math.Abs(yAngle)) yAngle = 0.0f;
            else xAngle = 0.0f;

            /* From leapmotion to unity:
                xAxis [Leapmotion] = yAxis [Unity]
                yAxis [Leapmotion] = zAxis [Unity] and this one is reverted

                Hence the weird-looking function call
            */
            this.Robot.Rotate(Quaternion.Euler(0, xAngle, -yAngle));
        }

    }

    ///<summary>
    /// Computes the X agnle of the current index (finger) position from a direction vector
    /// The referential is a pointing index, pointing forward. If the finger goes the the right, the X angle is positive, else it is negative
    ///</summary>
    private float computeXAngle(Vector vIndexDirection) { 
        int xAngleSign = vIndexDirection.x < 0 ? -1: 1;
        float xAngle = this.radToDeg((float) Math.Asin(Math.Abs(vIndexDirection.x))) * xAngleSign;
        return (Math.Abs(xAngle) < fErrorAngle) ? 0f : xAngle;
    }

    ///<summary>
    /// Computes the Y agnle of the current index (finger) position from a direction vector
    /// The referential is a pointing index, pointing forward. If the finger goes up, the Y angle is positive, else it is negative
    ///</summary>
    private float computeYAngle(Vector vIndexDirection) {
        int yAngleSign = this.OffsetYPosition(vIndexDirection.y) < 0 ? -1 : 1; 
        float yAngle = this.radToDeg((float) Math.Asin(Math.Abs(this.OffsetYPosition(vIndexDirection.y)))) * yAngleSign;
        return (Math.Abs(yAngle) < fErrorAngle) ? 0f: yAngle;
    }

    protected override void processOthers() { return; }
    
    ///<summary>
    /// handles the index naturally pointing down position
    ///</summary>
    private float OffsetYPosition(float y) { return y - this.fYIdlingOffset; }
    
    ///<summary>
    /// Computes a little zone where the index points, in which nothing happens => idling
    ///</summary>
    private bool isIdlingArea(Vector vector) { return Math.Abs(vector.x) < vIdlingArea.x && Math.Abs(this.OffsetYPosition(vector.y)) < vIdlingArea.y; }

    ///<summary>
    /// Converts an rad angle to degrees
    ///</summary>
    private float radToDeg(float rad) { return rad * (180 / (float) Math.PI); }

}
