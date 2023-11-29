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
        public void Place(RectTransform target)
        {
            m_TargetRT = target;

            float targetWidth = target.sizeDelta.x;
            float targetHeight = target.sizeDelta.y;
            float indicatorHeight = m_RectTransform.sizeDelta.y;
            float margin = 20;

            float indicatorDestination = (targetHeight / 2 + indicatorHeight / 2 + margin) * target.lossyScale.x;

            // Vector3 position = target.position;
            // Quaternion rotation = Quaternion.identity;
            // Vector3 scale;

            // Indicator가 배치 될 때의 방향을 구한다.
            float canvasWidth  = Screen.width;
            float canvasHeight = Screen.height;

            Vector2 centerPosition = new Vector2(canvasWidth / 2.0f, canvasHeight / 2.0f);
            Vector2 targetPosition = new Vector2(target.position.x, target.position.y);
            Vector3 direction = (targetPosition - centerPosition).normalized;
            Vector3 indicatorHeadDirection = new Vector2(0, -1);
            float angle = Vector3.Angle(indicatorHeadDirection, direction);

            if(targetPosition.x < centerPosition.x) 
            {
                m_RectTransform.rotation = Quaternion.Euler(0, 0, -angle);
            }
            else
            {
                m_RectTransform.rotation = Quaternion.Euler(0, 0, angle);
            }

            m_RectTransform.position = targetPosition;

            // if(placeOnRight)
            // {
            //     position.x += indicatordist;
            //     position.y -= indicatordist;
            //     scale = Vector3.one;
            // }
            // else
            // {
            //     position.x -= indicatordist;
            //     position.y -= indicatordist;
            //     scale = new Vector3(-1, 1, 1);
            // }

            // m_RectTransform.gameObject.SetActive(true);
            // m_RectTransform.position = position;
            // m_RectTransform.localRotation = rotation;

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