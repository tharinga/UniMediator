using UnityEngine;
using UnityEditor;

namespace UniMediator.Editor
{
    internal static class MenuItems 
    {
        [MenuItem("Window/UniMediator/Install Mediator")]
        internal static void Install()
        {
            InstallMediator();
        }
        
        [MenuItem("Window/UniMediator/Documentation")]
        internal static void ViewDocumentation()
        {
            Application.OpenURL("https://github.com/tharinga/UniMediator");
        }
        
        [MenuItem("Window/UniMediator/Report Issue...")]
        internal static void ReportIssue()
        {
            Application.OpenURL("https://github.com/tharinga/UniMediator/issues");
        }

        private static void InstallMediator()
        {
            foreach (var mediator in Object.FindObjectsOfType<MediatorImpl>())
            {
                Object.DestroyImmediate(mediator.gameObject);
            }

            var @object = new GameObject("UniMediator");
            @object.AddComponent<MediatorImpl>();
            @object.AddComponent<Mediator>();

            Undo.RegisterCreatedObjectUndo(@object, "Install Mediator");
        }
    }
}

