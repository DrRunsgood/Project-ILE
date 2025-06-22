// _Scripts/Packs/PackDatabase.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Scripts.Packs;

[CreateAssetMenu(menuName = "Packs/PackDatabase")]
public class PackDatabase : ScriptableObject
{
    [SerializeField] private PackDefinition[] packs;    // drag them in

    private static Dictionary<PackId, PackDefinition> _byId;

    // Called on first access in any peer (client or server)
    public static PackDefinition Get(PackId id)
    {
        if (_byId == null)
        {
            // Load once from Resources or Addressables
            var db = Resources.Load<PackDatabase>("PackDatabase");
            _byId = db.packs.ToDictionary(p => p.id, p => p);
        }
        _byId.TryGetValue(id, out var def);
        return def;
    }
}