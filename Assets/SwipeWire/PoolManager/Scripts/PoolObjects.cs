using UnityEngine;

namespace TP
{
    public class PoolObjects : MonoBehaviour
    {
        public string tagNameForParent;
        public bool active; //is this pool object active or not?
        private  GameObject parent;

        void Start()
        {
            if(!string.IsNullOrEmpty(tagNameForParent))
                parent = GameObject.FindWithTag(tagNameForParent);
        }

        // Disables a pool object.
        public void DisablePoolObject()
        {
            if (parent)
                this.transform.SetParent(parent.transform);

            this.gameObject.SetActive(false);
        }

        // Enables a pool object.
        public void ActivatePoolObject()
        {
            this.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            this.active = true;
        }

        private void OnDisable()
        {
            this.active = false;
        }
    }
}
