using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using Excellcube.EasyTutorial.Widget;

namespace Excellcube.EasyTutorial.Page
{
    public class DialogTutorialPageView : MonoBehaviour, IPointerClickHandler {
        [SerializeField]
        private Image m_LeftImage;
        public  Image LeftImage => m_LeftImage;

        [SerializeField]
        private Image m_RightImage;
        public  Image RightImage => m_RightImage;

        [SerializeField]
        private DialogArrow m_Arrow;
        public  DialogArrow Arrow => m_Arrow;

        [SerializeField]
        private TextMeshProUGUI m_NameText;
        public  TextMeshProUGUI NameText => m_NameText;

        [SerializeField]
        private TextMeshProUGUI m_DialogText;
        public  TextMeshProUGUI DialogText => m_DialogText;

        private UnityAction m_ClickAction;


        private int m_DialogCursurPos;
        private int m_DialogLength;


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

        public void StartTyping(string content)
        {
            StartCoroutine( StartTypingInternal(content) );
        }

        private IEnumerator StartTypingInternal(string content)
        {
            PrepareTyping(content);

            yield return RunTyping();

            FinishTyping();
        }

        private void PrepareTyping(string content)
        {
            m_Arrow.Hide();

            m_DialogCursurPos = 0;
            m_DialogLength = content.Length;

            m_DialogText.text = content;
            m_DialogText.maxVisibleCharacters = 0;
        }

        private IEnumerator RunTyping()
        {
            for(m_DialogCursurPos=1 ; m_DialogCursurPos<=m_DialogLength ; m_DialogCursurPos++)
            {
                yield return new WaitForSeconds(0.03f);
                m_DialogCursurPos = m_DialogCursurPos > m_DialogLength ? m_DialogLength : m_DialogCursurPos;
                m_DialogText.maxVisibleCharacters = m_DialogCursurPos;
            }
        }

        private void FinishTyping()
        {
            m_Arrow.Show();
            m_DialogCursurPos = m_DialogLength;
        }

        public void SkipTyping()
        {
            m_DialogCursurPos = m_DialogLength;
        }

        public bool IsTyping()
        {
            return m_DialogCursurPos != m_DialogLength;
        }
    }
}