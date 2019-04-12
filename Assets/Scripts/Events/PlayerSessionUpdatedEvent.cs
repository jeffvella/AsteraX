using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{

    [CreateAssetMenu(menuName = "Events/" + nameof(PlayerSessionUpdatedEvent), fileName = nameof(PlayerSessionUpdatedEvent))]
    public class PlayerSessionUpdatedEvent : GameEventBase<PlayerManager.PlayerSession> { }


    //[CreateAssetMenu(menuName = "Events/" + nameof(PlayerSessionUpdatedEvent), fileName = nameof(PlayerSessionUpdatedEvent))]
    //public class PlayerSessionUpdatedEvent : GameEventBase<PlayerSessionArgs> { }

    //public struct PlayerSessionArgs
    //{
    //    public PlayerSessionArgs(PlayerManager.PlayerSession session)
    //    {
    //        Session = session;
    //    }

    //    public PlayerManager.PlayerSession Session { get; }
    //}

}




