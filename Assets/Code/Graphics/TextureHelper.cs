using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace Voxel.Graphics
{
    public static class TextureHelper
    {
        private static bool IsPOTTexture(int width, int height)
        {
            if (width != height) return false;
            return (width & (width - 1)) == 0;
        }

        private static int CalcFinalTextureWidth(float textureWidth, int numTextures, float copyPercent)
        {
            float kw = textureWidth * numTextures;
            float l = kw * (1 + 2 * copyPercent);

            int log2c = Mathf.CeilToInt(Mathf.Log(l, 2));

            return (int)Mathf.Pow(2, log2c);
        }

        public static int AtlasTextureSize(int numTextures, float copyPercent)
        {
            float m = Mathf.CeilToInt(Mathf.Log(numTextures * (1 + 2 * copyPercent), 2));
            return (int)Mathf.Pow(2, m);
        }

        public static float PackTexturesWithTiling(this Texture2D atlas, Texture2D[] textures, float copypercent, int maximumAtlasSize, bool makeNoLongerReadable)
        {
            atlas.name = "Atlas";
             int textureSize = textures[0].width;

            bool sameSize = textures.All(tex => tex.width == textureSize && tex.height == textureSize);
            if (!sameSize)
                throw new ArgumentException("All textures must be the same size to fit in this pallate.");

            int texturePalateX = Mathf.CeilToInt(Mathf.Sqrt(textures.Length));

            int k = textureSize * AtlasTextureSize(textures.Length / 2, copypercent);
          
            
            atlas.Resize(k, k);

            //Calculate texture width/height
            //int textureWidth = CalcFinalTextureWidth(textures[0].width, textures.Length / 2, copypercent);

            for(int i = 0; i < textures.Length; i++)
            {
                Texture2D tex = textures[i];
                int width = tex.width;
                int height = tex.height;
                
                if (!IsPOTTexture(width, height)) throw new ArgumentException("All textures must be power of 2");

                int copyPixels = (int)(width * copypercent);


                int x = (i % texturePalateX) * (width + (copyPixels * 2));
                int y = (i / texturePalateX) * (height + (copyPixels * 2));
                              
                Color[] left = tex.GetPixels(0, 0, copyPixels, height);
                Color[] right = tex.GetPixels(width - copyPixels, 0, copyPixels, height);
                Color[] top = tex.GetPixels(0, 0, width, copyPixels);
                Color[] bottom = tex.GetPixels(0, height - copyPixels, width, copyPixels);

                
                atlas.SetPixels(x,  y + copyPixels, copyPixels, height, right); // set the right set of copied pixels to the left side of the atlas
                atlas.SetPixels(x + copyPixels, y, width, copyPixels, bottom);// set the top set to the bottom copied pixels
                atlas.SetPixels(x + copyPixels, y + copyPixels, width, height, tex.GetPixels());
                atlas.SetPixels(x + copyPixels, y + height + copyPixels, width, copyPixels, top);
                atlas.SetPixels(x + width + copyPixels, y + copyPixels, copyPixels, height, left);


                //mirror the corners into their respective...corners
                Color[] topLeft = tex.GetPixels(0, 0, copyPixels, copyPixels);
                atlas.SetPixels(x + width + copyPixels, y + height + copyPixels, copyPixels, copyPixels, topLeft);

                Color[] bottomRight = tex.GetPixels(width - copyPixels, height - copyPixels, copyPixels, copyPixels);
                atlas.SetPixels(x, y, copyPixels, copyPixels, bottomRight);

                Color[] topRight = tex.GetPixels(width - copyPixels, 0, copyPixels, copyPixels);
                atlas.SetPixels(x, y + height + copyPixels, copyPixels, copyPixels, topRight);

                Color[] bottomLeft = tex.GetPixels(0, height - copyPixels, copyPixels, copyPixels);
                atlas.SetPixels(x + width + copyPixels, y, copyPixels, copyPixels, bottomLeft); 


            }
            return texturePalateX;

        }


        public static Rect[] MipCorrectPackTextures(this Texture2D atlas, Texture2D[] textures, int padding, int maximumAtlasSize, bool makeNoLongerReadable)
        {
            List<Rect> UVs = new List<Rect>();
            for (int i = 0; i < textures.Length; i++)
            {
                Texture2D tex = textures[i];
                int width = tex.width;
                int height = tex.height;

                Color[] colors = tex.GetPixels(0, 0, width, height);

                for (int m = 0 ; m < tex.mipmapCount; m++)
                {
                    //determine the x and y position
                    int xx = (0 >> m);
                    int yy = ((i * height) >> m);

                    atlas.SetPixels(xx, yy, width >> m, height >> m, colors, m);

                    colors = GenerateMipmap(width >> m, height >> m, colors);

                }

                float pixelConvertW = 1.0f/atlas.width;
                float pixelCovertH = 1.0f/atlas.height;
                

                float x = 0;
                float y = ((i * height) * pixelCovertH);
                float w = (width * pixelConvertW) - pixelConvertW;
                float h = (height * pixelCovertH) - pixelCovertH;
                Rect UV = new Rect(x,  y, w, h);
                UVs.Add(UV);
            }
                    
            return UVs.ToArray();

        }

        public static Rect[] MipCorrectPackTextures(this Texture2D atlas, Texture2D[] textures, int padding, int maximumAtlasSize)
        {
            return MipCorrectPackTextures(atlas, textures, padding, maximumAtlasSize, false);
        }

        public static Rect[] MipCorrectPackTextures(this Texture2D atlas, Texture2D[] textures, int padding)
        {
            return MipCorrectPackTextures(atlas, textures, padding, atlas.width);
        }

        private static Color Average(Color a, Color b, Color c, Color d)
        {
            float red = (a.r + b.r + c.r + d.r) / 4;
            float green = (a.g + b.g + c.g + d.g) / 4;
            float blue = (a.b + b.b + c.b + d.b) / 4;
            float alpha = (a.a + b.a + c.a + d.a) / 4;

            return new Color(red, green, blue, alpha);
            //return Color.red;
        }

        private static int Index(int x, int y, int scanwidth)
        {
            return x * scanwidth + y;
        }

        public static void GenerateMipmaps(this Texture2D texture)
        {
            Color[] colors = texture.GetPixels(0, 0, texture.width, texture.height);
            for (int i = 1; i < texture.mipmapCount; i++)
            {
                colors = GenerateMipmap(texture.width >> (i-1), texture.height >> (i-1), colors);
                texture.SetPixels(colors, i);
            }

        }

        private static Color[] GenerateMipmap(int width, int height, Color[] OriginalColor)
        {
            Color[] outcolor = new Color[(width / 2) * (height / 2)];

            for (int x = 0; x < width / 2; x++ )
            {
                for(int y = 0; y < height/ 2; y++)
                {                    
                    Color a = OriginalColor[Index(x * 2, y * 2, width)];  
                    Color b = OriginalColor[Index((x * 2) + 1, y * 2, width)];                    
                    Color c = OriginalColor[Index((x * 2), (y * 2) + 1, width)];                    
                    Color d = OriginalColor[Index((x * 2) + 1, (y * 2) + 1, width)];                    
                    outcolor[Index(x, y, width/2)] = Average(a, b, c, d);                   

                }

            }
            return outcolor;
        }
    }
}
