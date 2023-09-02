using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial
{
    [System.Serializable]
    public enum IndicatorPosition
    {
        NONE, TOP, BOTTOM, LEFT, RIGHT
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
        private Transform m_HighlightTarget;
        public  Transform HighlightTarget
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


        /// <summary>
        /// 투명한 색의 터치 불가능 영역을 사용하는지 여부를 설정.
        /// </summary>
        [SerializeField]
        private bool m_UseTransparentBlockScreen = false;
        public  bool UseTransparentBlockScreen => m_UseTransparentBlockScreen;


        [SerializeField]
        private string m_CompleteKey;
        public  string CompleteKey => m_CompleteKey;

        [SerializeField]
        private ConditionKey m_ConditionKey;
        public ConditionKey conditionKey => m_ConditionKey;
    }
}