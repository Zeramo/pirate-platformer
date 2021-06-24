using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background : MonoBehaviour
{
    public GameObject go;
    float distanceFromCamera = 12.5f;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 centerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distanceFromCamera));
        go.transform.position = centerPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
