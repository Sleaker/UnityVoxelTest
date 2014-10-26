using System.Collections.Generic;
using System.Linq;
using ThreadDispatcher;
using UnityEngine;
using Voxel.Behavior;
using Voxel.Blocks;
using Voxel.Graphics;
using Voxel.Volumes;


namespace Voxel.Terrain
{
    [ExecuteInEditMode]
    public class Chunk : ModBehavior
    {
        IMesher mesher = null;
        public IMesher Mesher
        {
            get
            {
                return mesher;
            }
            set
            {
                mesher = value;
                BuildChunk();
            }
        }

        public static Material mcMaterial;
       
        MeshFilter filter;

        internal IVoxelDataSource<VoxelData> dataSource;
        internal Bounds bounds;

        public bool EditMode
        {
            get;
            set;
        }

        public Block GetBlockAt(Vector3 position)
        {
            World parent = transform.parent.gameObject.GetComponent<World>();
            Block b = parent.GetBlockAt(position);
            return b;

        }
        public void ChangeBlock(Vector3 position, Block block)
        {
            World parent = transform.parent.gameObject.GetComponent<World>();
            parent.ChangeBlock(position, block);
        }


        public void Awake()
        {            
            MeshRenderer r = gameObject.AddComponent<MeshRenderer>();
            r.material = Resources.Load("ChunkMaterial") as Material;
            mcMaterial = Resources.Load("MCMaterial") as Material;
            filter = gameObject.AddComponent<MeshFilter>();           
            gameObject.AddComponent<MeshCollider>();
            this.tag = "Chunk";
        }

        public void BuildChunk()
        {
            if (!EditMode)
            {
                UnityThreadHelper.TaskDistributor.Dispatch(do_BuildChunk);
            }
            else
            {
                do_BuildChunk();
            }
        }
        
        private void do_BuildChunk()
        {
            SimpleMesh mesh;

            if (mesher == null) mesher = new BlockMesher();

            mesher.GenerateMesh(dataSource, bounds, out mesh);

            if (!EditMode)
            {
                UnityThreadHelper.Dispatcher.Dispatch(() => ChunkRecieved(mesh));
            }
            else
            {
                ChunkRecieved(mesh);
            }
        }

        public void ChunkRecieved(SimpleMesh mesh)
        {

            MeshCollider collider = (MeshCollider)this.collider;
            Mesh oldmesh = filter.sharedMesh;

            filter.sharedMesh = null;
            collider.sharedMesh = null;

            if (oldmesh != null) Mesh.Destroy(oldmesh);

            Mesh m = mesh.CreateMesh();  

            MeshRenderer r = GetComponent<MeshRenderer>();
           
            if(mesh.Submeshes.Count > 0)
            {
                r.sharedMaterials = mesh.Submeshes.Select(me => me.RenderMaterial).ToArray();
            }
            else
            {
                r.sharedMaterial = mesh.RenderMaterial;
            }

            filter.sharedMesh = m;           
            collider.sharedMesh = m;                        
        }
    }
}
