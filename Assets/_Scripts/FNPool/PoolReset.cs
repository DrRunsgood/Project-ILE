using FishNet.Object;
using UnityEngine;

namespace _Scripts.FNPool
{
    [DisallowMultipleComponent]
    public sealed class PoolReset : NetworkBehaviour
    {
        private Vector3 _prefabLocalPosition;
        private Quaternion _prefabLocalRotation;
        private Vector3 _prefabLocalScale;

        private void Awake()
        {
            _prefabLocalPosition = transform.localPosition;
            _prefabLocalRotation = transform.localRotation;
            _prefabLocalScale = transform.localScale;
        }

        /*
         * Called when the object is taken from the pool, before its new
         * owner configures its parent or world pose.
         *
         * Do not modify transforms from OnStopClient/OnStopServer:
         * those callbacks may occur from NetworkObject.OnDestroy.
         */
        public void ResetForReuse()
        {
            transform.SetParent(null, false);

            transform.localPosition = _prefabLocalPosition;
            transform.localRotation = _prefabLocalRotation;
            transform.localScale = _prefabLocalScale;
        }
    }
}