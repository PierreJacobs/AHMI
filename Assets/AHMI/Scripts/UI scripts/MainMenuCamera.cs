using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    Quaternion targetRotation;

    void Start() { targetRotation = Random.rotation; }

    void Update()
    {

        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        float timeToComplete = angle / 2f;
        float donePercentage = Mathf.Min(1F, Time.deltaTime / timeToComplete);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, donePercentage);

        if (Quaternion.Angle(transform.rotation, targetRotation) <= 1.0f) targetRotation = Random.rotation;
    }

    float random(float min, float max) { return Random.Range(min, max) * Random.Range(0,2)*2-1; }
}
