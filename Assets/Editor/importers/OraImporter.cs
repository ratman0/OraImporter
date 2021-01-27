﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
using static com.szczuro.importer.ora.OraImporter;

namespace com.szczuro.importer.ora
{
    [ScriptedImporter(1, "ora")]
    public class OraImporter : ScriptedImporter
    {
        public enum ImportType
        {
            Single,
            Multi
        }

        [SerializeField] public ImportType ImportAs = ImportType.Single;
        private OraFile _oraFile;
        

        #region ScriptedImporter implementation

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null) return;

            var path = ctx.assetPath;
            Debug.Log($"Importing Ora Object {path}");
            var fileInfo = new FileInfo(path);
            
            //file helper
            Debug.Log($"Create Orafile {_oraFile}");
            _oraFile = new OraFile(path);
            

            // Register root prefab that will be visible in project window instead of file
            var filePrefab = registerMainPrefab(ctx, fileInfo.Name, _oraFile.getThumbnailSprite().texture);
            
            // storage place for sprites  
            Debug.Log("Create spritelib");
            var spritesLib = _oraFile.getLayers();
            Debug.Log($"SpriteLib length {spritesLib.Count}");

            //ctx.AddObjectToAsset("spriteLib", spritesLib);
            Debug.Log($"add spriteRenderers to prefab");
            addSpritesToPrefab(ctx, filePrefab, spritesLib);
            
            Debug.Log($"set main prefab");
            ctx.SetMainObject(filePrefab);
        }

        private void addSpritesToPrefab(AssetImportContext ctx, GameObject filePrefab, List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                var texGO = new GameObject(sprite.name);
                var spriteRenderer = texGO.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                Debug.Log(spriteRenderer.sprite);
                ctx.AddObjectToAsset(sprite.name,sprite);
                //texGO.transform.SetParent(filePrefab.transform);
            }
        }
        
        private static GameObject registerMainPrefab(AssetImportContext ctx, string name, Texture2D thumbNail)
        {
            
            var filePrefab = new GameObject($"{name}_GO");
            ctx.AddObjectToAsset("main", filePrefab, thumbNail);
            
            return filePrefab;
        }

        #endregion
    }

    [CustomEditor(typeof(OraImporter))]
    public class OraImporterEditor : ScriptedImporterEditor
    {
        private SerializedProperty ImportAs;

        public override void OnEnable()
        {
            ImportAs = serializedObject.FindProperty("ImportAs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            EditorGUI.showMixedValue = ImportAs.hasMultipleDifferentValues;
            ImportAs.intValue = EditorGUILayout.IntPopup(GUITexts.ImportTypeTitle,  ImportAs.intValue, 
                GUITexts.ImportTypeOptions, GUITexts.ImportTypeValues);
            EditorGUI.showMixedValue = false;

            switch ((ImportType)ImportAs.intValue)
            {
                case ImportType.Multi :
                    EditorGUILayout.LabelField("Layers");
                    MultiLayerImportGUI();
                    break;;
                case ImportType.Single:
                default:
                    EditorGUILayout.LabelField("Single");
                    SingleImageImportGUI();
                    //ImportAs.intValue = (int) ImportType.Single;
                    break;
                    
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void SingleImageImportGUI()
        {
            //throw new System.NotImplementedException();
        }

        private void MultiLayerImportGUI()
        {
            //throw new System.NotImplementedException();
        }
    }

    internal static class GUITexts
    {
        public static GUIContent ImportTypeTitle = new GUIContent("Texture Type", "What will this texture be used for?");
        public static GUIContent[] ImportTypeOptions =
        {
            new GUIContent("Merged", "This is the default Texture Type usage when an image is imported without a specific Texture Type selected"),
            new GUIContent("Layers", "Texture is imported as layers."),
        };
        public static int[] ImportTypeValues =
        {
            (int) ImportType.Single,
            (int) ImportType.Multi
        };
    }

}