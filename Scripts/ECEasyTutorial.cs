using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Excellcube.EasyTutorial
{
    using Page;
    using Utils;
    using Widget;
    using UI;

    public class ECEasyTutorial : MonoBehaviour
    {
        [SerializeField]
        private bool m_PlayOnAwake = true;
        [SerializeField]
        private bool m_UseLocalization;

        [SerializeField]
        private Utils.TextLocalizer m_TextLocalizer;
        [SerializeField]
        private string m_LocalizationTable;


        /// <summary>
        /// 튜토리얼 진행 정보를 저장. PlayerPref에 "Tutorial_XXX" 라는 형태로 저장된다.
        /// SaveComplete가 true일 경우 PlayerPref 기반으로 튜토리얼 완려 여부를 확인한다.
        /// SaveComplete가 false일 경우 기존의 클리어 정보를 삭제하고 PlayerPref 기반 튜토리얼 완료 여부 체크를 비활성화 한다.
        /// </summary>
        // [SerializeField]
        // private bool m_SaveProgress;

        /// <summary>
        /// 각 TutorialData의 Skip 항목을 사용할 것인지 여부를 확인한다.
        /// </summary>
        [SerializeField]
        private bool m_EnableSkipFlag;

        /// <summary>
        /// 사용자가 Skip Tutorial 버튼을 눌렀을때 모든 튜토리얼들을 스킵하게 만드는 플래그.
        /// </summary>
        private bool m_IsSkippingAll;

        private int m_LastClearIndex;
        private int m_CurrTutorialIndex;

        private DialogTutorialPageView m_DialogTutorialPageView;
        private ActionTutorialPageView m_ActionTutorialPageView;
        // private DetailTutorialPageView m_DetailTutorialPageView;

        [SerializeField]
        private List<TutorialPageMaker> m_TutorialPageMakers;
        private TutorialProgress m_TutorialProgress;

        private DialogTutorialPage m_DialogTutorialPage;
        private ActionTutorialPage m_ActionTutorialPage;
        private DetailTutorialPage m_DetailTutorialPage;

        private TutorialPageData m_CurrTutorialData = null;
        
        static public  bool IsCompleteTutorial
        {
            get {
                return (PlayerPrefs.GetInt("ECET_CLEAR_ALL", 0) == 1);
            }
        }

        private TouchBlockView m_TouchBlockView;
        private ECSkipButton m_SkipButton;

        public UnityEvent<UnityAction> m_OnTutorialsSkipped;


        private void Awake() 
        {
            m_TouchBlockView = GetComponentInChildren<TouchBlockView>(true);
            m_SkipButton = GetComponentInChildren<ECSkipButton>(true);

            m_DialogTutorialPageView = GetComponentInChildren<DialogTutorialPageView>(true);
            m_ActionTutorialPageView = GetComponentInChildren<ActionTutorialPageView>(true);
            // m_DetailTutorialPageView = GetComponentInChildren<DetailTutorialPageView>(true);

            m_DialogTutorialPage = new DialogTutorialPage(m_DialogTutorialPageView);
            m_ActionTutorialPage = new ActionTutorialPage(m_ActionTutorialPageView);
            // m_DetailTutorialPage = new DetailTutorialPage(m_DetailTutorialPageView);

            m_CurrTutorialIndex = 0;
            m_LastClearIndex = -1;
            m_IsSkippingAll = false;

            LoadTutorialProgress();

            if(!IsCompleteTutorial)
            {
                if(m_PlayOnAwake)
                {
                    StartTutorial();
                }
            }
            else
            {
                Debug.Log("[Easy Tutorial] Tutorial is already completed");
                TutorialEvent.Instance.UnlistenAll();
                Destroy(gameObject);
            }
        }

        private void LoadTutorialProgress()
        {
            if(IsCompleteTutorial)
            {
                SkipAllTutorials();
            }
        }

        public void StartTutorial() 
        {
            StartCoroutine( ShowNextTutorials() );
        }

        public void ShowSkipTutorialAlert()
        {
            if(m_OnTutorialsSkipped.GetPersistentEventCount() == 0)
            {
                SkipAllTutorials();
            }
            else
            {
                m_OnTutorialsSkipped.Invoke(()=>SkipAllTutorials());
            }
        }

        private void SkipAllTutorials()
        {
            m_IsSkippingAll = true;
            Complete();
        }

        private IEnumerator ShowNextTutorials() 
        {
            if(IsCompleteTutorial)
            {
                yield break;
            }

            m_CurrTutorialData = null;

            if(!m_IsSkippingAll)
            {
                FindNextTutorialData();
            }

            // 화면 비활성화 시 화면이 깜빡거리는 현상 방지.
            yield return new WaitForEndOfFrame();

            // 지난 튜토리얼 화면을 비활성화.
            HideAllTutorialPageViews();

            // 모든 튜토리얼들을 클리어한 경우.
            if(m_CurrTutorialData == null)
            {
                Debug.LogWarning("[Easy Tutorial] Complete all tutorials");
                PlayerPrefs.SetInt("ECET_CLEAR_ALL", 1);
                TutorialEvent.Instance.UnlistenAll();
                Destroy(gameObject);
                yield break;
            }

            // 딜레이 시간 동안 터치가 불가능한 경우.
            if(m_CurrTutorialData.BlockTouchDuringDelay) 
            {
                m_TouchBlockView.gameObject.SetActive(true);
            }
            else
            {
                m_TouchBlockView.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(m_CurrTutorialData.StartDelay);
            
            InitLocalizer();

            TutorialPage tutorialPage = CreateTutorialPage();

            m_CurrTutorialData.OnTutorialBegin.Invoke();
            tutorialPage.ShowUsingData(m_CurrTutorialData);

            if(m_CurrTutorialData.BlockTouchDuringDelay) 
            {
                m_TouchBlockView.gameObject.SetActive(false);
            }
        }

        private void FindNextTutorialData()
        {
            for(int i=m_CurrTutorialIndex ; i<m_TutorialPageMakers.Count ; i++)
            {
                if(i <= m_LastClearIndex)
                    continue;
                
                m_CurrTutorialIndex = i;
                m_CurrTutorialData = m_TutorialPageMakers[i].PageData;

                if(m_EnableSkipFlag && m_CurrTutorialData.Skip)
                    continue;

                break;
            }
        }

        private void InitLocalizer()
        {
            // Localization 사용 여부 설정.
            m_CurrTutorialData.UseLocalization = m_UseLocalization;
            if(m_UseLocalization)
            {
                if(m_TextLocalizer == null)
                {
                    Debug.LogError("[ECEasyTutorial] Text Localizer가 할당되지 않았음.");
                }
                else
                {
                    m_CurrTutorialData.TextLocalizer = m_TextLocalizer;
                    m_CurrTutorialData.LocalizationTable = m_LocalizationTable;
                }
            }
        }

        private TutorialPage CreateTutorialPage()
        {
            // Data를 기반으로 각 page들 구성.
            var type = m_CurrTutorialData.GetType();
            TutorialPage tutorialPage = null;

            if(type == typeof(DialogTutorialPageData)) 
            {
                tutorialPage = m_DialogTutorialPage;

                m_DialogTutorialPage.CompleteTutorial = Complete;
                m_DialogTutorialPageView.gameObject.SetActive(true);
            }
            else if(type == typeof(ActionTutorialPageData)) 
            {
                tutorialPage = m_ActionTutorialPage;

                m_ActionTutorialPage.CompleteTutorial = Complete;
                m_ActionTutorialPageView.gameObject.SetActive(true);
            }
            // else if(type == typeof(DetailTutorialPageContentData)) 
            // {
            //     tutorialPage = m_DetailTutorialPage;

            //     m_DetailTutorialPageView.gameObject.SetActive(true);
            // }
            else
            {
                Debug.LogError("[Easy Tutorial] Unsupported type of TutorialPage is detected");
            }

            m_SkipButton.gameObject.SetActive(!m_CurrTutorialData.HideSkipButton);
            
            return tutorialPage;
        }

        private void HideAllTutorialPageViews()
        {
            m_DialogTutorialPageView.gameObject.SetActive(false);
            m_ActionTutorialPageView.gameObject.SetActive(false);
            // m_DetailTutorialPageView.gameObject.SetActive(false);
        }

        private void Complete()
        {
            m_LastClearIndex = m_CurrTutorialIndex;
            m_CurrTutorialData?.OnTutorialEnded.Invoke();
            StartCoroutine( ShowNextTutorials() );
        }
    }
}