using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

namespace Events
{
    //public class BaseCollisionEvent<T1, T2> : GameEventBase<CollisionArgs<T1,T2>> 
    //    where T1 : MonoBehaviour 
    //    where T2 : MonoBehaviour  { }

    //public class BaseCollisionEvent<TEventInfo, T1, T2> : GameEventBase<TEventInfo, CollisionArgs<T1, T2>> 
    //    where T1 : MonoBehaviour 
    //    where T2 : MonoBehaviour
    //    where TEventInfo : struct { }

    //public struct CollisionArgs<TSource, TOther> where TSource : MonoBehaviour where TOther : MonoBehaviour
    //{
    //    public CollisionArgs(TSource origin, TOther other)
    //    {
    //        OriginId = origin.GetInstanceID();
    //        CollisionSourceId = origin.GetInstanceID();
    //        CollisionOtherId = other.GetInstanceID();
    //    }

    //    public int OriginId { get; }
    //    public int CollisionSourceId { get; }
    //    public int CollisionOtherId { get; }

    //    public TSource Source => EventHelper.FindObjectById<TSource>(CollisionSourceId);
    //    public TOther Other => EventHelper.FindObjectById<TOther>(CollisionOtherId);

    //    public void Deconstruct(out TSource source, out TOther other)
    //    {
    //        source = Source;
    //        other = Other;
    //    }
    //}

}

