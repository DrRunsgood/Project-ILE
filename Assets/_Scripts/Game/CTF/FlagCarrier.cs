using FishNet.Object;
using UnityEngine;
using _Scripts.Weapons;

namespace _Scripts.Game.CTF
{
    [DisallowMultipleComponent]
    public sealed class FlagCarrier : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform carryAnchor;

        private FlagObject _carriedFlag;

        public bool HasFlag => _carriedFlag != null;
        public FlagObject CarriedFlag => _carriedFlag;
        public Transform CarryAnchor => carryAnchor;

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

        [Server]
        public void Server_ProcessThrowInput(bool throwPressed, FirePose pose)
        {
            if (!throwPressed || _carriedFlag == null)
                return;

            _carriedFlag.Server_ThrowFromCarrier(this, pose);
        }
    }
}