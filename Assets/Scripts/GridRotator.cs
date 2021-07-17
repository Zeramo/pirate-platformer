using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GridRotator : MonoBehaviour
{
    [Header("Properties of sinus function")]
    public float amplitude = 5;
    public float period = 2;

    float time = 0f;                                //Time passed to sin function to calculate rotation, additive

    float rotValue = 0f;
    float prevRot = 0f;                             //To store previous rotation
    

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }
    // Update is called once per frame
    void Update()
    {
        //Remove when a time limit is implemented
        if (time > Mathf.PI/period * 999f)
            time = 0f;
        //Add time since last Update() to time
        time += Time.deltaTime;

        //Calculate rotation value as sin function
        rotValue = amplitude * Mathf.Sin(period * time);

        //Rotate by difference between current and previous value.
        //Looking at a graph for visual representation is helpful for a better understanding.
        transform.Rotate(0, 0, rotValue-prevRot);

        //Save as previous rotation
        prevRot = rotValue;
    }

}
