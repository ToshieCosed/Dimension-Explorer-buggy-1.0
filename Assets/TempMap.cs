using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace Assets
{
    class TempMap
    {
        string[] File_Loaded;
        int[,] TileMap;
        public void LoadMap(string filename)
        {
            try
            {
                File_Loaded = File.ReadAllLines(filename);
            }
            catch
            {
                throw new FileLoadException();
            }


        }

        public Vector2Int GetLoadedRegionSize()
        {
            string[] sizes = File_Loaded[0].Split(',');
            int size_x = Convert.ToInt32(sizes[0]);
            int size_y = Convert.ToInt32(sizes[1]);
            return new Vector2Int(size_x, size_y);
        }

        //Requires explicit size statement now.
        public TempMap(int width, int height)
        {
            this.TileMap = new int[width, height];
        }

        //Allow getting the 2d tilemap 
        public int[,] GetTileMapArray()
        {
            return TileMap;
        }

        public void ConvertTo2DMap(int width, int height)
        {
            if (!(File_Loaded == null))
            {
                //This will go row by row, line by line reading tiles in
                for (int Line = 1; Line < File_Loaded.Length; Line++)
                {
                    //Split the line into comma seperated array
                    string[] splitted = File_Loaded[Line].Split(',');
                    for (int y = 1; y < height; y++)
                    {

                       
                        try
                        {
                            TileMap[Line - 1, y] = Convert.ToInt32(splitted[y]);
                            Debug.Log("The value of splitted[y] is " + splitted[y]);
                            Debug.Log("Y is equal to " + y);
                        }
                        catch
                        {
                            ;
                        }
                    }
                }


            }
        }
    }
}

        
    

    
