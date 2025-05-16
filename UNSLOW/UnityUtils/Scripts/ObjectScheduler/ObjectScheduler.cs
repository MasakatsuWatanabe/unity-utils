using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UNSLOW.UnityUtils.ObjectScheduler
{
    /// <summary>
    ///     オブジェクトのスケジューラ
    /// </summary>
    public abstract class ObjectScheduler<T> : MonoBehaviour
        where T : Object
    {
        protected abstract void SetActive(T target, bool state);
        internal abstract bool IsActive(T target);
        
        [Serializable]
        public class Schedule
        {
            public float start;
            public float duration;
            public T target;

            /// <summary>
            ///     指定の時間が範囲内かどうか
            /// </summary>
            /// <param name="time"></param>
            /// <returns></returns>
            public bool IsWithinRange
                (float time) => time >= start && time < duration + start;
        }

        [Serializable]
        private class InitialState
        {
            public InitialState(T target, bool state)
            {
                this.target = target;
                this.state = state;
            }

            public T target;
            public bool state;
        }

        [SerializeField] private Schedule[] schedules;
        [SerializeField] private float duration;

        public Action OnScheduleEnd;

        private float _time;

        // 事前に求めた初期状態
        [SerializeField] [HideInInspector] private InitialState[] initialStates;

        private void UpdateScheduleTargetState(Schedule schedule, float lastTime, float nextTime)
        {
            var last = schedule.IsWithinRange(lastTime);
            var next = schedule.IsWithinRange(nextTime);

            if (last != next)
                SetActive(schedule.target, next);
        }
        
        private void OnEnable()
        {
            Debug.Assert(AreSchedulesSorted());

            _time = 0;

            SetInitialState();
        }

        private void OnDisable()
        {
            _time = 0;
        }

        private void Update()
        {
            if (_time >= duration)
                return;

            var next = _time + Time.deltaTime;

            foreach (var schedule in schedules)
            {
                if (schedule.start > next)
                    break;
                
                UpdateScheduleTargetState(schedule, _time, next);
            }

            _time = next;

            if (_time >= duration)
                OnScheduleEnd?.Invoke();
        }

        private bool AreSchedulesSorted()
        {
            for (var i = 0; i < schedules.Length - 1; i++)
                if (schedules[i].start > schedules[i + 1].start)
                    return false;

            return true;
        }

        /// <summary>
        ///     タイム0時点での初期状態の設定
        /// </summary>
        private void SetInitialState()
        {
            foreach (var initialState in initialStates)
            {
                SetActive(initialState.target, initialState.state);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var tail = 
                schedules.
                    Select(schedule => schedule.start + schedule.duration).
                    Prepend(0f).Max();
            
            duration = Mathf.Max(duration, tail);

            var map = new Dictionary<T, bool>();
            foreach (var schedule in schedules)
            {
                if (schedule.target == null)
                    continue;

                if (!map.ContainsKey(schedule.target))
                    map[schedule.target] = schedule.IsWithinRange(0);
                else
                    map[schedule.target] |= schedule.IsWithinRange(0);
            }

            initialStates = map.Select(pair => new InitialState(pair.Key, pair.Value)).ToArray();
        }
#endif
    }
}
