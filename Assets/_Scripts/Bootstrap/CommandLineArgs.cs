using System;
using UnityEngine;

namespace _Scripts.Bootstrap
{
    public static class CommandLineArgs
    {
        public static StartupConfig ApplyTo(StartupConfig defaults)
        {
            string[] args = Environment.GetCommandLineArgs();

            StartupConfig config = defaults;

            bool hasDedicatedArg = false;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (IsArg(arg, "-dedicated", "--dedicated") ||
                    IsArg(arg, "-server", "--server"))
                {
                    config.IsDedicatedServer = true;
                    hasDedicatedArg = true;
                    continue;
                }

                if (IsArg(arg, "-port", "--port"))
                {
                    if (TryGetNext(args, i, out string portValue))
                    {
                        if (ushort.TryParse(portValue, out ushort parsedPort))
                            config.Port = parsedPort;
                        else
                            Debug.LogWarning($"[CommandLineArgs] Invalid port value: {portValue}");

                        i++;
                    }
                    else
                    {
                        Debug.LogWarning("[CommandLineArgs] Missing value after -port.");
                    }

                    continue;
                }

                if (IsArg(arg, "-map", "--map"))
                {
                    if (TryGetNext(args, i, out string mapValue))
                    {
                        if (!string.IsNullOrWhiteSpace(mapValue))
                            config.MapSceneName = mapValue.Trim();
                        else
                            Debug.LogWarning("[CommandLineArgs] Empty map value.");

                        i++;
                    }
                    else
                    {
                        Debug.LogWarning("[CommandLineArgs] Missing value after -map.");
                    }

                    continue;
                }
            }

#if UNITY_SERVER
            config.IsDedicatedServer = true;
#endif

            if (Application.isBatchMode && hasDedicatedArg)
                config.IsDedicatedServer = true;

            Debug.Log(
                $"[CommandLineArgs] Final startup config: " +
                $"dedicated={config.IsDedicatedServer}, " +
                $"port={config.Port}, " +
                $"map={config.MapSceneName}, " +
                $"batchMode={Application.isBatchMode}");

            return config;
        }

        private static bool IsArg(string value, string shortName, string longName)
        {
            return string.Equals(value, shortName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(value, longName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryGetNext(string[] args, int index, out string value)
        {
            int nextIndex = index + 1;

            if (nextIndex >= args.Length)
            {
                value = null;
                return false;
            }

            value = args[nextIndex];
            return true;
        }
    }
}