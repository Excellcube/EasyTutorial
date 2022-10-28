using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Excellcube.EasyTutorial.Page
{
    [System.Serializable]
    public class TutorialPageData
    {
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
        private bool m_Skip = false;
        public  bool Skip
        {
            get => m_Skip;
        }

        [SerializeField]
        private bool m_HideSkipButton = false;
        public  bool HideSkipButton
        {
            get => m_HideSkipButton;
        }


        [SerializeField]
        private float m_StartDelay;  // second 단위의 대기 시간 입력.
        public  float StartDelay => m_StartDelay;

        [SerializeField]
        private bool m_BlockTouchDuringDelay;
        public  bool BlockTouchDuringDelay
        {
            get => m_BlockTouchDuringDelay;
        }


        [SerializeField]
        private UnityEvent m_OnTutorialBegin;
        public  UnityEvent OnTutorialBegin => m_OnTutorialBegin;

        [SerializeField]
        private UnityEvent m_OnTutorialEnded;
        public  UnityEvent OnTutorialEnded => m_OnTutorialEnded;
    }
}