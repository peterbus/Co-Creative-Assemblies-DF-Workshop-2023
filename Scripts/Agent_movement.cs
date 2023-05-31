using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_movement : MonoBehaviour
{


    Vector3 startPos_cube;
    public Transform transform_cube;
    float step;
    float distance;

    private void Awake()
    {
        startPos_cube = transform_cube.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveLeftRight();
        MoveUpDown();
    }

    public void MoveLeftRight()
    {

        step = 0.01f;
        if (Input.GetKey(KeyCode.L))
        {
            transform_cube.transform.position += new Vector3(step, 0, 0); //shifting the cube in one direction          
        }
        if (Input.GetKey(KeyCode.J))
        {
            transform_cube.transform.position += new Vector3(-step, 0, 0); //shifting the cube in one direction
        }
    }

    public void MoveUpDown()
    {
        step = 0.01f;

        if (Input.GetKey(KeyCode.I))
        {
            transform_cube.transform.position += new Vector3(0, step, 0); //shifting the cube in one direction
        }

        if (Input.GetKey(KeyCode.M))
        {
            transform_cube.transform.position += new Vector3(0, -step, 0); //shifting the cube in one direction
        }

    }


}
