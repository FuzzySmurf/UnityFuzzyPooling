using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Fuzzy.Pooling
{
    [Serializable]
    public class PoolManagerSettings {

        public static readonly string PoolManagerTagName = "PoolManager";

        public static readonly string SpawnHandlerTagName = "SpawnHandler";

        public static readonly string MiscSpawnHandlerName = "MiscSpawnHandler";

        //- Default Initial Spawn Amount (min 2)
        [Tooltip("The minimum amount of units to have in Spawn Pool")]
        public uint InitialSpawnAmount = 2;

        //- Overflow Max Int (mnust be higher then initial)
        /// <summary>
        /// The Maximum amount of GameObjects that can be Instantiated.
        /// </summary>
         [Tooltip("The maximum amount of units the spawnPool will have.")]
        public uint OverflowMaxInt = 10;
        
    }
}
