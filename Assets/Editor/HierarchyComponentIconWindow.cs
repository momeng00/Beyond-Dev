using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Beyond.EditorTools
{
    // Unity draws its own cube/prefab icon in the Hierarchy, independently of m_Icon.
    [InitializeOnLoad]
    internal static class HierarchyComponentIconDrawer
    {
        static HierarchyComponentIconDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawIcon;
            Undo.undoRedoPerformed += EditorApplication.RepaintHierarchyWindow;
            EditorApplication.RepaintHierarchyWindow();
        }

        private static void DrawIcon(int instanceId, Rect rowRect)
        {
            if (Event.current.type != UnityEngine.EventType.Repaint || rowRect.width < 16f)
                return;

            var target = EditorUtility.EntityIdToObject(instanceId) as GameObject;
            if (target == null)
                return;

            var icon = EditorGUIUtility.GetIconForObject(target);
            if (icon == null)
                return;

            // Unity 6.3 passes an indented row rect whose x is the native icon's x.
            var iconRect = new Rect(rowRect.x, rowRect.y, 16f, rowRect.height);
            var background = new Color(0.76f, 0.76f, 0.76f, 1f);
            if (EditorGUIUtility.isProSkin)
                background = new Color(0.2196078f, 0.2196078f, 0.2196078f, 1f);
            EditorGUI.DrawRect(iconRect, background);

            bool selected = Selection.Contains(target);
            var focusedWindow = EditorWindow.focusedWindow;
            bool focused = focusedWindow != null
                && focusedWindow.GetType().Name == "SceneHierarchyWindow";
            var selectionStyle = GUI.skin.FindStyle("TV Selection");
            if (selected && selectionStyle != null)
                selectionStyle.Draw(iconRect, false, false, true, focused);

            var color = GUI.color;
            if (!target.activeInHierarchy)
                color.a *= 0.5f;
            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true, 0f, color, 0f, 0f);
        }
    }
    // Install this file under Assets/Editor. Changes are made only on a button click.
    public sealed class HierarchyComponentIconWindow : EditorWindow
    {
        private Vector2 scroll;

        [MenuItem("Tools/Hierarchy/Component Icons")]
        [MenuItem("GameObject/Component Icon/Choose from Components...", false, 49)]
        private static void Open()
        {
            var window = GetWindow<HierarchyComponentIconWindow>();
            window.titleContent = new GUIContent("컴포넌트 아이콘");
            window.minSize = new Vector2(330f, 240f);
            window.Show();
        }

        [MenuItem("CONTEXT/Component/Use Icon for GameObject")]
        private static void UseComponentIcon(MenuCommand command)
        {
            var component = command.context as Component;
            if (component == null || EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            var icon = GetComponentIcon(component);
            if (icon != null)
                ApplyIcon(new[] { component.gameObject }, icon);
        }

        private void OnEnable()
        {
            Selection.selectionChanged += Repaint;
            Undo.undoRedoPerformed += RepaintViews;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= Repaint;
            Undo.undoRedoPerformed -= RepaintViews;
        }

        private void OnGUI()
        {
            var active = Selection.activeGameObject;
            var targets = Selection.gameObjects;
            EditorGUILayout.HelpBox(
                "오브젝트를 선택하고 컴포넌트 아이콘을 누르세요.\n" +
                "선택한 씬 오브젝트 모두에 적용합니다. 씬 저장 시 유지됩니다.\n" +
                "Inspector와 Scene 뷰의 오브젝트 아이콘도 함께 바뀝니다.",
                MessageType.Info);

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("아이콘 변경은 재생을 멈춘 뒤 사용할 수 있습니다.", MessageType.Info);
                return;
            }

            if (!CanEdit(active))
            {
                EditorGUILayout.HelpBox("하이어라키에서 수정할 오브젝트를 선택하세요.", MessageType.Info);
                return;
            }

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("아이콘 목록 기준", active, typeof(GameObject), true);

            int count = 0;
            foreach (var target in targets)
            {
                if (CanEdit(target))
                    count++;
            }
            EditorGUILayout.LabelField("적용 대상", count + "개 오브젝트");

            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var component in active.GetComponents<Component>())
            {
                if (component == null)
                    continue;

                var icon = GetComponentIcon(component);
                var label = ObjectNames.NicifyVariableName(component.GetType().Name);
                var content = new GUIContent(label, icon, "이 아이콘을 선택한 오브젝트에 적용");
                using (new EditorGUI.DisabledScope(icon == null))
                {
                    if (GUILayout.Button(content, GUILayout.Height(28f)))
                        ApplyIcon(targets, icon);
                }
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("기본 아이콘으로 복원", GUILayout.Height(28f)))
                ApplyIcon(targets, null);
        }

        private static Texture2D GetComponentIcon(Component component)
        {
            return EditorGUIUtility.ObjectContent(component, component.GetType()).image as Texture2D;
        }

        private static bool CanEdit(GameObject target)
        {
            return target != null
                && !EditorUtility.IsPersistent(target)
                && target.scene.IsValid()
                && target.scene.isLoaded
                && (target.hideFlags & HideFlags.NotEditable) == 0;
        }

        private static void ApplyIcon(GameObject[] targets, Texture2D icon)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Change GameObject Component Icon");

            foreach (var target in targets)
            {
                if (!CanEdit(target) || EditorGUIUtility.GetIconForObject(target) == icon)
                    continue;

                Undo.RecordObject(target, "Change GameObject Component Icon");
                EditorGUIUtility.SetIconForObject(target, icon);
                if (PrefabUtility.IsPartOfPrefabInstance(target))
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                EditorUtility.SetDirty(target);
                EditorSceneManager.MarkSceneDirty(target.scene);
            }

            Undo.CollapseUndoOperations(group);
            RepaintViews();
        }

        private static void RepaintViews()
        {
            EditorApplication.RepaintHierarchyWindow();
            SceneView.RepaintAll();
            HierarchyComponentIconUIToolkit.RefreshHierarchy();
        }
    }
}
