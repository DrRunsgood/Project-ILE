using UnityEngine;

namespace _Scripts.Bootstrap
{
    public static class ClientBootstrapSettings
    {
        private const string NameKey = "ProjectILE.DisplayName";
        private const string AddressKey = "ProjectILE.ServerAddress";
        private const string PortKey = "ProjectILE.ServerPort";

        public static string DisplayName
        {
            get => SanitizeName(PlayerPrefs.GetString(NameKey, "Player"));
            set
            {
                PlayerPrefs.SetString(NameKey, SanitizeName(value));
                PlayerPrefs.Save();
            }
        }

        public static string ServerAddress
        {
            get => PlayerPrefs.GetString(AddressKey, "localhost");
            set
            {
                PlayerPrefs.SetString(AddressKey, SanitizeAddress(value));
                PlayerPrefs.Save();
            }
        }

        public static ushort ServerPort
        {
            get
            {
                int value = PlayerPrefs.GetInt(PortKey, 7770);
                return (ushort)Mathf.Clamp(value, 1, ushort.MaxValue);
            }
            set
            {
                PlayerPrefs.SetInt(PortKey, value);
                PlayerPrefs.Save();
            }
        }

        public static string SanitizeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Player";

            value = value.Trim();

            if (value.Length > 24)
                value = value.Substring(0, 24);

            return value;
        }

        private static string SanitizeAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "localhost";

            return value.Trim();
        }
    }
}