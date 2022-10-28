using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Page
{
    [System.Serializable]
    public class DetailTutorialPageContentData : TutorialPageData 
    {
        [SerializeField]
        private Sprite m_DetailSprite;
        public  Sprite DetailSprite => m_DetailSprite;

        [SerializeField]
        private string m_Description;
        public  string Description => m_Description;

        [SerializeField]
        private string m_DescriptionKey;
        public  string DescriptionKey => m_DescriptionKey;
    }
}