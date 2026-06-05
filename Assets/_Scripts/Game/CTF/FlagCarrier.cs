using FishNet.Object;
using UnityEngine;
using FishNet.Connection;
using _Scripts.Player;

namespace _Scripts.Game.CTF
{
    [DisallowMultipleComponent]
    public sealed class FlagCarrier : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] Transform carryAnchor;

        FlagObject _carriedFlag;
        
        InputHandler _ih;

        public bool HasFlag => _carriedFlag != null;

        public FlagObject CarriedFlag => _carriedFlag;

        public Transform CarryAnchor => carryAnchor;
        
        void Awake()
        {
            _ih = GetComponent<InputHandler>();
        }
        
        void Update()
        {
            if (!IsOwner || _ih == null)
                return;

            if (_ih.ConsumeFlagThrow())
                Server_RequestThrowFlag(TimeManager.Tick);
        }

        [Server]
        public bool Server_CanCarryFlag()
        {
            return _carriedFlag == null;
        }

        [Server]
        public void Server_ClearFlag(FlagObject flag)
        {
            if (_carriedFlag == flag)
                _carriedFlag = null;
        }

        [Server]
        public void Server_SetFlag(FlagObject flag)
        {
            _carriedFlag = flag;
        }

        [Server]
        public void Server_DropCarriedFlagOnDeath()
        {
            if (_carriedFlag == null)
                return;

            _carriedFlag.Server_DropFromCarrier();
        }
        
        [ServerRpc(RequireOwnership = true)]
        void Server_RequestThrowFlag(uint clientTick, NetworkConnection sender = null)
        {
            if (_carriedFlag == null)
                return;

            _carriedFlag.Server_ThrowFromCarrier(this, clientTick);
        }
    }
}