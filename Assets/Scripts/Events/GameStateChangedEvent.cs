using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Events/" + nameof(GameStateChangedEvent), fileName = nameof(GameStateChangedEvent))]
    public class GameStateChangedEvent : GameEventBase<(GameState Previous, GameState Current)>
    {
        [SerializeField]
        private bool _debugLogging;

        protected override void OnAfterRaised((GameState Previous, GameState Current) args)
        {
            if (_debugLogging)
            {
                Debug.Log($"Game State changed from {args.Previous} to {args.Current}");
            }
        }
    }

}

