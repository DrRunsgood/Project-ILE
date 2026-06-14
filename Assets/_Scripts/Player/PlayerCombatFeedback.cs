using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using _Scripts.UI.HUD;

namespace _Scripts.Player
{
    public sealed class PlayerCombatFeedback : NetworkBehaviour
    {
        [Header("Hit Marker")]
        [SerializeField] float minHitMarkerIntervalSeconds = 0.06f;

        uint _nextServerHitMarkerTick;

        [Server]
        public void ServerNotifyHitMarker()
        {
            if (Owner == null || !Owner.IsValid)
                return;

            uint now = TimeManager.Tick;

            if (now < _nextServerHitMarkerTick)
                return;

            uint intervalTicks = TimeManager.TimeToTicks(
                Mathf.Max(0f, minHitMarkerIntervalSeconds));

            if (intervalTicks < 1u)
                intervalTicks = 1u;

            _nextServerHitMarkerTick = now + intervalTicks;

            Target_ShowHitMarker(Owner);
        }

        [TargetRpc]
        void Target_ShowHitMarker(NetworkConnection _)
        {
            HitMarkerUI ui = HitMarkerUI.Instance;

            if (ui == null)
                ui = FindFirstObjectByType<HitMarkerUI>();

            ui?.ShowHit();
        }
    }
}