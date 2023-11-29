using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Coffee.UIExtensions;

namespace Excellcube.EasyTutorial
{
    public class ActionTutorialPageView : MonoBehaviour, IPointerClickHandler {
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

        [SerializeField]
        private Image m_TapScreenTarget;
        public  Image TapScreenTarget => m_TapScreenTarget;

        [SerializeField]
        private Button m_CompleteButton;
        public  Button CompleteButton => m_CompleteButton;

        private UnityAction m_ClickAction;


        public void OnPointerClick(PointerEventData eventData)
        {
            if(m_ClickAction != null)
            {
                m_ClickAction();
            }
        }

        public void AddClickAction(UnityAction action)
        {
            m_ClickAction = action;
        }
    }
}