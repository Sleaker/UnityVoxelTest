using UnityEditor;
using UnityEngine;
using System.Collections;
using Voxel.Volumes;
using Voxel.Terrain;
using Voxel.Graphics;
using Voxel.Terrain.Generator;
using Voxel.Blocks;

namespace Voxel.Editor
{
    public class CreateWorldWizard : ScriptableWizard
    {

        private string worldName = "New World";
        private VolumeType type = VolumeType.Block;
        private string seed = "0";

        public static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<CreateWorldWizard>("Create World");
        }

        void OnCreateButton()
        {
            Close();
            BlockRegistry.LoadBlockType(new Grass());
            BlockRegistry.LoadBlockType(new Dirt());
            BlockRegistry.LoadBlockType(new Cobblestone());
            BlockRegistry.LoadBlockType(new MossyCobblestone());
            BlockRegistry.LoadBlockType(new Stone());

            GameObject ob = new GameObject("World-" + worldName, typeof(World));
            ob.transform.position = Vector3.zero;
            ob.transform.rotation = Quaternion.identity;
            World world = ob.GetComponent<World>();
            world.Seed = seed.GetHashCode();
            switch (type)
            {
                case VolumeType.MarchingCubes:
                    world.Mesher = new MCMesher();
                    break;
                case VolumeType.Block:
                default:
                    world.Mesher = new BlockMesher();
                    break;
            }
            world.EditMode = true;
            world.WorldGenerator = new GroundGenerator();
            world.GenerateWorld();

            // Load the world into the plugin API
            ModEngineEditor.LoadedWorld = ob;
        }

        void OnCancelButton()
        {
            Close();
        }

        void OnGUI()
        {
            GUIStyle textBlock = new GUIStyle(GUI.skin.label);
            textBlock.wordWrap = true;
            textBlock.alignment = TextAnchor.MiddleCenter;

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("To create a new world choose the world name, type, and seed and click create.", textBlock);
            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("World Name:", GUILayout.Width(80));
            worldName = EditorGUILayout.TextField(worldName, EditorStyles.textField, GUILayout.Width(150));
            GUILayout.Space(10);
            GUILayout.Label("Seed:", GUILayout.Width(50));
            seed = EditorGUILayout.TextField(seed, EditorStyles.textField, GUILayout.Width(50));
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("World Type:", GUILayout.Width(80));
            type = (VolumeType)EditorGUILayout.EnumPopup(type, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(40);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Create World", GUILayout.Width(120)))
            {
                OnCreateButton();
            }
            GUILayout.Space(40);
            if (GUILayout.Button("Cancel", GUILayout.Width(120)))
            {
                OnCancelButton();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

        }
    }
}