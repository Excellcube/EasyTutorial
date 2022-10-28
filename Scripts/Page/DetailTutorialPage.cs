using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Page
{
    public class DetailTutorialPage : TutorialPage 
    {
        private DetailTutorialPageView m_View;

        public DetailTutorialPage(DetailTutorialPageView view)
        {
            m_View = view;
        }

        protected override void ConfigureView()
        {
            
        }
    }
}