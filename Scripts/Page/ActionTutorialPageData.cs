using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Page
{
    [System.Serializable]
    public enum IndicatorPosition
    {
        TOP, BOTTOM
    }

    [System.Serializable]
    public class ActionTutorialPageData : TutorialPageData 
    {
        [SerializeField]
        private string m_ActionLog;
        public string ActionLog {
            get {
                if(m_UseLocalization) {
                    return m_TextLocalizer.GetLocalizedText(m_LocalizationTable, m_ActionLogKey);
                } else {
                    return m_ActionLog;
                }
            }
        }

        [SerializeField]
        private string m_ActionLogKey;
        public  string ActionLogKey => m_ActionLogKey;


        [SerializeField]
        private RectTransform m_HighlightTarget;
        public  RectTransform HighlightTarget
        {
            get => m_HighlightTarget;
            set => m_HighlightTarget = value;
        }

        [SerializeField]
        private Transform m_DynamicTargetRoot;
        public  Transform DynamicTargetRoot => m_DynamicTargetRoot;

        [SerializeField]
        private string m_DynamicTargetKey;
        public  string DynamicTargetKey => m_DynamicTargetKey;



        [SerializeField]
        private IndicatorPosition m_IndicatorPosition = IndicatorPosition.TOP;
        public  IndicatorPosition IndicatorPosition => m_IndicatorPosition;


        [SerializeField]
        private string m_CompleteKey;
        public  string CompleteKey => m_CompleteKey;
    }
}