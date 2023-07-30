using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Excellcube.EasyTutorial.Utils;

namespace Excellcube.EasyTutorial {
    public class TutorialExample : MonoBehaviour
    {
        public void PressTutorialButton()
        {
            TutorialEvent.Instance.Broadcast("TUTORIAL_BUTTON_01");
        }

        public void PressTutorialButton2()
        {
            TutorialEvent.Instance.Broadcast("TUTORIAL_BUTTON_02");
        }
    }
}
