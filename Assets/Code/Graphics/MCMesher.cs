using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxel;
using Voxel.Graphics;
using Voxel.Terrain;
using Voxel.Volumes;

namespace Voxel.Graphics
{
    public class MCMesher : IMesher
    {
        public void GenerateMesh(IVoxelDataSource<VoxelData> source, Bounds bounds, out SimpleMesh mesh)
        {
            mesh = new SimpleMesh {RenderMaterial = Chunk.mcMaterial};
            // Create a new bounds object for the volume to work with
            Bounds volume = new Bounds();
            volume.SetMinMax(bounds.min, bounds.max + new Vector3(1, 1, 1));
            VoxelData[, ,] data = source.SampleSpace(volume);
            int index;

            IEnumerable<int> distinctElements = data.Cast<int>().Where(i => i > 0).Distinct();

            int numTypes = distinctElements.Count();
            Dictionary<int, int> indices = new Dictionary<int, int>();

            
            for (int i = 0; i < numTypes; i++)
            {
                int blockID = distinctElements.First();

                SimpleMesh submesh = new SimpleMesh {RenderMaterial = Game.BlockRegistry[blockID].Data.RenderMaterial};
                mesh.Submeshes.Add(submesh);

                indices[blockID] = i;
                distinctElements = distinctElements.Skip(1);
            }

            SimpleMesh s = new SimpleMesh {RenderMaterial = Game.BlockRegistry[1].Data.RenderMaterial};
            mesh.Submeshes.Add(s);


            int[] idxs = new int[numTypes + 1];


            for (int x = 0; x < volume.size.x - 1; x++)
            {
                for (int y = 0; y < volume.size.y - 1; y++)
                {
                    for (int z = 0; z < volume.size.z - 1; z++)
                    {
                        int cubeIndex = 0;
                        if (data[x, y, z] != 0)
                            cubeIndex |= 16;
                        if (data[x + 1, y, z] != 0)
                            cubeIndex |= 32;
                        if (data[x, y + 1, z] != 0)
                            cubeIndex |= 1;
                        if (data[x + 1, y + 1, z] != 0)
                            cubeIndex |= 2;
                        if (data[x, y, z + 1]!= 0)
                            cubeIndex |= 128;
                        if (data[x + 1, y, z + 1] != 0)
                            cubeIndex |= 64;
                        if (data[x, y + 1, z + 1] != 0)
                            cubeIndex |= 8;
                        if (data[x + 1, y + 1, z + 1] != 0)
                            cubeIndex |= 4;


                        int inx = data[x, y, z];
                        if (inx <= 0)
                        {
                            int dx = (cubeIndex & 32) == 32 ? 1 : 0;
                            int dy = (cubeIndex & 1) == 1 ? 1 : 0;
                            int dz = (cubeIndex & 128) == 128 ? 1 : 0;
                            
                            inx = data[x + dx, y + dy, z + dz];
                        } 
                        SimpleMesh m = mesh.Submeshes[numTypes];
                        int l = numTypes;
                        if (inx > 0)
                        {
                            m = mesh.Submeshes[indices[inx]];
                            l = indices[inx];
                        }


                        
                        int edgeVal = GenericTable.MC_Edges[cubeIndex];
                        if (edgeVal == 0) continue;
                        Vector3[] points = new Vector3[12];

                        if ((edgeVal & 1) > 0)
                            points[0] = new Vector3(x + 0.5f, y + 1, z);
                        if ((edgeVal & 2) > 0)
                            points[1] = new Vector3(x + 1, y + 1, z + 0.5f);
                        if ((edgeVal & 4) > 0)
                            points[2] = new Vector3(x + 0.5f, y + 1, z + 1);
                        if ((edgeVal & 8) > 0)
                            points[3] = new Vector3(x, y + 1, z + 0.5f);
                        if ((edgeVal & 16) > 0)
                            points[4] = new Vector3(x + 0.5f, y, z);
                        if ((edgeVal & 32) > 0)
                            points[5] = new Vector3(x + 1, y, z + 0.5f);
                        if ((edgeVal & 64) > 0)
                            points[6] = new Vector3(x + 0.5f, y, z + 1);
                        if ((edgeVal & 128) > 0)
                            points[7] = new Vector3(x, y, z + 0.5f);
                        if ((edgeVal & 256) > 0)
                            points[8] = new Vector3(x, y + 0.5f, z);
                        if ((edgeVal & 512) > 0)
                            points[9] = new Vector3(x + 1, y + 0.5f, z);
                        if ((edgeVal & 1024) > 0)
                            points[10] = new Vector3(x + 1, y + 0.5f, z + 1);
                        if ((edgeVal & 2048) > 0)
                            points[11] = new Vector3(x, y + 0.5f, z + 1);
                            
                        short[] tris = GenericTable.MC_Triangles[cubeIndex];                                                   

                        for (int i = 0; tris[i] != -1; i += 3)
                        {
                            Vector2 uv1 = new Vector2(points[tris[i]].x, points[tris[i]].z);
                            Vector2 uv2 = new Vector2(points[tris[i + 2]].x, points[tris[i + 2]].z);
                            Vector2 uv3 = new Vector2(points[tris[i + 1]].x, points[tris[i + 1]].z);
                                
                            index = idxs[l];
                            m.Vertices.Add(points[tris[i]]);
                            m.Vertices.Add(points[tris[i + 2]]);
                            m.Vertices.Add(points[tris[i + 1]]);
                            m.UV1.Add(uv1);
                            m.UV1.Add(uv2);
                            m.UV1.Add(uv3);
                            m.Triangles.Add(index);
                            m.Triangles.Add(index + 1);
                            m.Triangles.Add(index + 2);
                            idxs[l] += 3;
                               
                        }
                    }
                }
            }
        }
    }
}
