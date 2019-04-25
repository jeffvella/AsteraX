using Events;
using UnityEngine;
namespace Assets.Scripts.Events
{
    [CreateAssetMenu(menuName = "Events/" + nameof(AchievementAttainedEvent))]
    public class AchievementAttainedEvent : GameEventBase<AchievementDefinition>
    {
        [SerializeField]
        private bool _debugLogging;

        protected override void OnAfterRaised(AchievementDefinition achievement)
        {
            if (_debugLogging)
            {
                Debug.Log($"Achievement '{achievement}' attained! ");
            }
        }
    }

}
