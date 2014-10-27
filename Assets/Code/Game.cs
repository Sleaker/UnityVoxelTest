using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Voxel.Blocks;
using Voxel.Volumes;
using Voxel.Terrain.Generator;
using Voxel.Graphics;

namespace Voxel
{
    public abstract class Game
    {
        private static readonly Dictionary<string, GameObject> LoadedWorlds = new Dictionary<string, GameObject>();
        public static readonly BlockRegistry BlockRegistry = new BlockRegistry();

        public static GameObject CreateWorld(string name, long seed, WorldGenerator<VoxelData> worldGenerator, IMesher mesher)
        {
            GameObject ob = new GameObject("World-" + name, typeof(Terrain.World));
            ob.transform.position = Vector3.zero;
            ob.transform.rotation = Quaternion.identity;

            Terrain.World world = ob.GetComponent<Terrain.World>();
            world.Seed = seed;
            world.Mesher = mesher;
            world.WorldGenerator = worldGenerator;
            world.GenerateWorld();

            LoadedWorlds[name] = ob;
            return ob;
        }

        public static void AddLoadedWorld(string name, GameObject world)
        {
            LoadedWorlds[name] = world;
        }

        public static GameObject CreateWorld(string name, WorldGenerator<VoxelData> WorldGenerator, IMesher mesher)
        {
            return CreateWorld(name, 0, WorldGenerator, mesher);
        }

       public static void TakeScreenshot()
        {
           if(!System.IO.Directory.Exists("Screenshots/")) System.IO.Directory.CreateDirectory("Screenshots/");
           string filename = string.Format("Screenshots/Screenshot-{0}.png", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
           Application.CaptureScreenshot(filename);
        }

        public static GameObject GetWorld(string name)
        {
            GameObject go = null;
            LoadedWorlds.TryGetValue(name, out go);
            return go;
        }

        public static List<GameObject> GetLoadedWorlds()
        {
            return LoadedWorlds.Values.ToList();
        }
    }
}
