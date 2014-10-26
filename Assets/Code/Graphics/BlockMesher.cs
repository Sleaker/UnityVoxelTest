using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Voxel.Volumes;
using Voxel.Blocks;

namespace Voxel.Graphics
{
    public class BlockMesher : IMesher
    {

        public static Vector3[] FrontFace =
        {
            new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 0),   
        };

        public static Vector3[] BackFace =
        {
            new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 0, 1),    
        };

        public static Vector3[] RightFace =
        {
            new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 1), new Vector3(0, 0, 1),     
        };

        public static Vector3[] LeftFace =
        {
            new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0), new Vector3(1, 0, 0),        
        };

        public static Vector3[] TopFace =
        {
            new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(0, 1, 1), 
        };

        public static Vector3[] BottomFace =
        {
            new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1),
        };

        public static Vector3[][] Sides =
        {
            FrontFace,
            BackFace,
            RightFace,
            LeftFace,
            TopFace,
            BottomFace,
        };


        public static int[] FaceTriangles =
        {
            //FRONT FACE
            3, 1, 0,
            3, 2, 1,
        };



        public static Vector2[] FaceUVs =
        {
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), 
        };        

        private static void AddMesh(ref SimpleMesh mesh, int side, Vector3 offset, int blockId)
        {
            Vector3[] Face = Sides[side];

    

            foreach (Vector3 v in Face)
            {
                Vector3 position = v + offset;
                mesh.Vertices.Add(position);
            }           
            Block b = Game.BlockRegistry[blockId];

            float mat1yp = (byte)b.Data.TextureIDs[2] / 255.0f;
            float mat1yn = (byte)b.Data.TextureIDs[3] / 255.0f;

            Color c32;
            c32.r = (byte)b.Data.TextureIDs[0] / 255f; //xz+ xz-
            c32.g = (byte)0;         
            c32.b = 0; //Blend.  0 for now
            c32.a = 0;

         

            for (int i = 0; i < 4; i++)
            {
                mesh.Colors.Add(c32);
            }

            for(int i = 0; i < 4; i++)
            {
                mesh.UV1.Add(new Vector2(mat1yp, mat1yn));
                mesh.UV2.Add(new Vector2(0, 0));
            } 
           
       


            int meshcount = ((mesh.Vertices.Count / 4) - 1) * Face.Length;           
            foreach (int t in FaceTriangles)
            {
                mesh.Triangles.Add(t + meshcount);
            }
        }


        public void GenerateMesh(IVoxelDataSource<VoxelData> data, Bounds cellSize, out SimpleMesh mesh)
        {            
            mesh = new SimpleMesh();
            mesh.RenderMaterial = Game.BlockRegistry.AtlasMaterial;

            int xvol = (int)cellSize.size.x;
            int yvol = (int)cellSize.size.y;
            int zvol = (int)cellSize.size.y;

            int xoff = (int)cellSize.min.x;
            int yoff = (int)cellSize.min.y;
            int zoff = (int)cellSize.min.z;

            //Determine how many distinct blocks we have in this chunk
            //VoxelData[, ,] chunkData = data.SampleSpace(cellSize);     
                
            for (int x = 0; x < xvol; x++)
            {
                for (int y = 0; y < yvol; y++)
                {
                    for (int z = 0; z < zvol; z++)
                    {
                        int xx = xoff + x;
                        int yy = yoff + y;
                        int zz = zoff + z;

                        int id = data.Sample(xx, yy, zz);
                        if (id <= 0) continue;
                           
                        Vector3 vec = new Vector3(x, y, z);
                        //Add Back Face
                        if (data.Sample(xx, yy, zz + 1) <= 0)
                        {
                            AddMesh(ref mesh, 1, vec, id);
                                    
                        }
                        //Add Front Face
                        if (data.Sample(xx, yy, zz - 1) <= 0)
                        {
                            AddMesh(ref mesh, 0, vec, id);     
                        }
                        //Add Top Face
                        if (data.Sample(xx, yy + 1, zz) <= 0)
                        {
                            AddMesh(ref mesh, 4, vec, id); 
                        }

                        //Add Bottom Face
                        if (data.Sample(xx, yy - 1, zz) <= 0)
                        {
                            AddMesh(ref mesh, 5, vec, id);                                                                       
                        }

                        //Add Left Face
                        if (data.Sample(xx + 1, yy, zz) <= 0) 
                        {
                            AddMesh(ref mesh, 3, vec, id);   
                        }

                        //Add Right Face
                        if (data.Sample(xx - 1, yy, zz) <= 0)
                        {
                            AddMesh(ref mesh, 2, vec, id);                                    
                        }
                    }
                }
            }
        }
    }
}
