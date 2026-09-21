using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;

    [Header("Enemigo")]
    [SerializeField] private TagController enemigo;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float movimientoX = 0f;
        float movimientoZ = 0f;

        if (Keyboard.current.aKey.isPressed)
            movimientoX = -1f;
        else if (Keyboard.current.dKey.isPressed)
            movimientoX = 1f;

        if (Keyboard.current.wKey.isPressed)
            movimientoZ = 1f;
        else if (Keyboard.current.sKey.isPressed)
            movimientoZ = -1f;

        Vector3 movimiento = new Vector3(
            movimientoX,
            0f,
            movimientoZ
        ).normalized;

        rb.linearVelocity = movimiento * velocidad;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            enemigo.PlayerTocoEnemigo();
        }
    }
}
