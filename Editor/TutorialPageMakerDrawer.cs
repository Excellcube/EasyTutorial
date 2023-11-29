using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Excellcube.EasyTutorial
{
    [CustomPropertyDrawer(typeof(TutorialPageMaker))]
    public class TutorialPageMakerDrawer : PropertyDrawer {
        private bool m_FoldOutEvents = false;
        private float m_TotalHeight = 0;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var foldOutProp   = property.FindPropertyRelative(Field.FoldOut);
            var pageTypeProp  = property.FindPropertyRelative(Field.PageType);

            string propName = GetPageDataName(pageTypeProp.enumValueIndex);
            var pageDataProp  = property.FindPropertyRelative(propName);

            float totalHeight = 0;
            totalHeight += EditorGUIUtility.singleLineHeight * 1.5f;          // Title Label.
            totalHeight += EditorGUIUtility.singleLineHeight;                 // 튜토리얼 이름 영역.
            totalHeight += EditorGUI.GetPropertyHeight(pageTypeProp, true);
            totalHeight += EditorGUI.GetPropertyHeight(pageDataProp, true);
            totalHeight += EditorGUIUtility.singleLineHeight * 0.7f;          // margin.

            float resultHeight = foldOutProp.boolValue ? totalHeight : EditorGUIUtility.singleLineHeight * 1.2f;
            return resultHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var foldOutProp  = property.FindPropertyRelative(Field.FoldOut);
            var pageTypeProp = property.FindPropertyRelative(Field.PageType);


            if(foldOutProp.boolValue) {
                position.y += EditorGUIUtility.singleLineHeight * 1.2f;

                // --  페이지 타입 그리기 -- //
                EditorGUI.PropertyField(position, pageTypeProp, new GUIContent("페이지 타입"), true);
                position.y += EditorGUIUtility.singleLineHeight * 1.2f;

                // -- 페이지 데이터 영역 그리기 -- //
                string propName  = GetPageDataName(pageTypeProp.enumValueIndex);
                var pageDataProp = property.FindPropertyRelative(propName);

                // 각 pageData의 property는 DialogTutorialPageDataDrawer와 ActionTutorialPageDataDrawer에 구현되어 있음.
                EditorGUI.PropertyField(position, pageDataProp);
            }

            EditorGUI.EndProperty();
        }

        private string GetPageDataName(int typeIndex)
        {
            switch(typeIndex)
            {
                case 0 :
                    return Field.DialogPageData;
                case 1 :
                    return Field.ActionPageData;
                default:
                    return Field.DialogPageData;
            }
        }
    }
}