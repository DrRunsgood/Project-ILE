namespace _Scripts.Bootstrap
{
    public struct StartupConfig
    {
        public bool IsDedicatedServer;
        public ushort Port;
        public string MapSceneName;

        public StartupConfig(bool isDedicatedServer, ushort port, string mapSceneName)
        {
            IsDedicatedServer = isDedicatedServer;
            Port = port;
            MapSceneName = mapSceneName;
        }
    }
}