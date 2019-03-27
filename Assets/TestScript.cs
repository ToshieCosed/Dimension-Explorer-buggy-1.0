using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestScript : MonoBehaviour {
    public FireTest testfireball;
    int spawnamount = 0; //If spawning entities, this is the amount to spawn before stopping.
    List<FireTest> followers = new List<FireTest>();
    private TileSet tset;
    private Texture2D TsetImage;
    //private Texture2D TileImage = new Texture2D(16 * 20, 16 * 20, TextureFormat.RGB24, false);
    private int scroll_amount_required = 0;
    private int max_scroll = 32;
    private TileSet scrollable;
    private int scrolldir = -1;
    public int Target = 0;
    public Texture2D facingup;
    public Texture2D facingdown;
    public Texture2D facingleft;
    public Texture2D facingright;
    public Texture2D openChest;
    public Texture2D closedChest;
    private AudioSource source;
    private int SwordID = 16; //The Entity Id of the sword will always be 15 don't ask why I just feel like it.
    private bool facingdirchangedlastframe;
    public Texture2D swordup;
    public Texture2D sworddown;
    public Texture2D swordleft;
    public Texture2D swordright;
    AudioClip SoundChestOpen;
    GameEntityAnimated animentity;
    GameEntityAnimated cointest;
    Texture2D[] Puff2 = new Texture2D[5];
    GameEntityAnimated Puff2_;
    //The below list is basically used to initialize a list of poof animations that happen
    //Before or after scroll, and before entities spawn in.
    List<GameEntityAnimated> scroll_poofs = new List<GameEntityAnimated>();


    ChestHandler chestHandler = new ChestHandler();
    int delay = 0; //Used to delay screen scroll by an amount of update cycles (cpu dependent be careful with this! I think it is, anyway)
    int maxdelay = 2; //The amount of maximum frames to delay between doing another scroll -- Used to artificially stall scrolling. Change this value to slow or speed up scroll animation.
    public static bool debug = false; //Debug flag, turns on debugging features for Unity Editor.
    World GameWorld = new World();
    float scroll_x = 0;
    float scroll_y = 0;

    //Todo refactor how collision detection works
    //Move it elsewhere.
    //These are movement flags , setting all to false prevents movement, if in Update() after CheckCollision call.
    bool move_up = true;
    bool move_down = true;
    bool move_left = true;
    bool move_right = true;
    bool openedchest = false;

    //Scroll Init Flags, animations for enemies poofing away have to finish playing before scroll is allowed.
    bool Init_Scroll_Left = false;
    bool Init_Scroll_Right = false;
    bool Init_Scroll_Up = false;
    bool Init_Scroll_Down = false;

    bool Move_Scroll_Lock = false;
    bool AnimationCheckScrollLeft = false;
    bool AnimationCheckScrollRight = false;
    bool AnimationCheckScrollUp = false;
    bool AnimationCheckScrollDown = false;

    //This is just testing it will be removed soon
    bool can_move = true;
    bool holding_sword = false;

    int facing_direction = 0;
    //0 = Up
    //1 = Down
    //2 = Left
    //3 = Right

    float playerspeed = 0.096f; //Player screen movement speed. warning messing with this outside of divisible by 32 factors could mess up collision with solid tiles
    private bool scrolling = false; //Whether or not we are in a scrolling state. 
    EntityHandler GenericEntities = new EntityHandler();

    // Use this for initialization
    void Start () {
        //TileSet Image initialization required.
       this.TsetImage = new Texture2D(32 * 20, 14 * 20, TextureFormat.RGB24, false);
       this.TsetImage = Resources.Load<Texture2D>("Textures/Fauna_And_Grasses") as Texture2D;
       this.tset = new TileSet(TsetImage, 0, 0, 40, 40);
       GameWorld.SetWorldXY(0, 0);
       GameWorld.LoadMapByCoords(0, 0, 40, 40);
       tset.SetMap(GameWorld.GetLoadedMap());
       //We always have to update it this way now.
       tset.InitTileMapForDraw(0, 0);
       Texture2D EnemyEntityTex = Resources.Load<Texture2D>("Textures/TestEntity");
       
        GenericEntities.SpawnEntity(6, 6, 7, EnemyEntityTex, 0);
        chestHandler.SpawnSmallChest(8, 7, 0, openChest, closedChest, false, 2500);
       
        Texture2D RobotGuy = Resources.Load<Texture2D>("Textures/Robot");
        GenericEntities.SpawnEntity(5, 6, 0, RobotGuy, CustomID: 25);
        GameEntity Robo = GenericEntities.GetEntityById(25);
        //Attempting to set Robo guy's position transform target to the player?
        Robo.SetTarget(new Vector2(this.transform.position.x, this.transform.position.y));
        EntityActionMethods meths = new EntityActionMethods();
        meths.Init();
        List<Action<GameEntity>> A = meths.GetActions();

        Action<GameEntity> fireball_action = A.FirstOrDefault(x => x.Method.Name == "ShootFireBallAt");

        for (int entity_amount = 0; entity_amount < 8; entity_amount++){
            int id = UnityEngine.Random.Range(0, 65535);
            GenericEntities.SpawnEntity(entity_amount * 2, 18, 0, EnemyEntityTex, id);
            GenericEntities.PassActionToEntityByID(id, fireball_action);
            GameEntity FireBall_Shooter = GenericEntities.GetEntityById(id);
            FireBall_Shooter.SetTarget(new Vector2(this.transform.position.x, this.transform.position.y));
            FireBall_Shooter.set_internal_type(98);
        }

        if (fireball_action != null)
        {
            
                GenericEntities.PassActionToEntityByID(25, fireball_action);

        }

        GenericEntities.RunActionsOnEntityByType(99);
        GenericEntities.RunActionsOnEntityByID(25);
        GenericEntities.UpdateMovingEntities();


       gameObject.AddComponent<AudioSource>();
       AudioSource source = gameObject.GetComponent<AudioSource>();
       SoundChestOpen = Resources.Load<AudioClip>("Audio/ChestOpenShort");
        //Debug.Log("Start ran");

        

        //Just to test continuous frame animation
        Texture2D[] TexArray = new Texture2D[6];
        TexArray[0] = Resources.Load<Texture2D>("Textures/Puff/Frame1");
        TexArray[1] = Resources.Load<Texture2D>("Textures/Puff/Frame2");
        TexArray[2] = Resources.Load<Texture2D>("Textures/Puff/Frame3");
        TexArray[3] = Resources.Load<Texture2D>("Textures/Puff/Frame4");
        TexArray[4] = Resources.Load<Texture2D>("Textures/Puff/Frame5");

        //Just to test continuous frame animation
        Texture2D[] CircArray = new Texture2D[17];
        CircArray[0] = Resources.Load<Texture2D>("Textures/Circle/Frame1");
        CircArray[1] = Resources.Load<Texture2D>("Textures/Circle/Frame2");
        CircArray[2] = Resources.Load<Texture2D>("Textures/Circle/Frame3");
        CircArray[3] = Resources.Load<Texture2D>("Textures/Circle/Frame4");
        CircArray[4] = Resources.Load<Texture2D>("Textures/Circle/Frame5");
        CircArray[5] = Resources.Load<Texture2D>("Textures/Circle/Frame6");
        CircArray[6] = Resources.Load<Texture2D>("Textures/Circle/Frame7");
        CircArray[7] = Resources.Load<Texture2D>("Textures/Circle/Frame8");
        CircArray[8] = Resources.Load<Texture2D>("Textures/Circle/Frame9");
        CircArray[9] = Resources.Load<Texture2D>("Textures/Circle/Frame10");
        CircArray[10] = Resources.Load<Texture2D>("Textures/Circle/Frame11");
        CircArray[11] = Resources.Load<Texture2D>("Textures/Circle/Frame12");
        CircArray[12] = Resources.Load<Texture2D>("Textures/Circle/Frame13");
        CircArray[13] = Resources.Load<Texture2D>("Textures/Circle/Frame14");
        CircArray[14] = Resources.Load<Texture2D>("Textures/Circle/Frame15");
        CircArray[15] = Resources.Load<Texture2D>("Textures/Circle/Frame16");
        CircArray[16] = Resources.Load<Texture2D>("Textures/Circle/Frame17");

        //These textures are for the puff animation
        //During before / after map scroll when entities spawn/despawn.
        //Load Puff2
        Texture2D[] Puff2 = new Texture2D[5];
        Puff2[0] = Resources.Load<Texture2D>("Textures/Puff2/Frame1");
        Puff2[1] = Resources.Load<Texture2D>("Textures/Puff2/Frame2");
        Puff2[2] = Resources.Load<Texture2D>("Textures/Puff2/Frame3");
        Puff2[3] = Resources.Load<Texture2D>("Textures/Puff2/Frame4");
        Puff2[4] = Resources.Load<Texture2D>("Textures/Puff2/Frame5");

        this.Puff2 = Puff2;
        



        GameEntityAnimated entity = new GameEntityAnimated(TexArray[0], 8 * 0.32f, 8 * 0.32f, 32f, 32f, 5, true, 0, 10);
        if (entity != null)
        {
            entity.InitializeContinuousFrameAnimation(TexArray, 6, 4, true);
            //Debug.Log("The amount of items in the array is " + TexArray.Length);
        }

        this.animentity = entity;

        GameEntityAnimated centity = new GameEntityAnimated(CircArray[0], 12 * 0.32f, 12 * 0.32f, 32f, 32f, 5, true, 0, 10);
        if (centity != null)
        {
            centity.InitializeContinuousFrameAnimation(CircArray, 6, 16, true);
           //Debug.Log("The amount of items in the array is " + CircArray.Length);
        }

        this.cointest = centity;

        GameEntityAnimated Puff2_ = new GameEntityAnimated(Puff2[0], 15 * 0.32f, 15 * 0.32f, 32f, 32f, 5, true, 0, 21);
        if (Puff2_ != null)
        {
            Puff2_.InitializeContinuousFrameAnimation(Puff2, 9, 4, true);
        }
        this.Puff2_ = Puff2_;

    }

    public void Check_Collision_Test()
    {
        playerspeed = 0.03f;
        float size = 0.29f;
        float multiplyfactor = 0.5f;

        Vector2 TopLeft = new Vector2(this.transform.position.x - (size * multiplyfactor), this.transform.position.y + (size * multiplyfactor));
        Vector2 TopRight = new Vector2(this.transform.position.x + (size * multiplyfactor), this.transform.position.y + (size * multiplyfactor));
        Vector2 BottomLeft = new Vector2(this.transform.position.x - (size * multiplyfactor), this.transform.position.y - (size * multiplyfactor));
        Vector2 BottomRight = new Vector2(this.transform.position.x + (size * multiplyfactor), this.transform.position.y - (size * multiplyfactor));

        Vector2 ExtraHorizontal = new Vector2(0.16f, 0);
        Vector2 ExtraVertical = new Vector2(0, 0.16f);


        //Setting to 1 seems to not work?
            //So it just magically worked now?
        
        //===Top And Bottom Collision Detect===//
        var TopLeftHit = Physics2D.Raycast(TopLeft, Vector2.up, playerspeed);
        var TopRightHit = Physics2D.Raycast(TopRight, Vector2.up, playerspeed);
        var BottomLeftHit = Physics2D.Raycast(BottomLeft, Vector2.down, playerspeed);
        var BottomRightHit = Physics2D.Raycast(BottomRight, Vector2.down, playerspeed);
        //===Left And Right Collision Detect===//
        var LeftSideTopHit = Physics2D.Raycast(TopLeft, Vector2.left, playerspeed);
        var LeftSideBottomHit = Physics2D.Raycast(BottomLeft, Vector2.left, playerspeed);
        var RightSideTopHit = Physics2D.Raycast(TopRight, Vector2.right, playerspeed);
        var RightSideBottomHit = Physics2D.Raycast(BottomRight, Vector2.right, playerspeed);

        //Reset collision detect state
        move_up = true;
        move_down = true;
        move_left = true;
        move_right = true;


        if (TopLeftHit && TopLeftHit.transform.name == "Solid_Tile")
        {
            float hit_distance = TopLeftHit.distance;
            if (hit_distance <= playerspeed)
            {
                move_up = false;
            }

            //Should de spawn chest 25 when hitting a solid tile from above
           
        }
       
        if (TopRightHit && TopRightHit.transform.name == "Solid_Tile")
        {
            float hit_distance = TopRightHit.distance;
            if (hit_distance <= playerspeed)
            {
                move_up = false;
            }
        }

        if (BottomLeftHit && BottomLeftHit.transform.name == "Solid_Tile")
        {
            float hit_distance = BottomLeftHit.distance;
            if (hit_distance <= playerspeed)
            {
                move_down = false;
            }
        }

        if (BottomRightHit && BottomRightHit.transform.name == "Solid_Tile")
        {
            float hit_distance = BottomRightHit.distance;
            if (hit_distance <= playerspeed)
            {
                move_down = false;
            }
        }

        if (LeftSideTopHit && LeftSideTopHit.transform.name == "Solid_Tile")
        {
            float hit_distance = LeftSideTopHit.distance;
            if (hit_distance <= playerspeed)
            {
                move_left = false;
            }
        }

        if (LeftSideBottomHit && LeftSideBottomHit.transform.name == "Solid_Tile")
        {
            float hit_distance = LeftSideBottomHit.distance;
            if (hit_distance <= playerspeed)
            {
                move_left = false;
            }
        }

        if (RightSideTopHit && RightSideTopHit.transform.name == "Solid_Tile")
        {
            float hit_distance = RightSideTopHit.distance;
            if (hit_distance <= playerspeed)
            {
                move_right = false;
            }
        }

        if (RightSideBottomHit && RightSideBottomHit.transform.name == "Solid_Tile")
        {
            float hit_distance = RightSideBottomHit.distance;
            if (hit_distance <= playerspeed)
            {
                move_right = false;
            }
        }

        //Entity Cases will need a re-factor much later.
        float EntityDistanceEpsilon = 0.011f;
        if (TopLeftHit && TopLeftHit.transform.name == "Solid_Entity")
        {
           GameObject collided = TopLeftHit.collider.gameObject;
           if (collided!= null)
            {
                //Debug.Log("The type is " + TopLeftHit.collider.gameObject.GetType());
                GameObject sourceObject = collided.transform.gameObject;



                //Debug.Log("The key found on the source object was " + sourceObject.GetInstanceID());
                GameEntity E = GenericEntities.GetEntity(sourceObject);
                bool took_action = false;
                //A null check in case the Entity doesn't exist and is a chest
                //Instead.
                //A null check in case the Entity doesn't exist and is a chest
                //Instead.


                if (E != null)
                {
                    int Despawn_ID = E.Get_ID();
                    if (E.GetInteralType() != 1)
                    {
                        GenericEntities.Despawn(Despawn_ID);
                        GenericEntities.Remove_Despawned();
                        //took_action = true;
                    }
                }

                GameEntity chest = chestHandler.GetEntity(sourceObject);
                if (chest != null)
                {
                    Debug.Log("The chest was not null");
                    if (!took_action) {
                        
                    int Despawn_ID = chest.Get_ID();
                            //Internal type ==1 is closed chest. 2 == open chest.
                        if (chest.GetInteralType() == 1)
                        {
                            Vector2 pos_ = chest.GetStartingXY();
                            chestHandler.DespawnChest(Despawn_ID);
                            float oldx = pos_.x / 0.32f;
                            float oldy = pos_.y / 0.32f;
                            chestHandler.SpawnSmallChest((int)oldx, (int)oldy, 0, openChest, closedChest, true, 2500);
                            took_action = true;
                            AudioSource source = gameObject.GetComponent<AudioSource>();
                            source.clip = this.SoundChestOpen;
                            source.Play();
                            Debug.Log("The name of the played file was " + this.SoundChestOpen.name);
                            Debug.Log("Spawned a chest that should be open");
                            //todo delete the below extra lines inside this later.
                            GameEntity newchest = chestHandler.GetEntityByID(2500);
                            newchest.ApplyGenericMovement(0, 0.08f, 40);
                            can_move = false;
                            openedchest = true;
                        }
                    }
                }
            }
            float hit_distance = TopLeftHit.distance;
            if (hit_distance <= EntityDistanceEpsilon)
            {
                move_up = false;
            }
                //Try to make adaptive collision, will cause slowdown when close to entities
                    //For now but meh.
            else
            {
                playerspeed = 0.01f;
            }
        }

        if (TopRightHit && TopRightHit.transform.name == "Solid_Entity")
        {
            GameObject collided = TopRightHit.collider.gameObject;
            if (collided != null)
            {
                GameObject sourceObject = collided.transform.gameObject;
                GameEntity E = GenericEntities.GetEntity(sourceObject);
                bool took_action = false;
                //A null check in case the Entity doesn't exist and is a chest
                //Instead.
                if (E != null)
                {
                    int Despawn_ID = E.Get_ID();
                    if (E.GetInteralType() != 1)
                    {
                        GenericEntities.Despawn(Despawn_ID);
                        GenericEntities.Remove_Despawned();
                        //took_action = true;
                    }
                }

                GameEntity chest = chestHandler.GetEntity(sourceObject);
                if (chest != null)
                {
                    Debug.Log("The chest was not null");
                    if (!took_action)
                    {
                        int Despawn_ID = chest.Get_ID();
                        //Internal type ==1 is closed chest. 2 == open chest.
                        if (chest.GetInteralType() == 1)
                        {
                            Vector2 pos_ = chest.GetStartingXY();
                            chestHandler.DespawnChest(Despawn_ID);
                            float oldx = pos_.x / 0.32f;
                            float oldy = pos_.y / 0.32f;
                            chestHandler.SpawnSmallChest((int)oldx, (int)oldy, 0, openChest, closedChest, true, 2500);
                            took_action = true;
                            AudioSource source = gameObject.GetComponent<AudioSource>();
                            source.clip = SoundChestOpen;
                            source.Play();
                            Debug.Log("The name of the played file was " + SoundChestOpen.name);
                            Debug.Log("Spawned a chest that should be open");
                            //todo delete the below extra lines inside this later.
                            GameEntity newchest = chestHandler.GetEntityByID(2500);
                            newchest.ApplyGenericMovement(0, 0.08f, 40);
                        }
                    }
                }
            }
            float hit_distance = TopRightHit.distance;
            if (hit_distance <= EntityDistanceEpsilon)
            {
                move_up = false;
            }

            //Try to make adaptive collision, will cause slowdown when close to entities
            //For now but meh.
            else
            {
                playerspeed = 0.01f;
            }
        }

        if (BottomLeftHit && BottomLeftHit.transform.name == "Solid_Entity")
        {
            float hit_distance = BottomLeftHit.distance;
            if (hit_distance <= EntityDistanceEpsilon)
            {
                move_down = false;
            }

            //Try to make adaptive collision, will cause slowdown when close to entities
            //For now but meh.
            else
            {
                playerspeed = 0.01f;
            }
        }

        if (BottomRightHit && BottomRightHit.transform.name == "Solid_Entity")
        {
            float hit_distance = BottomRightHit.distance;
            if (hit_distance <= EntityDistanceEpsilon)
            {
                move_down = false;
            }

            //Try to make adaptive collision, will cause slowdown when close to entities
            //For now but meh.
            else
            {
                playerspeed = 0.01f;
            }
        }

        if (LeftSideTopHit && LeftSideTopHit.transform.name == "Solid_Entity")
        {
            float hit_distance = LeftSideTopHit.distance;
            if (hit_distance <= EntityDistanceEpsilon)
            {
                move_left = false;
            }

            //Try to make adaptive collision, will cause slowdown when close to entities
            //For now but meh.
            else
            {
                playerspeed = 0.001f;
            }
        }

        if (LeftSideBottomHit && LeftSideBottomHit.transform.name == "Solid_Entity")
        {
            float hit_distance = LeftSideBottomHit.distance;
            if (hit_distance <= EntityDistanceEpsilon)
            {
                move_left = false;
            }

            //Try to make adaptive collision, will cause slowdown when close to entities
            //For now but meh.
            else
            {
                playerspeed = 0.01f;
            }
        }

        if (RightSideTopHit && RightSideTopHit.transform.name == "Solid_Entity")
        {
            float hit_distance = RightSideTopHit.distance;
            if (hit_distance <= EntityDistanceEpsilon)
            {
                move_right = false;
            }

            //Try to make adaptive collision, will cause slowdown when close to entities
            //For now but meh.
            else
            {
                playerspeed = 0.01f;
            }
        }

        if (RightSideBottomHit && RightSideBottomHit.transform.name == "Solid_Entity")
        {
            float hit_distance = RightSideBottomHit.distance;
            if (hit_distance <= EntityDistanceEpsilon)
            {
                move_right = false;
            }

            //Try to make adaptive collision, will cause slowdown when close to entities
            //For now but meh.
            else
            {
                playerspeed = 0.01f;
            }
        }
        //====End of entity cases===//



        if (debug)
        {
            //RayCast Player Collision debug draw.
            Debug.DrawRay(TopLeft, Vector2.up * playerspeed, Color.magenta, 0.5f);
            Debug.DrawRay(TopRight, Vector2.up * playerspeed, Color.magenta, 0.5f);
            Debug.DrawRay(BottomLeft, Vector2.down * playerspeed, Color.magenta, 0.5f);
            Debug.DrawRay(BottomRight, Vector2.down * playerspeed, Color.magenta, 0.5f);
            Debug.DrawRay(TopLeft, Vector2.left * playerspeed, Color.magenta, 0.5f);
            Debug.DrawRay(BottomLeft, Vector2.left * playerspeed, Color.magenta, 0.5f);
            Debug.DrawRay(TopRight, Vector2.right * playerspeed, Color.magenta, 0.5f);
            Debug.DrawRay(BottomRight, Vector2.right * playerspeed, Color.magenta, 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update ran");

        //Give Entity number 25 the player's new position.
        GameEntity FireBallShooter = GenericEntities.GetEntityById(25);
        List<GameEntity> entities = GenericEntities.GetAllEntities();
        

        if (FireBallShooter != null)
        {
            int can_shoot = UnityEngine.Random.Range(0, 25);
            if (can_shoot == 15) {
                FireBallShooter.SetTarget(new Vector2(this.transform.position.x, this.transform.position.y));
                List<int> Entity_IDs = new List<int>();
                foreach(GameEntity E in entities) {
                    if (E.GetInteralType() == 98)
                    {
                        E.SetTarget(new Vector2(this.transform.position.x, this.transform.position.y));
                        Entity_IDs.Add(E.Get_ID());
                    }
                }

                int sword_shoots = UnityEngine.Random.Range(0, 30);
                
                    foreach(GameEntity E in entities)
                    {

                        if (E.GetInteralType() == 97)
                        {
                           // E.setscale(new Vector3(1.2f, 1.2f, 1.2f));
                        //We found a sword fireball shooter
                            if (sword_shoots >= 20)
                            {
                            E.SetTarget(new Vector2(this.transform.position.x, this.transform.position.y));
                            int shot = UnityEngine.Random.Range(0, 20);
                            E.setscale(new Vector3(1.2f, 1.2f, 1.2f));
                            if (shot > 1) { E.QueueListSafeActions();
                                
                            } //make the sword shoot randomly.



                        }
                    }
                   
                    //GenericEntities.RunActionsOnEntityByType(97); //Update Swords to Shoot fireballs per frame.
                }

                //Now instead of doing E.RunAllActions we do E.QueueListSafeActions
                foreach(GameEntity E in entities) { if (E.GetInteralType() == 99) { E.QueueListSafeActions(); } }
                Action<GameEntity> run_action = (Action<GameEntity>)((GameEntity E) => { E.RunAllActionsListSafe(); });
                entities.Where(x => x.NeedActionsRun() == true).ToList<GameEntity>().ForEach(run_action);

                //Seems we have to cache this since running actions can spawn fireballs
                    //Which modifies the loaded Entities collection
                        //Which seems to stop iteration on the copy reference called entities?
                foreach(int id in Entity_IDs) { GenericEntities.RunActionsOnEntityByID(id); }
                
                //GenericEntities.RunActionsOnEntityByID(25); //Run the shoot action only conditionally =p which is abstracted away


            }
        }
        GenericEntities.RunActionsOnEntityByType(98); //Make the other ones shoot fire now too :)

        GenericEntities.RunActionsOnEntityByType(99); //Update Fireballs
        
        GenericEntities.UpdateMovingEntities();
        GenericEntities.Despawn_Queued();
        chestHandler.UpdateMovingEntities();
        Check_Collision_Test();
        if (animentity == null)
        {
            Debug.Log("The entity is null");

        } else
        {
            animentity.RunContinuousFrameAnimation();
        }

        //Animated coin entity test
        if (cointest == null)
        {
            Debug.Log("The entity is null");

        }
        else
        {
            cointest.RunContinuousFrameAnimation();
        }

        if (Puff2_ == null)
        {
            Debug.Log("The puffers is null >.<");
        } else
        {
            Puff2_.RunContinuousFrameAnimation();
        }

        
        //scrolldirs
        //1 = Left
        //2 = Right
        //3 = Up
        //4 = Down
        //Debug.Log("stuff");

        //Only allow if not scrolling.


        if (Input.GetKey(KeyCode.Space))
        {
            holding_sword = true;

            //Check if the sword isn't spawned yet
            //If it is check if matches facing dir
            if (GenericEntities.GetEntityById(SwordID) != null)
            {
                if (!GenericEntities.GetEntityById(SwordID).GetMovingState())
                {
                    GenericEntities.Despawn(SwordID);
                    GenericEntities.Remove_Despawned();
                } else
                    if (facingdirchangedlastframe)
                {
                    GenericEntities.Despawn(SwordID); //cause sword will respawn again.
                    GenericEntities.Remove_Despawned();
                    facingdirchangedlastframe = false;
                }
            }

            //note after this comment you cannot reliable use the
            //facingdirchangedlastframe value to test if the player changed direction.

            //And since that above case will only reset 
            //Facingdirchangedlast frame if the sword isn't null
            //We need to clear it anyways after that check.

            facingdirchangedlastframe = false;

            if (facing_direction == 0)
            {
                float posx = this.transform.position.x;
                float posy = this.transform.position.y;
                posx -= 0.14f;
                posy += 0.24f;

                EntityActionMethods meths = new EntityActionMethods();
                meths.Init();
                List<Action<GameEntity>> actions = meths.GetActions();
                Action<GameEntity> fireball_action = actions.FirstOrDefault(x => x.Method.Name == "ShootFireBallAt");
                Action<GameEntity> despawn_action = actions.FirstOrDefault(x => x.Method.Name == "DespawnIfTargetReached");
                for (int i = 0; i < 300; i++)
                {
                    GenericEntities.SpawnEntity((int)posx, (int)posy, 0, swordup, SwordID+i);
                    GameEntity swordheld = GenericEntities.GetEntityById(SwordID+i);
                    swordheld.SetTransform(posx, posy, 0);
                    swordheld.GetObj().transform.parent = this.transform;
                    swordheld.SetMovementRotation(60+i);
                    swordheld.ApplyGenericMovement(0, i * 0.16f, 20);
                    //swordheld.setscale(new Vector3(0, 0, 1.01f));
                    swordheld.SetTarget(new Vector2(this.transform.position.x, this.transform.position.y));
                    if (fireball_action != null)
                    {
                        //GenericEntities.PassActionToEntityByID(SwordID + i, fireball_action);
                        //GenericEntities.PassActionToEntityByID(SwordID + i, despawn_action);
                        //Set the shot sword to shoot fireballs at the player.
                        swordheld.set_internal_type(97);
                        
                    }
                }
                
            }

            if (facing_direction == 1)
            {
                float posx = this.transform.position.x;
                float posy = this.transform.position.y;
                posx -= 0.14f;
                posy -= 0.40f;
                GenericEntities.SpawnEntity((int)posx, (int)posy, 0, sworddown, SwordID);
                GameEntity swordheld = GenericEntities.GetEntityById(16);
                swordheld.SetTransform(posx, posy, 0);
                swordheld.GetObj().transform.parent = this.transform;
                swordheld.ApplyGenericMovement(0, -0.15f, 10);
            }

            if (facing_direction == 2)
            {
                float posx = this.transform.position.x;
                float posy = this.transform.position.y;
                posx -= 0.40f;
                posy -= 0.24f;
                GenericEntities.SpawnEntity((int)posx, (int)posy, 0, swordleft, SwordID);
                GameEntity swordheld = GenericEntities.GetEntityById(16);
                swordheld.SetTransform(posx, posy, 0);
                swordheld.GetObj().transform.parent = this.transform;
                swordheld.ApplyGenericMovement(-0.05f, 0, 10);
            }

            if (facing_direction == 3)
            {
                float posx = this.transform.position.x;
                float posy = this.transform.position.y;
                posx += 0.08f;
                posy -= 0.24f;
                GenericEntities.SpawnEntity((int)posx, (int)posy, 0, swordright, SwordID);
                GameEntity swordheld = GenericEntities.GetEntityById(16);
                swordheld.SetTransform(posx, posy, 0);
                swordheld.GetObj().transform.parent = this.transform;
                swordheld.ApplyGenericMovement(+0.05f, 0, 10);
            }



        }
        else
        {
            holding_sword = false;
            if (GenericEntities.GetEntityById(SwordID) != null)
            {
                if (!GenericEntities.GetEntityById(SwordID).GetMovingState())
                {
                    GenericEntities.Despawn(SwordID);
                    GenericEntities.Remove_Despawned();
                }
            }
        }

        GameEntity pickedupchest = chestHandler.GetEntityByID(2500);
        if (!pickedupchest.GetMovingState())
        {
            if (openedchest) { pickedupchest.GetObj().GetComponent<SpriteRenderer>().transform.parent = this.transform;
                can_move = true;

            }
        }

        if (can_move)
        {
            if (!this.scrolling)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    if (move_left)
                    {
                        Vector2 pos_ = this.transform.position;
                        pos_.x -= playerspeed;
                        this.transform.position = pos_;

                        if (GameWorld.GetLoadedAreaSize().x > 20 && pos_.x >= (0.32f * 10))
                        {
                            scroll_x += 1;
                            tset.SetNewUnsafePos(tset.scrollx + scroll_x * 0.32f, 0, GameWorld.GetLoadedAreaSize().x, GameWorld.GetLoadedAreaSize().y);
                            //Now we have to also cancel the normal movement
                            pos_ = this.transform.position;
                            pos_.x += playerspeed;
                            this.transform.position = pos_;

                        }

                        if (facing_direction != 2)
                        
                        {
                            facing_direction = 2; this.GetComponent<SpriteRenderer>().sprite = Sprite.Create(facingleft, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                            this.GetComponent<SpriteRenderer>().transform.SetPositionAndRotation(new Vector3(this.transform.position.x, this.transform.position.y), Quaternion.Euler(0, 0, 0));
                            facingdirchangedlastframe = true;
                        }

                    }
                }

                //Normal Right Movement.
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    if (move_right)
                    {
                        Vector2 pos_ = this.transform.position;
                        pos_.x += playerspeed;
                        this.transform.position = pos_;

                        if (GameWorld.GetLoadedAreaSize().x >= 20 && pos_.x >= (0.32f * 10))
                        {
                            scroll_x -= 1;
                            tset.SetNewUnsafePos(tset.scrollx + scroll_x * 0.32f, 0, GameWorld.GetLoadedAreaSize().x, GameWorld.GetLoadedAreaSize().y);

                            //Now we have to also cancel the normal movement
                            pos_ = this.transform.position;
                            pos_.x -= playerspeed;
                            this.transform.position = pos_;

                        }

                        if (facing_direction != 3)
                        {
                            facing_direction = 3; this.GetComponent<SpriteRenderer>().sprite = Sprite.Create(facingright, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                            this.GetComponent<SpriteRenderer>().transform.SetPositionAndRotation(new Vector3(this.transform.position.x, this.transform.position.y), Quaternion.Euler(0, 0, 0));
                            facingdirchangedlastframe = true;
                        }
                    }
                }

                //Normal Up Movement.
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    //Add the collision test.
                    if (move_up)
                    {
                        Vector2 pos_ = this.transform.position;
                        pos_.y += playerspeed;
                        this.transform.position = pos_;
                        if (facing_direction != 0)
                        {
                            facing_direction = 0; this.GetComponent<SpriteRenderer>().sprite = Sprite.Create(facingup, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                            this.GetComponent<SpriteRenderer>().transform.SetPositionAndRotation(new Vector3(this.transform.position.x, this.transform.position.y), Quaternion.Euler(0, 0, 0));
                            facingdirchangedlastframe = true;
                        }
                    }
                }


                //Normal Down Movement.
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    if (move_down)
                    {

                        Vector2 pos_ = this.transform.position;
                        pos_.y -= playerspeed;
                        this.transform.position = pos_;
                        if (facing_direction != 1)
                        {
                            facing_direction = 1; this.GetComponent<SpriteRenderer>().sprite = Sprite.Create(facingdown, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                            this.GetComponent<SpriteRenderer>().transform.SetPositionAndRotation(new Vector3(this.transform.position.x, this.transform.position.y), Quaternion.Euler(0, 0, 0));
                            facingdirchangedlastframe = true;
                        }
                    }
                }
            }
        }

        //===Begin Scroll and Screen-Edge Detection Code Here
        //Todo, move this elsewhere somehow. :/
        if (!this.scrolling)
        {

            //Avoid having it create multiple on multi -- input.
            bool docreate = true;

            if (!AnimationCheckScrollLeft)
            {
                if (this.transform.position.x <= 0)
                {
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        List<GameEntity> Elist = GenericEntities.GetAllEntities();
                        List<int> to_despawn = new List<int>();
                        foreach (GameEntity E in Elist)
                        {
                            int id = E.Get_ID();
                            to_despawn.Add(id); //Cache the id's to despawn
                            Vector2 E_pos = E.GetCurrentPos();
                            GameEntityAnimated poof = new GameEntityAnimated(Puff2[0], E_pos.x, E_pos.y, 32f, 32f, 5, false, 10, 0);
                            poof.InitializeContinuousFrameAnimation(Puff2, 7, 4, false);
                            scroll_poofs.Add(poof);


                        }
                        
                        foreach (int id in to_despawn) { GenericEntities.Despawn(id); } //Actually despawn map entities.
                        GenericEntities.Remove_Despawned();
                        Move_Scroll_Lock = true;
                        AnimationCheckScrollLeft = true;

                        //The case where there were just no poofs in the list as a result of there being no entities.
                        if (scroll_poofs.Count() <= 0)
                        {
                            Move_Scroll_Lock = false;
                            AnimationCheckScrollLeft = false;
                            Init_Scroll_Left = true;
                        }

                    }
                }
            }

            //Initialize Scroll_Left
            if (Init_Scroll_Left)
            {
                //Scroll the player as well
                Vector2 pos_ = this.transform.position;
                pos_.x += 0.32f;
                this.transform.position = pos_;
                tset.scrollx += 1;
                tset.SetPosInternally();
                scrolling = true; // set the scroll flag
                scroll_amount_required = 19;
                scrolldir = 1;
                //Set up the tileset that will scroll away from us into focus.
                scrollable = new TileSet(TsetImage, tset.scrollx - 20, tset.scrolly, 20, 20);
                //Move the Game World and set draw.
                GameWorld.MoveWest();
                scrollable.SetMap(GameWorld.GetLoadedMap());
                //We have to call this externally from now on.
                scrollable.InitTileMapForDraw(tset.scrollx - 20, tset.scrolly);
                scrollable.SetNewPos(tset.scrollx - 20, tset.scrolly);
                docreate = false;
                Init_Scroll_Left = false;

            }

            if (!AnimationCheckScrollRight)
            {
                //Adapt scrolling to the size of the map X-wise
                if ( (this.transform.position.x >= (GameWorld.GetLoadedAreaSize().x * 0.32f) + 0.16f && GameWorld.GetLoadedAreaSize().x <20) || (scroll_x < GameWorld.GetLoadedAreaSize().x))
                {
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        List<GameEntity> Elist = GenericEntities.GetAllEntities();
                        List<int> to_despawn = new List<int>();
                        foreach (GameEntity E in Elist)
                        {
                            int id = E.Get_ID();
                            to_despawn.Add(id); //Cache the id's to despawn
                            Vector2 E_pos = E.GetCurrentPos();
                            GameEntityAnimated poof = new GameEntityAnimated(Puff2[0], E_pos.x, E_pos.y, 32f, 32f, 5, false, 10, 0);
                            poof.InitializeContinuousFrameAnimation(Puff2, 7, 4, false);
                            scroll_poofs.Add(poof);


                        }

                        foreach (int id in to_despawn) { GenericEntities.Despawn(id); } //Actually despawn map entities.
                        GenericEntities.Remove_Despawned();
                        Move_Scroll_Lock = true;
                        AnimationCheckScrollRight = true;

                        //The case where there were just no poofs in the list as a result of there being no entities.
                        if (scroll_poofs.Count() <= 0)
                        {
                            Move_Scroll_Lock = false;
                            AnimationCheckScrollRight = false;
                            Init_Scroll_Right = true;
                        }
                    }
                }
            }

            //Initialize Scroll_Right
            if (Init_Scroll_Right)
            {
                if (docreate)
                {
                    //Scroll the player as well
                    Vector2 pos_ = this.transform.position;
                    pos_.x -= 0.32f;
                    this.transform.position = pos_;
                    tset.scrollx -= 1;
                    tset.SetPosInternally();
                    scrolling = true; // set the scroll flag
                    scroll_amount_required = 19;
                    scrolldir = 2;
                    //Set up the tileset that will scroll away from us into focus.
                    scrollable = new TileSet(TsetImage, tset.scrollx + 20, tset.scrolly, 20, 20);
                    //Move the Game World and set draw.
                    GameWorld.MoveEast();
                    scrollable.SetMap(GameWorld.GetLoadedMap());
                    //We have to call this externally from now on.
                    scrollable.InitTileMapForDraw(tset.scrollx + 20, tset.scrolly);
                    scrollable.SetNewPos(tset.scrollx + 20, tset.scrolly);
                    docreate = false;
                    Init_Scroll_Right = false;
                }
            }

            if (this.transform.position.y >= (GameWorld.GetLoadedAreaSize().y * 0.32f) + 0.16f)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    List<GameEntity> Elist = GenericEntities.GetAllEntities();
                    List<int> to_despawn = new List<int>();
                    foreach (GameEntity E in Elist)
                    {
                        int id = E.Get_ID();
                        to_despawn.Add(id); //Cache the id's to despawn
                        Vector2 E_pos = E.GetCurrentPos();
                        GameEntityAnimated poof = new GameEntityAnimated(Puff2[0], E_pos.x, E_pos.y, 32f, 32f, 5, false, 10, 0);
                        poof.InitializeContinuousFrameAnimation(Puff2, 7, 4, false);
                        scroll_poofs.Add(poof);


                    }

                    foreach (int id in to_despawn) { GenericEntities.Despawn(id); } //Actually despawn map entities.
                    GenericEntities.Remove_Despawned();
                    Move_Scroll_Lock = true;
                    AnimationCheckScrollUp = true;

                    //The case where there were just no poofs in the list as a result of there being no entities.
                    if (scroll_poofs.Count() <= 0)
                    {
                        Move_Scroll_Lock = false;
                        AnimationCheckScrollUp = false;
                        Init_Scroll_Up= true;
                    }
                }
            }

            if (Init_Scroll_Up)
            {
                if (docreate)
                {
                    tset.scrolly -= 1;
                    tset.SetPosInternally();
                    scrolling = true; // set the scroll flag
                    scroll_amount_required = 19;
                    scrolldir = 3;
                    //Set up the tileset that will scroll away from us into focus.
                    scrollable = new TileSet(TsetImage, tset.scrollx, tset.scrolly + 20, 20, 20);
                    //Move the Game World North
                    GameWorld.MoveNorth();
                    scrollable.SetMap(GameWorld.GetLoadedMap());
                    scrollable.InitTileMapForDraw(tset.scrollx, tset.scrolly + 20);
                    scrollable.SetNewPos(tset.scrollx, tset.scrolly + 20);
                    docreate = false;
                    Init_Scroll_Up = false;
                }
            }

            if (this.transform.position.y <= 0)
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    List<GameEntity> Elist = GenericEntities.GetAllEntities();
                    List<int> to_despawn = new List<int>();
                    foreach (GameEntity E in Elist)
                    {
                        int id = E.Get_ID();
                        to_despawn.Add(id); //Cache the id's to despawn
                        Vector2 E_pos = E.GetCurrentPos();
                        GameEntityAnimated poof = new GameEntityAnimated(Puff2[0], E_pos.x, E_pos.y, 32f, 32f, 5, false, 10, 0);
                        poof.InitializeContinuousFrameAnimation(Puff2, 7, 4, false);
                        scroll_poofs.Add(poof);


                    }

                    foreach (int id in to_despawn) { GenericEntities.Despawn(id); } //Actually despawn map entities.
                    GenericEntities.Remove_Despawned();
                    Move_Scroll_Lock = true;
                    AnimationCheckScrollDown = true;

                    //The case where there were just no poofs in the list as a result of there being no entities.
                    if (scroll_poofs.Count() <= 0)
                    {
                        Move_Scroll_Lock = false;
                        AnimationCheckScrollDown = false;
                        Init_Scroll_Down = true;
                    }
                }
            }

            if (Init_Scroll_Down)
            {
                if (docreate)
                {
                    //Scroll the player as well
                    Vector2 pos_ = this.transform.position;
                    pos_.y += 0.32f;
                    this.transform.position = pos_;
                    tset.scrolly += 1;
                    tset.SetPosInternally();
                    scrolling = true; // set the scroll flag
                    scroll_amount_required = 19;
                    scrolldir = 4;
                    //Set up the tileset that will scroll away from us into focus.
                    scrollable = new TileSet(TsetImage, tset.scrollx, tset.scrolly - 20, 20, 20);
                    //Move the Game World South
                    GameWorld.MoveSouth();
                    scrollable.SetMap(GameWorld.GetLoadedMap());
                    scrollable.InitTileMapForDraw(tset.scrolly, tset.scrolly - 20);
                    scrollable.SetNewPos(tset.scrollx, tset.scrolly - 20);
                    docreate = false;
                    Init_Scroll_Down = false;
                }
            }
        }

        //Given we want to poof away all Loaded Game Entities Manually. Replace them with poof animations
        //And then wait for those animations to finish we need to now call this function after scroll init Checks

        if (AnimationCheckScrollDown || AnimationCheckScrollUp || AnimationCheckScrollLeft || AnimationCheckScrollRight)
        {
            AnimationChecks(); //This will allow setting or unsetting some variables based on conditions more cleanly.
        }

        //===End of Begin scrolling code, and edge detection code.

        //===Continuous Scrolling handler, outside single Update Cycle===
        if (scrolling)
        {
            //Game State is asking to scroll Left!
            if (scrolldir == 1)
            {
                delay = delay + 1;
                if (delay > maxdelay)
                {
                    delay = 0;
                    if (scroll_amount_required > 0)
                    {
                        tset.scrollx += 1;
                        scrollable.scrollx += 1;
                        tset.SetPosInternally();
                        scrollable.SetPosInternally();
                        scroll_amount_required -= 1;
                        //Scroll player too
                        Vector2 pos_ = this.transform.position;
                        pos_.x += 0.32f;
                        this.transform.position = pos_;
                    }
                }
            }

            //Game State is asking to scroll right!
            if (scrolldir == 2)
            {
                delay = delay + 1;
                if (delay > maxdelay)
                {
                    delay = 0;
                    if (scroll_amount_required > 0)
                    {
                        tset.scrollx -= 1;
                        scrollable.scrollx -= 1;
                        tset.SetPosInternally();
                        scrollable.SetPosInternally();
                        scroll_amount_required -= 1;
                        Vector2 pos_ = this.transform.position;
                        pos_.x -= 0.32f;
                        this.transform.position = pos_;
                    }
                }
            }

            //Scrolling Up
            if (scrolldir == 3)
            {
                delay = delay + 1;
                if (delay > maxdelay)
                {
                    delay = 0;
                    if (scroll_amount_required > 0)
                    {
                        tset.scrolly -= 1;
                        scrollable.scrolly -= 1;
                        tset.SetPosInternally();
                        scrollable.SetPosInternally();
                        scroll_amount_required -= 1;
                        //Scroll the player as well
                        Vector2 pos_ = this.transform.position;
                        pos_.y -= 0.32f;
                        this.transform.position = pos_;
                    }
                }
            }

            //Scrolling down
            if (scrolldir == 4)
            {
                delay = delay + 1;
                if (delay > maxdelay)
                {
                    delay = 0;
                    if (scroll_amount_required > 0)
                    {
                        tset.scrolly += 1;
                        scrollable.scrolly += 1;
                        tset.SetPosInternally();
                        scrollable.SetPosInternally();
                        scroll_amount_required -= 1;
                        //Scroll player too
                        Vector2 pos_ = this.transform.position;
                        pos_.y += 0.32f;
                        this.transform.position = pos_;
                    }
                }
            }

            if (scroll_amount_required <= 0)
            {
                Vector2 pos_ = this.transform.position;
                switch (scrolldir)
                {
                    
                    //Patch Player Coords at end of scroll so the player is in 
                    //A good position.
                    case 1:
                        pos_ = this.transform.position;
                        pos_.x = 19 * 0.32f - 0.16f;
                        this.transform.position = pos_;
                        break;
                    case 2:
                        pos_ = this.transform.position;
                        pos_.x = 0.32f;
                        this.transform.position = pos_;
                        break;
                    case 3:
                        pos_ = this.transform.position;
                        pos_.y = 0.32f;
                        this.transform.position = pos_;
                        break;
                    case 4:
                        pos_ = this.transform.position;
                        pos_.y = (19 * 0.32f) - 0.16f;
                        this.transform.position = pos_;
                        break;

                }

                //We aren't scrolling anymore.
                //Disable all scroll flags, and remove excess Tile Sprites.
                scrolling = false; //Disable scrolling.
                scrolldir = -1; //For safety.
                scroll_amount_required = 0; //No more scroll is required, uses whole integers.
                tset.destroyallsprites(); //Clears sprites of old TileSet/Draw instance. (erase tiles on screen and in mem)
                tset = scrollable; //This swaps the temporary scrollable TileSetMap with the Old one and moves the old one to the scrollable's new position.
            }

        }
        ///===End of Continuous Scrolling handler outside of one Update Cycle.===

            //Currently un-used. May interfere with raycast.
            if (spawnamount < 10)
            {
                //SpawnTestFire();
                //spawnamount += 1;
            }
            //Make them keep following you dynamically!
           // ManageFollowers();   
    }

    //This function currently unused. Will refactor later. It's for spawning Entities. =p
    public void SpawnTestFire()
    {
        float thisx = this.transform.position.x;
        float thisy = this.transform.position.y;
        //GameObject obj = Instantiate(testfireball);
        FireTest obj = Instantiate(testfireball, transform.position, transform.rotation);
        float posx = UnityEngine.Random.Range(thisx - 1f, thisx + 1f);
        float posy = UnityEngine.Random.Range(thisy - 1f, thisy + 1f);
        Vector2 SpawnPos = new Vector2(posx, posy);
        obj.transform.position = SpawnPos;
        obj.SetTarget(thisx, thisy);
        SpriteRenderer r = obj.GetComponent<SpriteRenderer>();
        r.sortingOrder = 5;
        //r.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        // obj.transform.TransformDirection(Vector3.forward * 10);
        Sprite s = obj.GetComponent<SpriteRenderer>().sprite;
       
        //s = Sprite.Create(tset.TileSetArray[Random.Range(0, 100)], new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        //obj.GetComponent<SpriteRenderer>().sprite = s;
        followers.Add(obj);
    }

    public void AnimationChecks()
    {
        //There's clearly no more.
        if (scroll_poofs.Count <= 0)
        {
            if (AnimationCheckScrollLeft) { AnimationCheckScrollLeft = false; Move_Scroll_Lock = false; Init_Scroll_Left = true; }
            if (AnimationCheckScrollRight) { AnimationCheckScrollRight = false; Move_Scroll_Lock = false; Init_Scroll_Right = true; }
            if (AnimationCheckScrollUp) { AnimationCheckScrollUp = false; Move_Scroll_Lock = false; Init_Scroll_Up = true; }
            if (AnimationCheckScrollDown) { AnimationCheckScrollDown = false; Move_Scroll_Lock = false; Init_Scroll_Down = true; }
            return;

        }

        //Index which Entities are done their animation remove and de spawn
        List<GameEntityAnimated> removable = scroll_poofs.FindAll(x => x.IsAnimationRunning() == false);
        removable.All(x => x.Despawn() == true);
        scroll_poofs.RemoveAll(x => x.is_despawned() == true);
        foreach(GameEntityAnimated e in scroll_poofs) { e.RunContinuousFrameAnimation(); }

    }

    //Manage the spawned Entities. Will re-factor later.
    public void ManageFollowers()
    {
        float thisx = this.transform.position.x;
        float thisy = this.transform.position.y;
        foreach (FireTest f in followers)
        {
            f.UpdateTarget(thisx, thisy);
        }

        //Enable this for cool stuff! turn it off for not
        //==TileEater== 1.0 XD
        //=================================
        //tset.set_target(thisx, thisy);
        //tset.ManageFollowers();
        //================================
    }

 }



