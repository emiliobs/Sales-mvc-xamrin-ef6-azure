namespace Sales.Helpers
{
    using Plugin.Settings;
    using Plugin.Settings.Abstractions;
    public static class Settings
    {
        #region Setting Constants

        private const string accessToken = "AccessToken";
        private const string tokenType = "TokenType";
        private const string isRemembered = "IsRemembered";
        private const string userASP = "UserASP";
        private static readonly string stringDefault = string.Empty;
        private static readonly bool booleanDefault = false;

        #endregion

        #region Constructor
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }
        #endregion


        #region Properties
        public static string UserASP
        {
            get
            {
                return AppSettings.GetValueOrDefault(userASP, stringDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(userASP, value);
            }
        }

        public static string TokenType
        {
            get
            {
                return AppSettings.GetValueOrDefault(tokenType, stringDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(tokenType, value);
            }
        }

        public static bool IsRemembered
        {
            get
            {
                return AppSettings.GetValueOrDefault(isRemembered, booleanDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(isRemembered, value);
            }
        }

    public static string AccessToken
    {
            get
            {
                return AppSettings.GetValueOrDefault(accessToken, stringDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(accessToken, value);
            }
        }


        #endregion

    }
}
