// _Scripts/Packs/PackId.cs
namespace _Scripts.Packs
{
    /// Up to 8 packs ⇒ fits in 3 bits.
    public enum PackId : byte
    {
        None      = 0,
        Energy    = 1,
        Shield    = 2,
        // add more: Repair = 3, etc…
    }
}