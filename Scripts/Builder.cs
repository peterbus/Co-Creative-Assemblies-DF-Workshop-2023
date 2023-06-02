using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;


public class Builder : Agent
{
    public Transform training_environment;
    [SerializeField] private GameObject builder;
    //Prefab definition in case of matching assembly (Mode2)
    [SerializeField] private GameObject Prefab;
    [SerializeField] private GameObject trainingPrefab;
    public Vector3 prefabSize = new Vector3(1, 2, 1);
    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private List<GameObject> trainingAssemblies = new List<GameObject>();
    private HashSet<Vector3> assembledPositions = new HashSet<Vector3>();
    private int totalpositions = 50;
    public Vector3 cellsize;
    public Vector3 gridSize;

    Rigidbody agentRigidbody;

    public Create dCreate;
    public float speed;
    GameObject activePrefab;
    public List<Transform> assemblyPositions;

    //reference collision detector
    ColliderDetector cd;

    private void Awake()
    {
        agentRigidbody = builder.GetComponent<Rigidbody>();
        cd = GetComponent<ColliderDetector>();

    }
    public override void OnEpisodeBegin()
    {
        dCreate.SetupScene();

        int[] array = new int[] { 0, 1, 2, 3, 4, 5 };
        int ranNum = array[UnityEngine.Random.Range(0, array.Length)];
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-ranNum, -ranNum), UnityEngine.Random.Range(ranNum, ranNum), UnityEngine.Random.Range(-ranNum, ranNum));
        var environment = gameObject.transform.parent.gameObject;
        activePrefab = GameObject.FindWithTag("activePrefab");

        if (instantiatedPrefabs.Count > 0)
        {
            foreach (GameObject prefab in instantiatedPrefabs)
            {
                Destroy(prefab);
            }
        }


        if (trainingAssemblies.Count > 0)
        {
            foreach (GameObject prefab in trainingAssemblies)
            {
                Destroy(prefab);
            }
        }


        instantiatedPrefabs.Clear();//the created components during the building process - Mode1
        assembledPositions.Clear();//positions that are filled out when assembling pre-defined assembly - Mode2
        trainingAssemblies.Clear(); //random assemblies for training purposes to train an agent filling out the space 

        createRandomAssembly();
  

    }
    public override void CollectObservations(VectorSensor sensor)

    {
        Vector3 objectPosition = dCreate.GetPosition();//vector of the active Prefab
       //Debug.Log(objectPosition);
       //Mode1 - there will be 6 observations and 3 actions
       //observation of the newly added component
        /*sensor.AddObservation(objectPosition.x);
          sensor.AddObservation(objectPosition.y);
          sensor.AddObservation(objectPosition.z);*/


        sensor.AddObservation(builder.transform.localPosition.x);//position of the learning agent
        sensor.AddObservation(builder.transform.localPosition.y);//position of the learning agent
        sensor.AddObservation(builder.transform.localPosition.z);//position of the learning agent


        //Mode2 = 150 observations in case we have 50 positions to be assembled!
        foreach (Vector3 assemblyPos in assembledPositions)
        {
            sensor.AddObservation(assemblyPos.x);//position of the learning agent
            sensor.AddObservation(assemblyPos.y);//position of the learning agent
            sensor.AddObservation(assemblyPos.z);//position of the learning agent
        }
    }

    public override void OnActionReceived(ActionBuffers actions) //move agent, this is the definition of the random movement in case of default-standard self-learning process
    {
        MoveAgent(actions.ContinuousActions);

    }

    //the principle is to always follow masterbuilder's clicks to re-create/imitate any assembly provided by the masterbuilder. Masterbuilder's assemblies can be created in real time during the inference assembly process with the generated brain.
    public override void Heuristic(in ActionBuffers actionsOut)//this is the setup of controls for a demonstrator to position an agent component, which is then observed 
                                                               //also don't forget for a Decision requester script to add!
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Input.GetKey(KeyCode.LeftArrow)) // movement along the x axis negative
        {
            continuousActionsOut[0] = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow)) // movement along the x axis positive
        {
            continuousActionsOut[0] = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow)) // movement along the y axis negative
        {
            continuousActionsOut[1] = -1f;
        }
        else if (Input.GetKey(KeyCode.UpArrow)) // movement along the y axis positive
        {
            continuousActionsOut[1] = 1f;
        }
        else if (Input.GetKey(KeyCode.Home)) // movement along the z axis positive
        {
            continuousActionsOut[2] = 1f;
        }
        else if (Input.GetKey(KeyCode.End)) // movement along the z axis negative
        {
            continuousActionsOut[2] = -1f;
        }
    }

    void FixedUpdate()
    {
        if (gameObject.transform.localPosition.y < -1 || gameObject.transform.localPosition.y > 10) //modify this
        {
            //When the agent goes too far away in vertical direction
            AddReward(-10);
            EndEpisode();


        }

        //When the agent goes beyond the space. It depends of the boundary which determines the assembly space
        if (gameObject.transform.localPosition.x < -7 || gameObject.transform.localPosition.x > 1
            || gameObject.transform.localPosition.z < -1 || gameObject.transform.localPosition.z > 7)
        {
            AddReward(-10);
            EndEpisode();


        }
    }

    public void MoveAgent(ActionSegment<float> act) //builder movement during default training
    {
        var action0 = act[0]; // Mathf.FloorToInt(act[0]); 
        var action1 = act[1];
        var action2 = act[2];

        //initial distance
        float initialDistanceToTarget = Vector3.Distance(activePrefab.transform.localPosition, transform.localPosition);

        float xNew = builder.transform.localPosition.x + (action0 * speed * Time.deltaTime);
        float yNew = builder.transform.localPosition.y + (action1 * speed * Time.deltaTime);
        float zNew = builder.transform.localPosition.z + (action2 * speed * Time.deltaTime);
        builder.transform.localPosition = new Vector3(xNew, yNew, zNew); //movement towards the active_prefab (following the masterbuilder)

        //distance measurement towards the activePrefab, while agent apporaching the desired component
        float distance2target = Vector3.Distance(activePrefab.transform.localPosition, transform.localPosition);
        //if Mode1 is running, switch these on ....move towards the target and according the distance, add some reward      
        /*  if (distance2target < initialDistanceToTarget && initialDistanceToTarget < 2.5f)
          {
              AddReward(0.05f);
          }
          else
          {
              AddReward(-0.5f);//little punishment
          }
        */
        //modify the positions to avoid other objects
    }

    //MODE 1 OF CO-CREATIVE ASSEMBLY - CREATING with AI IN REAL-TIME
    //when the agents collides with the active prefab, it will create a component on the spot where the active prefab is located
    //in this way we can co-create an assembly with AI during the inference training with the generated brain
    //this is not about to make a precise copy of the predefined assemnbly, but rather to create something new and intutitively together with AI.
    /*    private void OnTriggerEnter(Collider other)
         {      
             if (other.gameObject.CompareTag("activePrefab"))//if the agent collides with the active prefab
             {
                 GameObject currentObject = activePrefab;  

                 dCreate.AddOnePrefab(currentObject, currentObject.transform.localPosition);
                 //dCreate.AddComponentAgent();  
                 StartCoroutine(dCreate.RunGrowth());//the same thing with some delay
                // transform.localPosition = currentObject.transform.localPosition;
                 AddReward(0.5f);
            }
         }*/

    //MODE 2 OF CO-CREATIVE ASSEMBLY - AI MATCHING WITH PREDEFINED MASTER BUILDER ASSEMBLY
    //if we want to create a precise assembly according to the human player
    // We need to define the assembly positions created previously by a human player in Inspector by drag & drop the Transform positions (do not forget to lock the inspector first)
    //and train the agent during the heuristic training to collide with the predefined position
    //in this case we will have more observations, as each positions will be stored as a separate observation

    private void OnTriggerEnter(Collider other_Prefab) //when this collider/rigid body will start touching other rigid bodies
    {

        if (other_Prefab.gameObject.CompareTag("Prefab"))
        {

            Vector3 position = other_Prefab.transform.position;
            //Check if the position has already been assembled
            if (!assembledPositions.Contains(position))
            {
                Debug.Log("There is a collision");
                //then call the instantiate function
                GameObject newPrefab = Instantiate(Prefab, position, Quaternion.identity);

                PositionsAssembled(position);
                instantiatedPrefabs.Add(newPrefab);
                Debug.Log("Component added on a masterbuilder position");
                AddReward(1f);
            }

        }

    }

    private void OnTriggerStay(Collider other) //OnTriggerStay is called once per physics update for every Collider other that is touching the trigger(agent).
    {
        if (other.gameObject.CompareTag("Prefab") || other.gameObject.CompareTag("activePrefab"))
        {
            AddReward(-0.05f);
        }
    }

    private void PositionsAssembled(Vector3 position)
    {
        assembledPositions.Add(position);//add to HashSet
        if (assembledPositions.Count == totalpositions)
        {
            //if the entire assembly is completed
            AddReward(3f);//the treat
            EndEpisode();
        }
    }

    //for Mode2, instead of custom predefined assembly
    void createRandomAssembly()//inside the 3d grid
    {
        int gridWidth = Mathf.FloorToInt(gridSize.x / cellsize.x);
        int gridHeight = Mathf.FloorToInt(gridSize.y / cellsize.y);
        int gridDepth = Mathf.FloorToInt(gridSize.z / cellsize.z);

        Vector3 gridOffset = training_environment.position; // -new Vector3(gridSize.x/2f , gridSize.y/2f, gridSize.z/2f);//in case an origin is in the middle of the grid
        //GameObject parentObject = new GameObject("PrefabParent");

        //populate the entire grid with the copmponents and put it in the parent object
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int z = 0; z < gridDepth; z++)

                {
                    Vector3 instPosition = gridOffset + new Vector3(-x * cellsize.x, y * cellsize.y, z * cellsize.z);
                    GameObject instance = Instantiate(trainingPrefab, instPosition, Quaternion.identity, training_environment);
                    instance.transform.SetParent(training_environment);
                    trainingAssemblies.Add(instance);                                                   
                }
            }
        }
     
        int allComponents = gridWidth * gridHeight * gridDepth;//amount of all components in the grid=the grid size
        int rest = allComponents - totalpositions;
        for (int i = 0; i< rest; i++)//only the amount of total positions will remain, all the others will be destroyed
        {
            int randomIndex = Random.Range(0, trainingAssemblies.Count);
            GameObject prefabtoDestroy = trainingAssemblies[randomIndex];
            trainingAssemblies.RemoveAt(randomIndex);
            Destroy(prefabtoDestroy);
        }
       
    }

}

