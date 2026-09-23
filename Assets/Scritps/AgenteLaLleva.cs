using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
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

    float distanciaAnterior;

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

        distanciaAnterior = Vector3.Distance(transform.localPosition, oponente.transform.localPosition);
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
        float deltaDistancia = distanciaObjetivo - distanciaAnterior;

        if (lalleva)
        {
            // se alejó → recompensa; se acercó → castigo
            AddReward(deltaDistancia * 0.05f);
        }
        else
        {
            // se acercó → recompensa; se alejó → castigo
            AddReward(-deltaDistancia * 0.05f);
        }

        distanciaAnterior = distanciaObjetivo;
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
                otherAgente.AddReward(-1.0f);

                lalleva = true;
                ultimaColision = Time.time;
                LaLlevaIndicadorVisual();
                AddReward(1.0f);

                EndEpisode();
                otherAgente.EndEpisode();
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            AddReward(-0.003f);
        }
    }

    public void LaLlevaIndicadorVisual()
    {
        hojaLaLleva.SetActive(lalleva);
    }
}