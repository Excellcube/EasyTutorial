using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial.Page
{
    [System.Serializable]
    public class TutorialPageMaker
    {
        [SerializeField]
        private PageType m_PageType;

        public  TutorialPageData PageData {
            get {
                switch(m_PageType)
                {
                    case PageType.Dialog : {
                        if(m_DialogPageData == null) {
                            m_DialogPageData = new DialogTutorialPageData();
                        }
                        return m_DialogPageData;
                    }
                    case PageType.Action : {
                        if(m_ActionPageData == null) {
                            m_ActionPageData = new ActionTutorialPageData();
                        }
                        return m_ActionPageData;
                    }
                    // case PageType.Detail : {
                    //     if(m_DetailContentPageData == null) {
                    //         m_DetailContentPageData = new DetailTutorialPageContentData();
                    //     }
                    //     return m_DetailContentPageData;
                    // }
                    default : {
                        return m_DialogPageData;
                    }
                }
            }
        }

        [SerializeField]
        private DialogTutorialPageData m_DialogPageData;

        [SerializeField]
        private ActionTutorialPageData m_ActionPageData;

        [SerializeField]
        private DetailTutorialPageContentData m_DetailContentPageData;
    }
}