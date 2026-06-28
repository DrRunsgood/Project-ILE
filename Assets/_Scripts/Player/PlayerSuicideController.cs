using FishNet.Object;
using UnityEngine;

namespace _Scripts.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(InputHandler))]
    [RequireComponent(typeof(PlayerHealth))]
    public sealed class PlayerSuicideController : NetworkBehaviour
    {
        [SerializeField] private bool allowSuicide = true;

        private InputHandler _input;
        private PlayerHealth _health;
        private AdvancedPredictedController _ctrl;

        private void Awake()
        {
            _input = GetComponent<InputHandler>();
            _health = GetComponent<PlayerHealth>();
            _ctrl = GetComponent<AdvancedPredictedController>();
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            if (!allowSuicide)
                return;

            if (_input == null || !_input.ConsumeSuicide())
                return;

            if (_ctrl != null && _ctrl.IsFrozen)
                return;

            Server_RequestSuicide();

            Server_RequestSuicide();
        }

        [ServerRpc(RequireOwnership = true)]
        private void Server_RequestSuicide()
        {
            if (_health == null)
                _health = GetComponent<PlayerHealth>();

            if (_health == null)
                return;

            _health.ServerSuicide();
        }
    }
}