using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace FluentTaskScheduler.Services
{
    public static class LocalizationService
    {
        private static readonly HashSet<string> _supportedLanguages = new(StringComparer.OrdinalIgnoreCase)
        {
            "en-US",
            "zh-CN"
        };

        private static string _currentLanguage = "en-US";

        public static event EventHandler? LanguageChanged;

        public static string CurrentLanguage => _currentLanguage;

        public static void Initialize()
        {
            ApplyLanguage(NormalizeLanguage(SettingsService.Language), raiseEvent: false);
        }

        public static bool ChangeLanguage(string language)
        {
            string normalized = NormalizeLanguage(language);
            if (string.Equals(_currentLanguage, normalized, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            ApplyLanguage(normalized, raiseEvent: true);
            return true;
        }

        public static string GetString(string key, string fallback = "")
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return fallback;
            }

            try
            {
                string value = ResourceLoader.GetForViewIndependentUse().GetString(key);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            catch
            {
                // Ignore and fall back.
            }

            return string.IsNullOrEmpty(fallback) ? key : fallback;
        }

        private static void ApplyLanguage(string language, bool raiseEvent)
        {
            _currentLanguage = language;
            SettingsService.Language = language;

            try
            {
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = language;
            }
            catch
            {
                // Ignore override failures and keep app running.
            }

            if (raiseEvent)
            {
                LanguageChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        private static string NormalizeLanguage(string? language)
        {
            if (!string.IsNullOrWhiteSpace(language) && _supportedLanguages.Contains(language))
            {
                return language;
            }

            return "en-US";
        }
    }
}
