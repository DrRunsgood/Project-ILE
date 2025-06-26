using FishNet.Object;
using UnityEngine;

public sealed class PoolReset : NetworkBehaviour
{
    Vector3 _prefabPos;
    Quaternion _prefabRot;
    Vector3 _prefabScale;

    void Awake()            // runs once, even for pooled objects
    {
        _prefabPos   = transform.localPosition;
        _prefabRot   = transform.localRotation;
        _prefabScale = transform.localScale;
    }

    /* Fish-Net 4.5.x – use the two callbacks that
       still exist in that version. They are invoked
       just before the object is returned to the pool. */
    //public override void OnStopServer()  => ResetTRS();

    void ResetTRS()
    {
        transform.SetParent(null, false);         // detach
        transform.localPosition = _prefabPos;     // restore native TRS
        transform.localRotation = _prefabRot;
        transform.localScale    = _prefabScale;
    }
}