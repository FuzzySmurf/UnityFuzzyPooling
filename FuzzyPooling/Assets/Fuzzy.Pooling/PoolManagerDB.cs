using System.Collections;
using System.Collections.Generic;
using Fuzzy.Pooling;
using UnityEngine;

namespace Fuzzy.Pooling
{
    public class PoolManagerDB {
        public List<SpawnHandler> spawnHandlers;
        public List<SpawnObject> spawnObjects;

        public PoolManagerDB() {
            spawnHandlers = new List<SpawnHandler>();
            spawnObjects = new List<SpawnObject>();
        }
    }
}
