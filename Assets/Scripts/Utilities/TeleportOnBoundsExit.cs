using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class TeleportOnBoundsExit : MonoBehaviour
{
    private Collider _collider;

    void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    void OnTriggerExit(Collider col)
    {
        //GetComponent<Asteroid>()?.Despawn(); // debug asteroids exploding on border

        transform.position = Game.Wrap.GetReEntryPosition(transform.position, _collider.bounds);       
    }

}
