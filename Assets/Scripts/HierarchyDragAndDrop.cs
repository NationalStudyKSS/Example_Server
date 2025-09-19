using UnityEngine;
using UnityEditor;
using System.Linq;

public class HierarchyDragAndDrop : MonoBehaviour
{
    [InitializeOnLoadMethod]

    static void Initialize()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.DragUpdated)
        {
            Debug.Log("드래그중");
            if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences.All(obj => obj is MonoScript))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                currentEvent.Use();
            }
        }
        else if (currentEvent.type == EventType.DragPerform)
        {
            Debug.Log("드랍");
            Object[] draggedObjects = DragAndDrop.objectReferences;
            if (draggedObjects.Length > 0 && draggedObjects.All(obj => obj is MonoScript))
            {
                DragAndDrop.AcceptDrag();

                foreach (Object draggedObject in draggedObjects)
                {
                    MonoScript monoScript = draggedObject as MonoScript;
                    if (monoScript != null && monoScript.GetClass() != null && monoScript.GetClass().IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        GameObject newGameObject = new GameObject(monoScript.name);
                        Debug.Log($"{newGameObject.name} 오브젝트 생성됨");
                        newGameObject.AddComponent(monoScript.GetClass());
                        Undo.RegisterCreatedObjectUndo(newGameObject, "Create " + newGameObject.name);
                        GameObject parentObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                        Debug.Log("parentObject: " + parentObject);
                        if (parentObject != null)
                        {
                            newGameObject.transform.SetParent(parentObject.transform); // 직접 지정
                            Undo.SetTransformParent(newGameObject.transform, parentObject.transform, "Set Parent of " + newGameObject.name);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("드래그된 스크립트가 MonoBehaviour를 상속하지 않음: " + draggedObject.name);
                    }
                }
                currentEvent.Use();

            }
        }
    }
}

//using UnityEngine;
//using UnityEditor;
//using System.Linq;

//public class HierarchyDragAndDrop : MonoBehaviour
//{
//    [InitializeOnLoadMethod]
//    static void Initialize()
//    {
//        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
//    }

//    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
//    {
//        Event currentEvent = Event.current;
//        if (!selectionRect.Contains(currentEvent.mousePosition))
//            return; // 마우스가 해당 오브젝트 위에 있을 때만 처리

//        if (currentEvent.type == EventType.DragUpdated)
//        {
//            if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences.All(obj => obj is MonoScript))
//            {
//                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
//                currentEvent.Use();
//            }
//        }
//        else if (currentEvent.type == EventType.DragPerform)
//        {
//            Object[] draggedObjects = DragAndDrop.objectReferences;
//            if (draggedObjects.Length > 0 && draggedObjects.All(obj => obj is MonoScript))
//            {
//                DragAndDrop.AcceptDrag();

//                foreach (Object draggedObject in draggedObjects)
//                {
//                    MonoScript monoScript = draggedObject as MonoScript;
//                    if (monoScript != null && monoScript.GetClass() != null && monoScript.GetClass().IsSubclassOf(typeof(MonoBehaviour)))
//                    {
//                        GameObject newGameObject = new GameObject(monoScript.name);
//                        newGameObject.AddComponent(monoScript.GetClass());
//                        Undo.RegisterCreatedObjectUndo(newGameObject, "Create " + newGameObject.name);

//                        GameObject parentObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
//                        Debug.Log($"instanceID: {instanceID}, parentObject: {parentObject}");
//                        if (parentObject != null)
//                        {
//                            newGameObject.transform.SetParent(parentObject.transform); // 직접 부모 지정
//                            Undo.SetTransformParent(newGameObject.transform, parentObject.transform, "Set Parent of " + newGameObject.name);
//                        }
//                    }
//                }
//                currentEvent.Use();
//            }
//        }
//    }
//}