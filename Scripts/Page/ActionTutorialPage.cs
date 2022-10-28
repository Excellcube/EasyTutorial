using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Excellcube.EasyTutorial.Utils;

namespace Excellcube.EasyTutorial.Page
{
    public class ActionTutorialPage : TutorialPage 
    {
        private ActionTutorialPageView m_View;
        private UnityAction m_CompleteTutorial;
        public  UnityAction CompleteTutorial
        {
            get => m_CompleteTutorial;
            set => m_CompleteTutorial = value;
        }


        public ActionTutorialPage(ActionTutorialPageView view)
        {
            m_View = view;
        }

        protected override void ConfigureView()
        {
            ActionTutorialPageData data = m_Data as ActionTutorialPageData;
            if(data == null)
            {
                Debug.LogError("[ActionTutorialPage] Fail to configure the view. Data type isn't matched with ActionTutorialPageData");
                return;
            }

            // Highlight target 탐색.
            SearchDynamicHighlightTarget(ref data);

            m_View.ActionLogText.text = data.ActionLog;
            if(data.HighlightTarget != null)
            {
                m_View.UnmaskPanel.transform.parent.gameObject.SetActive(true);
                m_View.UnmaskPanel.fitTarget = data.HighlightTarget;

                m_View.Indicator.Place(data.HighlightTarget, data.IndicatorPosition == Page.IndicatorPosition.TOP);
                m_View.Indicator.Show(data.HighlightTarget);
            }
            else
            {
                m_View.UnmaskPanel.transform.parent.gameObject.SetActive(false);
                m_View.Indicator.gameObject.SetActive(false);
            }

            TutorialEvent.Instance.Listen(data.CompleteKey, this, ()=>{
                TutorialEvent.Instance.UnlistenAll();
                if(m_CompleteTutorial == null)
                {
                    Debug.LogError("[ActionTutorialPage] CompleteTutorial UnityAction isn't assigned!");
                }
                m_CompleteTutorial();
            });
        }

        private void SearchDynamicHighlightTarget(ref ActionTutorialPageData data) 
        {
            if(data.HighlightTarget == null)
            {
                if(data.DynamicTargetRoot != null)
                {
                    var targets = data.DynamicTargetRoot.GetComponentsInChildren<TutorialSelectionTarget>();
                    if(targets.Length == 0)
                    {
                        Debug.LogWarning("[ActionTutorialPage] 할당한 DyamicTargetRoot의 children 중에서 TutorialSelectiondTarget을 만족하는 오브젝트를 찾을 수 없음");
                    }
                    else
                    {
                        var key = data.DynamicTargetKey;
                        var targetList = new List<TutorialSelectionTarget>(targets);
                        var target = targetList.Find(e => e.Key == key);
                        if(target != null)
                        {
                            data.HighlightTarget = target.GetComponent<RectTransform>();
                        }
                        else
                        {
                            Debug.LogWarning($"[ActionTutorialPage] 탐색된 TutorialSelectionTarget의 리스트 내에서 DynamicTargetKey({data.DynamicTargetKey})에 해당하는 TutorialSelectionTarget을 찾을 수 없음");
                        }
                    }
                }
            }
        }

    }
}