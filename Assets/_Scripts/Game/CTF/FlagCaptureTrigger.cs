using FishNet.Object;
using UnityEngine;
using _Scripts.Game.Teams;
using _Scripts.Player;

namespace _Scripts.Game.CTF
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class FlagCaptureTrigger : NetworkBehaviour
    {
        [Header("Setup")]
        [SerializeField] TeamId team;

        void Awake()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;

            PlayerIdentity identity = other.GetComponentInParent<PlayerIdentity>();
            if (identity == null || identity.Team != team)
                return;

            PlayerHealth hp = other.GetComponentInParent<PlayerHealth>();
            if (hp == null || hp.IsDead)
                return;

            FlagCarrier carrier = other.GetComponentInParent<FlagCarrier>();
            if (carrier == null || !carrier.HasFlag)
                return;

            CTFManager.Instance?.Server_TryCapture(carrier, team);
        }
    }
}