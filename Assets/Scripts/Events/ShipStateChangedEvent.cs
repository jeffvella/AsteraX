using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Events/" + nameof(ShipStateChangedEvent))]
    public class ShipStateChangedEvent : GameEventBase<ShipStateChangedEventInfo, ShipStatusEventArgument>
    {

    }

    public struct ShipStatusEventArgument
    {
        public ShipStatusEventArgument(int shipInstanceId, ShipController.ShipStatus status)
        {
            GameObjectId = shipInstanceId;
            CurrentStatus = status;
        }

        public int GameObjectId { get; }

        public ShipController.ShipStatus CurrentStatus { get; }
    }

    [Serializable]
    public struct ShipStateChangedEventInfo
    {
        [SerializeField]
        private ShipState _newState;

        public ShipState NewState => _newState;
    }
}

