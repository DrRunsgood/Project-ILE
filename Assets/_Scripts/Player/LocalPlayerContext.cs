using System;
using _Scripts.Packs;
using _Scripts.Player;
using _Scripts.Weapons;
using UnityEngine;

namespace _Scripts.Player
{
    /// <summary>
    /// Single local-owner registry for UI, camera, and other local-only systems.
    /// Keeps local-player discovery out of presentation scripts.
    /// </summary>
    public static class LocalPlayerContext
    {
        public static AdvancedPredictedController Controller { get; private set; }
        public static PlayerHealth Health { get; private set; }
        public static PackManager PackManager { get; private set; }
        public static WeaponManager WeaponManager { get; private set; }
        public static InputHandler InputHandler { get; private set; }

        public static bool IsReady => Controller != null;

        public static event Action<AdvancedPredictedController> OnLocalPlayerReady;
        public static event Action OnLocalPlayerCleared;

        public static void Register(AdvancedPredictedController controller)
        {
            if (controller == null)
            {
                Debug.LogWarning("[LocalPlayerContext] Tried to register a null controller.");
                return;
            }

            Controller = controller;
            Health = controller.GetComponent<PlayerHealth>();
            PackManager = controller.GetComponent<PackManager>();
            WeaponManager = controller.GetComponent<WeaponManager>();
            InputHandler = controller.GetComponent<InputHandler>();

            Debug.Log($"[LocalPlayerContext] Registered local player: {controller.name}");
            OnLocalPlayerReady?.Invoke(controller);
        }

        public static void Clear(AdvancedPredictedController controller)
        {
            if (controller != null && Controller != controller)
                return;

            Debug.Log("[LocalPlayerContext] Cleared local player.");

            Controller = null;
            Health = null;
            PackManager = null;
            WeaponManager = null;
            InputHandler = null;

            OnLocalPlayerCleared?.Invoke();
        }
    }
}