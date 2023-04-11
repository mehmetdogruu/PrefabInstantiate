using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CameraMove : MonoBehaviour
{
    float mainSpeed = 5.0f; //regular speed
    float shiftAdd = 25.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 100.0f; //Maximum speed when holdin gshift
    public Vector2 turn;
    public float sens = .5f;
    public Vector3 deltaMove;
    public float camSpeed;
    private float totalRun = 1.0f;

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            turn.x += Input.GetAxis("Mouse X") * sens;
            turn.y += Input.GetAxis("Mouse Y") * sens;
            transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);

            deltaMove = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * camSpeed * Time.deltaTime;
            transform.Translate(deltaMove);

        }

        //Mouse  camera angle done.  

        //Keyboard commands

        Vector3 p = GetBaseInput();
            if (p.sqrMagnitude > 0)
            { // only move while a direction key is pressed
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    totalRun += Time.deltaTime;
                    p = p * totalRun * shiftAdd;
                    p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                    p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                    p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
                }
                else
                {
                    totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                    p = p * mainSpeed;
                }

                p = p * Time.deltaTime;
                Vector3 newPosition = transform.position;
                if (Input.GetKey(KeyCode.Space))
                { //If player wants to move on X and Z axis only
                    transform.Translate(p);
                    newPosition.x = transform.position.x;
                    newPosition.z = transform.position.z;
                    transform.position = newPosition;
                }
                else
                {
                    transform.Translate(p);
                }
            }
        }
     
    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;


     }
    }
