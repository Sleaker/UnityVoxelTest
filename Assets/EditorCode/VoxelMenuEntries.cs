using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Voxel.Editor
{
    public class VoxelMenuEntries : MonoBehaviour
    {

        [MenuItem("Voxel/New Voxel World")]
        static void CreateVoxelWorld()
        {
            CreateWorldWizard.CreateWizard();
        }

        [MenuItem("Voxel/Save Current World")]
        static void SaveVoxelWorld()
        {
        }

        [MenuItem("Voxel/Load Voxel World")]
        static void LoadVoxelWorld()
        {

            // Check if a world is already loaded into the current scene.

            // Get input for what values we want to generate (width, height, depth, generator etc)

            // Generate the terrain volume

        }
    }
}
