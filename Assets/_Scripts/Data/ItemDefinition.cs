// _Scripts/Data/ItemDefinition.cs
using UnityEngine;
using FishNet.Object;   // for world prefabs

public enum ItemId : byte { None, Frag, EMP, HealthKit, Beacon }

[CreateAssetMenu(menuName = "Items/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public ItemId    id;
    public string    displayName;
    public Sprite    icon;
    public byte      maxStack   = 2;
    public NetworkObject worldPickupPrefab;   // lying on ground
    public NetworkObject useSpawnPrefab;      // grenade / beacon; null for med-kit
}