using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunChosenHand : MonoBehaviour
{
    public static void loadScene(bool righthand) {
        ChosenHand.righthand = righthand;
        SceneManager.LoadScene("TheExpanse");
    }
}
