using UnityEngine;
using FishNet;                       // InstanceFinder
using _Scripts.Player;               // controller + enums

namespace _Scripts.Player
{
    /// Per-render-frame pitch smoothing (yaw already comes from NTS).
    public sealed class ViewInterpolation : MonoBehaviour
    {
        [Tooltip("Camera pivot that pitches up/down.")]
        [SerializeField] Transform view;

        [Tooltip("Predicted controller supplying Prev / Current pitch.")]
        [SerializeField] AdvancedPredictedController source;

        void Awake()
        {
            if (source == null)
                source = GetComponentInParent<AdvancedPredictedController>();
            if (view == null)
                view = transform;
        }

        void LateUpdate()
        {
            if (source == null || view == null) return;

            /* How far we are between last simulated tick and the next one. */
            double dt   = Time.timeAsDouble - source.LastTickTime;
            double tLen = InstanceFinder.TimeManager.TickDelta;   // seconds per tick
            float  a    = Mathf.Clamp01((float)(dt / tLen));

            /* Interpolate only pitch – yaw is already smoothed by NTS. */
            float pitch = Mathf.Lerp(source.PrevPitch, source.CurrentPitch, a);
            Vector3 eul = view.localEulerAngles;
            eul.x = pitch;
            view.localEulerAngles = eul;
        }
    }
}