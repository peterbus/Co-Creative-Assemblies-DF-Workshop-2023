using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid_movement : MonoBehaviour
{

    Vector3 forward = Vector3.zero;
    Vector3 up = Vector3.zero;
    Vector3 down = Vector3.zero;
    Vector3 right = new Vector3(0, 90, 0);
    Vector3 back = new Vector3(0, 180, 0);
    Vector3 left = new Vector3(0, 270, 0);
    Vector3 currentDirection = Vector3.zero;

    Vector3 nextPos, destination, direction;
    float speed = 5f;
    float rayLength = 1f;
    bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        currentDirection = up;
        nextPos = Vector3.forward;
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed*Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.I)) //or GetKey
        {
            nextPos = Vector3.forward;
            currentDirection = forward;
            canMove = true;

        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            nextPos = Vector3.back;
            currentDirection = back;
            canMove = true;

        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            nextPos = Vector3.right;
            currentDirection = right;
            canMove = true;

        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            nextPos = Vector3.left;
            currentDirection = left;
            canMove = true;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            nextPos = Vector3.up*2f;
            currentDirection = up;//rotation vector
            canMove = true;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            nextPos = Vector3.down*2f;
            currentDirection = down;//rotation vector
            canMove = true;
        }


        if (Vector3.Distance(destination, transform.position) <= 0.00001f)//the destination has been reach
        {
            //rotate itself first
            transform.localEulerAngles = currentDirection;
            //update movement
            if (canMove)
            {
                if(Valid())
                {
                    destination = transform.position + nextPos;
                    direction = nextPos; //nextPos= always 1 unit in Unity. So multiply if needed. 
                    canMove = false;
                }                     
            }
        }
    }

    bool Valid()
    {
        Ray myRay = new Ray(transform.position + new Vector3(0, 0.25f, 0), transform.forward);
        RaycastHit hit;
        Debug.DrawRay(myRay.origin, myRay.direction.normalized, Color.red);
        if(Physics.Raycast(myRay,out hit, rayLength))
        {//check whether hitting an obstacle
            if (hit.collider.tag == "Prefab")//avoiding the already existing built prefab
            {
                return false;
            }
        }
        return true;//when never hitting something#
    }
}