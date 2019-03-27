using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class TileSet
    {
        private static int TileWidth_ = 32;
        private static int TileHeight_ = 32;
        private static int tile_countx_ = 10;
        private static int tile_county_ = 14;
        public Texture2D TileImage = new Texture2D(tile_countx_ * TileWidth_, tile_county_ * TileHeight_, TextureFormat.RGB24, false);
        public Texture2D[] TileSetArray = new Texture2D[tile_countx_ * tile_county_]; //Our list of tile textures
        private GameObject[,] DisplayMap;
        int seperation = 0;
        int sepdir = 0;
        public int scrollx = 0;
        public int scrolly = 0;
        int TargetTile { get; set; }
        static TempMap map;
        //cannot use numbers at the start of a class instance --odd.
        int[,] DimensionalMap;
        int[,] tileIdMap = new int[32, 32];
        private GameObject Temp_parent;
        List<FireTest> followers = new List<FireTest>();
        float target_x = 0;
        float target_y = 0;
        private float Offset_X = 0;
        private float Offset_Y = 0;
        int area_width_ = 0;
        int area_height_ = 0;

        //Todo, move drawing code to a map renderer class instead
        //Todo use this only for rendering the current TileMap to that renderer class. Somehow.
        public void SetMap(int[,] InputMap)
        {
            DimensionalMap = InputMap;
        }

        public TileSet(Texture2D TileImage_, int offsetx, int offsety, int area_width, int area_height) //Fill it with a constructor =p
        {

            //Texture2D TestTex = new Texture2D(16 * 20, 16 * 20, TextureFormat.ARGB32, false);
            //Apparently we have to create the textures as well
            InitTileMapTextures();
            //Debug.Log(TileImage.GetType().ToString());
            this.TileImage = TileImage_;
            SplitTilesLocally(TileImage_, 32, 32);
            SetTileIdMap();
            map = new TempMap(area_width, area_height);
            DimensionalMap = new int[area_width, area_height];
            DisplayMap = new GameObject[area_width, area_height];
            this.area_width_ = area_width;
            this.area_height_ = area_height;


        }

        public void SetTileIdMap()
        {
            int id = 0;
            for (int y = 1; y < 32; y++)
            {
                for (int x = 1; x < 32; x++)
                {
                    tileIdMap[x, y] = id;
                    id++;
                }
            }
        }



        public void destroyallsprites()
        {
            GameObject.Destroy(Temp_parent);

            foreach (GameObject obj in DisplayMap)
            {
                GameObject.Destroy(obj.gameObject);

            }
        }

        public void InitTileMapTextures()
        {
            for (int i = 0; i < 140; i++)
            {
                //Todo~ Investigate not being forced to use this format

                if (!(TileSetArray[i] = null)) {
                    TileSetArray[i] = new Texture2D(32, 32);
                }
            }
        }

        public void CheckClick()
        {
            foreach (GameObject obj in DisplayMap)
            {
                Sprite tempsp = obj.GetComponent<SpriteRenderer>().GetComponent<Sprite>();
            }
        }

        public int GetTileByID(int id)
        {
            int id_ = 0;
            bool found = false;
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {

                    if (tileIdMap[x, y] == id)
                    {
                        id_ = id;
                        return id_;
                    }
                }
            }
            //we haven't found crap so it must not exist
            id_ = -1;
            return id_;
        }


        //Only needs once per tilemap display
        public void InitTileMapForDraw(int offsetx, int offsety)
        {
            //Note:
            //As of now, requires external SetMap to be run first.


            //And like magic we should have re-factored map loading -- hopefully.
            //map.LoadMap("Assets/map.txt");
            //map.ConvertTo2DMap();
            //DimensionalMap = map.GetTileMapArray();
            int tileid = 0;
            int changex = 10;
            int changey = 10;
            DimensionalMap[changex, changey] = TargetTile;
            TargetTile++;
            if (TargetTile > 140 - 1) { TargetTile = 0; }

            this.Temp_parent = new GameObject();
            Temp_parent.name = "TileMap_Parent";

            for (int y = 0; y < this.area_height_; y++)
            {
                for (int x = 0; x < this.area_width_; x++)
                {
                    //Renderer rend = new Renderer();
                    //rend.material.mainTexture = TileSetArray[1];
                    int targx = x * 32;
                    int targy = y * 32;
                    this.DisplayMap[x, y] = new GameObject();
                    DisplayMap[x, y].name = "MapTile";
                    DisplayMap[x, y].transform.SetParent(Temp_parent.transform);

                    Debug.Log("Area width is " + this.area_width_);

                    //==TileEater==e was g
                    //====Part of tile eater effect, turn this off -- mostly. Please. XD
                    //DisplayMap[x, y].AddComponent<FireTest>();
                    //UpdateSpawnTestFire(DisplayMap[x, y]);
                    //==================================================================

                    tileid = DimensionalMap[x, y];
                    //tileid = GetTileByID(tileid);
                    //int TNum = GetTileByID(tile);
                    //Here we are getting the Tile by ID which my editor would use
                    //since the indexing is not the same at all.
                    if (tileid > 140 - 1) { tileid = 140 - 1; }
                    // if (TNum> TileSetArray.Length) { TNum = 0; }
                    Sprite newsprite = Sprite.Create(TileSetArray[tileid], new Rect(0, 0, 32, 32), new Vector2(0, 0));
                    SpriteRenderer renderer = DisplayMap[x, y].AddComponent<SpriteRenderer>();
                    renderer.sprite = newsprite;
                    //Debug.Log("Adding the renderer yeah");
                    int targetx = x + offsetx;
                    int targety = y + offsety;
                    //DisplayMap[x, y].GetComponent<SpriteRenderer>().material.mainTexture = TileSetArray[UnityEngine.Random.Range(0, 100)];
                    DisplayMap[x, y].GetComponent<SpriteRenderer>().transform.SetPositionAndRotation(new Vector3(targetx * 0.32f, targety * 0.32f, 0), Quaternion.Euler(0, 0, 0));

                    //First test for solid collisions, just testing.
                    //Todo refactor into collision class somehow later.
                    if (DimensionalMap[x, y] == 3)
                    {
                        DisplayMap[x, y].AddComponent<BoxCollider2D>();
                        DisplayMap[x, y].transform.name = "Solid_Tile";
                    }
                }
            }
        }
    

            
        

        public void SetNewPos(int offsetx, int offsety)
        {
            for (int x = 0; x < this.area_width_; x++)
            {
                for (int y = 0; y < this.area_height_; y++)
                {
                    int targetx = x + offsetx;
                    int targety = y + offsety;
                    //DisplayMap[x, y].GetComponent<SpriteRenderer>().material.mainTexture = TileSetArray[1];
                    DisplayMap[x, y].GetComponent<SpriteRenderer>().transform.SetPositionAndRotation(new Vector3(targetx * 0.32f, targety * 0.32f, 0), Quaternion.Euler(0, 0, 0));
                    scrollx = offsetx; scrolly = offsety;
                }
            }
        }

        //Utilized for larger maps, not fully tested yet.
        public void SetNewUnsafePos(float offsetx, float offsety, int Area_Width, int Area_Height)
        {
            for (int x = 0; x < Area_Width; x++)
            {
                for (int y = 0; y<Area_Height; y++)
                {
                    float target_x = x + offsetx;
                    float target_y = y + offsety;
                    DisplayMap[x, y].GetComponent<SpriteRenderer>().transform.SetPositionAndRotation(new Vector3(target_x * 0.32f, target_y * 0.32f, 0), Quaternion.Euler(0, 0, 0));
                   
                }
            }
        }

        public Vector2 GetFloatingOffSetPos()
        {
            return new Vector2(this.Offset_X, this.Offset_Y);
        }

        public void SetPosInternally()
        {
            SetNewPos(scrollx, scrolly);
        }

        public void ScrollTileMap(float x, float y)
        {
            foreach(GameObject g in DisplayMap)
            {
                if (!(g == null))
                {
                    Vector3 pos = g.transform.position;
                    pos.x += x;
                    pos.y += y;
                    g.transform.position = pos;
                }
            }
        }

        public void set_target(float x, float y)
        {
            target_x = x;
            target_y = y;
        }

        public void UpdateSpawnTestFire(GameObject temp_object)
        {
            float thisx = temp_object.transform.position.x;
            float thisy = temp_object.transform.position.y;
            //GameObject obj = Instantiate(testfireball);

            float posx = UnityEngine.Random.Range(thisx - 1f, thisx + 1f);
            float posy = UnityEngine.Random.Range(thisy - 1f, thisy + 1f);
            Vector2 SpawnPos = new Vector2(posx, posy);
            temp_object.transform.position = SpawnPos;
            // temp_object.SetTarget(thisx, thisy);
            temp_object.GetComponent<FireTest>().SetTarget(target_x, target_y);
            //SpriteRenderer r = temp_object.GetComponent<SpriteRenderer>();
            //r.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
            // obj.transform.TransformDirection(Vector3.forward * 10);
            //Sprite s = obj.GetComponent<SpriteRenderer>().sprite;
            //s = Sprite.Create(tset.TileSetArray[Random.Range(0, 100)], new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            //obj.GetComponent<SpriteRenderer>().sprite = s;
            followers.Add(temp_object.GetComponent<FireTest>());
        }

        public void ManageFollowers()
        {
            float thisx = this.target_x;
            float thisy = this.target_y;
            foreach (FireTest f in followers)
            {
                f.UpdateTarget(thisx, thisy);
            }
        }

        private Texture2D Get_Tile_At(Texture2D source, int startx, int starty, int width, int height)
        {
            Color[] tempcol = source.GetPixels(startx, starty, width, height, 0);
            Texture2D temptex = new Texture2D(width, height, TextureFormat.RGB24, false);
            temptex.SetPixels(tempcol);
            //tempcol = source.GetPixels((startx), (starty), width, height, 1);
            //temptex.SetPixels(tempcol, 1);
            temptex.Apply();
            return temptex;
        }

        public void SplitTilesLocally(Texture2D TileImage_, int TileWidth, int TileHeight) //Fill it with a constructor =p
        {
            //Texture2D TestTex = new Texture2D(16 * 20, 16 * 20, TextureFormat.ARGB32, false);
            //Apparently we have to create the textures as well
            //InitTileMapTextures();
            //Debug.Log(TileImage.GetType().ToString());
            //this.TileImage = TileImage_;
            //int id = 0;
            int tile_countx = TileImage.width / TileWidth;
            int tile_county = TileImage.height / TileHeight;
            for (int y = 0; y < tile_county; y++)
            {
                for (int x = 0; x < tile_countx; x++)
                {
                    int index = (y * tile_countx) + x;
                    //get the position in the bigger image we are copying from.
                    int srcx = x * TileWidth;
                    int srcy = y * TileHeight;
                    
                    if (TileSetArray[index] != null && TileImage_ != null)
                    {
                        //Revision because for some reason Graphics.Copy messes everything up >.>
                        //Graphics.CopyTexture(TileImage_, 0, 0, srcx, srcy, 16, 16, TileSetArray[id], 0, 0, 0, 0);

                        srcy = (TileImage_.height - TileHeight) - srcy; //This fixes the texture not rendering properly 
                        //--Due to being UPSIDE DOWN in memory!! Well sort of. Y is always the bottom at 0.
                        Texture2D temptile = Get_Tile_At(TileImage_, srcx, srcy, TileWidth, TileHeight);

                        //CopyTextureToDest(TileImage_, srcx, srcy, 16, 16, temptile, 0, 0);

                            //Using an array for now will fix it to linear after.
                        this.TileSetArray[index] = temptile;
                        
                        //None of that stuff below worked!!
                        //this.TileSetArray[id].Apply(); //this could have been messing it up!!
                        //GUI.DrawTexture(new Rect((float)srcx, (float)srcy, 16, 16), TileSetArray[id]);
                    }
                    //id++; //iterate the ID for our texture array =p
                }
            }

        }

        public void DrawTileSetLocally(Texture2D TileImage_, int TileWidth, int TileHeight) //Fill it with a constructor =p
        {
            //Texture2D TestTex = new Texture2D(16 * 20, 16 * 20, TextureFormat.ARGB32, false);
            //Apparently we have to create the textures as well
            //InitTileMapTextures();
            //Debug.Log(TileImage.GetType().ToString());
            //this.TileImage = TileImage_;
            //int id = 0;
            //GUI.DrawTexture(new Rect(0, 0, 320, 320), TileImage_);
            int tile_countx = TileImage.width / TileWidth;
            int tile_county = TileImage.height / TileHeight;
            for (int y = 0; y < tile_county; y++)
            {
                for (int x = 0; x < tile_countx; x++)
                {

                    int index = (y * 10) +x;
                    //get the position in the bigger image we are copying from.
                    int srcx = x * TileWidth;
                    int srcy = y * TileHeight;
                    //function call signature is CopyTexture(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY);

                    //Debug.Log("The format of source texture is" + TileImage_.format.ToString() + " And the format of the dest is " + TileSetArray[id].format.ToString());

                    if (TileSetArray[index] != null && TileImage_ != null)
                    {
                        //Revision because for some reason Graphics.Copy messes everything up >.>
                        //Graphics.CopyTexture(TileImage_, 0, 0, srcx, srcy, 16, 16, TileSetArray[id], 0, 0, 0, 0);
                        Texture2D temp = TileSetArray[index];
                        GUI.DrawTextureWithTexCoords(new Rect(srcx, srcy, TileWidth, TileHeight), temp, new Rect(0f, 0f, 1f, 1f));
                       
                        //id++; //iterate the ID for our texture array =p
                        //GUI.DrawTexture(new Rect(0, 0, 320, 320), TileImage_);
                    }
                   
                }
            }
            

            
        }

        /* Do not use this. It is broken.
        public void CopyTextureToDest(Texture2D src, int srcx, int srcy, int srcwidth, int srcheight, Texture2D dest, int destx, int desty)
        {
            for (int x=0; x<16; x++)
            {
                for (int y=0; y<16; y++)
                {
                    Color pixel = src.GetPixel(srcx + x, srcy + y);
                    dest.SetPixel(x, y, pixel);
                }
            }
            dest.Apply();
        }
        */

       bool Seperator = true ; //Useless just to seperate

            /* This is also useless.
        public void TestDraw()
        {
            int id = 0;
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    //get the position in the bigger image we are copying from.
                    int srcx = x * 16;
                    int srcy = y * 16;
                    //The top left corner of our target tile! always 0, 0
                    int destx = 0;
                    int desty = 0;
                    //function call signature is CopyTexture(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY);

                    //Debug.Log("The format of source texture is" + TileImage.format.ToString() + " And the format of the dest is " + TileSetArray[id].format.ToString());

                    if (TileSetArray[id] != null)
                    {
                        GUI.DrawTexture(new Rect((float)srcx, (float)srcy, 16, 16), this.TileSetArray[id]);
                        //GUI.DrawTextureWithTexCoords(new Rect(srcx, srcy, 16, 16), TileSetArray[id], new Rect(0, 0, 16, 16));
                    }
                    id++; //iterate the ID for our texture array =p
                }
            }
        }
        */
    }
}
