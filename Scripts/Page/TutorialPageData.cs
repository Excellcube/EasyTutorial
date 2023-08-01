using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Excellcube.EasyTutorial.Page
{
    [System.Serializable]
    public class TutorialPageData
    {
        [SerializeField]
        private string m_Name;
        public string name => m_Name;

        protected bool m_UseLocalization;
        public  bool UseLocalization 
        {
            get => m_UseLocalization;
            set => m_UseLocalization = value;   
        }

        protected Utils.TextLocalizer m_TextLocalizer;
        public  Utils.TextLocalizer TextLocalizer
        {
            get => m_TextLocalizer;
            set => m_TextLocalizer = value;
        }

        protected string m_LocalizationTable;
        public string LocalizationTable
        {
            get => m_LocalizationTable;
            set => m_LocalizationTable = value;
        }

        [SerializeField]
        private float m_StartDelay;  // second 단위의 대기 시간 입력.
        public  float StartDelay => m_StartDelay;


        /// <summary>
        /// Tutorial 실행 직전에 호출되는 이벤트.
        /// </summary>
        [SerializeField]
        private UnityEvent m_OnTutorialBegin;
        public  UnityEvent OnTutorialBegin => m_OnTutorialBegin;

        /// <summary>
        /// Tutorial 실행 직후에 호출되는 이벤트
        /// </summary>
        [SerializeField]
        private UnityEvent m_OnTutorialInvoked;
        public  UnityEvent OnTutorialInvoked => m_OnTutorialInvoked;

        /// <summary>
        /// Tutorial 종료 후 호출되는 이벤트.
        /// </summary>
        [SerializeField]
        private UnityEvent m_OnTutorialEnded;
        public  UnityEvent OnTutorialEnded => m_OnTutorialEnded;
    }
}