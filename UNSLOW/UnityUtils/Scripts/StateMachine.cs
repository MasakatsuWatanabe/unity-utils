using System;
using System.Collections.Generic;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    /// 汎用のステートマシンクラス
    /// 初期化済み辞書を用いることでセットアップコスト削減を図っている
    /// </summary>
    public class StateMachine<TOwner, TState, TEvent> where TState : Enum where TEvent : Enum
    {
        public record StateBehavior
        {
            public Action<TOwner> OnEnter;
            public Action<TOwner> OnUpdate;
            public Action<TOwner> OnExit;

            public StateBehavior
                (Action<TOwner> onEnter = null, Action<TOwner> onUpdate = null, Action<TOwner> onExit = null)
            {
                OnEnter = onEnter;
                OnUpdate = onUpdate;
                OnExit = onExit;
            }

            public StateBehavior WithEnter(Action<TOwner> action)
            {
                OnEnter = action;
                return this;
            }

            public StateBehavior WithUpdate(Action<TOwner> action)
            {
                OnUpdate = action;
                return this;
            }

            public StateBehavior WithExit(Action<TOwner> action)
            {
                OnExit = action;
                return this;
            }

            public StateBehavior WithAll(Action<TOwner> enter, Action<TOwner> update, Action<TOwner> exit)
            {
                OnEnter = enter;
                OnUpdate = update;
                OnExit = exit;
                return this;
            }

            public StateBehavior AddEnter(Action<TOwner> action)
            {
                OnEnter += action;
                return this;
            }

            public StateBehavior AddUpdate(Action<TOwner> action)
            {
                OnUpdate += action;
                return this;
            }

            public StateBehavior AddExit(Action<TOwner> action)
            {
                OnExit += action;
                return this;
            }
        }

        public TState CurrentState { get; private set; }

        private TOwner _owner;
        private StateBehavior _currentStateBehavior;

        /// <summary>
        /// ステートのリスト
        /// </summary>
        private readonly Dictionary<TState, StateBehavior> _states;

        /// <summary>
        /// イベント受信時の遷移先ステート辞書、現在のステート別で遷移先が変わる場合のもの
        /// </summary>
        private readonly Dictionary<(TEvent, TState), TState> _transitions;

        /// <summary>
        /// イベント受信時の遷移先ステート辞書
        /// </summary>
        private readonly Dictionary<TEvent, TState> _transitionsAny;

        /// <summary>
        /// ステートの辞書を作成する
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        public static Dictionary<TState, StateBehavior> BuildStates(params (TState, StateBehavior)[] states)
        {
            var dict = new Dictionary<TState, StateBehavior>();
            foreach (var (key, value) in states)
                dict[key] = value;
            return dict;
        }

        /// <summary>
        /// 遷移元をキーとした遷移先の辞書を作成する
        /// </summary>
        /// <param name="transitions"></param>
        /// <returns></returns>
        public static Dictionary<(TEvent, TState), TState> BuildTransitions
            (params ((TEvent, TState), TState)[] transitions)
        {
            var dict = new Dictionary<(TEvent, TState), TState>();
            foreach (var ((ev, state), next) in transitions)
                dict[(ev, state)] = next;
            return dict;
        }

        /// <summary>
        /// イベントと遷移先の対応辞書を作成する
        /// </summary>
        /// <param name="transitions"></param>
        /// <returns></returns>
        public Dictionary<TEvent, TState> BuildTransitionsAny(params (TEvent, TState)[] transitions)
        {
            var dict = new Dictionary<TEvent, TState>();
            foreach (var (ev, state) in transitions)
                dict[ev] = state;
            return dict;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="states"></param>
        /// <param name="transitions"></param>
        /// <param name="transitionsAny"></param>
        public StateMachine
        (Dictionary<TState, StateBehavior> states,
            Dictionary<(TEvent, TState), TState> transitions = null,
            Dictionary<TEvent, TState> transitionsAny = null)
        {
            _states = states;
            _transitions = transitions;
            _transitionsAny = transitionsAny;
        }

        /// <summary>
        /// ステートマシンの開始
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="initialState"></param>
        public void Start(TOwner owner, TState initialState)
        {
            _owner = owner;
            CurrentState = initialState;

            _currentStateBehavior = _states[CurrentState];
            _currentStateBehavior.OnEnter?.Invoke(_owner);
        }

        public void SendEvent(TEvent ev)
        {
            if (_transitions != null &&
                _transitions.TryGetValue((ev, CurrentState), out var next))
            {
                ChangeState(next);
                return;
            }

            if (_transitionsAny == null)
                throw new Exception("No transition found.");

            ChangeState(_transitionsAny[ev]);
        }

        /// <summary>
        /// ステートを変更する
        /// イベントの設定と無関係で動作するので注意
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(TState newState)
        {
            _currentStateBehavior.OnExit?.Invoke(_owner);

            CurrentState = newState;
            _currentStateBehavior = _states[CurrentState];

            _currentStateBehavior.OnEnter?.Invoke(_owner);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            var curr = _states[CurrentState];
            curr.OnUpdate?.Invoke(_owner);
        }
    }
}
