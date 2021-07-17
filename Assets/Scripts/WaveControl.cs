using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveControl : MonoBehaviour
{
    /*



    transform rotationcenter;


    [Header("")]

    [Header("origin of Wave (only y is important)")]
    Vector2 talloffset,shallowoffset;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
    [Header("Wave objects")]
    public GameObject tallwave, shallowwave;
    public GameObject camera; 
    public Vector2 xcomp = new Vector2(1, 0);

    [Range(0, 5)] 
    public float RotateSpeedtall = 1f;
    [Range(0, 5)]
    public float Radiustall = 1f;

    [Range(0, 5)] 
    public float RotateSpeedshallow = 1f;
    [Range(0, 5)]
    public float Radiusshallow = 1f;

    [Range(0, 350)]
    public float shallowangleoffset; //relevant für Start
    private Vector2 origin;
    private Vector2 originwcam;
    private float angletall;
    private float angleshallow;

    private void Start()
    {
        origin = tallwave.transform.position;
        angleshallow = angletall + shallowangleoffset;
    }

    private void Update()
    {     
        //get x from Camera
        //origin += Velocity * Time.deltaTime;
        originwcam = origin + (xcomp *camera.transform.position.x);

        angletall += RotateSpeedtall * Time.deltaTime;
        angleshallow += RotateSpeedshallow * Time.deltaTime;

        if(angletall > 360){
            angletall = 0;
        }
        if(angleshallow > 360){
            angleshallow = 0;
        }


        var offsettall = new Vector2(Mathf.Sin(angletall), Mathf.Cos(angletall)) * Radiustall;
        var offsetshallow = new Vector2(Mathf.Sin(angleshallow), Mathf.Cos(angleshallow)) * Radiusshallow;


        tallwave.transform.position = originwcam + offsettall;
        shallowwave.transform.position = originwcam + offsetshallow;
        //add both waves
    }

}
