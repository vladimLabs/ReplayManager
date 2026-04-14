using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReplaySystem.Core;

namespace ReplaySystem.Playback
{
    public class Player : MonoBehaviour
    {
        private ReplayData _currentReplay;
        private Dictionary<int, List<ICommand>> _timedCommands;
        private int _currentTick;
        private bool _isPlaying;
        private Coroutine _playbackCoroutine;
        private Rigidbody2D _targetRigidbody;
        
        public bool IsPlaying => _isPlaying;
        public event System.Action OnPlaybackFinished;
        public event System.Action<int> OnTick;

        public void LoadReplay(ReplayData replay)
        {
            _currentReplay = replay;
            _timedCommands = new Dictionary<int, List<ICommand>>();
            
            foreach (var serialized in replay.Commands)
            {
                if (!_timedCommands.ContainsKey(serialized.Tick))
                    _timedCommands[serialized.Tick] = new List<ICommand>();
                
                var command = DeserializeCommand(serialized);
                _timedCommands[serialized.Tick].Add(command);
            }
        }

        private ICommand DeserializeCommand(SerializedCommand serialized)
        {
            object payload = serialized.Type switch
            {
                CommandType.Move => JsonUtility.FromJson<MoveCommand.MovePayload>(serialized.PayloadJson),
                CommandType.Jump => JsonUtility.FromJson<JumpCommand.JumpPayload>(serialized.PayloadJson),
                CommandType.Dash => JsonUtility.FromJson<DashCommand.DashPayload>(serialized.PayloadJson),
                _ => null
            };
            
            return serialized.Type switch
            {
                CommandType.Move => new MoveCommand(serialized.Tick, (MoveCommand.MovePayload)payload),
                CommandType.Jump => new JumpCommand(serialized.Tick, (JumpCommand.JumpPayload)payload),
                CommandType.Dash => new DashCommand(serialized.Tick, (DashCommand.DashPayload)payload),
                _ => null
            };
        }

        public void Play(GameObject target)
        {
            if (_isPlaying) Stop();
            
            TeleportToStartPosition(target);
            
            _isPlaying = true;
            _currentTick = 0;
            _playbackCoroutine = StartCoroutine(PlaybackCoroutine(target));
        }
        
        private void TeleportToStartPosition(GameObject target)
        {
            if (_currentReplay == null)
            {
                return;
            }
            
            if (target == null)
            {
                return;
            }
            Vector3 startPos = Vector3.zero;
            Vector3 startScale = Vector3.one;
            
            if (_currentReplay.HasValidStartData)
            {
                startPos = _currentReplay.StartPosition.ToVector3();
                startScale = _currentReplay.StartScale.ToVector3();
            }
            else
            {
                startPos = target.transform.position;
                startScale = target.transform.localScale;
            }
            
            if (startScale == Vector3.zero)
            {
                startScale = Vector3.one;
            }
            
            if (float.IsNaN(startPos.x) || float.IsInfinity(startPos.x))
            {
                startPos = Vector3.zero;
            }
   
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
            bool wasKinematic = false;
            if (rb != null)
            {
                wasKinematic = rb.isKinematic;
                rb.isKinematic = true;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            
            target.transform.position = startPos;
            target.transform.localScale = startScale;
            
            if (rb != null)
            {
                rb.isKinematic = wasKinematic;
            }
            
            PlayerController controller = target.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.ResetState();
                controller.Teleport(startPos);
            }
            
        }

        private IEnumerator PlaybackCoroutine(GameObject target)
        {
            float tickTime = 1f / _currentReplay.TickRate;
            var wait = new WaitForFixedUpdate();
            
            while (_currentTick <= _currentReplay.TotalTicks)
            {
                if (_timedCommands.TryGetValue(_currentTick, out var commands))
                {
                    foreach (var cmd in commands)
                    {
                        cmd.Execute(target);
                    }
                }
                
                OnTick?.Invoke(_currentTick);
                _currentTick++;
                
                yield return wait;
            }
            
            _isPlaying = false;
            OnPlaybackFinished?.Invoke();
        }

        public void Stop()
        {
            if (_playbackCoroutine != null)
                StopCoroutine(_playbackCoroutine);
            _isPlaying = false;
        }
    }
}