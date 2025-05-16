using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace UNSLOW.UnityUtils
{
    [RequireComponent(typeof(Animator))]
    public class SimpleAnimator : MonoBehaviour
    {
        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixer;
        private AnimationClipPlayable _prePlayable, _currentPlayable;
        private Coroutine _coroutinePlayAnimation;

        public List<AnimationClip> clipList;

        private void Awake()
        {
            var animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = null;
            _graph = animator.playableGraph;
        }

        private void Start()
        {
            _mixer = AnimationMixerPlayable.Create(_graph, 2);

            var output = _graph.GetOutput(0);
            output.SetSourcePlayable(_mixer);

            if (clipList.Count == 0)
                return;
            
            _currentPlayable = AnimationClipPlayable.Create(_graph, clipList[0]);
            _mixer.ConnectInput(0, _currentPlayable, 0);
            _mixer.SetInputWeight(0, 1);
            _graph.Play();
        }

        public void CrossFade(string animationName, float fadeLength)
        {
            CrossFade(clipList.Find(c => c.name == animationName), fadeLength);
        }

        public void CrossFade(string animationName)
        {
            CrossFade(animationName, 0.3f);
        }

        public void CrossFade(AnimationClip clip)
        {
            CrossFade(clip, 0.3f);
        }

        public void CrossFade(AnimationClip clip, float fadeLength)
        {
            if (_coroutinePlayAnimation != null)
                StopCoroutine(_coroutinePlayAnimation);

            _coroutinePlayAnimation = StartCoroutine(PlayAnimation(clip, fadeLength));
        }

        public void Play(string clipName)
        {
            Play(clipList.Find(c => c.name == clipName));
        }

        public void Play(AnimationClip newAnimation)
        {
            if (_currentPlayable.IsValid())
                _currentPlayable.Destroy();

            DisconnectPlayable();

            _currentPlayable = AnimationClipPlayable.Create(_graph, newAnimation);
            _currentPlayable.SetTime(0);

            _mixer.ConnectInput(0, _currentPlayable, 0);
            _mixer.SetInputWeight(1, 0);
            _mixer.SetInputWeight(0, 1);
        }

        public bool IsPlaying
        {
            get
            {
                if (!_currentPlayable.IsValid())
                    return false;

                return
                    _currentPlayable.GetTime() <=
                    _currentPlayable.GetAnimationClip().length;
            }
        }

        private void DisconnectPlayable()
        {
            _graph.Disconnect(_mixer, 0);
            _graph.Disconnect(_mixer, 1);

            if (_prePlayable.IsValid())
                _prePlayable.Destroy();
        }

        private IEnumerator PlayAnimation(AnimationClip newAnimation, float transitionTime)
        {
            DisconnectPlayable();

            _prePlayable = _currentPlayable;
            _currentPlayable = AnimationClipPlayable.Create(_graph, newAnimation);
            _mixer.ConnectInput(1, _prePlayable, 0);
            _mixer.ConnectInput(0, _currentPlayable, 0);

            // 指定時間でアニメーションをブレンド
            var waitTime = Time.timeSinceLevelLoad + transitionTime;

            yield return new WaitWhile(() =>
            {
                var diff = waitTime - Time.timeSinceLevelLoad;
                var rate = Mathf.Clamp01(diff / transitionTime);

                _mixer.SetInputWeight(1, rate);
                _mixer.SetInputWeight(0, 1 - rate);
                return diff > 0;
            });

            _coroutinePlayAnimation = null;
        }
    }
}