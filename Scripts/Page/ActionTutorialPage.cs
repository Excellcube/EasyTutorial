using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Data.Common;

namespace Excellcube.EasyTutorial
{
    public class ActionTutorialPage : TutorialPage 
    {
        private ActionTutorialPageView m_View;

        private MaskImages m_MaskImages;
        public  MaskImages MaskImages {
            get => m_MaskImages;
            set => m_MaskImages = value;
        }

        private UnityAction m_CompleteTutorial;
        public  UnityAction CompleteTutorial
        {
            get => m_CompleteTutorial;
            set => m_CompleteTutorial = value;
        }

        int[] m_PrefLayerIds;

        private RenderMode m_RenderMode;


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

            ConsistCompletionChecker(data);
        }

        /// <summary>
        ///   튜토리얼 페이지 종료 환경을 구성
        /// </summary>
        private void ConsistCompletionChecker(ActionTutorialPageData data)
        {
            switch(data.conditionKey)
            {
                case ConditionKey.TapScreen :
                    ConsistTapScreenTutorial(data);
                    break;
                case ConditionKey.PressButton :
                    ConsistPressButtonTutorial(data);
                    break;
                case ConditionKey.ListenEvent :
                    ConsisteListenEventTutorial(data);
                    break;
            }
        }


#region -- TapScreen 튜토리얼 구성 --

        private void ConsistTapScreenTutorial(ActionTutorialPageData data)
        {
            m_View.ActionLogText.text = data.ActionLog;

            InitTapScreenUI();

            m_View.AddClickAction(TouchView);
        }

        private void TouchView()
        {
            if(m_CompleteTutorial == null)
            {
                Debug.LogError("[ActionTutorialPage] CompleteTutorial UnityAction isn't assigned!");
            }
            
            m_CompleteTutorial();
        }
#endregion


#region -- PressButton 튜토리얼 구성 --

        private void ConsistPressButtonTutorial(ActionTutorialPageData data)
        {
            m_View.ActionLogText.text = data.ActionLog;

            // TapScreen 반응 영역 비활성화.
            InitPressButtonUI();

            // CompleteButton 이벤트 등록.
            RegisterCompleteButtonEvent(data.onClickButton);
            
            // Block 영역 투명 여부 설정.
            ApplyBlockImageTransparency();

            // Highlight target 탐색.
            SearchDynamicHighlightTarget(ref data);

            m_View.ActionLogText.text = data.ActionLog;
            if(data.HighlightTarget != null)    
            {
                // 현재 구현은 메인 UI 카메라가 ScreenSpaceOverlay 모드일 경우.
                if(data.HighlightTarget is RectTransform) {
                    RectTransform targetRectTrans = data.HighlightTarget as RectTransform;
                    HighlightTargetOnCanvas(targetRectTrans, data.IndicatorPosition);                    
                } else {
                    HighlightTargetInScene(data.HighlightTarget, data.IndicatorPosition);
                }
            }
            else
            {
                m_View.UnmaskPanel.transform.parent.gameObject.SetActive(false);
                m_View.Indicator.gameObject.SetActive(false);
            }
        }

        private void RegisterCompleteButtonEvent(UnityEvent onClickEvent)
        {
            m_View.CompleteButton.onClick.RemoveAllListeners();
            m_View.CompleteButton.onClick.AddListener(()=>{
                onClickEvent.Invoke();
                m_CompleteTutorial.Invoke();
            });
        }

        private void ApplyBlockImageTransparency()
        {
            ActionTutorialPageData data = m_Data as ActionTutorialPageData;
            if(data.UseTransparentBlockScreen) {
                m_View.BlockScreenImage.color = new Color(0,0,0,0);
            }
        }

