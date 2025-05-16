using UnityEngine;

namespace UNSLOW.UnityUtils.ObjectScheduler
{
    /// <summary>
    /// コンポーネントのスケジューラ
    /// </summary>
    public class ComponentScheduler : ObjectScheduler<MonoBehaviour>
    {
        protected override void SetActive(MonoBehaviour target, bool state) => target.enabled = state;
        internal override bool IsActive(MonoBehaviour target) => target.enabled;
    }
}
