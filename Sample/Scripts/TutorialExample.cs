using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial {
    public class TutorialExample : MonoBehaviour
    {
        public ECEasyTutorial m_Tutorial;


        private void Awake()
        {
            m_Tutorial.AddOnStepChangedListener(OnTutorialStepChanged);
        }

        public void PressTutorialButton()
        {
            Debug.Log("첫 번째 버튼 터치");
        }

        public void PressTutorialButton2()
        {
            Debug.Log("두 번째 버튼 터치");
        }

        public void BroadcastCompleteEvent()
        {
            Tutorial.Complete("EventComplete");
        }

        public void OnTutorialStepChanged(int step)
        {
            Debug.Log("튜토리얼 스텝 : " + step);
        }
    }
}
