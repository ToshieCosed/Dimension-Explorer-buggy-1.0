using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    public class SpriteDrawTileClass
    {
        public SpriteMetaData metadata;
        public struct SpriteMetaData
        {
            public SpriteMetaData(int id, int map_ptr, int locx, int locy)
            {
                SprID = id; MapPtr = map_ptr; Locx = locx; Locy = locy;
            }
            int SprID;
            int MapPtr;
            int Locx;
            int Locy;
        }
        public SpriteDrawTileClass(int spriteID, int Map_Ptr, int locationx, int locationy)
        {
            metadata = new SpriteMetaData(spriteID, Map_Ptr, locationx, locationx);
        }

        public void DrawOnMap()
        {
            //call your draw map function with the stuff
        }

    }

    public class MapSpriteGroupContainer
    {
        public int MapID { get; set; }
        public List<SpriteDrawTileClass> SpriteGroup = new List<SpriteDrawTileClass>();

        public MapSpriteGroupContainer(int MapID_)
        {
            MapID = MapID_;
        }

        public void Add_Sprite(SpriteDrawTileClass spr)
        {
            SpriteGroup.Add(spr);
        }

        public void Draw_All()
        {
            foreach(SpriteDrawTileClass BatchedSprite in SpriteGroup)
            {
                BatchedSprite.DrawOnMap();
            }
        }

    }
}
