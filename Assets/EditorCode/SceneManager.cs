using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Voxel.Editor
{
    [InitializeOnLoad]
    public static class SceneLoader
    {
        static SceneLoader()
        {
            EditorApplication.playmodeStateChanged += OnPlayModeChanged;
        }

        // Check what kind of state change we are handling when the user pressed the play button
        private static void OnPlayModeChanged()
        {
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (!OnStartedPlaying())
                {
                    EditorApplication.isPlaying = false;
                }
            }
            else if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                OnStoppedPlaying();
            }
        }

        // Executed when the user starts playing, return false if the operation should cancel the playmode change
        private static bool OnStartedPlaying() 
        {
            // TODO: pop a save dialogue before loading into the game.
            if (ModEngineEditor.LoadedWorld)
            {
                // Do saving here if we have a world loaded
            }
            // Hide the loaded world in the editor.
            ModEngineEditor.HideWorld();
            return true;
        }

        // Executed when the user stops playing
        private static void OnStoppedPlaying()
        {
            // Re-activate any world in the editor.
            ModEngineEditor.ShowWorld();
        }
    }

    public static class ModEngineEditor
    {
        public static GameObject LoadedWorld
        {
            get;
            set;
        }

        public static void HideWorld()
        {
            if (LoadedWorld)
            {
                LoadedWorld.SetActive(false);
            }
        }

        public static void ShowWorld()
        {
            if (LoadedWorld)
            {
                LoadedWorld.SetActive(true);
            }
        }
    }
}
