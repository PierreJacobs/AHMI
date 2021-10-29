using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Attributes;
using Leap.Unity;

public class ChangeArmsGesture : Gesture
{

    float fElaspedTime;

    public int xVelocity;
    public float fRestTime;


    protected override bool needLeftHand() {
        return true;
    }

    protected override bool needRightHand() {
        return true;
    }

    protected new bool checkLeftHand() {
        return this.checkExtendedFingers(this.getLeftHand(), PointingState.Extended, PointingState.Extended, PointingState.Extended, PointingState.Extended, PointingState.Extended) && this.getLeftHand().PalmNormal.y > 0;
    }

    protected new bool checkRightHand() {
        return /* fElaspedTime <= 0 && */ this.getRightHand().PalmNormal.x < -0.5;  
    }

    protected override void processGestures() {

        /*
        Problèmes ici (les conditions viennent de ChangeArm.cs):
        * on ne check pas fElaspedTime <= 0, donc il n'y a aucun cooldown et c'est pour ça que le robot tournait plusieurs fois
        * on ne check pas this.PalmsOrientationScript.IsLeftPalmUp(), donc on ne prend même pas en compte le main gauche (alors qu'on devrait)
        * on ne check pas hRightHand.PalmNormal.x < -0.5, donc on ne prend même pas en compte l'orientation de la main droite
        De ce que je vois, les fonctions checkLeftHand() et checkRightHand() s'occupent de ça, mais ne sont pas appelées.
        */

        //Ce que je rajoute: (avec ça ça devrait marcher normalement)
        if (!this.checkLeftHand() || !this.checkRightHand() || this.fElaspedTime > 0) return;

        if (this.getRightHand().PalmVelocity.x < -xVelocity) { 
                fElaspedTime = fRestTime;
                this.GetRobot().TurnLeft();
        }
        else if (this.getRightHand().PalmVelocity.x > xVelocity) {
                fElaspedTime = fRestTime;
                this.GetRobot().TurnRight();
        }
        
    }

    protected override void processOthers()
    {
        fElaspedTime -= Time.deltaTime;
    }
}
