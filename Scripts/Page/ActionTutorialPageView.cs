using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Coffee.UIExtensions;

namespace Excellcube.EasyTutorial
{
    public class ActionTutorialPageView : MonoBehaviour {
        [SerializeField]
        private Text m_ActionLogText;
        public  Text ActionLogText => m_ActionLogText;

        [SerializeField]
        private Indicator m_Indicator;
        public  Indicator Indicator => m_Indicator;

        [SerializeField]
        private Unmask m_UnmaskPanel;
        public  Unmask UnmaskPanel => m_UnmaskPanel;

        [SerializeField]
        private Image m_BlockScreenImage;
        public  Image BlockScreenImage => m_BlockScreenImage;
    }
}