using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScriptableObjects
{
    using System;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    namespace Editors
    {
        public class EditorProjectWindowExtensions
        {
            [MenuItem("Assets/Find ScriptableObject Instances", false, 30)]
            private static void DoSomethingWithVariable()
            {
                SetSearchString("t:" + Selection.activeObject.name);
            }

            // Note that we pass the same path, and also pass "true" to the second argument.
            [MenuItem("Assets/Find ScriptableObject Instances", true)]
            private static bool NewMenuOptionValidation()
            {
                if (!(Selection.activeObject is MonoScript))
                {
                    return false;
                }
                MonoScript o = (MonoScript)Selection.activeObject;
                Type ot = o.GetClass();
                return ot.IsSubclassOf(typeof(ScriptableObject));
            }

            private static void SetSearchString(string searchString)
            {
                Type projectBrowserType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
                if (projectBrowserType == null)
                {
                    return;
                }

                MethodInfo setSearchMethodInfo = projectBrowserType.GetMethod("SetSearch", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                if (setSearchMethodInfo == null)
                {
                    return;
                }

                EditorWindow window = EditorWindow.GetWindow(projectBrowserType);
                setSearchMethodInfo.Invoke(window, new object[] { searchString });
            }
        }
    }
}
