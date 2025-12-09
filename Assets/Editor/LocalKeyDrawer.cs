using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LocalKey))]
public class LocalKeyDrawer : PropertyDrawer
{
    private static Dictionary<string, Dictionary<string, List<string>>> hierarchy;
    private static string[] mainCategories;
    private static bool isInitialized = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 데이터가 없으면 초기화 (컴파일 직후 등)
        if (!isInitialized || hierarchy == null) InitializeData();

        EditorGUI.BeginProperty(position, label, property);

        // 1) 현재 선택된 값 가져오기
        SerializedProperty keyProp = property.FindPropertyRelative("key");
        string currentFullKey = keyProp.enumNames[keyProp.enumValueIndex]; // 예: "St1_Tuto_Jump"

        // 2) 현재 값의 카테고리 역추적 (초기 선택값 설정을 위해)
        string currentMain = "Etc";
        string currentSub = "Etc";

        // 이름 쪼개기 ("St1_Tuto_Jump" -> "St1", "Tuto")
        string[] parts = currentFullKey.Split('_');

        if (currentFullKey == "None")
        {
            currentMain = "None"; currentSub = "None";
        }
        else if (parts.Length >= 3)
        {
            currentMain = parts[0];
            currentSub = parts[1];
        }
        else if (parts.Length == 2)
        {
            currentMain = parts[0];
            currentSub = "General"; // 중분류가 없으면 General로 취급
        }

        // 데이터 딕셔너리에 없는 키일 경우 안전장치 (Error 방지)
        if (!hierarchy.ContainsKey(currentMain)) currentMain = mainCategories[0];
        if (!hierarchy[currentMain].ContainsKey(currentSub)) currentSub = hierarchy[currentMain].Keys.First();


        // --- 3) 화면 그리기 (Rect 계산) ---

        // 전체 한 줄을 가져와서 라벨 영역을 먼저 그립니다.
        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        EditorGUI.LabelField(labelRect, label);

        // 남은 오른쪽 공간을 계산합니다.
        float contentWidth = position.width - EditorGUIUtility.labelWidth;
        float startX = position.x + EditorGUIUtility.labelWidth;

        // 공간을 3등분 합니다 (대분류 / 중분류 / 소분류)
        float fieldWidth = contentWidth / 3f;

        Rect mainRect = new Rect(startX, position.y, fieldWidth - 2, position.height);
        Rect subRect = new Rect(startX + fieldWidth, position.y, fieldWidth - 2, position.height);
        Rect keyRect = new Rect(startX + (fieldWidth * 2), position.y, fieldWidth, position.height);


        // --- 4) 팝업 1: 대분류 (Main Category) ---
        int mainIndex = Array.IndexOf(mainCategories, currentMain);
        if (mainIndex < 0) mainIndex = 0;

        int newMainIndex = EditorGUI.Popup(mainRect, mainIndex, mainCategories);
        string newMain = mainCategories[newMainIndex];

        // --- 5) 팝업 2: 중분류 (Sub Category) ---
        // 선택된 대분류에 속한 중분류 목록만 가져옵니다.
        string[] subCats = hierarchy[newMain].Keys.ToArray();
        int subIndex = Array.IndexOf(subCats, currentSub);

        // 대분류를 바꿨으면 중분류 인덱스가 안 맞을 수 있으니 0으로 리셋
        if (subIndex < 0 || newMain != currentMain) subIndex = 0;

        int newSubIndex = EditorGUI.Popup(subRect, subIndex, subCats);
        string newSub = subCats[newSubIndex];

        // --- 6) 팝업 3: 최종 키 (Key) ---
        // 선택된 중분류에 속한 키 목록만 가져옵니다.
        string[] keys = hierarchy[newMain][newSub].ToArray();

        // 표시용 이름 만들기 (뒷부분만 보여주기: "Jump")
        // 전체 이름("St1_Tuto_Jump")을 다 보여주면 칸이 좁으니까요.
        string[] displayOptions = keys.Select(k => k.Split('_').Last()).ToArray();
        if (newMain == "None") displayOptions = new string[] { "None" };

        int keyIndex = Array.IndexOf(keys, currentFullKey);
        if (keyIndex < 0 || newSub != currentSub || newMain != currentMain) keyIndex = 0;

        int newKeyIndex = EditorGUI.Popup(keyRect, keyIndex, displayOptions);
        string newKey = keys[newKeyIndex]; // 실제 저장할 값은 전체 이름

        // --- 7) 값 변경 적용 ---
        // 사용자가 선택을 바꿨다면 실제 프로퍼티에 적용합니다.
        if (newKey != currentFullKey)
        {
            keyProp.enumValueIndex = Array.IndexOf(keyProp.enumNames, newKey);
        }

        EditorGUI.EndProperty();
    }

    // --- 2. 데이터 초기화 (분류 작업) ---
    private void InitializeData()
    {
        hierarchy = new Dictionary<string, Dictionary<string, List<string>>>();

        // Enum의 모든 이름을 가져옵니다.
        string[] allKeys = Enum.GetNames(typeof(LocalizationKeys));

        foreach (string key in allKeys)
        {
            string main = "Etc";
            string sub = "Etc";

            if (key == "None")
            {
                main = "None"; sub = "None";
            }
            else
            {
                string[] parts = key.Split('_');
                if (parts.Length >= 3)
                {
                    main = parts[0]; // St1
                    sub = parts[1];  // Tuto
                }
                else if (parts.Length == 2)
                {
                    main = parts[0];
                    sub = "General";
                }
            }

            // 딕셔너리 구조 잡기
            if (!hierarchy.ContainsKey(main))
                hierarchy[main] = new Dictionary<string, List<string>>();

            if (!hierarchy[main].ContainsKey(sub))
                hierarchy[main][sub] = new List<string>();

            // 키 추가
            hierarchy[main][sub].Add(key);
        }

        mainCategories = hierarchy.Keys.ToArray();
        isInitialized = true;
    }
}