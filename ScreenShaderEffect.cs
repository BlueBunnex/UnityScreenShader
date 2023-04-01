using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ScreenShaderEffect : MonoBehaviour {

    public Shader shader;

    [HideInInspector][SerializeField]
    private Material material;

    void OnValidate() {
        if (!shader) {
            this.enabled = false;
            material = null;
        } else if (!material) {
            this.enabled = true;
            material = new Material(shader);
        }
    }

    // Creates a private material used to the effect
    void Awake() {
        material = new Material(shader);
    }
    
    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit (source, destination, material);
    }

    #if (UNITY_EDITOR)
    [CustomEditor(typeof(ScreenShaderEffect))]
    class ScreenShaderEffectEditor : Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            ScreenShaderEffect script = (ScreenShaderEffect) target;

            if (!script.material) { return; }
            
            foreach (MaterialProperty prop in MaterialEditor.GetMaterialProperties(new Object[] {script.material})) {
                
                EditorGUILayout.Space();

                switch (prop.type) {
                    case MaterialProperty.PropType.Color:
                        script.material.SetColor(prop.name, EditorGUILayout.ColorField(prop.name, script.material.GetColor(prop.name)));
                        break;
                    case MaterialProperty.PropType.Vector:
                        script.material.SetVector(prop.name, EditorGUILayout.Vector4Field(prop.name, script.material.GetVector(prop.name)));
                        break;
                    case MaterialProperty.PropType.Float:
                        script.material.SetFloat(prop.name, EditorGUILayout.FloatField(prop.name, script.material.GetFloat(prop.name)));
                        break;
                    case MaterialProperty.PropType.Range:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(prop.name);
                        script.material.SetFloat(prop.name, EditorGUILayout.Slider(script.material.GetFloat(prop.name), prop.rangeLimits.x, prop.rangeLimits.y));
                        EditorGUILayout.EndHorizontal();
                        break;
                    case MaterialProperty.PropType.Texture:
                        GUILayout.BeginVertical();
                        var style = new GUIStyle(GUI.skin.label);
                        style.alignment = TextAnchor.UpperCenter;
                        style.fixedWidth = 70;
                        GUILayout.Label(prop.name, style);
                        var result = (Texture2D)EditorGUILayout.ObjectField(prop.textureValue, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.EndVertical();

                        script.material.SetTexture(prop.name, result);
                        break;
                }

            }

            //EditorGUILayout.HelpBox("This is a help box", MessageType.None);
        }

    }
    #endif
}