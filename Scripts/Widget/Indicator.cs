using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excellcube.EasyTutorial;

namespace Excellcube.EasyTutorial
{
    public class Indicator : MonoBehaviour
    {
        public float m_MovingDistance;
        public float m_Duration;

        private Canvas m_Canvas;
        private RectTransform m_CanvasRT;
        private RectTransform m_RectTransform;
        private bool m_IsRunning;

        private RectTransform m_TargetRT;

        private void Awake() 
        {
            m_Canvas = GetComponentInParent<Canvas>();
            m_CanvasRT = m_Canvas.GetComponent<RectTransform>();
            m_RectTransform = GetComponent<RectTransform>();    
        }

        private void OnEnable() 
        {
            m_IsRunning = true;
        }

        private void OnDisable() 
        {
            m_IsRunning = false;
        }

        /// <summary>
        /// Indicator를 target에 배치. 화면 중앙을 기준으로 target을 향해 indicator가 배치 된다.
        /// </summary>
        /// <param name="target"></param>
        public void Place(RectTransform target, IndicatorPosition indicatorPosition)
        {
            m_TargetRT = target;

            Vector3 newPosition = target.position;
            Quaternion newRotation = Quaternion.identity;
            Vector3 marginPosition = Vector3.zero;
            
            switch(indicatorPosition)
            {
                case IndicatorPosition.TOP :
                {
                    newPosition.y += target.sizeDelta.y * 0.5f;
                    newRotation = Quaternion.Euler(0, 0, 0);
                    break;
                }
                case IndicatorPosition.BOTTOM :
                {
                    newPosition.y -= target.sizeDelta.y * 0.5f;
                    newRotation = Quaternion.Euler(0, 0, 180);
                    break;
                }
                case IndicatorPosition.RIGHT :
                {
                    newPosition.x += target.sizeDelta.x * 0.5f;
                    newRotation = Quaternion.Euler(0, 0, -90);
                    break;
                }
                case IndicatorPosition.LEFT :
                {
                    newPosition.x -= target.sizeDelta.x * 0.5f;
                    newRotation = Quaternion.Euler(0, 0, 90);
                    break;
                }
            }

            m_RectTransform.gameObject.SetActive(true);
            m_RectTransform.position = newPosition;
            m_RectTransform.localRotation = newRotation;
            // m_RectTransform.localScale = scale;
        }

        public void Show(RectTransform target)
        {
            StartCoroutine( MoveInternal(target) );
        }

        private IEnumerator MoveInternal(RectTransform target)
        {
            Vector3 currPosition = m_RectTransform.position;
            Vector3 direction = (target.position - currPosition).normalized;
            Vector3 diffPosition = direction * m_MovingDistance;
            Vector3 destPosition = currPosition + diffPosition;
            Vector3 delta = diffPosition / m_Duration;

            bool toDest = true;

            float accumTime = 0;

            while(m_IsRunning)
            {
                yield return null;

                float dt = Time.deltaTime;
                Vector3 dist = delta * dt;

                if(toDest)
                {
                    m_RectTransform.position += dist;
                    accumTime += dt;
                }
                else
                {
                    m_RectTransform.position -= dist;
                    accumTime -= dt;
                }

                if(accumTime >= m_Duration)
                    toDest = false;
                else if(accumTime <= 0)
                    toDest = true;
            }
        }

        void OnDrawGizmos()
        {
            if(m_TargetRT == null)
            {
                return;
            }

            if(m_RectTransform == null)
            {
                m_RectTransform = GetComponent<RectTransform>();
            }
            Vector3 indicatorPosition = m_RectTransform.TransformPoint(Vector3.zero);
            Vector3 targetPosition = m_TargetRT.TransformPoint(Vector3.zero);

            Gizmos.color = Color.red;

            Gizmos.DrawSphere(indicatorPosition, 10f);
            Gizmos.DrawSphere(targetPosition, 10f);

            // // 또는 라인, 큐브 등을 그릴 수 있습니다.
            Gizmos.DrawLine(indicatorPosition, targetPosition);
        }
    }
}