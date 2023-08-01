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
        private SerializedProperty m_UseLocalizationProp;
        private SerializedProperty m_TextLocalizerProp;
        private SerializedProperty m_LocalizationTableProp;

        private SerializedProperty m_TutorialPageMakersProp;

        private ReorderableList m_TutorialPageMakersRO;

        private bool[] m_FoldoutStates; // 각 요소의 foldout 상태를 추적하는 배열



        private int m_CurrSelectedIndex = -1;


        private void OnEnable() 
        {
            m_Target = (ECEasyTutorial) target;
            m_FoldoutStates = new bool[128];

            m_PlayOnAwakeProp = serializedObject.FindProperty(Field.PlayOnAwake);
            m_UseLocalizationProp = serializedObject.FindProperty(Field.UseLocalization);
            m_TextLocalizerProp = serializedObject.FindProperty(Field.TextLocalizer);
            m_LocalizationTableProp = serializedObject.FindProperty(Field.LocalizationTable);

            m_TutorialPageMakersProp = serializedObject.FindProperty(Field.TutorialPageMaker);

            m_TutorialPageMakersRO = new ReorderableList(serializedObject, m_TutorialPageMakersProp, true, false, true, true);
            m_TutorialPageMakersRO.drawHeaderCallback = OnDrawTutorialDataListHeader;
            m_TutorialPageMakersRO.drawElementCallback = OnDrawTutorialDataListItems;
            m_TutorialPageMakersRO.elementHeightCallback = delegate(int index) {
                var element = m_TutorialPageMakersRO.serializedProperty.GetArrayElementAtIndex(index);
                var margin = EditorGUIUtility.standardVerticalSpacing;
                var height = margin;
                height += EditorGUI.GetPropertyHeight(element, true);
                return height;
            };
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_PlayOnAwakeProp, new GUIContent("Awake 호출 시 튜토리얼 시작"));
            EditorGUILayout.PropertyField(m_UseLocalizationProp, new GUIContent("Localization 사용"));

            GlobalContext.useLocalization = m_UseLocalizationProp.boolValue;
            if(GlobalContext.useLocalization)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(m_TextLocalizerProp);
                EditorGUILayout.PropertyField(m_LocalizationTableProp);
                EditorGUI.indentLevel = 0;
            }

            EditorGUILayout.Space();

            DrawTutorialDataList();

            if (GUI.changed) 
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawTutorialDataList()
        {
            m_TutorialPageMakersRO.DoLayoutList();
        }

        private void OnDrawTutorialDataListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Tutorial Page Data", EditorStyles.boldLabel);
        }

        private void OnDrawTutorialDataListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            var elemProp      = m_TutorialPageMakersRO.serializedProperty.GetArrayElementAtIndex(index);
            var pageDataProp  = elemProp.FindPropertyRelative(Field.PageData);
            var foldOutProp   = elemProp.FindPropertyRelative(Field.FoldOut);
            var yPositionProp = elemProp.FindPropertyRelative(Field.PositionY);
            var pageNameProp  = pageDataProp.FindPropertyRelative(Field.Name);


            // -- Foldout 영역 그리기 -- //
            Rect foldoutRect = rect;
            foldoutRect.x += 15;
            foldoutRect.y += 1;

            m_FoldoutStates[index] = EditorGUI.Foldout(new Rect(foldoutRect.x, foldoutRect.y, 15, EditorGUIUtility.singleLineHeight), m_FoldoutStates[index], GUIContent.none);
            foldOutProp.boolValue = m_FoldoutStates[index];


            // -- Label 영역 그리기 -- //
            
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            GUIContent labelContent = new GUIContent($"STEP {index + 1} - {pageNameProp.stringValue}");

            Rect labelRect = new Rect(foldoutRect.x + 10, foldoutRect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, labelContent, labelStyle);
            

            if(m_FoldoutStates[index])
            {
                // -- 튜토리얼 이름 영역 그리기 -- //

                float labelWidth = EditorGUIUtility.currentViewWidth;
                float labelHeight = EditorStyles.label.CalcHeight(labelContent, labelWidth);

                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight * 0.5f;

                EditorGUI.PropertyField(rect, pageNameProp, new GUIContent("튜토리얼 이름"), true);
            }

            yPositionProp.floatValue = rect.y;


            // 변경 사항을 실제 오브젝트에 적용.
            serializedObject.ApplyModifiedProperties();


            // -- 튜토리얼 데이터 영역 그리기 -- //

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

    /// <summary>
    /// ECEasyTutorial의 custom editor에서 접근할 serialized property의 이름들.
    /// </summary>
    internal struct Field {
        public const string PlayOnAwake = "m_PlayOnAwake";
        public const string UseLocalization = "m_UseLocalization";
        public const string TextLocalizer = "m_TextLocalizer";
        public const string LocalizationTable = "m_LocalizationTable";
        public const string TutorialPageMaker = "m_TutorialPageMakers";
        public const string PageData = "m_PageData";
        public const string DialogPageData = "m_DialogPageData";
        public const string ActionPageData = "m_ActionPageData";
        public const string PageType = "m_PageType";
        public const string FoldOut = "m_FoldOut";
        public const string PositionY = "m_PositionY";
        public const string Name = "m_Name";


        // -- Tutorial Page Data -- //
        public const string StartDelay         = "m_StartDelay";
        public const string OnTutorialBegin    = "m_OnTutorialBegin";
        public const string OnTutorialInvoked  = "m_OnTutorialInvoked";
        public const string OnTutorialEnded    = "m_OnTutorialEnded";


        // -- Dialog Tutorial Page Data -- //
        public const string LeftSprite       = "m_LeftSprite";
        public const string RightSprite      = "m_RightSprite";
        public const string CharacterName    = "m_CharacterName";
        public const string CharacterNameKey = "m_CharacterNameKey";
        public const string Dialog           = "m_Dialog";
        public const string DialogKey        = "m_DialogKey";


        // -- Action Tutorial Page Data -- //
        public const string ActionLog         = "m_ActionLog";
        public const string ActionLogKey      = "m_ActionLogKey";
        public const string HighlightTarget   = "m_HighlightTarget";
        public const string DynamicTargetRoot = "m_DynamicTargetRoot";
        public const string DynamicTargetKey  = "m_DynamicTargetKey";
        public const string IndicatorPosition = "m_IndicatorPosition";
        public const string CompleteKey       = "m_CompleteKey";
    }
}
#endif