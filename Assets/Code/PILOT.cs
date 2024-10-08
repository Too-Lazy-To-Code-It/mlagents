using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;

public class PILOT : Agent
{
    [SerializeField] private Material _lOSE;
    [SerializeField] private Material _win;

    [SerializeField] private MeshRenderer floorMesh;

    [SerializeField] private Transform spawnObject;

    [SerializeField] private Transform goalObject;

    private Rigidbody _rigidObject;

    private Vector3 _spawnArea;



    public override void Initialize()
    {
        _rigidObject = GetComponent<Rigidbody>();
        _spawnArea = spawnObject.GetComponent<MeshRenderer>().bounds.size;
        ResetPosition();
    }

    public void ResetPosition()
    {
        goalObject.localPosition = new Vector3(Random.Range(-_spawnArea.x / 2, _spawnArea.x / 2), _spawnArea.y + 0.5f, Random.Range(-_spawnArea.z / 2, _spawnArea.z / 2));
        transform.localPosition = new Vector3(Random.Range(-_spawnArea.x / 2, _spawnArea.x / 2), _spawnArea.y + 0.5f, Random.Range(-_spawnArea.z / 2, _spawnArea.z / 2));
    }

    public override void OnEpisodeBegin()
    {
        ResetPosition();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_spawnArea);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goalObject.localPosition);
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var moveDir = Vector3.zero;
        var moveAgent = act[0];

        switch (moveAgent)
        {
            case 1:
                moveDir = transform.right;
                break;
            case 2:
                moveDir = -transform.right;
                break;
            case 3:
                moveDir = transform.forward;
                break;
            case 4:
                moveDir = -transform.forward;
                break;
        }
        _rigidObject.MovePosition(_rigidObject.position+moveDir*4.0f*Time.deltaTime);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        AddReward(0.001f);
        MoveAgent(actions.DiscreteActions);

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Goal"))
        {
            AddReward(+1.0f);
            floorMesh.material=_win;
        }
        else if(other.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.01f);
                        floorMesh.material=_lOSE;

        }
        
        
    }

}
