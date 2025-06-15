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

            // Añadir un poco de espacio entre el inspector y los botones
            GUILayout.Space(30);

            // Obtener el tipo del objeto al que está asociado este Editor
            var targetType = target.GetType();

            // Obtener todos los métodos públicos y no públicos del script
            var methods = targetType.GetMethods(System.Reflection.BindingFlags.Instance |
                                                System.Reflection.BindingFlags.Public |
                                                System.Reflection.BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                // Verificar si el método tiene el atributo ContextMenu
                var contextMenuAttributes = method.GetCustomAttributes(typeof(ContextMenu), true);
                if (contextMenuAttributes.Length > 0)
                {
                    if (GUILayout.Button(method.Name))
                    {
                        // Invocar el método al presionar el botón
                        method.Invoke(target, null);
                    }
                }
            }
        }
    }
}
