using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Beyond.EditorTools
{
    // Unity 6.3's new Hierarchy uses internal UI Toolkit types, not the IMGUI callback.
    [InitializeOnLoad]
    internal static class HierarchyComponentIconUIToolkit
    {
        private const BindingFlags Members = BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.Static;
        private static readonly Type WindowType;
        private static readonly FieldInfo NodeField;
        private static readonly PropertyInfo HandlerProperty;
        private static readonly PropertyInfo IconProperty;
        private static readonly MethodInfo GetGameObject;
        private static readonly EventInfo BindEvent;
        private static readonly Delegate BindCallback;
        private static readonly ConditionalWeakTable<VisualElement, PreviousIcon> Previous =
            new ConditionalWeakTable<VisualElement, PreviousIcon>();

        private sealed class PreviousIcon
        {
            public StyleBackground Background;
            public Texture2D Applied;
        }

        static HierarchyComponentIconUIToolkit()
        {
            WindowType = Type.GetType("Unity.Hierarchy.Editor.HierarchyWindow, UnityEditor.HierarchyModule");
            var itemType = Type.GetType("Unity.Hierarchy.HierarchyViewItem, UnityEngine.HierarchyModule");
            var handlerType = Type.GetType("Unity.Hierarchy.Editor.HierarchyGameObjectHandler, UnityEditor.HierarchyModule");
            if (WindowType == null || itemType == null || handlerType == null)
                return;

            NodeField = itemType.GetField("m_Node", Members);
            HandlerProperty = itemType.GetProperty("Handler", Members);
            IconProperty = itemType.GetProperty("Icon", Members);
            GetGameObject = handlerType.GetMethod("GetGameObject", Members);
            BindEvent = WindowType.GetEvent("BindViewItem", Members);
            if (NodeField == null || HandlerProperty == null || IconProperty == null
                || GetGameObject == null || BindEvent == null)
            {
                Debug.LogWarning("컴포넌트 아이콘: 현재 Unity 하이어라키 API를 확인할 수 없습니다.");
                return;
            }

            var method = typeof(HierarchyComponentIconUIToolkit).GetMethod(nameof(OnBindItem), Members);
            BindCallback = Delegate.CreateDelegate(BindEvent.EventHandlerType, method);
            BindEvent.AddEventHandler(null, BindCallback);
            Undo.undoRedoPerformed += RefreshHierarchy;
            EditorApplication.delayCall += RefreshHierarchy;
            AssemblyReloadEvents.beforeAssemblyReload += Unsubscribe;
        }

        private static void OnBindItem(object item)
        {
            var iconElement = IconProperty.GetValue(item) as VisualElement;
            if (iconElement == null)
                return;

            // Virtualized rows are reused. Remove only our previous override.
            if (Previous.TryGetValue(iconElement, out var previous))
            {
                if (iconElement.style.backgroundImage.value.texture == previous.Applied)
                    iconElement.style.backgroundImage = previous.Background;
                Previous.Remove(iconElement);
            }

            var handler = HandlerProperty.GetValue(item);
            if (handler == null || !GetGameObject.DeclaringType.IsInstanceOfType(handler))
                return;

            var target = GetGameObject.Invoke(handler, new[] { NodeField.GetValue(item) }) as GameObject;
            if (target == null)
                return;

            var icon = EditorGUIUtility.GetIconForObject(target);
            if (icon == null)
                return;

            Previous.Add(iconElement, new PreviousIcon
            {
                Background = iconElement.style.backgroundImage,
                Applied = icon
            });
            iconElement.style.backgroundImage = new StyleBackground(icon);
        }

        internal static void RefreshHierarchy()
        {
            if (WindowType == null || BindCallback == null)
                return;

            foreach (var value in Resources.FindObjectsOfTypeAll(WindowType))
            {
                var window = value as EditorWindow;
                if (window == null)
                    continue;
                var list = window.rootVisualElement.Q<MultiColumnListView>();
                if (list != null)
                    list.RefreshItems();
            }
        }

        private static void Unsubscribe()
        {
            BindEvent.RemoveEventHandler(null, BindCallback);
            Undo.undoRedoPerformed -= RefreshHierarchy;
            EditorApplication.delayCall -= RefreshHierarchy;
            AssemblyReloadEvents.beforeAssemblyReload -= Unsubscribe;
        }
    }
}