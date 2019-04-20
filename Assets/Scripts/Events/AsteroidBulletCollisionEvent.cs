using Events;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/" + nameof(AsteroidBulletCollisionEvent), fileName = nameof(AsteroidBulletCollisionEvent))]
public class AsteroidBulletCollisionEvent : GameEventBase<(Asteroid Asteroid, Bullet Bullet)>
{
    [SerializeField]
    private bool _debugLogging;

    protected override void OnAfterRaised((Asteroid Asteroid, Bullet Bullet) args)
    {
        if (_debugLogging)
        {
            Debug.Log($"Asteroid {args.Asteroid.name} collided with a bullet {args.Bullet.name}");
        }
    }

}

//[CreateAssetMenu(menuName = "Events/" + nameof(AsteroidBulletCollisionEvent), fileName = nameof(AsteroidBulletCollisionEvent))]
//public class AsteroidBulletCollisionEvent : GameEventBase<CollisionArgs<Asteroid, Bullet>>
//{
//    public bool DebugLogging;

//    public override void OnAfterRaised(CollisionArgs<Asteroid, Bullet> args)
//    {
//        if (DebugLogging)
//        {
//            (Asteroid asteroid, Bullet bullet) = args;

//            Debug.Log($"Asteroid {asteroid.name} collided with a bullet {bullet.name}");
//        }
//    }
//}