using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube
{
    public class TutorialHandler : MonoBehaviour
    {
        public void InvokePressButton() {
            Tutorial.Complete(ConditionKey.PRESS_BUTTON_1);
        }
    }
}
