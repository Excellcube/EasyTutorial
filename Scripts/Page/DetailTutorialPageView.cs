using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Excellcube.EasyTutorial.Page
{
    public class DetailTutorialPageView : MonoBehaviour {
        [SerializeField]
        private Image m_DetailImage;
        public  Image DetailImage => m_DetailImage;
        [SerializeField]
        private Text m_DescriptionText;
        public  Text DescriptionText => m_DescriptionText;
        [SerializeField]
        private DetailTutorialPopupView m_PopupView;
        public  DetailTutorialPopupView PopupView => m_PopupView;
    }
}