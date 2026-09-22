using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjetivoController : MonoBehaviour
{
    [SerializeField] Transform objetivo;
    float speed = 6f;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, objetivo.position, speed * Time.deltaTime);
    }
}
