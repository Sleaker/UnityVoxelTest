using UnityEngine;
using System.Collections;
using System.Linq;
using Voxel.Volumes;
using Voxel.Graphics;
using ThreadDispatcher;

namespace Voxel.Behavior
{
    /// <summary>
    /// Basic component for rendering a Voxel Source.  
    /// Useful for any voxel based rendering.
    /// 
    /// Changes to the underlying Data Source requires that MakeDirty() be called to rebuild the mesh
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class VoxelRenderer : ModBehavior
    {

         MeshFilter filter;

        private IMesher mesher;
        /// <summary>
        /// Sets the Meshing technique for this object
        /// </summary>        
        public IMesher Mesher
        {
            get
            {
                return mesher;
            }
            set
            {
                mesher = value;
                MakeDirty();
            }
        }

        IVoxelDataSource<VoxelData> dataSource;

        /// <summary>
        /// Sets the Voxel Data Source that this renderer will mesh and render.
        /// </summary>
        public IVoxelDataSource<VoxelData> VoxelDataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                dataSource = value;
                MakeDirty();
            }
        }

        public Bounds RenderBounds
        {
            get;
            set;
        }

        /// <summary>
        /// Queues the renderer to rebuild the mesh and draw it.
        /// </summary>
        public void MakeDirty()
        {
            Rebuild();
        }

        private void Rebuild()
        {
            UnityThreadHelper.TaskDistributor.Dispatch(do_BuildMesh);
        }

        private void do_BuildMesh()
        {
            SimpleMesh Mesh;

            Mesher.GenerateMesh(dataSource, RenderBounds, out Mesh);

            UnityThreadHelper.Dispatcher.Dispatch(() => MeshDone(Mesh));
        }

        void Awake()
        {
            filter = GetComponent<MeshFilter>();
        }

        private void MeshDone(SimpleMesh mesh)
        {
            Mesh m = mesh.CreateMesh();

            MeshRenderer r = GetComponent<MeshRenderer>();

            if (mesh.Submeshes.Count > 0)
            {
                r.materials = mesh.Submeshes.Select(me => me.RenderMaterial).ToArray();
            }
            else
            {
                r.material = mesh.RenderMaterial;
            }

            filter.sharedMesh = m;
            MeshCollider collider = (MeshCollider)this.collider;
            if(collider != null) collider.sharedMesh = m;            
        }          
    }
}
