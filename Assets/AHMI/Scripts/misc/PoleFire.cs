using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleFire : MonoBehaviour
{
    Material drawMaterial;

    bool isSet;

    float lastCollision;
    float power;
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        drawMaterial = GetComponent<Renderer>().material;
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void Update() {
        if(lastCollision > 0f) {
            power += (0.2f-power*0.2f)*Time.deltaTime;
            lastCollision -= Time.deltaTime;
        }
        else if(power > 0)
            power -= 0.1f*Time.deltaTime;
        else isSet = false;
        drawMaterial.SetFloat("_Power", power);
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = other.GetComponent<ParticleSystem>().GetCollisionEvents(gameObject, collisionEvents);
        if (numCollisionEvents > 0)
        {      
            if(!isSet) {
                Vector3 vector = collisionEvents[0].intersection;
                drawMaterial.SetVector("_Coordinate", new Vector4(vector.x, vector.y, vector.z, 0));
                isSet = true;
            }
            lastCollision = 0.1f;         
        }
    }
}
