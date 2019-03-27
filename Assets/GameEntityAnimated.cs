using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class GameEntityAnimated
    {
        
        GameObject SpriteObject;
        float x = 0; //Todo, update this to be named something more along the lines of originalspawnxy
        float y = 0;
        float current_x = 0;
        float current_y = 0;
        bool can_move = false;
        int move_counter = 1000;
        int internal_type = 0; //0 is tile or solid entity
        int internal_id = 0;
        float distance_x = 0;
        float distance_y = 0;
        float stepdistance_x = 0;
        float stepdistance_y = 0;
        float stepsleft = 0;
        bool alreadymoving = false;

        //Animation Variables for Animated GameEntity instance here
        bool AnimationRunning = false;
        int AnimationCurrentFrame = 0;
        int AnimationCurrentDelayStep = 0;
        int AnimationTotalDelaySteps = 0;
        int AnimationFrameAmount = 0;
        bool RepeatAnimation = false;
        bool despawned = false;
        Texture2D[] TexArray;
        

        public GameEntityAnimated(Texture2D EntitySpriteTex, float startingx, float startingy, float width, float height, int sortingorder, bool Solid, int intern_type, int intern_id) { 
            
            SpriteObject = new GameObject();
            SpriteObject.AddComponent<SpriteRenderer>();
            SpriteRenderer renderer = SpriteObject.GetComponent<SpriteRenderer>();
            Sprite newsprite = Sprite.Create(EntitySpriteTex, new Rect(0, 0, 0 + width, 0 + height), new Vector2(0, 0));
            renderer.sprite = newsprite;
            //Debug.Log("Adding the renderer yeah");
            //DisplayMap[x, y].GetComponent<SpriteRenderer>().material.mainTexture = TileSetArray[UnityEngine.Random.Range(0, 100)];
            SpriteObject.GetComponent<SpriteRenderer>().transform.SetPositionAndRotation(new Vector3(startingx, startingy, 0), Quaternion.Euler(0, 0, 0));
            renderer.sortingOrder = sortingorder;
            if (Solid)
            {
                SpriteObject.AddComponent<BoxCollider2D>();
                SpriteObject.name = "Solid_Entity";
            }

            if (intern_type == 1)
            {
                SpriteObject.name = "Solid_Entity";
            }
            //1 = closed chest
            //2 = open chest
            //3 = probably pushable blocks.
            //Just assign the internal ID and types which will be rolled upon creation or instantiaton.
            internal_type = intern_type;
            internal_id = intern_id;

            //Set the X/Y internally.
            x = startingx;
            y = startingy;

            Debug.Log("Created Sprite object with instance ID of " + SpriteObject.GetInstanceID());
            

        }

        // Function to check if the Animation is running or not
        public bool IsAnimationRunning()
        {
            return AnimationRunning;
        }

        public bool is_despawned()
        {
            return despawned;
        }
        //Runs the continuous animation on the entity
        public void RunContinuousFrameAnimation()
        {
            AnimationCurrentDelayStep += 1;
            if (AnimationCurrentDelayStep >= AnimationTotalDelaySteps)
            {
                AnimationCurrentDelayStep = 0;
                AnimationCurrentFrame += 1;
                if (AnimationCurrentFrame > AnimationFrameAmount)
                {
                    if (!RepeatAnimation)
                    {
                        AnimationRunning = false;
                        AnimationCurrentFrame = AnimationFrameAmount;
                        AnimationCurrentDelayStep = 0;
                        AnimationFrameAmount = 0;
                        AnimationTotalDelaySteps = 0;
                    }
                    else
                    {
                        AnimationCurrentFrame = 0;
                        AnimationCurrentDelayStep = 0;
                    }
                }

                
                //Update the sprite's texture in the array
                if (SpriteObject.GetComponent<SpriteRenderer>() == null) { Debug.Log("It's shitting itself"); }
                //Debug.Log(AnimationCurrentFrame + " Is the current frame");
                //if (TexArray[AnimationCurrentFrame] == null) { Debug.Log("The animation frame is null you derp"); }
                Sprite sp = SpriteObject.GetComponent<SpriteRenderer>().sprite;
                GameObject.Destroy(sp);
                SpriteObject.GetComponent<SpriteRenderer>().sprite = null;
                SpriteObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(TexArray[AnimationCurrentFrame], new Rect(0, 0, 32, 32), new Vector2(0.05f, 0.05f));
                //SetTransform(current_x, current_x, 0);
            }
        }

        public bool Despawn()
        {
          GameObject.Destroy(SpriteObject);
            despawned = true;
            return true;
        }

        //Initializes the Texture2D animation
        public void InitializeContinuousFrameAnimation(Texture2D[] TexArray, int FrameDelay, int FrameAmount, bool Repeat)
        {
            this.TexArray = TexArray;
            AnimationTotalDelaySteps = FrameDelay;
            AnimationFrameAmount = FrameAmount;
            AnimationCurrentDelayStep = 0;
            AnimationCurrentFrame = 0;
            AnimationRunning = true;
            this.RepeatAnimation = Repeat;
        }

        public int GetId()
        {
            return internal_id;
        }
    }
}
