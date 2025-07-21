// _Scripts/Items/ItemDatabase.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] ItemDefinition[] items;          // assign in Inspector

    static Dictionary<ItemId, ItemDefinition> _map;

    /* -------- public API -------- */
    public static ItemDefinition Get(ItemId id)
    {
        if (_map == null)
            LoadDatabase();

        _map.TryGetValue(id, out var def);
        return def;
    }

    /* -------- helpers -------- */
    static void LoadDatabase()
    {
        /* look for an asset named "ItemDatabase" anywhere in Resources */
        var db = Resources.Load<ItemDatabase>("ItemDatabase");
        if (db == null)
        {
            Debug.LogError("ItemDatabase asset not found in a Resources folder!");
            _map = new Dictionary<ItemId, ItemDefinition>();
            return;
        }

        _map = db.items
            .Where(d => d)                     // skip null slots
            .ToDictionary(d => d.id, d => d);
    }
}