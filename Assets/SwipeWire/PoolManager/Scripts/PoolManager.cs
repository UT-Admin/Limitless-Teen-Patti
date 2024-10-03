using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TP
{
    [Serializable]
    public class PoolObjectItem
    {
        public string itemName;
        public int poolAmount = 1;
        public Transform parentHolder;
        public PoolObjects objectToPool;
    }

    public class PoolManager : SingletonMonoBehaviour<PoolManager>
    {
        //this is the list that contains all the objects PREFABS that we want to pool
        public List<PoolObjectItem> itemsToPool;
        public Transform parentGameObject;

        //Dictionary containig a key for each PoolObject and the list of PoolObjects that might be used.
        private Dictionary<int, List<PoolObjects>> poolDictionary = new Dictionary<int, List<PoolObjects>>();

        private void Start()
        {
            if (itemsToPool.Count == 0)
            {
                Debug.LogWarning("Please make sure that you set the list of poolable objects.");
            }

            foreach (PoolObjectItem item in itemsToPool)
            {
                if (item == null)
                {
                    Debug.LogWarning("PoolObjectItem object: " + item.ToString() + " is null.");
                }
                else
                {
                    AddObjectToPoolDictionary(item);// item.objectToPool, item.poolAmount);
                }
            }
        }

        /// <summary>
        /// Adds a new pool object to our dictionary.
        /// This is called from GameplayManager or from Destroyable objects that have debris.
        /// </summary>
        /// <param name="newPoolObject">The new to pool object.</param>
        public void AddObjectToPoolDictionary(PoolObjectItem item)//PoolObjects newPoolObject, int size)
        {
            //Lets make sure that we don't have any pool object with this id.
            int poolKey = item.objectToPool.GetInstanceID();

            if (poolDictionary.ContainsKey(poolKey) == false)
            {
                //If we don't have the id of this pool object in our pool manager 
                //Then just create a new entry for our dictionary
                //We will Instantiate one object and add it to the list
                //You can extend it and spawn like 10 or more objects
                List<PoolObjects> newPoolObjectsList = new List<PoolObjects>();
                for (int i = 0; i < item.poolAmount; i++)
                {
                    InstantiateNewPoolObject(item.objectToPool, newPoolObjectsList, item.parentHolder);
                }
                poolDictionary.Add(poolKey, newPoolObjectsList);
            }
            else
            {
                Debug.LogWarning("You've tried to add a new PoolObject to our dictionary, but it already contains the object.");
            }
        }



        /// <summary>
        /// Get the selected pool object from an existing pool.
        /// </summary>
        /// <param name="poolObject">The pool object.</param>
        /// <returns></returns>
        /// 
        public GameObject GetNewFromPool(string key, GameObject inGameTrans = null)
        {
            PoolObjectItem item = itemsToPool.Where(x => x.itemName == key).FirstOrDefault();
            return inGameTrans == null ? EnableObjectFromPool(GetObjectFromPool(item)) : EnableObjectFromPool(GetObjectFromPool(item), inGameTrans);
        }


        /// <summary>
        /// Get the selected pool object from an existing pool.
        /// </summary>
        /// <param name="poolObject">The pool object.</param>
        /// <returns></returns>
        /*public GameObject GetNewFromPool(string key)
        {
            PoolObjectItem item = itemsToPool.Where(x => x.itemName == key).FirstOrDefault();
            return EnableObjectFromPool( GetObjectFromPool(item), parentGameObject.gameObject);
        }*/

        /// <summary>
        /// Get the selected pool object from an existing pool.
        /// </summary>
        /// <param name="poolObject">The pool object.</param>
        /// <returns></returns>
        public PoolObjects GetObjectFromPool(string key)
        {
            PoolObjectItem item = itemsToPool.Where(x => x.itemName == key).FirstOrDefault();
            return GetObjectFromPool(item);
        }

        /// <summary>
        /// Get the selected pool object from an existing pool.
        /// </summary>
        /// <param name="poolObject">The pool object.</param>
        /// <returns></returns>
        public PoolObjects GetObjectFromPool(PoolObjectItem item)
        {
            //Get the pool key for this object.
            int poolKey = item.objectToPool.GetInstanceID();

            if (poolDictionary.ContainsKey(poolKey))
            {
                //Get the list
                List<PoolObjects> poolObjectsList = poolDictionary[poolKey];

                //If the object exists return the first available one.
                //If we don't find any abailable object we will instantiate a new one.
                //This is why it is very important to disable the PoolObjects
                for (int i = 0; i < poolObjectsList.Count; i++)
                {
                    if (poolObjectsList[i].gameObject.activeSelf == false)
                        return poolObjectsList[i];
                }

                //Now, spawn a new one, if we don't find anyone
                InstantiateNewPoolObject(item.objectToPool, poolObjectsList, item.parentHolder);

                //return the last object added to the list
                return poolObjectsList[poolObjectsList.Count - 1];
            }
            else
            {
                Debug.LogError("You are trying to get a PoolObject that doesn't exist in our poolDictionary. Please make sure that you add the pool object before trying to get them.");
                return null;
            }
        }

        /// <summary>
        /// Instantiates a new PoolObject and adds it to the correct list.
        /// </summary>
        /// <param name="poolObjectToSpawn">Object to instnatiate.</param>
        /// <param name="poolListToAdd">List to add.</param>
        private void InstantiateNewPoolObject(PoolObjects poolObjectToSpawn, List<PoolObjects> poolListToAdd, Transform parentHolder)
        {
            //New poolObject
            PoolObjects newPoolObject = (PoolObjects)Instantiate(poolObjectToSpawn, parentHolder, false);
            //First, we set it to be disabled
            newPoolObject.DisablePoolObject();
            //newPoolObject.transform.SetParent(parentHolder);
            //Add to the pool list
            poolListToAdd.Add(newPoolObject);
        }


        /// <summary>
        /// Get the selected pool object from an existing pool.
        /// </summary>
        /// <returns></returns>
        public void DisableObjectFromPool(string key)
        {
            PoolObjects item = itemsToPool.Where(x => x.itemName == key).FirstOrDefault()?.objectToPool;
            if(item != null)
                DisablePoolObjectList(item);
        }

        /// <summary>
        /// Disables all pool object selected.
        /// This is called, for example, when the player chooses to continue the game.
        /// </summary>
        /// <param name="poolObject">The pool object to disable</param>
        public void DisablePoolObjectList(PoolObjects poolObject)
        {
            //Get the pool key of this object
            int poolKey = poolObject.GetInstanceID();

            //If it exists, then for each PoolObject in the list, disable it
            if (poolDictionary.ContainsKey(poolKey) == true)
            {
                for (int i = 0; i < poolDictionary[poolKey].Count; i++)
                {
                    poolDictionary[poolKey][i].DisablePoolObject();
                    poolDictionary[poolKey][i].transform.SetParent(itemsToPool[0].parentHolder);
                }
            }
            else
            {
                Debug.LogWarning("You've tried to disable all pool objects but you didn't add the current pool object to the dictionary. Pool object: " + poolObject.name);
            }
        }



        /// <summary>
        /// Disables pool object selected.
        /// </summary>
        /// <param name="poolObject">The pool object to disable</param>
        public void DisablePoolObjectItem(PoolObjects poolObject)
        {
            poolObject.DisablePoolObject();
        }

        #region Enable Object From Pool
        /// <summary>
        /// Enable pool object
        /// </summary>
        /// <param name="poolObject">The pool object to disable</param>
        public GameObject EnableObjectFromPool(PoolObjects poolObject)
        {
            GameObject newObject = poolObject.gameObject;
            poolObject.ActivatePoolObject();
            newObject.transform.SetAsLastSibling();
            return newObject;
        }

        /// <summary>
        /// Enable pool object
        /// </summary>
        /// <param name="poolObject">The pool object to be enabled</param>
        /// <param name="itemPosition">The pool object position</param>
        /// <param name="itemRotation">The pool object rotation</param>
        public GameObject EnableObjectFromPool(PoolObjects poolObject, Vector2 itemPosition, Quaternion itemRotation)
        {
            GameObject newObject = poolObject.gameObject;
            newObject.transform.position = itemPosition;
            newObject.transform.rotation = itemRotation;
            poolObject.ActivatePoolObject();
            return newObject;
        }

        /// <summary>
        /// Enable pool object
        /// </summary>
        /// <param name="poolObject">The pool object to be enabled</param>
        /// <param name="itemPosition">The pool object position</param>
        /// <param name="itemRotation">The pool object rotation</param>
        /// <param name="myParent">The pool object to be child of gameobject</param>
        public GameObject EnableObjectFromPool(PoolObjects poolObject, Vector2 itemPosition, Quaternion itemRotation, GameObject myParent)
        {
            GameObject newObject = poolObject.gameObject;
            newObject.transform.position = itemPosition;
            newObject.transform.rotation = itemRotation;
            newObject.transform.SetParent(myParent.transform);
            poolObject.ActivatePoolObject();
            return newObject;
        }

        /// <summary>
        /// Enable pool object
        /// </summary>
        /// <param name="poolObject">The pool object to be enabled</param>
        /// <param name="myParent">The pool object to be child of gameobject</param>
        public GameObject EnableObjectFromPool(PoolObjects poolObject, GameObject myParent)
        {
            GameObject newObject = poolObject.gameObject;
            newObject.transform.position = myParent.transform.position;
            newObject.transform.rotation = myParent.transform.rotation;
            newObject.transform.SetParent(myParent.transform);
            poolObject.ActivatePoolObject();
            return newObject;
        }
        #endregion
    }
}