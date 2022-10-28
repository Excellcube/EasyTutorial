using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Excellcube.EasyTutorial.Page
{
    [CustomPropertyDrawer(typeof(TutorialPageMaker))]
    public class TutorialPageMakerDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = 0;

            SerializedProperty pageTypeProp = property.FindPropertyRelative("m_PageType");
            totalHeight += EditorGUI.GetPropertyHeight(pageTypeProp, true);

            string propName = GetPageDataName(pageTypeProp.enumValueIndex);

            SerializedProperty pageDataProp;
            pageDataProp = property.FindPropertyRelative(propName);
            totalHeight += EditorGUI.GetPropertyHeight(pageDataProp, true);

            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {                        
            EditorGUI.BeginProperty(position, label, property);

            ///// Draw page type.

            SerializedProperty pageTypeProp = property.FindPropertyRelative("m_PageType");
            EditorGUI.PropertyField(position, pageTypeProp, label, true);
            
            ///// Draw page data.

            string propName = GetPageDataName(pageTypeProp.enumValueIndex);

            SerializedProperty pageDataProp;
            pageDataProp = property.FindPropertyRelative(propName);

            Rect pageDataPosition = position;
            float pageTypeFieldHeight = EditorGUI.GetPropertyHeight(pageTypeProp, true);
            pageDataPosition.position += new Vector2(0, pageTypeFieldHeight);

            EditorGUI.PropertyField(pageDataPosition, pageDataProp, true);

            EditorGUI.EndProperty();
        }

        private string GetPageDataName(int typeIndex)
        {
            switch(typeIndex)
            {
                case 0 :
                    return "m_DialogPageData";
                case 1 :
                    return "m_ActionPageData";
                case 2 :
                    return "m_DetailContentPageData";
                default:
                    return "m_DialogPageData";
            }
        }
    }
}