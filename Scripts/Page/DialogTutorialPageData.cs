using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Page
{
    [System.Serializable]
    public class DialogTutorialPageData : TutorialPageData 
    {
        [SerializeField]
        private Sprite m_LeftSprite;
        public  Sprite LeftSprite => m_LeftSprite;

        [SerializeField]
        private Sprite m_RightSprite;
        public  Sprite RightSprite => m_RightSprite;

        [SerializeField]
        private string m_CharacterName;
        public string CharacterName {
            get {
                if(m_UseLocalization) {
                    return m_TextLocalizer.GetLocalizedText(m_LocalizationTable, m_CharacterNameKey);
                } else {
                    return m_CharacterName;
                }
            }
        }

        [SerializeField]
        private string m_CharacterNameKey;
        public  string CharacterNameKey => m_CharacterNameKey;

        [SerializeField]
        private string m_Dialog;
        public string Dialog {
            get {
                if(m_UseLocalization) {
                    return m_TextLocalizer.GetLocalizedText(m_LocalizationTable, m_DialogKey);
                } else {
                    return m_Dialog;
                }
            }
        }

        [SerializeField]
        private string m_DialogKey;
        public  string DialogKey => m_DialogKey;
    }
}