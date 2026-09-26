using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class AgenteLaLleva : Agent
{
    [Header("Referencias de Componentes")]
    public Transform objetivoTransform;
    public MeshRenderer miRenderer;
    public MeshRenderer rendererJugador;
    public Material materialPerseguidor;
    public Material materialEvasor;

    [Header("Configuración de Roles")]
    public bool esPerseguidor = true;
    public float velocidad = 5f;
    private Rigidbody rb;
    private Vector3 posicionInicialAgente;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        posicionInicialAgente = transform.localPosition;
        ActualizarAspectoRol();
    }

    public override void OnEpisodeBegin()
    {
        // Reiniciar posiciones relativas a la Arena
        transform.localPosition = posicionInicialAgente;
        rb.linearVelocity = Vector3.zero;

        if (objetivoTransform != null)
        {
            objetivoTransform.localPosition = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4f, 4f));
        }

        // Aleatorizar el rol al inicio de cada intento (50% de probabilidad)
        esPerseguidor = Random.value > 0.5f;
        ActualizarAspectoRol();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. Posición Local del Agente - 3 valores (X, Y, Z)
        sensor.AddObservation(transform.localPosition);

        // 2. Posición Local del Objetivo - 3 valores (X, Y, Z)
        sensor.AddObservation(objetivoTransform.localPosition);

        // 3. Rol Actual (Booleano) - 1 valor
        sensor.AddObservation(esPerseguidor);

        // Total de observaciones (Space Size) = 3 + 3 + 1 = 7
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Acciones continuas dadas por la red neuronal
        float moverX = actions.ContinuousActions[0];
        float moverZ = actions.ContinuousActions[1];

        Vector3 movimiento = new Vector3(moverX, 0, moverZ);
        rb.AddForce(movimiento * velocidad);

        // Lógica de recompensas según el rol
        if (esPerseguidor)
        {
            // Castigo de tiempo para obligarlo a atrapar rápido
            AddReward(-1f / MaxStep);
        }
        else
        {
            // Pequeña recompensa por sobrevivir cada paso mientras huye
            AddReward(1f / MaxStep);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Control manual para probar físicas con el teclado
        var accionesContinuas = actionsOut.ContinuousActions;
        ; ; ; ;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Agente"))
        {
            if (esPerseguidor)
            {
                SetReward(1.0f); // ¡Atrapó al jugador! Premio mayor.
            }
            else
            {
                SetReward(-1.0f); // ¡Fue atrapado! Castigo mayor.
            }
            EndEpisode(); // Finaliza la ronda para reiniciar posiciones
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Castigo por chocar torpemente contra la pared
            AddReward(-0.1f);
        }
    }

    private void ActualizarAspectoRol()
    {
        if (miRenderer != null && rendererJugador != null)
        {
            miRenderer.material = esPerseguidor ? materialPerseguidor : materialEvasor;
            rendererJugador.material = esPerseguidor ? materialEvasor : materialPerseguidor;
        }
    }


}