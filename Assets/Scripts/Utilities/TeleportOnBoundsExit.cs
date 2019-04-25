using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class TeleportOnBoundsExit : MonoBehaviour
{
    private Collider _collider;
    private IEdgeWrappable _wrappable;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _wrappable = GetComponent<IEdgeWrappable>();
    }

    void OnTriggerExit(Collider col)
    {
        //GetComponent<Asteroid>()?.Despawn(); // debug asteroids exploding on border

        _wrappable.HasWrapped = true;

        transform.position = Game.Wrap.GetReEntryPosition(transform.position, _collider.bounds);       
    }

}
