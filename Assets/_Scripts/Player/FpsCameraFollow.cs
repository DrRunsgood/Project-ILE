using UnityEngine;
using _Scripts.Player;

[RequireComponent(typeof(Camera))]
public class FpsCameraFollow : MonoBehaviour
{
    /* runtime references */
    AdvancedPredictedController target;
    Transform head;
    Camera   cam;
    InputHandler ih;
    Renderer[] characterRenderers;

    /* inspector: third-person offset */
    [Header("Third-Person Offset")]
    [SerializeField] float back  = 10f;   // distance behind
    [SerializeField] float up    = 2f; // height above head

    const string FP_LAYER = "FP_Only";
    const string TP_LAYER = "TP_Only";

    bool inThirdPerson;

    /* external setter (called by player) */
    public void SetTarget(AdvancedPredictedController t)
    {
        target = t;
        head   = t ? t.HeadAnchor : null;
        ih     = t ? t.GetComponent<InputHandler>() : null;   // ← add this line
        
        if (target != null)
            characterRenderers = target.GetComponentsInChildren<Renderer>(true);
    }

    void Start()
    {
        cam = GetComponent<Camera>();

        /* auto-find owner if not set */
        if (target == null)
            foreach (var pc in FindObjectsByType<AdvancedPredictedController>(FindObjectsSortMode.None))
                if (pc.IsOwner) { SetTarget(pc); break; }

        ApplyCulling(false);               // start FP
        if (head) transform.position = head.position;
    }

    void Update()
    {
        if (ih && ih.ConsumeViewToggle())
        {
            inThirdPerson = !inThirdPerson;
            ApplyCulling(inThirdPerson);
        }
    }
    
    void LateUpdate()
    {
        if (!target || !head) return;
        
        float yaw = target.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(target.CurrentPitch, yaw, 0f);
        
        if (inThirdPerson)
        {
            Vector3 pos = head.position - transform.forward * back + Vector3.up * up;   // use camera’s own forward
            transform.position = pos;
        }
        else
            transform.position = head.position;
    }

    /* layer toggle */
    void ApplyCulling(bool tpMode)
    {
        int fp = 1 << LayerMask.NameToLayer(FP_LAYER);
        int tp = 1 << LayerMask.NameToLayer(TP_LAYER);

        cam.cullingMask = tpMode
            ? (cam.cullingMask | tp) & ~fp   // show TP, hide FP
            : (cam.cullingMask | fp) & ~tp;   // show FP, hide TP
    }
}
