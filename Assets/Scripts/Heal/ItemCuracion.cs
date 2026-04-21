using UnityEngine;

public class ItemCuracion : MonoBehaviour
{
    [SerializeField] private int cantidadCuracion = 20;
    [SerializeField] private AudioClip sonidoCuracion;
    [SerializeField] private float velocidadLevitacion = 2f;
    [SerializeField] private float distanciaLevitacion = 0.5f;
    [SerializeField] private float velocidadParpadeo = 3f;
    [SerializeField] private float distanciaMaximaFall = 3f;

    private Vector3 posicionAleatoria;
    private bool tocoPiso = false;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float distanciaRecorrida = 0f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        posicionAleatoria = transform.position;
    }

    private void Update()
    {
        if (tocoPiso)
        {
            Levitar();
            Parpadear();
        }
        else
        {
            distanciaRecorrida += rb.velocity.y * Time.deltaTime;
        }
    }

    private void Levitar()
    {
        float nuevoY = posicionAleatoria.y + Mathf.Sin(Time.time * velocidadLevitacion) * distanciaLevitacion;
        transform.position = new Vector3(posicionAleatoria.x, nuevoY, posicionAleatoria.z);
    }

    private void Parpadear()
    {
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * velocidadParpadeo));
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!tocoPiso)
        {
            tocoPiso = true;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            posicionAleatoria = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HeroHealth heroHealth = other.GetComponent<HeroHealth>();

            if (heroHealth != null)
            {
                heroHealth.Heal(cantidadCuracion);
                Debug.Log("Jugador curado: +" + cantidadCuracion);
            }

            if (sonidoCuracion != null)
            {
                AudioSource.PlayClipAtPoint(sonidoCuracion, transform.position);
            }

            Destroy(gameObject);
        }
    }
}