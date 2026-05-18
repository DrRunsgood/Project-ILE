using UnityEngine;
using _Scripts.Player;

[DisallowMultipleComponent]
public sealed class PlayerIFFTarget : MonoBehaviour
{
    [SerializeField] Transform iffAnchor;

    public Transform Anchor => iffAnchor != null ? iffAnchor : transform;

    public PlayerIdentity Identity { get; private set; }
    public PlayerHealth Health { get; private set; }

    void Awake()
    {
        Identity = GetComponent<PlayerIdentity>();
        Health = GetComponent<PlayerHealth>();
    }
}