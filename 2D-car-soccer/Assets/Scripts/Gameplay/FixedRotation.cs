using UnityEngine;

public class FixedRotation : MonoBehaviour
{
    [Header("UI Positionierung")]
    [Tooltip("Wie viel Einheiten über dem Auto soll die Leiste im Weltraum schweben?")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.5f, 0f); 

    private Transform carTransform;

    void Start()
    {
        carTransform = transform.parent;
    }

    void LateUpdate()
    {
        if (carTransform != null)
        {
            transform.position = carTransform.position + worldOffset;
            transform.rotation = Quaternion.identity;
        }
    }
}