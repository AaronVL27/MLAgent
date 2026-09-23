using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;

public class AgenteLaLleva : Agent
{
    [SerializeField] GameObject oponente;
    [SerializeField] GameObject hojaLaLleva;
    [SerializeField] float velocidad;
    [SerializeField] float enfriamiento;
    float ultimaColision;
    Rigidbody rb;
    [SerializeField] bool lalleva;
    public bool Lalleva { get => lalleva; set => lalleva = value; }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.localPosition = new Vector3(Random.Range(-4, 4), 0.5f, Random.Range(-4, 4));
        oponente.transform.localPosition = new Vector3(Random.Range(-4, 4), 0.5f, Random.Range(-4, 4));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(oponente.transform.localPosition);

        sensor.AddObservation(oponente.GetComponent<Rigidbody>().linearVelocity);
        sensor.AddObservation(rb.linearVelocity);

        sensor.AddObservation(lalleva ? 1.0f : 0.0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float movimientoX = actions.ContinuousActions[0];
        float movimientoY = actions.ContinuousActions[1];

        Vector3 fuerza = new Vector3(movimientoX, 0, movimientoY);
        rb.AddForce(fuerza * velocidad);

        float distanciaObjetivo = Vector3.Distance(transform.localPosition, oponente.transform.localPosition);

        if (Lalleva && distanciaObjetivo > 1.5f)
        {
            AddReward(0.002f);
        }
        else if (Lalleva && distanciaObjetivo <= 1.5f) 
        {
            AddReward(-0.001f);
        }
        if (!Lalleva && distanciaObjetivo < 1.5f)
        {
            AddReward(0.002f);
        }
        else if (!Lalleva && distanciaObjetivo > 1.5f)
        {
            AddReward(-0.001f);
        }
        if (transform.position.y < 0)
        {
            AddReward(-1.0f);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Agente") && !lalleva)
        {
            AgenteLaLleva otherAgente = collision.gameObject.GetComponent<AgenteLaLleva>();

            if (Time.time >= ultimaColision + enfriamiento)
            {
                otherAgente.Lalleva = false;
                otherAgente.ultimaColision = Time.time;
                otherAgente.LaLlevaIndicadorVisual();

                lalleva = true;
                ultimaColision = Time.time;
                LaLlevaIndicadorVisual();

                AddReward(1.0f);
            }
        }
    }

    public void LaLlevaIndicadorVisual()
    {
        hojaLaLleva.SetActive(lalleva);
    }
}