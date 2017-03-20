using UnityEngine;
using System.Collections;

namespace Fuzzy.Pooling
{

    [System.Serializable]
    public class SpawnHandlerDetails
    {
        public bool setPoolManagerParent;
        public uint initialSpawnAmount;
        public uint overflowMaxSpawnAmount;
    }
}