using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Utils
{
    public class TutorialSelectionTarget : MonoBehaviour
    {
        [SerializeField]
        private string m_Key;
        public  string Key 
        {
            get => m_Key;
            set => m_Key = value;
        }
    }
}
