using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReplaySystem.Core
{
    [Serializable]
    public class ReplayData
    {
        public int Seed;
        public int TickRate;
        public int TotalTicks;
        public Vector3Serialized StartPosition;
        public Vector3Serialized StartScale;
        public List<SerializedCommand> Commands = new List<SerializedCommand>();
        
        public bool HasValidStartData;
    }

    [Serializable]
    public class SerializedCommand
    {
        public int Tick;
        public CommandType Type;
        public string PayloadJson;
    }

    [Serializable]
    public class Vector3Serialized
    {
        public float x;
        public float y;
        public float z;

        public Vector3Serialized(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 ToVector3() => new Vector3(x, y, z);
        
        public bool IsValid() => !float.IsNaN(x) && !float.IsInfinity(x);
    }
}