        /// <summary>
        /// Canvas 상의 오브젝트를 하이라이팅 할 때 사용하는 메서드.
        /// </summary>
        private void HighlightTargetOnCanvas(RectTransform target, IndicatorPosition indicatorPosition)
        {
            // target이 위치한 Canvas.
            Canvas targetCanvas = target.GetComponentInParent<Canvas>();
            RectTransform maskRectTrasnform = m_MaskImages.GetComponent<RectTransform>();
            Vector2 position = Camera.main.WorldToScreenPoint(target.transform.position);
            
            if(targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay) 
            {
                HighlightTargetOnOverlayCanvas(target, indicatorPosition);
            }
            else if(targetCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                m_View.UnmaskPanel.transform.parent.gameObject.SetActive(true);
                var unmaskRT = m_View.UnmaskPanel.GetComponent<RectTransform>();

                unmaskRT.pivot = new Vector2(0.5f, 0.5f);
                Vector2 unmaskSize = target.sizeDelta * target.localScale.x * targetCanvas.scaleFactor;
                Vector3 targetAnchorMin = target.anchorMin;
                Vector3 targetAnchorMax = target.anchorMax;

                float scaledWidth = unmaskSize.x;
                float scaledHeight = unmaskSize.y;

                // float canvasScaleFactor = rectTransform.lossyScale.x;

                // unmaskRT.position = rectTransform.position;
                maskRectTrasnform.position = position;
                maskRectTrasnform.position += new Vector3((0.5f - target.pivot.x) * scaledWidth, (0.5f - target.pivot.y) * scaledHeight, 0);

                maskRectTrasnform.sizeDelta = target.sizeDelta;

                FitUnmaskToMaskImage(maskRectTrasnform);
                ShowIndicator(maskRectTrasnform, indicatorPosition);
            }
            else
            {
                Debug.LogError("World Space의 캔버스를 대상으로 highlight를 적용하는 기능은 구현되지 않았음");
            }
        }

        /// <summary>
        /// 3D 공간 상의 오브젝트를 하이라이팅 할 때 사용하는 메서드
        /// </summary>
        private void HighlightTargetInScene(Transform target, IndicatorPosition indicatorPosition) 
        {
            // 1. target 하위의 MeshRenderer들을 가져온다.
            MeshRenderer[] meshRenderers = target.GetComponentsInChildren<MeshRenderer>();
            if(meshRenderers.Length == 0) {
                Debug.LogError($"[ActionTutorialPage] MeshRenderer가 없는 대상은 하이라이트를 할 수 없음");
                return;
            }

            SavePrevLayerIds(meshRenderers);

            Camera mainCamera = Camera.main;

            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            // 2. mesh들을 이용하여 2d bounding box를 구한다.
            foreach(MeshRenderer mr in meshRenderers)
            {
                Bounds bounds = mr.bounds;
                Vector3 extent = bounds.extents;

                List<Vector2> projPoints = new List<Vector2>();
                projPoints.Add(mainCamera.WorldToScreenPoint(bounds.center + new Vector3( extent.x,  extent.y,  extent.z)));
                projPoints.Add(mainCamera.WorldToScreenPoint(bounds.center + new Vector3( extent.x,  extent.y, -extent.z)));
                projPoints.Add(mainCamera.WorldToScreenPoint(bounds.center + new Vector3( extent.x, -extent.y,  extent.z)));
                projPoints.Add(mainCamera.WorldToScreenPoint(bounds.center + new Vector3( extent.x, -extent.y, -extent.z)));
                projPoints.Add(mainCamera.WorldToScreenPoint(bounds.center + new Vector3(-extent.x,  extent.y,  extent.z)));
                projPoints.Add(mainCamera.WorldToScreenPoint(bounds.center + new Vector3(-extent.x,  extent.y, -extent.z)));
                projPoints.Add(mainCamera.WorldToScreenPoint(bounds.center + new Vector3(-extent.x, -extent.y,  extent.z)));
                projPoints.Add(mainCamera.WorldToScreenPoint(bounds.center + new Vector3(-extent.x, -extent.y, -extent.z)));

                foreach(var p in projPoints)
                {
                    if(p.x < minX)
                        minX = p.x;
                    if(p.y < minY)
                        minY = p.y;
                    if(p.x > maxX)
                        maxX = p.x;
                    if(p.y > maxY)
                        maxY = p.y;
                }
            }

            RectTransform rectTransform =  m_MaskImages.GetComponent<RectTransform>();
            rectTransform.position = new Vector3((maxX + minX) / 2.0f, (maxY + minY) / 2.0f, 1);
            rectTransform.sizeDelta = new Vector3((maxX - minX) / rectTransform.lossyScale.x, (maxY - minY) / rectTransform.lossyScale.x, 1);

            m_View.UnmaskPanel.FitTo(rectTransform);

            HighlightTargetOnOverlayCanvas(rectTransform, indicatorPosition);
        }

