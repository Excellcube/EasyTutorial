using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Page
{
    public abstract class TutorialPage
    {
        protected TutorialPageData m_Data;
        
        public virtual void ShowUsingData(TutorialPageData data)
        {
            m_Data = data;
            ConfigureView();
        }

        /// <summary>
        /// ShowUsingData 메서드를 통해 할당된 TutorialPageData를 이용해 화면을 구성하게 만드는 메서드.
        /// </summary>
        protected abstract void ConfigureView();
    }
}