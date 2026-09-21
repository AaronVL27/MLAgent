using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;
public class AgenteBuscador : Agent
{
    [SerializeField] GameObject objetivo;
    [SerializeField] float velocidad;
    Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.localPosition = new Vector3(Random.Range(-4, 4), 0.5f, Random.Range(-4, 4));
        objetivo.transform.localPosition = new Vector3(Random.Range(-4, 4), 0.5f, Random.Range(-4, 4));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(objetivo.transform.localPosition);

        sensor.AddObservation(rb.linearVelocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float movimientoX = actions.ContinuousActions[0];
        float movimientoY = actions.ContinuousActions[1];

        Vector3 fuerza = new Vector3(movimientoX, 0, movimientoY);
        rb.AddForce(fuerza * velocidad);

        AddReward(0.0001f);

        float distanciaObjetivo = Vector3.Distance(transform.localPosition, objetivo.transform.localPosition);

        if (distanciaObjetivo < 1.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if (transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var accionesConinuas = actionsOut.ContinuousActions;

        if (Keyboard.current.aKey.isPressed)
        {
            accionesConinuas[0] = -1.0f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            accionesConinuas[0] = 1.0f;
        }
        if (Keyboard.current.wKey.isPressed)
        {
            accionesConinuas[1] = 1.0f;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            accionesConinuas[1] = -1.0f;
        }

    }
}