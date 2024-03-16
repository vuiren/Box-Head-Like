using Mirror;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class MirrorSimpleObjectPooler : MMSimpleObjectPooler
    {
        SimpleObjectPoolerNetworkBehaviour networkBehaviour;

        protected override void Awake()
        {
        }

        private void Start()
        {
            networkBehaviour = GetComponentInParent<SimpleObjectPoolerNetworkBehaviour>();
            networkBehaviour.AssignSimpleObjectPooler(this);
            if (networkBehaviour.isServer)
                FillObjectPool();
        }

        protected override bool CreateWaitingPool()
        {
            if (!MutualizeWaitingPools)
            {
                // we create a container that will hold all the instances we create
                _waitingPool = new GameObject(DetermineObjectPoolName());
                SceneManager.MoveGameObjectToScene(_waitingPool, this.gameObject.scene);
                _objectPool = _waitingPool.AddComponent<MMObjectPool>();
                _objectPool.PooledGameObjects = new List<GameObject>();
                ApplyNesting();
                return true;
            }
            else
            {
                MMObjectPool objectPool = ExistingPool(DetermineObjectPoolName());
                if (objectPool != null)
                {
                    _objectPool = objectPool;
                    _waitingPool = objectPool.gameObject;
                    return false;
                }
                else
                {
                    _waitingPool = new GameObject(DetermineObjectPoolName());
                    SceneManager.MoveGameObjectToScene(_waitingPool, this.gameObject.scene);
                    _objectPool = _waitingPool.AddComponent<MMObjectPool>();
                    _objectPool.PooledGameObjects = new List<GameObject>();
                    ApplyNesting();
                    AddPool(_objectPool);
                    return true;
                }
            }
        }

        protected override GameObject AddOneObjectToThePool()
        {
            networkBehaviour.CmdAddProjectileToPool();
            return null;
        }

        public GameObject GetWaitingPool => _waitingPool;
        public MMObjectPool GetObjectPool => _objectPool;
    }
}