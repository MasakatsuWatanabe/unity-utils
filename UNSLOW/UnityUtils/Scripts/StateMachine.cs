using System;
using System.Collections.Generic;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    ///     汎用のステートマシンクラス
    ///     初期化済み辞書を用いることでセットアップコスト削減を図っている
    ///     <typeparam name="TOwner">
    ///         管理対象クラス
    ///     </typeparam>
    ///     <typeparam name="TState">
    ///         enum継承のステート
    ///     </typeparam>
    ///     <typeparam name="TEvent">
    ///         ステート遷移をトリガーするenum継承のイベント
    ///     </typeparam>
    /// </summary>
    public class StateMachine<TOwner, TState, TEvent>
        where TState : Enum
        where TEvent : Enum
    {
        /// <summary>
        /// イベント別のステート遷移先定義
        /// </summary>
        public record Transition
        {
            public readonly TEvent Event;
            public readonly TState State;

            public Transition(TEvent ev, TState state)
            {
                Event = ev;
                State = state;
            }
        }

        /// <summary>
        ///     イベント遷移先の定義
        /// </summary>
        public class TransitionMap
        {
            private readonly Dictionary<TEvent, TState> _transitions;

            public TransitionMap(params Transition[] transitions)
            {
                _transitions = new Dictionary<TEvent, TState>();
                foreach (var transition in transitions)
                    _transitions[transition.Event] = transition.State;
            }
            
            public bool TryGetState(TEvent ev, out TState state)
            {
                return _transitions.TryGetValue(ev, out state);
            }
        }
        
        /// <summary>
        ///     ステートの定義
        ///     ステートイン、更新、ステートアウトの各コールバックと
        ///     ステート依存のイベント遷移先を登録する
        /// </summary>
        public record StateBehaviour
        {
            public readonly TState State;
            public readonly Action<TOwner> OnEnter;
            public readonly Action<TOwner> OnUpdate;
            public readonly Action<TOwner> OnExit;
            private readonly TransitionMap _transitionMap;

            /// <summary>
            ///     コンストラクタ
            /// </summary>
            /// <param name="state"></param>
            /// <param name="onEnter"></param>
            /// <param name="onUpdate"></param>
            /// <param name="onExit"></param>
            /// <param name="transitions"></param>
            public StateBehaviour
            (   
                TState state,
                Action<TOwner> onEnter,
                Action<TOwner> onUpdate,
                Action<TOwner> onExit,
                params Transition[] transitions)
            {
                State = state;
                OnEnter = onEnter;
                OnUpdate = onUpdate;
                OnExit = onExit;

                if (transitions == null)
                    return;
                
                // 必要であれば遷移先マップを作成
                _transitionMap = new TransitionMap(transitions);
            }
            
            /// <summary>
            ///     イベントに対応した遷移先を得る
            /// </summary>
            /// <param name="ev"></param>
            /// <param name="state"></param>
            /// <returns></returns>
            public bool TryGetNextState(TEvent ev, out TState state)
            {
                return _transitionMap.TryGetState(ev, out state);
            }
        }
        
        /// <summary>
        ///     ステートの定義マップ
        /// </summary>
        public class StateMap
        {
            private readonly Dictionary<TState, StateBehaviour> _stateMap;
            
            public StateMap(params StateBehaviour[] states)
            {
                _stateMap = new Dictionary<TState, StateBehaviour>();
                
                foreach (var state in states)
                    _stateMap[state.State] = state;
            }
            
            public StateBehaviour this[TState state] => _stateMap[state];
        }

        public TState CurrentState { get; private set; }

        private TOwner _owner;
        private StateBehaviour _currentStateBehaviour;

        /// <summary>
        ///     ステートのマップ
        /// </summary>
        private StateMap _stateMap;

        /// <summary>
        ///     ステートに依存しないイベント遷移先のマップ
        /// </summary>
        private TransitionMap _transitionMapAny;

        /// <summary>
        ///     ステートマシンの開始
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="initialState"></param>
        /// <param name="states"></param>
        /// <param name="transitions"></param>
        public void Start
        (
            TOwner owner,
            TState initialState,
            StateMap states,
            TransitionMap transitions = null)
        {
            _owner = owner;
            _stateMap = states;
            _transitionMapAny = transitions;
            
            CurrentState = initialState;
            _currentStateBehaviour = _stateMap[CurrentState];
            _currentStateBehaviour.OnEnter?.Invoke(owner);
        }

        /// <summary>
        ///     イベント送信とそれに伴うステート遷移
        /// </summary>
        public bool SendEvent(TEvent ev)
        {
            if (_currentStateBehaviour == null)
                throw new Exception("No states defined.");

            // ステートに遷移先が登録されているのであれば遷移
            if (_currentStateBehaviour.TryGetNextState(ev, out var state))
            {
                ChangeState(state);
                return true;
            }

            // 無ければ遷移先マップを参照
            if (_transitionMapAny != null &&
                _transitionMapAny.TryGetState(ev, out var next))
            {
                ChangeState(next);
                return true;
            }

            // どちらにも登録されていなければ無視
            return false;
        }

        /// <summary>
        ///     ステートを変更する
        ///     イベントの設定と無関係で動作する
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(TState newState)
        {
            _currentStateBehaviour.OnExit?.Invoke(_owner);

            CurrentState = newState;
            _currentStateBehaviour = _stateMap[CurrentState];

            _currentStateBehaviour.OnEnter?.Invoke(_owner);
        }

        /// <summary>
        ///     更新
        /// </summary>
        public void Update()
        {
            var curr = _stateMap[CurrentState];
            curr.OnUpdate?.Invoke(_owner);
        }
    }
}
