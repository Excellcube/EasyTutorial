#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Excellcube.EasyTutorial.Page;

namespace Excellcube.EasyTutorial
{
    [CustomEditor(typeof(ECEasyTutorial))]
    public class ECEasyTutorialEditor : Editor {
        private ECEasyTutorial m_Target;

        private SerializedProperty m_PlayOnAwakeProp;
        private SerializedProperty m_SkipProp;
        private SerializedProperty m_UseLocalizationProp;
        private SerializedProperty m_TextLocalizerProp;
        private SerializedProperty m_LocalizationTableProp;

        private SerializedProperty m_EnableSkipFlagProp;
        private SerializedProperty m_TutorialPageMakersProp;
        private SerializedProperty m_OnTutorialsSkippedProp;

        private ReorderableList m_TutorialPageMakersRO;


        private int m_CurrSelectedIndex = -1;


        private void OnEnable() 
        {
            m_Target = (ECEasyTutorial) target;

            m_PlayOnAwakeProp = serializedObject.FindProperty("m_PlayOnAwake");
            m_UseLocalizationProp = serializedObject.FindProperty("m_UseLocalization");
            m_TextLocalizerProp = serializedObject.FindProperty("m_TextLocalizer");
            m_LocalizationTableProp = serializedObject.FindProperty("m_LocalizationTable");

            m_EnableSkipFlagProp = serializedObject.FindProperty("m_EnableSkipFlag");
            m_TutorialPageMakersProp = serializedObject.FindProperty("m_TutorialPageMakers");

            m_OnTutorialsSkippedProp = serializedObject.FindProperty("m_OnTutorialsSkipped");

            m_TutorialPageMakersRO = new ReorderableList(serializedObject, m_TutorialPageMakersProp, true, true, true, true);
            m_TutorialPageMakersRO.drawHeaderCallback = OnDrawTutorialDataListHeader;
            m_TutorialPageMakersRO.drawElementCallback = OnDrawTutorialDataListItems;
            m_TutorialPageMakersRO.elementHeightCallback = delegate(int index) {
                var element = m_TutorialPageMakersRO.serializedProperty.GetArrayElementAtIndex(index);
                var margin = EditorGUIUtility.standardVerticalSpacing + 20;
                var height = margin;
                height += EditorGUI.GetPropertyHeight(element, true);
                return height;
            };
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_PlayOnAwakeProp);
            EditorGUILayout.PropertyField(m_UseLocalizationProp);
            if(m_UseLocalizationProp.boolValue)
            {
                EditorGUILayout.PropertyField(m_TextLocalizerProp);
                EditorGUILayout.PropertyField(m_LocalizationTableProp);
            }

            EditorGUILayout.PropertyField(m_OnTutorialsSkippedProp);

            EditorGUILayout.PropertyField(m_EnableSkipFlagProp);

            DrawTutorialDataList();

            if(GUILayout.Button("Clear tutorial completion flag")) {
                Debug.Log("[Easy Tutorial] Clear tutorial completion flag");
                PlayerPrefs.SetInt("ECET_CLEAR_ALL", 0);
            }

            if (GUI.changed) 
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawTutorialDataList()
        {
            // EditorGUILayout.BeginHorizontal();
            // GUILayout.Label("Select", EditorStyles.boldLabel, GUILayout.Width(45));
            // GUILayout.Label("Tutorial Type", EditorStyles.boldLabel, GUILayout.Width(100));
            // GUILayout.Label("Tutorial Data", EditorStyles.boldLabel);
            // GUILayout.Label("Delete", EditorStyles.boldLabel, GUILayout.Width(45));
            // EditorGUILayout.EndHorizontal();

            m_TutorialPageMakersRO.DoLayoutList();

            // EditorGUILayout.PropertyField(m_TutorialPageMakersProp);
        }

        private void OnDrawTutorialDataListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Tutorial Page Data", EditorStyles.boldLabel);
        }

        private void OnDrawTutorialDataListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            var elemProp = m_TutorialPageMakersRO.serializedProperty.GetArrayElementAtIndex(index);            
            EditorGUI.PropertyField (rect, elemProp);
        }

        private PageType GetPageType(int typeIndex)
        {
            switch(typeIndex)
            {
                case 0 :
                    return PageType.Dialog;
                case 1 :
                    return PageType.Action;
                // case 2 :
                //     return PageType.Detail;
                default:
                    return PageType.Dialog;
            }
        }
    }
}
#endif