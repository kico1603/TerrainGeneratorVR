using UnityEngine;
using UnityEditor;

//LO PONGO EN NEURO DE MOMENTO PERO ESTO DEBE DE SER IGUAL QUE EN PAIN OSEA ES DUPLICADA Y HABRIA QUE ELIMINARLA

namespace Neuro
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ContextMenuButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Muestra el inspector original del script
            DrawDefaultInspector();

            // A�adir un poco de espacio entre el inspector y los botones
            GUILayout.Space(30);

            // Obtener el tipo del objeto al que est� asociado este Editor
            var targetType = target.GetType();

            // Obtener todos los m�todos p�blicos y no p�blicos del script
            var methods = targetType.GetMethods(System.Reflection.BindingFlags.Instance |
                                                System.Reflection.BindingFlags.Public |
                                                System.Reflection.BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                // Verificar si el m�todo tiene el atributo ContextMenu
                var contextMenuAttributes = method.GetCustomAttributes(typeof(ContextMenu), true);
                if (contextMenuAttributes.Length > 0)
                {
                    if (GUILayout.Button(method.Name))
                    {
                        // Invocar el m�todo al presionar el bot�n
                        method.Invoke(target, null);
                    }
                }
            }
        }
    }
}
