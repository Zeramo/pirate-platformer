using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skiff : MonoBehaviour
{
    public float destinationDistance = 50f;
    public float speed = 15f;
    Rigidbody2D rigidBody;

    [Header("Sideward floating movement properties")]
    public float amplitude = 5;
    public float period = 2;

    float time = 0f;
    float addSpeedValue = 0f;
    float prevAddSpeed = 0f;

    float orgXPos;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        rigidBody.freezeRotation = true;
        rigidBody.mass = 500f;

        orgXPos = transform.position.x;
    }

    private void Update()
    {
        if (!GameManager.exitStatus())
            return;

        time += Time.deltaTime;

        addSpeedValue = amplitude * 1 / 3 * Mathf.Sin(period * time);

        if (orgXPos - destinationDistance > transform.position.x)
            speed = 0f;

        rigidBody.velocity = new Vector2(-speed + addSpeedValue - prevAddSpeed, 0);

        prevAddSpeed = addSpeedValue;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            GameManager.PlayerWon();
    }

    //Function to drive the boat up to allow the player to jump on it
    public void allowAccess()
    {
        time += Time.deltaTime;

        addSpeedValue = amplitude * 1 / 3 * Mathf.Sin(period * time);

        if (orgXPos - destinationDistance > transform.position.x)
            speed = 0f;

        rigidBody.velocity = new Vector2(-speed + addSpeedValue - prevAddSpeed, 0);

        prevAddSpeed = addSpeedValue;
    }
}
