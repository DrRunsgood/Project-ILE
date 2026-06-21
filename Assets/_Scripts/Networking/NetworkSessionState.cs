namespace _Scripts.Networking
{
    public enum NetworkSessionState : byte
    {
        Offline = 0,

        ClientMenu = 10,
        Connecting = 20,
        Connected = 30,
        LoadingGameplay = 40,
        InGame = 50,

        Disconnecting = 60,

        ServerStarting = 100,
        ServerLoadingGameplay = 105,
        ServerRunning = 110,
        ServerStopping = 120,

        Failed = 200
    }
}