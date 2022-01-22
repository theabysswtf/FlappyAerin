using UnityEngine;
using UnityEngine.Events;

public class Hittable : MonoBehaviour
{
    [SerializeField] UnityEvent onHit;
    [SerializeField] UnityEvent onReset;

    void OnTriggerEnter2D(Collider2D other)
    {
        onHit.Invoke();
    }

    public void ResetObject()
    {
        onReset.Invoke();
    }
}
