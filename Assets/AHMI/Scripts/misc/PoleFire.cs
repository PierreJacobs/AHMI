/*using System.Collections;
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
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleFire : MonoBehaviour
{
    Material drawMaterial;

    bool isSet;
    
    int count = -1;
    float lastCollision;
    float lastPower = 0.2f;

    Vector3 lastPosition;
   
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        drawMaterial = GetComponent<Renderer>().material;
        collisionEvents = new List<ParticleCollisionEvent>();
        Vector4[] array = new Vector4[] { Vector4.zero, Vector4.zero,Vector4.zero,Vector4.zero,Vector4.zero };
        drawMaterial.SetVectorArray("_Coordinate", array);
        drawMaterial.SetInt("_CoordinatesCount", 0);
        drawMaterial.SetFloatArray("_Power", new float[]{0f,0f,0f,0f,0f});
    }

    void Update() {
        float[] powers = drawMaterial.GetFloatArray("_Power");
        for(int index = 0; index < drawMaterial.GetInt("_CoordinatesCount"); index++) {
            //print(powers.Length);
            float power = powers[index];
            if(index == count && lastCollision > 0f) {
                power += (lastPower-power*lastPower)*Time.deltaTime;
                lastPower += 0.5f*Time.deltaTime;
                
            }
            else if(power > 0) {
                power -= 0.1f*Time.deltaTime;
            }
            if(index == count && lastPower > 0.2f)
                lastPower -= 0.4f*Time.deltaTime;
            //else if(lastCollision < -1f) drawMaterial.SetInt("_CoordinatesCount", 1);
            powers[index] = power;
        }
        drawMaterial.SetFloatArray("_Power", powers);
        if(lastCollision > 0f)
            lastCollision -= Time.deltaTime;
    }

    void OnParticleCollision(GameObject other)
    {
        if(lastCollision > 0f)
            return;
        int numCollisionEvents = other.GetComponent<ParticleSystem>().GetCollisionEvents(gameObject, collisionEvents);
        if (numCollisionEvents > 0)
        {        
 
            Vector3 vector = collisionEvents[0].intersection;
            if(lastPosition == null || Vector3.Distance(vector, lastPosition) > 0.15f) {
                Vector4[] array = drawMaterial.GetVectorArray("_Coordinate");
                count = (count+1)%5;
                array[count] = vector;
                drawMaterial.SetVectorArray("_Coordinate",array);
                
                int ccount = drawMaterial.GetInt("_CoordinatesCount");
                if(ccount < 5)
                    drawMaterial.SetInt("_CoordinatesCount", ccount+1);
            
                lastPosition = new Vector3(vector.x,vector.y,vector.z); 
                
            }   
            lastCollision = 0.5f;   
        }
    }
}
