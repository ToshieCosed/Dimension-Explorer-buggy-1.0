using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class GameEntity
    {
        GameObject SpriteObject;
        List<Action<GameEntity>> entity_actions = new List<Action<GameEntity>>();
        private float x = 0; //Todo, update this to be named something more along the lines of originalspawnxy
        private float y = 0;
        bool can_move = false;
        int move_counter = 1000;
        int internal_type = 0; //0 is tile or solid entity
        int internal_id = 0;
        private float distance_x = 0;
        private float distance_y = 0;
        private float stepdistance_x = 0;
        private float stepdistance_y = 0;
        private float rotation_degrees = 0;
        float stepsleft = 0;
        bool alreadymoving = false;
        float current_x = 0;
        float current_y = 0;
        bool need_despawn = false;
        bool despawned = false;
        float rotation_step_amount = 0;
        float rotated_angle = 0;
        private bool need_actions_run = false;
        Vector2 Target = Vector2.zero;
    
        public void AddAction(Action<GameEntity> A)
        {
            entity_actions.Add(A);
        }

        public void QueueListSafeActions()
        {
            need_actions_run = true;
        }

        public bool NeedActionsRun()
        {
            return need_actions_run;
        }

        public bool needs_despawn()
        {
            return need_despawn;
        }

        public void queue_despawn()
        {
            need_despawn = true;
        }

        public bool is_despawned()
        {
            return despawned;
        }

        public void RunAllActions()
        {
            foreach(Action<GameEntity> a in entity_actions)
            {
                a.Invoke(this);
            }
        }

        public bool RunAllActionsListSafe()
        {
            foreach (Action<GameEntity> a in entity_actions)
            {
                a.Invoke(this);
            }
            need_actions_run = false; //reset the flag
            return true;
        }

        public int Get_ID()
        {
            return internal_id;
        }

        public void set_internal_type(int type)
        {
            internal_type = type;
        }

        public Vector2 GetCurrentPos()
        {
            Vector2 cur_pos = new Vector2(current_x, current_y);
            return cur_pos;
        }

        public void SetTarget(Vector2 targ)
        {
            Target = targ;
        }

        public Vector2 GetTarget()
        {
            return Target;
        }

        public GameEntity(Texture2D EntitySpriteTex, float startingx, float startingy, float width, float height, int sortingorder, bool Solid, int intern_type, int intern_id)
        {
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

        public void Update()
        {
            //int move = 100;
           int move = UnityEngine.Random.Range(0, 4);
            switch (move)
            {
                case 0:
                    this.y -= 0.01f;
                    SetTransform(this.x, this.y, 0);
                    break;
                case 1:
                    this.y += 0.01f;
                    SetTransform(this.x, this.y, 0);
                    break;
                case 2:
                    this.x += 0.01f;
                    SetTransform(this.x, this.y, 0);
                    break;
                case 3:
                    this.x -= 0.01f;
                    SetTransform(this.x, this.y, 0);
                    break;
            }
            can_move = false;
        }

        public void setscale(Vector3 scale)
        {
            Vector3 scale_ = SpriteObject.GetComponent<SpriteRenderer>().transform.localScale;
            Vector3 newscale = Vector3.Scale(scale, scale_);
            SpriteObject.GetComponent<SpriteRenderer>().transform.localScale = newscale;
            

        }

        public void SetTransform(float x, float y, float z)
        {
            if (rotation_degrees <= 0)
            {
                SpriteObject.GetComponent<SpriteRenderer>().transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.Euler(0, 0, 0));
               
            }else
            {
                SpriteObject.GetComponent<SpriteRenderer>().transform.position = new Vector2(x, y);
            }
            current_x = x;
            current_y = y;
        }

        public void Despawn()
        {
            GameObject.Destroy(SpriteObject);
            despawned = true;
        }

        public GameObject GetObj()
        {
            return this.SpriteObject;
        }

        public int GetInteralType()
        {
            return internal_type;
        }

        public Vector2 GetStartingXY()
        {
            Vector2 ret = new Vector2(x, y);
            return ret;
        }
        //Todo, change this to timestep over time /x frames whatever not, computational steps.
        public void ApplyGenericMovement(float distance_x, float distance_y, float steps)
        { //todo rename function local vars to not match class local vars
            this.distance_x = distance_x;
            this.distance_y = distance_y;
            this.stepsleft = steps;
            this.alreadymoving = true; //change states.
            this.stepdistance_x = distance_x / steps;
            this.stepdistance_y = distance_y / steps;
            this.rotation_step_amount = rotation_degrees / steps;

        }

        public void SetMovementRotation(float targetdegrees)
        {
            this.rotation_degrees = targetdegrees;
        }

        public bool GetMovingState()
        {
            return this.alreadymoving;
        }

        public void RunContinuousMovementStep()
        {
            Vector2 this_position = SpriteObject.transform.position;
            this_position.x += this.stepdistance_x;
            this_position.y += this.stepdistance_y;
            this.stepsleft -= 1;
            SetTransform(this_position.x, this_position.y, 0);
            rotated_angle += rotation_step_amount;
            if (rotation_degrees > 0)
            {
                SpriteObject.transform.Rotate(0, 0, rotation_step_amount, Space.Self);
            }
            if (this.alreadymoving)
            {
                if (this.stepsleft <= 0)
                {
                    this.alreadymoving = false;
                    this.stepdistance_x = 0;
                    this.stepdistance_y = 0;
                    this.distance_x = 0;
                    this.distance_y = 0;
                }
            }
        }

    }
}