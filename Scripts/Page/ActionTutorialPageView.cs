using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Coffee.UIExtensions;

namespace Excellcube.EasyTutorial.Page
{
    public class ActionTutorialPageView : MonoBehaviour {
        [SerializeField]
        private Text m_ActionLogText;
        public  Text ActionLogText => m_ActionLogText;

        [SerializeField]
        private Widget.Indicator m_Indicator;
        public  Widget.Indicator Indicator => m_Indicator;

        [SerializeField]
        private Unmask m_UnmaskPanel;
        public  Unmask UnmaskPanel => m_UnmaskPanel;
    }
}