        private void HighlightTargetOnOverlayCanvas(RectTransform maskImageRT, IndicatorPosition indicatorPosition) 
        {
            m_View.UnmaskPanel.transform.parent.gameObject.SetActive(true);
            FitUnmaskToMaskImage(maskImageRT);
            FitTutorialButtonToMaskImage(maskImageRT);
            ShowIndicator(maskImageRT, indicatorPosition);
        }

        private void ShowIndicator(RectTransform maskImageRT, IndicatorPosition indicatorPosition)
        {
            if(indicatorPosition == IndicatorPosition.NONE) {
                m_View.Indicator.gameObject.SetActive(false);
            } else {
                m_View.Indicator.gameObject.SetActive(true);
                m_View.Indicator.Place(maskImageRT, indicatorPosition);
                m_View.Indicator.Show(maskImageRT);
            }
        }

        private void FitUnmaskToMaskImage(RectTransform maskImageRT)
        {
            m_View.UnmaskPanel.FitTo(maskImageRT);
            // m_View.UnmaskPanel.transform.localScale = Vector3.one;

            // // var maskImageScale = maskImageRT.lossyScale;
            // var maskImageScale = Vector3.one;

            // var unmaskRT = m_View.UnmaskPanel.GetComponent<RectTransform>();

            // unmaskRT.pivot = new Vector2(0.5f, 0.5f);
            // Vector2 unmaskSize = unmaskRT.sizeDelta;
            // Vector3 targetAnchorMin = maskImageRT.anchorMin;
            // Vector3 targetAnchorMax = maskImageRT.anchorMax;

            // float scaledWidth = unmaskSize.x * maskImageScale.x;
            // float scaledHeight = unmaskSize.y * maskImageScale.y;

            // float canvasScaleFactor = maskImageRT.lossyScale.x;

            // unmaskRT.position = maskImageRT.position;

            // // Mask 영역을 focusing하는 용도. 이 부분은 언제 사용하지?
            // // unmaskRT.position += new Vector3((0.5f - targetAnchorMin.x) * scaledWidth * canvasScaleFactor, (0.5f - targetAnchorMin.y) * scaledHeight * canvasScaleFactor, 0);

            // float startScale = maskImageScale.x * 1.3f;
            // float endScale = maskImageScale.x;

            // unmaskRT.localScale = new Vector3(endScale, endScale, endScale);
        }

        private void FitTutorialButtonToMaskImage(RectTransform maskImageRT)
        {
            Transform completeButtonTrans = m_View.CompleteButton.transform;
            completeButtonTrans.position = maskImageRT.transform.position;
            completeButtonTrans.rotation = maskImageRT.transform.rotation;
            completeButtonTrans.localScale = maskImageRT.transform.localScale;

            RectTransform completeButtonRT = m_View.CompleteButton.GetComponent<RectTransform>();
            
            completeButtonRT.anchorMin = maskImageRT.anchorMin;
            completeButtonRT.anchorMax = maskImageRT.anchorMax;
            completeButtonRT.anchoredPosition = maskImageRT.anchoredPosition;
            completeButtonRT.sizeDelta = maskImageRT.sizeDelta;
        }

