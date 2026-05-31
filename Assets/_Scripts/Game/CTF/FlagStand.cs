using FishNet.Object;
using UnityEngine;
using _Scripts.Game.Teams;

namespace _Scripts.Game.CTF
{
    [DisallowMultipleComponent]
    public sealed class FlagStand : NetworkBehaviour
    {
        [Header("Setup")]
        [SerializeField] TeamId team;
        [SerializeField] Transform homePoint;
        [SerializeField] FlagObject flagPrefab;

        FlagObject _flagInstance;

        public TeamId Team => team;

        public Transform HomePoint => homePoint;

        public FlagObject CurrentFlag => _flagInstance;

        public bool IsFlagHome =>
            _flagInstance != null &&
            _flagInstance.State == FlagState.Home;

        public override void OnStartServer()
        {
            base.OnStartServer();

            SpawnFlag();
        }

        [Server]
        void SpawnFlag()
        {
            if (_flagInstance != null)
                return;

            FlagObject flag = Instantiate(
                flagPrefab,
                homePoint.position,
                homePoint.rotation);

            Spawn(flag.gameObject);

            flag.Server_Initialize(this);

            _flagInstance = flag;
        }
    }
}