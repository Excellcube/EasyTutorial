using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Coffee.UIExtensions;

namespace Excellcube.EasyTutorial.Page
{
    

    public class ActionTutorialPageView : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI m_ActionLogText;
        public  TextMeshProUGUI ActionLogText => m_ActionLogText;

        [SerializeField]
        private Widget.Indicator m_Indicator;
        public  Widget.Indicator Indicator => m_Indicator;

        [SerializeField]
        private Unmask m_UnmaskPanel;
        public  Unmask UnmaskPanel => m_UnmaskPanel;
    }
}