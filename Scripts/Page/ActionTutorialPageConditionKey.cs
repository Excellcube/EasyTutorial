using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용 편의를 위해 namespace 최소화.
namespace Excellcube
{
    [SerializeField]
    public enum ConditionKey
    {

        TapScreen,
        PressButton,
        WaitCondition,
        ListenEvent
    }
}