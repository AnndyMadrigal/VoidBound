using UnityEngine;

public class MovimientoLlave : MonoBehaviour
{
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float distancia = 0.5f;

    private Vector3 posicionInicial;

    private void Start()
    {
        posicionInicial = transform.position;
    }

    private void Update()
    {
        float nuevoY = posicionInicial.y + Mathf.Sin(Time.time * velocidad) * distancia;
        transform.position = new Vector3(posicionInicial.x, nuevoY, posicionInicial.z);
    }
}