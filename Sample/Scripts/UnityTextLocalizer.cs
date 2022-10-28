using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Excellcube.EasyTutorial.Utils {
    public class UnityTextLocalizer : TextLocalizer
    {
        public override string GetLocalizedText(string table, string key)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
        }
    }
}