        private void SavePrevLayerIds(MeshRenderer[] meshRenderers)
        {
            int tutorialLayerId = LayerMask.NameToLayer("Tutorial");

            // 원래의 layer들을 저장 후 Tutorial 레이어로 교체.
            m_PrefLayerIds = new int[meshRenderers.Length];
            for(int i=0 ; i<meshRenderers.Length ; i++) {
                m_PrefLayerIds[i] = meshRenderers[i].gameObject.layer;
                meshRenderers[i].gameObject.layer = tutorialLayerId;
            }
        }

        private void LoadPrevLayerIds(Transform target)
        {
            if(target == null) {
                return;
            }

            MeshRenderer[] meshRenderers = target.GetComponentsInChildren<MeshRenderer>();
            if(meshRenderers.Length == 0) {
                // RectTransform가 넘어온 경우에는 무시.
                return;
            }

            for(int i=0 ; i<meshRenderers.Length ; i++) {
                meshRenderers[i].gameObject.layer = m_PrefLayerIds[i];
            }
        }

        private void SearchDynamicHighlightTarget(ref ActionTutorialPageData data) 
        {
            if(data.HighlightTarget == null)
            {                
                if(FindTarget(ref data))
                {
                    return;
                }

                Debug.LogWarning($"[ActionTutorialPage] 탐색된 TutorialSelectionTarget의 리스트 내에서 DynamicTargetKey({data.DynamicTargetKey})에 해당하는 TutorialSelectionTarget을 찾을 수 없음");
            }
        }

        private bool FindTarget(ref ActionTutorialPageData data)
        {
            var key = data.DynamicTargetKey;

            Transform target;

            if(data.DynamicTargetRoot != null)
            {
                target = data.DynamicTargetRoot.Find(data.DynamicTargetKey);
            }
            else
            {
                target = GameObject.Find(data.DynamicTargetKey)?.transform;
            }

            data.HighlightTarget = target;

            return data.HighlightTarget != null;
        }
        
#endregion


#region -- Listen Event 튜토리얼 구성 --

        private void ConsisteListenEventTutorial(ActionTutorialPageData data)
        {
            m_View.ActionLogText.text = data.ActionLog;
            
            InitListenEventUI();

            // 튜토리얼 페이지 완료 조건 이벤트 등록.
            RegisterFinishEvent(data);
        }

        private void RegisterFinishEvent(ActionTutorialPageData data)
        {
            TutorialEvent.Instance.Listen(data.finishEventKey.ToString(), this, ()=>{
                TutorialEvent.Instance.UnlistenAll();
                if(m_CompleteTutorial == null)
                {
                    Debug.LogError("[ActionTutorialPage] CompleteTutorial UnityAction isn't assigned!");
                }
                LoadPrevLayerIds(data.HighlightTarget);
                m_CompleteTutorial();
            });
        }

#endregion

        private void InitTapScreenUI()
        {
            m_View.TapScreenTarget.gameObject.SetActive(true);
            m_View.UnmaskPanel.transform.parent.gameObject.SetActive(false);
            m_View.Indicator.gameObject.SetActive(false);
            m_View.CompleteButton.gameObject.SetActive(false);
        }

        private void InitPressButtonUI()
        {
            // TapScreen 반응 영역 비활성화.
            m_View.TapScreenTarget.gameObject.SetActive(false);
            m_View.UnmaskPanel.transform.parent.gameObject.SetActive(true);
            m_View.Indicator.gameObject.SetActive(true);
            m_View.CompleteButton.gameObject.SetActive(true);
        }

        private void InitListenEventUI() {
            m_View.TapScreenTarget.gameObject.SetActive(false);
            m_View.UnmaskPanel.transform.parent.gameObject.SetActive(false);
            m_View.Indicator.gameObject.SetActive(false);
            m_View.CompleteButton.gameObject.SetActive(false);
        }

    }
}