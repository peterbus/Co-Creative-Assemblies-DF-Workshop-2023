//using the libraries or external tools
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Google.Protobuf.WellKnownTypes;
using UnityEditor;

public class Agent_builder : Agent //declaration of our building agent class
{
    //global variables and references to other classes
    public Create create;
    public Masterbuilder masterbuilder;
    Rigidbody agent_body;//rigid body declaration, it is useful if collides with other objects in the environments, like prefabs, etc...
    public Transform environment;
    GameObject activePrefab;
    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    public float speed = 3f;//try to modify this in the inspector 

    public void Awake()
    {
        //activate the agent
        agent_body = GetComponent<Rigidbody>();

    }

    //initialise the variables and position of the agent
    public override void OnEpisodeBegin()
    {
        
        //calling the Create class with its settings
        create.SetupScene();

        //set the random positiion of the agent within certain range at the beginning of each episode
        int[] array = new int[] { 0, 1, 2, 3, 4, 5, 6 };
        int ranNUm = array[UnityEngine.Random.Range(0, array.Length)];
        //define it via Vector3
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-ranNUm, ranNUm), UnityEngine.Random.Range(ranNUm, ranNUm), UnityEngine.Random.Range(-ranNUm, ranNUm));
        transform.parent = environment.transform;//parent Game object definition = definition of the hierarchy in Unity hierarchy window
       
        //lokalize or activate the active prefab via tag
        activePrefab = GameObject.FindWithTag("activePrefab");

        //delete all previously created objects in the previous episode, it uses the Unity's Destroy method
        if (instantiatedPrefabs.Count > 0)
        {
            foreach (GameObject prefab in instantiatedPrefabs)
            {
                Destroy(prefab);
            }
        }
        //and clear up the list of objects
        instantiatedPrefabs.Clear();

    }

    //each training starts with observations, using the default ML agent tool method here
    public override void CollectObservations(VectorSensor sensor)
    {
        //what are we going to observe? The position of the agent itself and the position of the active prefab.
        //this serves as an input information for the training scenario, the agent has to learn,
        //that this is something we need to learn to find a relationship between those two objects. 

        //this is the lokalisation of the active prefab taken from Create class from GetPosition method
        Vector3 objectPosition = create.GetPosition();
        //observing each x y z values
        sensor.AddObservation(objectPosition.x);
        sensor.AddObservation(objectPosition.y);
        sensor.AddObservation(objectPosition.z);


        //position of the agent itself, again x y z values
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(transform.localPosition.z);

    }

    //What to do, what kind of action the agent has to execute. Movig!
    public override void OnActionReceived(ActionBuffers actions)
    {
        //definition what the agent has to execute, all is defined in the MoveAgent method down below
        MoveAgent(actions.ContinuousActions);//standard ML Agent tool syntax
    }


    //Heuristic method defines the controls when the training is in the heuristic mode
    //(not then default one)
    //The human player can control the agent positions via keyboard in this case
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //we are adding certain floating value in order to move with the agent 
        //to each directions. According to predefined ContinuousActions [0] [1] and [2]
        //in MoveAgent method, we are adding values to each of x y z direction 
        var continuousActionsOut = actionsOut.ContinuousActions;

        //based on input keys on a keyboard
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            continuousActionsOut[0] = -1f; //add minus 1 value, etc...
            //minus value means oposite direction than plus values

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            continuousActionsOut[0] = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))

        {
            continuousActionsOut[1] = -1f;

        }
        else if (Input.GetKey(KeyCode.UpArrow))

        {
            continuousActionsOut[1] = 1f;

        }
        else if (Input.GetKey(KeyCode.Home))

        {
            continuousActionsOut[2] = 1f;
        }
        else if (Input.GetKey(KeyCode.End))

        {
            continuousActionsOut[2] = -1f;

        }
    }


    private void FixedUpdate()//modification of the standard Unity method Update()
    {
        //statements about the agent position,
        //that is being updated during the execution of the training or while in a gameplay mode
        if (transform.localPosition.y < -1 || transform.localPosition.y > 10)
        {
            //adding a negative reward if the agent goes outside of the boundary (bounding box) of the environment in Y direction(up and down)
            AddReward(-10f);
            //and end the episode and start from the beginning
            EndEpisode();

        }

        if (transform.localPosition.x < -8 || transform.localPosition.x > -1
            || (transform.localPosition.z < -1 || transform.localPosition.z > 8))
                    
        {
            //adding the negative reward if the agent goes outside of the boundaries in X and Z directions(right, left, forward, backward)
            AddReward(-10f);
            //end the process and start from the beginning
            EndEpisode();

        }

    }

    public void MoveAgent(ActionSegment<float> act)
    {

        //here we are defining the movement of the agent. IN the default training,
        //the agent moves in time randomly in each directions as he wishes. 
        //then, while randomly moving, he learns from the environment certain logic (policy)
        //and tries to minimise the negative reward and maximise the positive reward.
        //In that way it learns.


        //syntax for ML Agent tool, definiton of the actions. We have 3.
        var action0 = act[0];
        var action1 = act[1];
        var action2 = act[2];

        //declaration of the variable
        //distance measurement before the agent moves
        float initialDistanceToTarget = Vector3.Distance(activePrefab.transform.localPosition, transform.localPosition);//distance between the agent and an active prefab

        //declaration of the changes of the position (movement) based on speed and time.
        //If the speed is higher, the agents moves more faster.
        //The speed is defined in the global variables at the beginning of this script or via inspector,
        //if this is public variable (accessible through the interface in Unity). 

        //add to the current position certain floating value, based on the speed and time (movement of the GameObject in Unity)
        float xNew = transform.localPosition.x+  (action0 * speed*Time.deltaTime);
        float yNew = transform.localPosition.y + (action1 * speed * Time.deltaTime);
        float zNew = transform.localPosition.z + (action2 * speed * Time.deltaTime);
        
        //update of the agent's position
        transform.localPosition = new Vector3(xNew, yNew, zNew); 


        //measure the distance again, after the movement
         float distanceToTarget = Vector3.Distance(activePrefab.transform.localPosition, transform.localPosition);

        //and according to the distance towards the active prefab (our target), we define the reward
        if (distanceToTarget< initialDistanceToTarget && initialDistanceToTarget < 3.5f)
        {
            //if if gets closer and closer within certain range of 3.5f - you can modify this value
            AddReward(0.05f);//add a reward

        }else
        {
            //if the agent goes away from the target or it is outside of the range
            AddReward(-0.05f);//adding a punishment.
        }

    }

    //what happens if the agent collides with the active prefab
    private void OnTriggerEnter(Collider other)
    {
        //if there is a collison between the agent and the active prefab
        if (other.gameObject.CompareTag("activePrefab"))
        {
            //declaration of the variable for the activePrefab
            GameObject currentObject = activePrefab;
            //calling the create method for adding additional building block on the exact 
            //position where the active prefab is located = also the location of the agent is the same
            create.AddOneprefab(currentObject, currentObject.transform.position);
            //calling the StartCoroutine method from the Create class
            StartCoroutine(create.RunGrowth());

            //and add some reward as the agent reached the desired target=active prefab.
            AddReward(0.5f);

        }
        
    }


}
