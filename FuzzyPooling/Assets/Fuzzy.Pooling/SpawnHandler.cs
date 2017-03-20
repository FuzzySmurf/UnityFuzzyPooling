using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Fuzzy.Pooling
{
    public sealed class SpawnHandler : MonoBehaviour
    {
        private GameObject _spawnHandler;
        private int _spawnHandlerKey;
        private bool _isKeyAdded = false;
        private bool _invokedInstantiateAll = false;

        public GameObject spawnHandler
        {
            get { return _spawnHandler; }
        }

        public List<GameObject> spawnLocations;
        public PoolManager poolManager;

        public int spawnHandlerKey
        {
            get { return _spawnHandlerKey; }
        }

        public uint initialSpawnAmount;

        public int maxActiveUnits
        {
            get { return gameObject.GetComponentsInChildren<SpawnObject>(true).Count(); }
        }

        public uint overFlowMaxSpawnAmount;
        private int _curSpawnAmount;

        public void Awake() {
            spawnLocations = new List<GameObject>();
            poolManager = GameObject.FindGameObjectWithTag(PoolManagerSettings.PoolManagerTagName).GetComponent<PoolManager>();
            _spawnHandler = this.gameObject;
            initialSpawnAmount = poolManager.poolManagerDefaultSettings.InitialSpawnAmount;
            _curSpawnAmount = 0;
        }

        public void AddDetails(SpawnHandlerDetails handlerDetails, int key) {
            if (!_isKeyAdded) {
                _spawnHandlerKey = key;
                _isKeyAdded = true;
                initialSpawnAmount = (handlerDetails.initialSpawnAmount > 0) ? handlerDetails.initialSpawnAmount : poolManager.poolManagerDefaultSettings.InitialSpawnAmount;
                overFlowMaxSpawnAmount = (handlerDetails.overflowMaxSpawnAmount > initialSpawnAmount)
                    ? handlerDetails.overflowMaxSpawnAmount
                    : poolManager.poolManagerDefaultSettings.OverflowMaxInt;
                //overFlowMaxSpawnAmount = 150;
            }
        }

        public void AddSpawnerLocation(GameObject obj) {
            spawnLocations.Add(obj);
        }

        public void AddSpawnerLocation() {
            AddSpawnerLocation(this.gameObject);
        }

        public SpawnObject SpawnObject(SpawnObject spawnObj, Transform location = null) {
            SpawnObject sObj = null;
            //Get All InactiveGameObjects from the SpawnHandler.
            IEnumerable<SpawnObject> spawnObjs = gameObject.GetComponentsInChildren<SpawnObject>(true)
                .Where(s => s.gameObject.activeSelf.Equals(false) &&
                            s.spawnObjectKey.Equals(spawnObj.spawnObjectKey));

            if (spawnObjs.Any()) {
                if (location == null) {
                    int num = UnityEngine.Random.Range(0, spawnLocations.Count);
                    //Vector3 pos = spawnLocations.ElementAt(num).transform.position;
                    Transform trans = spawnLocations.ElementAt(num).transform;
                    sObj = spawnObjs.First();
                    sObj.ActivateObject(trans);
                }
                else {
                    sObj = spawnObjs.First();
                    sObj.ActivateObject(location);
                }
            }
            else if (overFlowMaxSpawnAmount > _curSpawnAmount) {
                if (_invokedInstantiateAll)
                    InstantiateObject(spawnObj);
                else
                    InstantiateAllObjects(spawnObj);
                ////If the current Spawn Amount equals the overFlowMaxSpawnAmount
                ////increase the overFlowMaxSpawnAmount.
                ////This could be an issue, as it was just an alternate way of fixing an issue.
                //if (overFlowMaxSpawnAmount == _curSpawnAmount)
                //    overFlowMaxSpawnAmount++;
                sObj = SpawnObject(spawnObj, location);
            } else 
                throw new UnityException("No inactive spawn objects available in pool. Need to increase OverflowMax, or wait for next available Spawn.");
            return sObj;
        }

        public void InstantiateObject(SpawnObject obj) {
            SpawnObject gObj = GameObject.Instantiate(obj, this.transform.position, Quaternion.identity) as SpawnObject;
            gObj.transform.SetParent(_spawnHandler.transform);
            gObj.SetSpawnObject(obj.spawnObjectKey, this);
            gObj.DeactivateObject();
            _curSpawnAmount++;
        }

        private void InstantiateAllObjects(SpawnObject obj) {
            for (int i = 0; i < initialSpawnAmount; i++)
                InstantiateObject(obj);
            _invokedInstantiateAll = true;
        }
    }
}