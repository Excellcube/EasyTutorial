using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.EasyTutorial
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
                            m_PageData = m_DialogPageData;
                        }
                        return m_DialogPageData;
                    }
                    case PageType.Action : {
                        if(m_ActionPageData == null) {
                            m_ActionPageData = new ActionTutorialPageData();
                            m_PageData = m_ActionPageData;
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
                        m_PageData = m_DialogPageData;
                        return m_DialogPageData;
                    }
                }
            }
        }

        [SerializeField]
        private TutorialPageData m_PageData;

        [SerializeField]
        private DialogTutorialPageData m_DialogPageData;

        [SerializeField]
        private ActionTutorialPageData m_ActionPageData;

        [SerializeField]
        private DetailTutorialPageContentData m_DetailContentPageData;

        /// <summary>
        /// Custom Editor를 위한 Serialized Fields.
        /// </summary>
        [SerializeField]
        private bool m_FoldOut;

        [SerializeField]
        private float m_PositionY;
    }
}