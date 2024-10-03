using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP
{
    public class TeenPattiPoolCards : MonoBehaviour
    {
        public static TeenPattiPoolCards PoolCardInstance;
        [SerializeField] private List<GameObject> pooledCards;
        [SerializeField] private GameObject cardToPool;
        [SerializeField] private int amountToPool;
        [SerializeField] private GameObject cardsParent;
        private void Awake()
        {
            PoolCardInstance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            pooledCards = new List<GameObject>();
    
            for(int i = 0; i < amountToPool; i++)
            {
                cardsParent = Instantiate(cardToPool,transform);
                cardsParent.SetActive(false);
                pooledCards.Add(cardsParent);
            }
        }


        public GameObject GetPooledCards()
        {
            for(int i = 0; i < amountToPool; i++)
            {
                if (!pooledCards[i].activeInHierarchy)
                {
                    return pooledCards[i];
                }
            }
            return null;
        }

    
    }
}
