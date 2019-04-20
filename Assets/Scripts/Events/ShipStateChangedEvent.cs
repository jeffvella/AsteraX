using System;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Events/" + nameof(ShipStateChangedEvent), fileName = nameof(ShipStateChangedEvent))]
    public class ShipStateChangedEvent : GameEventBase<ShipStateChangedEventInfo, ShipStatusArgs> { }

    public struct ShipStatusArgs
    {
        public ShipStatusArgs(int shipInstanceId, Ship.ShipStatus status)
        {
            GameObjectId = shipInstanceId;
            CurrentStatus = status;
        }

        public int GameObjectId { get; }

        public Ship.ShipStatus CurrentStatus { get; }
    }

    [Serializable]
    public struct ShipStateChangedEventInfo
    {
        [SerializeField]
        private ShipState _newState;

        public ShipState NewState => _newState;
    }
}

