using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace Assets
{
    class World
    {
        int current_x = 0;
        int current_y = 0;
        int LoadedAreaWidth = 0;
        int LoadedAreaHeight = 0;
        private int[,] LoadedMap;


        public void SetWorldXY(int x, int y)
        {
            this.current_x = x;
            this.current_y = y;
        }

        public Vector2Int LoadMapByCoords(int x, int y, int assumed_width, int assumed_height)
        {
            bool redo = false;
            this.LoadedMap = new int[assumed_width, assumed_height];
            TempMap MapLoader = new TempMap(assumed_width, assumed_height);
            if (Pos_Exist(x, y))
            {
                string filename = "Assets/" + x + "_" + y + ".txt";
                MapLoader.LoadMap(filename);
                Vector2Int RegionSize = MapLoader.GetLoadedRegionSize();
                if (RegionSize.x > assumed_width) { redo = true; }
                if (redo)
                {
                    Vector2Int FinalSize = LoadMapByCoords(x, y, RegionSize.x, RegionSize.y);
                    return FinalSize;
                }
                MapLoader.ConvertTo2DMap(assumed_width, assumed_height);
                LoadedMap = MapLoader.GetTileMapArray();
                this.LoadedAreaWidth = RegionSize.x;
                this.LoadedAreaHeight = RegionSize.y;
                return new Vector2Int(RegionSize.x, RegionSize.y);
            }
            else
                Debug.Log("Loading of map failed at" + x + ", " + y);
            return new Vector2Int(-1, -1);
        }

        public Vector2Int GetLoadedAreaSize()
        {
            return new Vector2Int(this.LoadedAreaWidth, this.LoadedAreaHeight);
        }

        public bool Pos_Exist(int x, int y)
        {
            string filename = "Assets/" + x + "_" + y + ".txt";
            if (File.Exists(filename)){
                return true;
            }
            else
            {
                return false;
            }
        }

        public int[,] GetLoadedMap()
        {
            return LoadedMap;
        }

        public void MoveEast()
        {
            current_x += 1;
            LoadMapByCoords(current_x, current_y, 20, 20);
        }

        public void MoveWest()
        {
            current_x -= 1;
            LoadMapByCoords(current_x, current_y, 20, 20);
        }

        public void MoveNorth()
        {
            current_y -= 1;
            LoadMapByCoords(current_x, current_y, 20, 20);
        }

        public void MoveSouth()
        {
            current_y += 1;
            LoadMapByCoords(current_x, current_y, 20, 20);
        }
    }
}
