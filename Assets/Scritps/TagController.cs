using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.InferenceEngine;

public class TagController : Agent
{
    [Header("Objetivo")]
    [SerializeField] private GameObject player;

    [Header("Redes neuronales")]
    [SerializeField] private ModelAsset modeloPerseguir;
    [SerializeField] private ModelAsset modeloHuir;

    [Header("Configuración")]
    [SerializeField] private string behaviorName = "Tag";

    private Rigidbody rb;

    // true = persigue
    // false = huye
    private bool persiguiendo = true;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();

        // Comenzamos persiguiendo al jugador
        CambiarModelo(modeloPerseguir);
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(player.transform.localPosition);
        sensor.AddObservation(rb.linearVelocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float movimientoX = actions.ContinuousActions[0];
        float movimientoZ = actions.ContinuousActions[1];

        Vector3 movimiento = new Vector3(
            movimientoX,
            0f,
            movimientoZ
        );

        rb.AddForce(movimiento * 10f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var acciones = actionsOut.ContinuousActions;

        acciones[0] = 0f;
        acciones[1] = 0f;
    }

    private void CambiarModelo(ModelAsset nuevoModelo)
    {
        SetModel(
            behaviorName,
            nuevoModelo,
            InferenceDevice.Default
        );
    }

    // El enemigo toca al Player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CambiarAHuir();
        }
    }

    // El Player toca al enemigo
    public void PlayerTocoEnemigo()
    {
        CambiarAPerseguir();
    }

    private void CambiarAHuir()
    {
        if (!persiguiendo)
            return;

        persiguiendo = false;

        CambiarModelo(modeloHuir);

        Debug.Log("IA → AHORA HUYE");
    }

    private void CambiarAPerseguir()
    {
        if (persiguiendo)
            return;

        persiguiendo = true;

        CambiarModelo(modeloPerseguir);

        Debug.Log("IA → AHORA PERSIGUE");
    }
}
