using System;
using System.Collections.Generic;
using UnityEngine;
using ReplaySystem.Core;

namespace ReplaySystem.Recording
{
    public class Recorder : MonoBehaviour
    {
        [SerializeField] private int tickRate = 50;
        [SerializeField] private GameObject playerObject;
        
        private List<SerializedCommand> _commands = new List<SerializedCommand>();
        private int _currentTick;
        private bool _isRecording;
        private IRng _rng;
        private int _startSeed;
        
        private Vector3 _startPosition;
        private Vector3 _startScale;

        public bool IsRecording => _isRecording;
        public IRng Rng => _rng;
        public event Action<int> OnTick;

        void FixedUpdate()
        {
            if (!_isRecording) return;
            _currentTick++;
            OnTick?.Invoke(_currentTick);
        }

        public void StartRecording(int seed)
        {
            _commands.Clear();
            _currentTick = 0;
            _startSeed = seed;
            _rng = new FixedRng(seed);
            _isRecording = true;
            if (playerObject != null)
            {
                _startPosition = playerObject.transform.position;
                _startScale = playerObject.transform.localScale;
                
                
                if (float.IsNaN(_startPosition.x) || float.IsInfinity(_startPosition.x))
                {
                    _startPosition = Vector3.zero;
                }
                
                if (_startScale == Vector3.zero)
                {
                    _startScale = Vector3.one;
                }
            }
            else
            {
                _startPosition = Vector3.zero;
                _startScale = Vector3.one;
            }
            
        }

        public void RecordCommand(ICommand command)
        {
            if (!_isRecording) return;
            
            var serialized = new SerializedCommand
            {
                Tick = _currentTick,
                Type = command.Type,
                PayloadJson = JsonUtility.ToJson(command.GetPayload())
            };
            
            _commands.Add(serialized);
        }

        public ReplayData StopRecording()
        {
            _isRecording = false;
            
            var data = new ReplayData
            {
                Seed = _startSeed,
                TickRate = tickRate,
                TotalTicks = _currentTick,
                StartPosition = new Vector3Serialized(_startPosition),
                StartScale = new Vector3Serialized(_startScale),
                Commands = _commands,
                HasValidStartData = true 
            };
            
            return data;
        }

        public int GetCurrentTick() => _currentTick;
        
        public void SetPlayerObject(GameObject player)
        {
            playerObject = player;
        }
        
        public void ForceSetStartPosition(Vector3 pos)
        {
            _startPosition = pos;
        }
    }
}