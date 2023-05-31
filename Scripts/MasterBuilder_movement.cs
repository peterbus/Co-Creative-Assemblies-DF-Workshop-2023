using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterBuilder_movement : MonoBehaviour
{


    Vector3 startPos_cube;
    public Transform transform_cube;

    public float step=0.25f;
    public float snapDistance = 0.25f;
    Vector3 targetPosition;
    Vector3 startPosition;
    bool moving;


    private void Awake()
    {
        //startPos_cube = transform_cube.position;
        startPosition = transform_cube.position;
    }

    // Update is called once per frame
    void Update()
    {
        // MoveLeftRight();
        // MoveUpDown();

        if (moving)
        {
            if (Vector3.Distance(startPosition, transform_cube.transform.position) > 1.1f)
            {
                transform_cube.position = targetPosition;
                moving = false;
                return;
            }

            transform_cube.transform.position += (targetPosition - startPosition) * step * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.L))
        {
            targetPosition = transform_cube.transform.position + Vector3.forward;
            startPosition = transform_cube.transform.position;
            moving = true;
        }
        else if (Input.GetKey(KeyCode.J))
        {
            targetPosition = transform_cube.transform.position + Vector3.back;
            startPosition = transform_cube.transform.position;
            moving = true;
        }
        else if (Input.GetKey(KeyCode.I))
        {
            targetPosition = transform_cube.transform.position + Vector3.left;
            startPosition = transform_cube.transform.position;
            moving = true;
        }
        else if (Input.GetKey(KeyCode.M))
        {
            targetPosition = transform_cube.transform.position + Vector3.right;
            startPosition = transform_cube.transform.position;
            moving = true;
        }

    
    }       
    

  /*               
      void MoveLeftRight() 
      {
    

        step = 0.025f;


        if (Input.GetKey(KeyCode.L))
        {
            transform_cube.transform.position += new Vector3(step, 0, 0); //shifting the cube in one direction
                                                                        
        }
        if (Input.GetKey(KeyCode.J))
        {
            transform_cube.transform.position += new Vector3(-step, 0, 0); //shifting the cube in one direction
            
     
        }
    
    
    
      }

    void MoveUpDown()
    {
        step = 0.025f;

        if (Input.GetKey(KeyCode.I))
        {
            transform_cube.transform.position += new Vector3(0, step, 0); //shifting the cube in one direction
        }

        if (Input.GetKey(KeyCode.M))
        {
            transform_cube.transform.position += new Vector3(0, -step, 0); //shifting the cube in one direction
        }

    }*/


}
