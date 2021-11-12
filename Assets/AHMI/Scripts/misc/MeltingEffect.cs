using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltingEffect : MonoBehaviour
{
    Material drawMaterial;
    
    int count = -1; //index of the last melting point
    float lastCollision;
    float lastPower = 0.0f; //new melting power will start at this value

   
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        drawMaterial = GetComponent<Renderer>().material;
        collisionEvents = new List<ParticleCollisionEvent>();
        Vector4[] array = new Vector4[20];
        drawMaterial.SetVectorArray("_Coordinate", array); // init positions array in shader
        drawMaterial.SetInt("_CoordinatesCount", 0);
        drawMaterial.SetFloatArray("_Power", new float[20]); // init powers array in shader
    }

    void Update() {
        float[] powers = drawMaterial.GetFloatArray("_Power");
        for(int index = 0; index < drawMaterial.GetInt("_CoordinatesCount"); index++) {
            float power = powers[index];
            if(index == count && lastCollision > 0f) { //increases only the current melting power
                if(power < lastPower)
                    power = lastPower;
                power += (0.2f-power*0.2f)*Time.deltaTime; // soft cap to 1.0f
                
            }
            else if(power > 0) {
                power -= 0.1f*Time.deltaTime; //decreases melting power if no recent collision or not the last melting point
            }
            if(index == count)
                lastPower = power; //new melting power will start at this value
            powers[index] = power; //set the current melting power in the loop.
        }
        drawMaterial.SetFloatArray("_Power", powers); //send melting powers to the shader
        if(lastCollision > 0f)
            lastCollision -= Time.deltaTime;
    }

    ///<summary>
    /// Send collision position to the shader when a particle system collide with this object
    ///</summary>
    void OnParticleCollision(GameObject other)
    {
        if(lastCollision > 0f)
            return;
        int numCollisionEvents = other.GetComponent<ParticleSystem>().GetCollisionEvents(gameObject, collisionEvents);
        if (numCollisionEvents > 0)
        {        
 
            Vector3 vector = collisionEvents[0].intersection;
            Vector4[] array = drawMaterial.GetVectorArray("_Coordinate");
            count = (count+1)%20;
            array[count] = vector; //set last position to current collision
            drawMaterial.SetVectorArray("_Coordinate",array); //send positions to shader
            
            int ccount = drawMaterial.GetInt("_CoordinatesCount");
            if(ccount < 20)
                drawMaterial.SetInt("_CoordinatesCount", ccount+1);
                          
            lastCollision = 0.1f;   
        }
    }
}
