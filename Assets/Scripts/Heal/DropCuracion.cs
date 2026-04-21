using UnityEngine;

public class DropCuracion : MonoBehaviour
{
    [SerializeField] private GameObject itemCuracion;
    [SerializeField] private int cantidadDrops = 1;
    [SerializeField] private float rangoDispersion = 2f;

    public void DropearItem()
    {
        if (itemCuracion == null)
        {
            Debug.LogError("Item de curacion no asignado");
            return;
        }

        for (int i = 0; i < cantidadDrops; i++)
        {
            Vector3 posicionAleatoria = transform.position + new Vector3(
                Random.Range(-rangoDispersion, rangoDispersion),
                Random.Range(0, rangoDispersion),
                0
            );

            Instantiate(itemCuracion, posicionAleatoria, Quaternion.identity);
        }

        Debug.Log("Items dropeados: " + cantidadDrops);
    }
}