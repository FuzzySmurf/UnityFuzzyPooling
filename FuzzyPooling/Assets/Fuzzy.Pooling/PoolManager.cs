using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fuzzy.Pooling
{
    public sealed class PoolManager : MonoBehaviour {
        private int _keys;
        private int _spawnObjectsKey;

        private bool _isInitialized = false;
        private PoolManagerDB poolManagerDB;

        public PoolManagerSettings poolManagerDefaultSettings;

        private static PoolManager _instance;

        public static SpawnObject AddGOToSpawnPool(GameObject spawnObject, int spawnHandlerKey) {
            return _instance.AddToSpawnPool(spawnObject, spawnHandlerKey);
        }

        public static SpawnObject SpawnGO(SpawnObject spawnObject) {
            return _instance.Spawn(spawnObject);
        }

        public static SpawnObject SpawnGOAt(SpawnObject spawnObject, Transform location) {
            return _instance.SpawnAt(spawnObject, location);
        }

        public static bool AssignTransformToSpawnHandler(GameObject spawnLocation, int spawnHandlerKey) {
            bool ret = false;
            SpawnHandler handler = _instance.poolManagerDB.spawnHandlers.FirstOrDefault(s => s.spawnHandlerKey == spawnHandlerKey);
            if (handler != null) {
                handler.AddSpawnerLocation(spawnLocation);
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Creates a new SpawnHandler from an existing object.
        /// </summary>
        /// <param name="spawnHandler">The SpawnHandler to Add.</param>
        /// <param name="setPoolManagerParent">Should this spawnHandler be a child of the PoolManager?</param>
        /// <returns>The Spawn Handler Number.</returns>
        public static int CreateNewSpawnHandler(GameObject spawnHandler, SpawnHandlerDetails handlerDetails) {
            return _instance.CreateSpawnHandler(spawnHandler, handlerDetails);
        }

        public static void DeactivateObjects(SpawnObject obj) {
            _instance.DeactivateObject(obj);
        }

        public void Awake() {
            this.tag = PoolManagerSettings.PoolManagerTagName;
            if (!_isInitialized)
                Initalize();
        }

        private void Initalize() {
            poolManagerDB = new PoolManagerDB();
            _isInitialized = true;
            _instance = this;
            _keys = CreateSpawnHandler(PoolManagerSettings.MiscSpawnHandlerName,
                                            poolManagerDefaultSettings.OverflowMaxInt);
            _spawnObjectsKey = 0;
        }

        #region SpawnHandler

        private int CreateSpawnHandler(string spawnHandlerName, uint overflowMax) {
            GameObject obj = new GameObject(spawnHandlerName);
            SpawnHandlerDetails sph = new SpawnHandlerDetails() {
                initialSpawnAmount = poolManagerDefaultSettings.InitialSpawnAmount,
                overflowMaxSpawnAmount = overflowMax,
                setPoolManagerParent = true
            };
            obj.tag = PoolManagerSettings.SpawnHandlerTagName;
            return CreateSpawnHandler(obj, sph);
        }

        private int CreateSpawnHandler(GameObject spawnHandler, SpawnHandlerDetails handlerDetails) {
            if (!_isInitialized)
                Initalize();
            SpawnHandler handler = spawnHandler.GetComponent<SpawnHandler>();
            if (handler == null) {
                spawnHandler.AddComponent<SpawnHandler>();
                return CreateSpawnHandler(spawnHandler, handlerDetails);
            }
            if (handlerDetails.setPoolManagerParent)
                spawnHandler.transform.SetParent(this.transform);
            int value = ++_keys;
            handler.AddSpawnerLocation();
            handler.AddDetails(handlerDetails, value);
            poolManagerDB.spawnHandlers.Add(handler);
            //allSpawnHandlers.Add(value, handler);
            return value;
        }

        #endregion

        #region Spawn Object Options
        private SpawnObject AddToSpawnPool(GameObject spawnObject, int spawnHandlerKey) {
            SpawnObject spawnObj = spawnObject.GetComponent<SpawnObject>();
            if (spawnObj == null)
                spawnObj = spawnObject.AddComponent<SpawnObject>();
            SpawnHandler handler = poolManagerDB.spawnHandlers.FirstOrDefault(s => s.spawnHandlerKey == spawnHandlerKey);
            if(handler == null)
                throw new UnityException("Missing SpawnHandler for SpawnPool. Spawn Handler may not Exist!");

            //Check if the object is already in the pool with a matching handler.
            if (poolManagerDB.spawnObjects.Any()) {
                List<SpawnObject> spawnObjs =
                    poolManagerDB.spawnObjects.Where(s => s.spawnHandler.spawnHandlerKey.Equals(spawnHandlerKey)).ToList();
                if (spawnObjs.Any()) {
                    SpawnObject sObj = spawnObjs.FirstOrDefault(s => s.name.Contains(spawnObject.name.Trim()));
                    //if the object exists, with a handler, return the existing object.
                    if (sObj != null) {
                        return sObj;
                    }
                }
            }

            int value = ++_spawnObjectsKey;
            spawnObj.SetSpawnObject(value, handler);
            poolManagerDB.spawnObjects.Add(spawnObj);
            return spawnObj;
        }

        private SpawnObject Spawn(SpawnObject spawnObject) {
            return SpawnAt(spawnObject, null);
        }

        //Spawn this object @ location.
        private SpawnObject SpawnAt(SpawnObject spawnObject, Transform location) {
            if (spawnObject == null)
                throw new UnityException("SpawnObject was not Given!");
            SpawnObject sObj = poolManagerDB.spawnObjects
                        .FirstOrDefault(s => s.spawnObjectKey.Equals(spawnObject.spawnObjectKey));
            if(sObj == null)
                throw new UnityException("SpawnObject does not Exist in Pool Manager!");
            //Get me a list of all Handlers that reference this "spawnerKey"
            SpawnHandler referenceHandlers = poolManagerDB.spawnHandlers
                        .FirstOrDefault(s => s.spawnHandlerKey == sObj.spawnHandler.spawnHandlerKey);
            return referenceHandlers.SpawnObject(sObj, location);
        }

        #endregion

        #region deactivatingObjects

        public void DeactivateObject(SpawnObject obj) {
            obj.DeactivateObject();
        }

        #endregion
    }
}