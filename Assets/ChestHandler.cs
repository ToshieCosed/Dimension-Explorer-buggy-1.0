using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class ChestHandler
    {
        public List<Chest> LoadedChestEntities = new List<Chest>();
        static Dictionary<int, int> ChestEntityKeyValuePairs = new Dictionary<int, int>();
        public void SpawnSmallChest(int startx, int starty, int ItemID, Texture2D ChestOpen, Texture2D ChestClosed, bool OpenState, int CustomID)
        {
            //Debug.Log("This ran");
            if (OpenState)
            {
                int id = UnityEngine.Random.Range(0, 5000);
                int EntityId = id.GetHashCode();
                if (CustomID > 0) { EntityId = CustomID; }
                bool collision = false;
                foreach (Chest c in LoadedChestEntities)
                {
                    if (c.GetChestEntity().Get_ID() == EntityId) { collision = true; }
                }
                if (!collision)
                {
                    GameEntity chestEntity = new GameEntity(ChestOpen, startx * 0.32f, starty * 0.32f, 32f, 32f, 5, true, 2, EntityId);
                    Chest chest = new Chest(OpenState, startx, starty, ItemID);
                    chest.SetEntity(chestEntity);
                    LoadedChestEntities.Add(chest);
                    int key = chestEntity.GetObj().GetInstanceID();
                    int value = EntityId;
                    ChestEntityKeyValuePairs.Add(key, value);
                    Debug.Log("Spawned a small open chest");
                    foreach(KeyValuePair<int,int> p in ChestEntityKeyValuePairs)
                    {
                        Debug.Log("There is a chest stored here and it's ID is " + p.Value + " and it's key Obj id is " + p.Key);
                    }


                }else
                {
                    Debug.Log("There was a collision spawning a chest entity");
                }
            }

            if (!OpenState)
            {
                int id = UnityEngine.Random.Range(0, 5000);
                int EntityId = id.GetHashCode();
                if (CustomID > 0) { EntityId = CustomID; }
                bool collision = false;
                foreach (Chest c in LoadedChestEntities)
                {
                    if (c.GetChestEntity().Get_ID() == EntityId) { collision = true; }
                }
                if (!collision)
                {
                    GameEntity chestEntity = new GameEntity(ChestClosed, startx * 0.32f, starty * 0.32f, 32f, 32f, 5, true, 1, EntityId);
                    Chest chest = new Chest(OpenState, startx, starty, ItemID);
                    chest.SetEntity(chestEntity);
                    LoadedChestEntities.Add(chest);
                    int key = chestEntity.GetObj().GetInstanceID();
                    int value = EntityId;
                    ChestEntityKeyValuePairs.Add(key, value);
                    Debug.Log("Spawned a small closed chest");
                    foreach (KeyValuePair<int, int> p in ChestEntityKeyValuePairs)
                    {
                        Debug.Log("There is a chest stored here and it's ID is " + p.Value + " and it's key Obj id is " + p.Key);
                    }

                }
                else
                {
                    Debug.Log("There was a collision spawning a chest entity");
                }
            }


        }

        public void DespawnChest(int ChestID)
        {
            Chest c = LoadedChestEntities.FirstOrDefault(chest => chest.GetChestEntity().Get_ID() == ChestID);
            if (c != null)
            {
                LoadedChestEntities.Remove(c);
                int key = c.GetChestEntity().GetObj().GetInstanceID();
                ChestEntityKeyValuePairs.Remove(key);
                c.GetChestEntity().Despawn();
            }
        }

        public GameEntity GetEntity(GameObject obj)
        {
            GameEntity gameEntity = null;
            int key = obj.GetInstanceID();
            Debug.Log("ChestDictionary: the instance id is  + ["+key+"]");
            if (ChestEntityKeyValuePairs.ContainsKey(key))
            {
                Debug.Log("ChestDictionary: The key does exist in the dictionary");
                int value = ChestEntityKeyValuePairs[key];
                //Chest gamechest = LoadedChestEntities.Where<(entity => entity.GetChestEntity().Get_ID() == value);
                IEnumerable<Chest> query =
                LoadedChestEntities.Where(chest_ => chest_.GetChestEntity().GetObj().GetInstanceID() == key);
                Chest gamechest = query.FirstOrDefault();   

                if (gamechest != null)
                {
                    gameEntity = gamechest.GetChestEntity();
                    Debug.Log("I should have a game entity here");
                }
            }
            return gameEntity;
        }

        public GameEntity GetEntityByID(int EntityID)
        {
            GameEntity returnEntity = null;
            foreach (Chest chest in LoadedChestEntities) { 
               if (chest.GetChestEntity().Get_ID() == EntityID)
                {
                    returnEntity = chest.GetChestEntity();
                    break;
                }
            }
            return returnEntity;
        }

        public void UpdateMovingEntities()
        {
            foreach (Chest E in LoadedChestEntities)
            {
                GameEntity ChestEntity = E.GetChestEntity();
                if (ChestEntity.GetMovingState())
                {
                    ChestEntity.RunContinuousMovementStep();
                }
            }
        }
    }
}
