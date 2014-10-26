using System.Collections.Generic;
using UnityEngine;
using Voxel.Volumes;

namespace Voxel.Graphics
{
    public class SimpleMesh
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector2> UV1 = new List<Vector2>();
        public List<Vector2> UV2 = new List<Vector2>();
        public List<int> Triangles = new List<int>();
        public List<Color> Colors = new List<Color>();
        public List<Vector3> Normals = new List<Vector3>();

        public List<SimpleMesh> Submeshes = new List<SimpleMesh>();

        public Material RenderMaterial;

        public void RecalculateNormals()
        {
            Normals.Clear();
            for (int i = 0; i < Vertices.Count; i++)
            {
                Normals.Add(Vector3.zero);
            }

            for (int i = 0; i < Triangles.Count; i += 3)
            {
                Vector3 p1 = Vertices[i];
                Vector3 p2 = Vertices[i + 1];
                Vector3 p3 = Vertices[i + 2];

                Vector3 normal = Vector3.Cross(p1 - p2, p2 - p3);

                Normals[i] = Normals[i] + normal;
                Normals[i + 1] = Normals[i + 1] + normal;
                Normals[i + 2] = Normals[i + 2] + normal;
            }

            for (int i = 0; i < Normals.Count; i++)
            {
                Normals[i].Normalize();
            }
        }

        public Mesh CreateMesh()
        {
            Mesh m = new Mesh
            {
                vertices = Vertices.ToArray(),
                uv = UV1.ToArray(),
                uv2 = UV2.ToArray(),
                triangles = Triangles.ToArray(),
                colors = Colors.ToArray()
            };

            if(Submeshes.Count > 0)
            {
                CombineInstance[] instances = new CombineInstance[Submeshes.Count];
                for(int i = 0; i < Submeshes.Count; i++)
                {
                    CombineInstance ins = new CombineInstance
                    {
                        mesh = Submeshes[i].CreateMesh(),
                        transform = Matrix4x4.identity
                    };

                    instances[i] = ins;
                    
                }
                m.CombineMeshes(instances, false);
            }

            
            m.RecalculateNormals();
            m.RecalculateBounds();
            m.Optimize();

            return m;
        }
    }


    public interface IMesher
    {
        /// <summary>
        /// Generates a SimpleMesh for the given Voxel Data Source.
        /// </summary>
        /// <param name="source">Source field of voxels.</param>
        /// <param name="meshArea">Area to build the mesh.</param>
        /// <param name="mesh">Output mesh</param>
        void GenerateMesh(IVoxelDataSource<VoxelData> source, Bounds meshArea, out SimpleMesh mesh);
    }
}
