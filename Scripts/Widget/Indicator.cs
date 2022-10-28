using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Widget
{
    public class Indicator : MonoBehaviour
    {
        public float m_MovingDistance;
        public float m_Duration;

        private RectTransform m_RectTransform;
        private bool m_IsRunning;

        private void Awake() 
        {
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

        public void Place(RectTransform target, bool placeOnTop)
        {
            float targetWidth = target.sizeDelta.x;
            float targetHeight = target.sizeDelta.y;
            float indicatorHeight = m_RectTransform.sizeDelta.y;
            float margin = 20;

            float indicatordist = targetHeight / 2 + indicatorHeight / 2 + margin;

            Vector3 position = target.position;
            Quaternion rotation;

            if(placeOnTop)
            {
                position.y += indicatordist;
                rotation = Quaternion.identity;
            }
            else
            {
                position.y -= indicatordist;
                rotation = Quaternion.Euler(0, 0, 180.0f);
            }

            m_RectTransform.gameObject.SetActive(true);
            m_RectTransform.position = position;
            m_RectTransform.localRotation = rotation;
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
    }
}