using Mirror;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SimpleObjectPoolerNetworkBehaviour : NetworkBehaviour
    {
        [SerializeField] private MirrorSimpleObjectPooler simpleObjectPooler;

        public void AssignSimpleObjectPooler(MirrorSimpleObjectPooler simpleObjectPooler)
        {
            this.simpleObjectPooler = simpleObjectPooler;
        }

        [Command]
        public void CmdAddProjectileToPool()
        {
            var GameObjectToPool = simpleObjectPooler.GameObjectToPool;
            if (GameObjectToPool == null)
            {
                Debug.LogWarning("The " + gameObject.name + " ObjectPooler doesn't have any GameObjectToPool defined.", gameObject);
                return;
            }

            bool initialStatus = GameObjectToPool.activeSelf;
            GameObjectToPool.SetActive(false);
            GameObject newGameObject = (GameObject)Instantiate(GameObjectToPool);
            GameObjectToPool.SetActive(initialStatus);
            SceneManager.MoveGameObjectToScene(newGameObject, this.gameObject.scene);
            if (simpleObjectPooler.NestWaitingPool)
            {
                newGameObject.transform.SetParent(simpleObjectPooler.GetWaitingPool.transform);
            }
            newGameObject.name = GameObjectToPool.name + "-" + simpleObjectPooler.GetObjectPool.PooledGameObjects.Count;

            simpleObjectPooler.GetObjectPool.PooledGameObjects.Add(newGameObject);

            return;
        }
    }
}