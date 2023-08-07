using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Excellcube.EasyTutorial.Utils;
using Excellcube.EasyTutorial.Widget;

namespace Excellcube.EasyTutorial.Page
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

            // Block 영역 투명 여부 설정.
            ApplyBlockImageTransparency();

            // Highlight target 탐색.
            SearchDynamicHighlightTarget(ref data);

            m_View.ActionLogText.text = data.ActionLog;
            if(data.HighlightTarget != null)    
            {
                if(data.HighlightTarget is RectTransform) {
                    RectTransform targetRectTrans = data.HighlightTarget as RectTransform;
                    HighlightTarget(targetRectTrans, data.IndicatorPosition);                    
                } else {
                    HighlightTarget(data.HighlightTarget, m_MaskImages, data.IndicatorPosition);
                }
            }
            else
            {
                m_View.UnmaskPanel.transform.parent.gameObject.SetActive(false);
                m_View.Indicator.gameObject.SetActive(false);
            }

            // 튜토리얼 페이지 완료 조건에 해당하는 이벤트 등록.
            TutorialEvent.Instance.Listen(data.CompleteKey, this, ()=>{
                TutorialEvent.Instance.UnlistenAll();
                if(m_CompleteTutorial == null)
                {
                    Debug.LogError("[ActionTutorialPage] CompleteTutorial UnityAction isn't assigned!");
                }
                LoadPrevLayerIds(data.HighlightTarget);
                m_CompleteTutorial();
            });
        }

        private void ApplyBlockImageTransparency()
        {
            ActionTutorialPageData data = m_Data as ActionTutorialPageData;
            if(data.UseTransparentBlockScreen) {
                m_View.BlockScreenImage.color = new Color(0,0,0,0);
            }
        }

        // Canvas가 World space에 있는 경우.
        private void HighlightTarget(RectTransform target, Page.IndicatorPosition indicatorPosition)
        {
            Canvas canvas = target.GetComponentInParent<Canvas>();

            if(canvas.renderMode != RenderMode.ScreenSpaceOverlay) {
                
                Vector2 position = Camera.main.WorldToScreenPoint(target.transform.position);

                // MaskImage의 위치와 크기를 target에 맞게 갱신.
                RectTransform rectTransform = m_MaskImages.GetComponent<RectTransform>();

                if(canvas.renderMode == RenderMode.WorldSpace) {
                    rectTransform.position = position;
                    rectTransform.sizeDelta = target.sizeDelta * canvas.scaleFactor;
                } else {
                    m_View.UnmaskPanel.transform.parent.gameObject.SetActive(true);
                    var unmaskRT = m_View.UnmaskPanel.GetComponent<RectTransform>();

                    unmaskRT.pivot = new Vector2(0.5f, 0.5f);
                    Vector2 unmaskSize = target.sizeDelta * target.localScale.x * canvas.scaleFactor;
                    Vector3 targetAnchorMin = target.anchorMin;
                    Vector3 targetAnchorMax = target.anchorMax;

                    float scaledWidth = unmaskSize.x;
                    float scaledHeight = unmaskSize.y;

                    // float canvasScaleFactor = rectTransform.lossyScale.x;

                    // unmaskRT.position = rectTransform.position;
                    rectTransform.position = position;
                    rectTransform.position += new Vector3((0.5f - target.pivot.x) * scaledWidth, (0.5f - target.pivot.y) * scaledHeight, 0);

                    rectTransform.sizeDelta = target.sizeDelta;
                }

                FitUnmaskToMaskImage(rectTransform);
                ShowIndicator(rectTransform, indicatorPosition);

            } else {
                HighlightTargetOnOverlayCanvas(target, indicatorPosition);
            }
        }

        // scene 내의 3d 오브젝트 등을 대해 하이라이트.
        private void HighlightTarget(Transform target, MaskImages maskImage, Page.IndicatorPosition indicatorPosition) 
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

            RectTransform rectTransform =  maskImage.GetComponent<RectTransform>();
            rectTransform.position = new Vector3((maxX + minX) / 2.0f, (maxY + minY) / 2.0f, 1);
            rectTransform.sizeDelta = new Vector3((maxX - minX) / rectTransform.lossyScale.x, (maxY - minY) / rectTransform.lossyScale.x, 1);

            m_View.UnmaskPanel.FitTo(rectTransform);

            HighlightTargetOnOverlayCanvas(rectTransform, indicatorPosition);
        }

        private void HighlightTargetOnOverlayCanvas(RectTransform maskImageRT, Page.IndicatorPosition indicatorPosition) 
        {
            m_View.UnmaskPanel.transform.parent.gameObject.SetActive(true);
            FitUnmaskToMaskImage(maskImageRT);
            ShowIndicator(maskImageRT, indicatorPosition);
        }

        private void ShowIndicator(RectTransform maskImageRT, Page.IndicatorPosition indicatorPosition)
        {
            if(indicatorPosition == IndicatorPosition.NONE) {
                m_View.Indicator.gameObject.SetActive(false);
            } else {
                m_View.Indicator.gameObject.SetActive(true);
                m_View.Indicator.Place(maskImageRT, indicatorPosition == IndicatorPosition.RIGHT);
                m_View.Indicator.Show(maskImageRT);
            }
        }

        private void FitUnmaskToMaskImage(RectTransform maskImageRT)
        {
            m_View.UnmaskPanel.FitTo(maskImageRT);
            m_View.UnmaskPanel.transform.localScale = Vector3.one;

            // var maskImageScale = maskImageRT.lossyScale;
            var maskImageScale = Vector3.one;

            var unmaskRT = m_View.UnmaskPanel.GetComponent<RectTransform>();

            unmaskRT.pivot = new Vector2(0.5f, 0.5f);
            Vector2 unmaskSize = unmaskRT.sizeDelta;
            Vector3 targetAnchorMin = maskImageRT.anchorMin;
            Vector3 targetAnchorMax = maskImageRT.anchorMax;

            float scaledWidth = unmaskSize.x * maskImageScale.x;
            float scaledHeight = unmaskSize.y * maskImageScale.y;

            float canvasScaleFactor = maskImageRT.lossyScale.x;

            unmaskRT.position = maskImageRT.position;
            unmaskRT.position += new Vector3((0.5f - targetAnchorMin.x) * scaledWidth * canvasScaleFactor, (0.5f - targetAnchorMin.y) * scaledHeight * canvasScaleFactor, 0);

            float startScale = maskImageScale.x * 1.3f;
            float endScale = maskImageScale.x;
            // unmaskRT.localScale = new Vector3(startScale, startScale, startScale);
            // unmaskRT.DOScale(endScale, 0.2f);
            unmaskRT.localScale = new Vector3(endScale, endScale, endScale);
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
                if(data.DynamicTargetRoot != null)
                {
                    // 지정 대상 내에 TutorialSelectionTarget이 존재하는지 확인.
                    if(FindTarget(ref data, false))
                    {
                        return;
                    }

                    // 지정 대상 내에 TutorialSelectionTarget이 존재하지 않을 경우 전체 탐색으로 확인.
                    if(FindTarget(ref data, true))
                    {
                        return;
                    }

                    Debug.LogWarning($"[ActionTutorialPage] 탐색된 TutorialSelectionTarget의 리스트 내에서 DynamicTargetKey({data.DynamicTargetKey})에 해당하는 TutorialSelectionTarget을 찾을 수 없음");
                }
            }
        }

        private bool FindTarget(ref ActionTutorialPageData data, bool global)
        {
            var key = data.DynamicTargetKey;


            // 주어진 대상 혹은 전체 탐색으로 TutorialSelectionTarget이 존재하는지 확인.
            TutorialSelectionTarget[] targets;
            if(global) {
                targets = GameObject.FindObjectsOfType<TutorialSelectionTarget>();
            } else {
                targets = data.DynamicTargetRoot.GetComponentsInChildren<TutorialSelectionTarget>();
            }

            // 탐색된 TutorialSelectionTarget 내에서 TutorialAction의 Key값에 해당하는 대상을 탐색.
            var targetList = new List<TutorialSelectionTarget>(targets);
            var target = targetList.Find(e => e.Key == key);
            if(target != null)
            {
                data.HighlightTarget = target.transform;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}