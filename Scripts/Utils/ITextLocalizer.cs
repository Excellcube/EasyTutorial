using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Utils {
    [System.Serializable]
    abstract public class TextLocalizer : MonoBehaviour
    {
        public abstract string GetLocalizedText(string table, string key);
    }
}