using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class Controller : Agent
{
    Rigidbody rBody;
    void Start () {
        rBody = GetComponent<Rigidbody>();
    }
//mlagents-learn config/floater_config.yaml --run-id=a
    public Transform Target;
    public float multiplier;
    public float rewardCounter;
    public override void OnEpisodeBegin()
    {
       // If the Agent fell, zero its momentum
       Debug.Log("New");
        if (this.transform.localPosition.y < 0)
        {
            this.transform.localPosition = new Vector3( 0, 0.5f, 0);
            this.rBody.velocity = Vector3.zero;
        }
        if (this.transform.localPosition.y > 100)
        {
            this.transform.localPosition = new Vector3( 0, 0.5f, 0);
            this.rBody.velocity = Vector3.zero;
        }
        
        rewardCounter=0f;
        // Move the target to a new spot
        Target.localPosition = new Vector3(0f,
                                           Random.value * multiplier+0.5f,
                                           0f);
    }
public override void CollectObservations(VectorSensor sensor)
{
    sensor.AddObservation(this.transform.localPosition-Target.localPosition);
    // Target and Agent positions
    //sensor.AddObservation(Target.localPosition);

    // Agent velocity
    sensor.AddObservation(rBody.velocity.y);
}

public float forceMultiplier = 100;
public override void OnActionReceived(ActionBuffers actionBuffers)
{
    // Actions, size = 2
    Vector3 controlSignal = Vector3.zero;
    controlSignal.y = actionBuffers.ContinuousActions[0];
    rBody.AddForce(controlSignal * forceMultiplier);

    // Rewards
    float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
   // Debug.Log(rewardCounter);
    
    
    if (distanceToTarget < 0.1f)
    {
        SetReward(10f);
        rewardCounter=rewardCounter+10f;
    }
    else{
        SetReward(-2f);
        rewardCounter=rewardCounter-2f;
    }
    /*
    
    if (distanceToTarget < 1.42f)
    {
        SetReward(10.0f);
        EndEpisode();
    }
    */

    // Fell off platform
    if (this.transform.localPosition.y < 0f)
    {
        EndEpisode();
    }

    if (this.transform.localPosition.y > 100f)
    {
        EndEpisode();
    }
    
    if (rewardCounter>1000f){
        EndEpisode();
    }
    if (rewardCounter<-5000f){
        EndEpisode();
    }
    
    

}
public override void Heuristic(in ActionBuffers actionsOut)
{
    var continuousActionsOut = actionsOut.ContinuousActions;
    continuousActionsOut[0] = Input.GetAxis("Vertical");

}
}