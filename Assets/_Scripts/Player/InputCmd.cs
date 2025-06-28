namespace _Scripts.Player                // use one consistent namespace
{
    /* ------------------------------------------------------------ *
     *  Bit-flags for all boolean inputs (one byte = eight buttons) *
     * ------------------------------------------------------------ */
    [System.Flags]
    public enum InputButtons : byte
    {
        None    = 0,
        Jump    = 1 << 0,
        Sprint  = 1 << 1,
        Crouch  = 1 << 2,
        Jetpack = 1 << 3,
        Ski     = 1 << 4,
        WallRun = 1 << 5,
        Fire    = 1 << 6,
        // add more as needed (keep ≤ 8 flags)
    }
}