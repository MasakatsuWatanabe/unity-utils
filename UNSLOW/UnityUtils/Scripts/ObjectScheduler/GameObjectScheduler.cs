using UnityEngine;

namespace UNSLOW.UnityUtils.ObjectScheduler
{
    /// <summary>
    /// GameObjectのスケジューラ
    /// </summary>
    public class GameObjectScheduler : ObjectScheduler<GameObject>
    {
        protected override void SetActive(GameObject target, bool state) => target.SetActive(state);
        internal override bool IsActive(GameObject target) => target.activeSelf;
    }
}
