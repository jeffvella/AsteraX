using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;
using UnityEngine;

namespace Assets.Scripts.Events
{
    [CreateAssetMenu(menuName = "Events/" + nameof(BulletFiredEvent))]
    public class BulletFiredEvent : GameEventBase<Bullet>
    {
        [SerializeField]
        private bool _debugLogging;

        protected override void OnAfterRaised(Bullet bullet)
        {
            if (_debugLogging)
            {
                Debug.Log($"Bullet fired! {bullet}");
            }
        }
    }
}
