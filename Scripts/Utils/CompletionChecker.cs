using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Utils
{
    public class CompletionChecker
    {
        private int m_CurrentTutorialIndex;
        public  int CurrentTutorialIndex => m_CurrentTutorialIndex;

        private int m_TutorialPagesCount = 0;
        

        public void Initialize(int tutorialPagesCount)
        {
            m_TutorialPagesCount = tutorialPagesCount;
        }

        public void FindLastIncompletedTutorialIndex()
        {

        }

        public void Complete(int index)
        {

        }
    }
}
