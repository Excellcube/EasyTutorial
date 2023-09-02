using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 사용 편의를 위해 최소한의 namespace를 갖는 Tutorial 클래스 제공.
namespace Excellcube
{
    public class Tutorial {
        private static EasyTutorial.ECEasyTutorial[] m_AllTutorials;
        private static Dictionary<string, int> m_KeyValues = new Dictionary<string, int>();

        public static bool IsComplete(string key) {
            if(m_AllTutorials == null) {
                m_AllTutorials = GameObject.FindObjectsOfType<EasyTutorial.ECEasyTutorial>();
            }

            // 모든 튜토리얼을 클리어 했는지 검사.
            bool isCompleteAllTutorials = true;
            foreach(var tutorial in m_AllTutorials) {
                bool isClearTutorial = EasyTutorial.ECEasyTutorial.IsCompleteTutorial(tutorial.key);
                isCompleteAllTutorials &= isClearTutorial;
            }

            // 이미 모든 튜터리얼을 클리어 했다면 종료.
            if(isCompleteAllTutorials) {
                return true;
            }

            // 아직 모든 튜토리얼 클리어 전이라면 해당 튜토리얼 클리어 여부 조사.
            return EasyTutorial.ECEasyTutorial.IsCompleteTutorial(key);
        }

        public static void Complete(ConditionKey key) {
            EasyTutorial.Utils.TutorialEvent.Instance.Broadcast(key.ToString());
        }

        public static int GetValue(string key) {
            int result;
            if(m_KeyValues.TryGetValue(key, out result)) {
                return result;
            } else {
                return 0;
            }
        }

        public static void SetValue(string key, int value) {
            m_KeyValues[key] = value;
        }
    }
}

namespace Excellcube.EasyTutorial
{
    using Page;
    using Utils;
    using Widget;
    using UI;

    public class ECEasyTutorial : MonoBehaviour
    {
        /// <summary>
        /// 튜토리얼 종류를 구분하는 키값. 튜토리얼마다 고유해야함. 키값을 이용하여 튜토리얼 별로 클리어 여부를 구별한다.
        /// </summary>
        [SerializeField]
        private string m_Key = "";
        public string key => m_Key;

        [SerializeField]
        private bool m_PlayOnAwake = true;
        [SerializeField]
        private bool m_UseLocalization;

        [SerializeField]
        private Utils.TextLocalizer m_TextLocalizer;
        [SerializeField]
        private string m_LocalizationTable;

        private int m_LastClearIndex;
        [SerializeField]
        private int m_CurrTutorialIndex = -1;

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

        private MaskImages m_MaskImages;

        public static bool IsCompleteTutorial(string key)
        {
            #if UNITY_EDITOR
            // Editor에서는 항상 튜토리얼을 보여준다.
            return false;
            #else
            return (PlayerPrefs.GetInt($"ECET_CLEAR_{key}", 0) == 1);
            #endif
        }

        public static void SetClearTutorial(string key)
        {
            #if !UNITY_EDITOR
            PlayerPrefs.SetInt($"ECET_CLEAR_{key}", 1);
            #endif
        }

        private TouchBlockView m_TouchBlockView;
        private ECSkipButton m_SkipButton;


        private void Awake() 
        {
            m_TouchBlockView = GetComponentInChildren<TouchBlockView>(true);
            m_SkipButton = GetComponentInChildren<ECSkipButton>(true);

            m_DialogTutorialPageView = GetComponentInChildren<DialogTutorialPageView>(true);
            m_ActionTutorialPageView = GetComponentInChildren<ActionTutorialPageView>(true);
            // m_DetailTutorialPageView = GetComponentInChildren<DetailTutorialPageView>(true);

            m_MaskImages = GetComponentInChildren<MaskImages>(true);

            m_DialogTutorialPage = new DialogTutorialPage(m_DialogTutorialPageView);
            m_ActionTutorialPage = new ActionTutorialPage(m_ActionTutorialPageView);
            // m_DetailTutorialPage = new DetailTutorialPage(m_DetailTutorialPageView);

            m_CurrTutorialIndex = -1;
            m_LastClearIndex = -1;

            LoadTutorialProgress();

            if(!IsCompleteTutorial(m_Key))
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
            if(IsCompleteTutorial(m_Key))
            {
                Debug.LogWarning("튜토리얼 완료. 튜토리얼 실행 방지");
            }
        }

        public void StartTutorial() 
        {
            StartCoroutine( ShowNextTutorials() );
        }

        private IEnumerator ShowNextTutorials() 
        {
            if(IsCompleteTutorial(m_Key))
            {
                yield break;
            }

            m_CurrTutorialData = null;

            // 다음 튜토리얼 데이터가 있는지 탐색.
            FindNextTutorialData();

            // 화면 비활성화 시 화면이 깜빡거리는 현상 방지.
            yield return new WaitForEndOfFrame();

            // 지난 튜토리얼 화면을 비활성화.
            HideAllTutorialPageViews();

            // 모든 튜토리얼들을 클리어한 경우.
            if(m_CurrTutorialData == null)
            {
                Debug.LogWarning("[Easy Tutorial] Complete all tutorials");
                SetClearTutorial(m_Key);
                TutorialEvent.Instance.UnlistenAll();
                Destroy(gameObject);
                yield break;
            }

            // 딜레이 시간 동안 터치 제한.
            m_TouchBlockView.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(m_CurrTutorialData.StartDelay);
            
            InitLocalizer();

            TutorialPage tutorialPage = CreateTutorialPage();

            m_CurrTutorialData.OnTutorialBegin.Invoke();

            Debug.Log("[ECEasyTutorial] 튜토리얼 출력 : " + m_CurrTutorialData.Name);
            Debug.Log("[ECEasyTutorial] 페이지 타입  : " + m_CurrTutorialData.StartDelay);
            tutorialPage.ShowUsingData(m_CurrTutorialData);

            m_TouchBlockView.gameObject.SetActive(false);

            m_CurrTutorialData.OnTutorialInvoked.Invoke();
        }

        private void FindNextTutorialData()
        {
            for(int i=m_CurrTutorialIndex ; i<m_TutorialPageMakers.Count ; i++)
            {
                if(i <= m_LastClearIndex)
                    continue;
                
                m_CurrTutorialIndex = i;
                m_CurrTutorialData = m_TutorialPageMakers[i].PageData;

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
                m_ActionTutorialPage.MaskImages = m_MaskImages;
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

        public void Complete(ConditionKey key)
        {
            Utils.TutorialEvent.Instance.Broadcast(key.ToString());
        }
    }
}