using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Excellcube.EasyTutorial.Utils;

namespace Excellcube.EasyTutorial.Page
{
    public class DialogTutorialPage : TutorialPage 
    {
        private DialogTutorialPageView m_View;
        private UnityAction m_CompleteTutorial;
        public  UnityAction CompleteTutorial
        {
            get => m_CompleteTutorial;
            set => m_CompleteTutorial = value;
        }

        public DialogTutorialPage(DialogTutorialPageView view)
        {
            m_View = view;
        }

        protected override void ConfigureView()
        {
            DialogTutorialPageData data = m_Data as DialogTutorialPageData;
            if(data == null)
            {
                Debug.LogError("[DialogTutorialPage] Fail to configure the view. Data type isn't matched with DialogTutorialPageData");
                return;
            }

            if(data.LeftSprite != null)
            {
                m_View.RightImage.gameObject.SetActive(false);
                m_View.RightImage.sprite = null;

                m_View.LeftImage.gameObject.SetActive(true);
                m_View.LeftImage.sprite = data.LeftSprite;
            }
            else if(data.RightSprite != null)
            {
                m_View.RightImage.gameObject.SetActive(true);
                m_View.RightImage.sprite = data.RightSprite;

                m_View.LeftImage.gameObject.SetActive(false);
                m_View.LeftImage.sprite = null;
            }
            else
            {
                m_View.RightImage.gameObject.SetActive(false);
                m_View.RightImage.sprite = null;
                m_View.LeftImage.gameObject.SetActive(false);
                m_View.LeftImage.sprite = null;
            }

            m_View.NameText.text = data.CharacterName;

            m_View.AddClickAction(TouchView);

            // 화면 구성이 끝나면 텍스트 타이핑 시작.
            m_View.StartTyping(data.Dialog);
        }

        private void TouchView()
        {
            if(m_View.IsTyping())
            {
                m_View.SkipTyping();
            }
            else
            {
                if(m_CompleteTutorial == null)
                {
                    Debug.LogError("[DialogTutorialPage] CompleteTutorial UnityAction isn't assigned!");
                }
                
                m_CompleteTutorial();
            }
        }
    }
}