using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Voxel;
using Voxel.Blocks;
using Voxel.Graphics;
using Voxel.Player;
using Voxel.Terrain;
using Voxel.Terrain.Generator;
using System;

namespace Voxel
{
    public class BlockGame
    {
        public void Start()
        {
            Screen.lockCursor = true;
            GameObject o = Game.CreateWorld("Ground World", new GroundGenerator(), new BlockMesher());
            World w = o.GetComponent<World>();
            GameObject player = new GameObject("Player");
            //player.AddComponent<Equipment>();
            //player.AddComponent<UI>();
            player.transform.position = new Vector3((16 * 16) / 2, 20, (16 * 16) / 2);
            PlayerController pc = player.AddComponent<PlayerController>();
            pc.World = w;
        }

        public void Load()
        {
            BlockRegistry.LoadBlockType(new Grass());
            BlockRegistry.LoadBlockType(new Dirt());
            BlockRegistry.LoadBlockType(new Cobblestone());
            BlockRegistry.LoadBlockType(new MossyCobblestone());
            BlockRegistry.LoadBlockType(new Stone());
            Game.BlockRegistry.RegistrationComplete();
            Debug.Log("Finished loading assets");

        }

        public void Shutdown()
        {
            
        }
    }
}
