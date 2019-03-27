using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class EntityHandler
    {
        static List<GameEntity> LoadedGameEntities = new List<GameEntity>();
        static Dictionary<int, int> EntityObjectKeyValuePairs = new Dictionary<int, int>();
        public void SpawnEntity(int startx, int starty, int ItemID, Texture2D EntityGFX, int CustomID)
        {
            int id = UnityEngine.Random.Range(0, 5000);
            int EntityId = id.GetHashCode();
            if (CustomID > 0) { EntityId = CustomID; }
            bool collision = false;
            foreach (GameEntity e in LoadedGameEntities)
            {
                if (e.Get_ID() == EntityId) { collision = true; }
            }
            if (!collision)
            {
                GameEntity gameEntity = new GameEntity(EntityGFX, startx * 0.32f, starty * 0.32f, EntityGFX.width, EntityGFX.height, 5, true, 0, EntityId);
                int key = gameEntity.GetObj().GetInstanceID();
                int value = EntityId;
                EntityObjectKeyValuePairs.Add(key, value);
                LoadedGameEntities.Add(gameEntity);
            }
            else
            {
                Debug.Log("There was a collision spawning a game entity");
            }
        }

        public void Despawn(int EntityID)
        {
            GameEntity gameEntity = LoadedGameEntities.FirstOrDefault(entity => entity.Get_ID() == EntityID);
            if (gameEntity != null)
            {
                int key = gameEntity.GetObj().GetInstanceID();
                EntityObjectKeyValuePairs.Remove(key);
                gameEntity.Despawn();
            }
        }

        public void Remove_Despawned()
        {
            LoadedGameEntities.RemoveAll(x => x.is_despawned() == true);
        }

        public List<GameEntity> GetAllEntities()
        {
            return LoadedGameEntities;
        }

        public GameEntity GetEntity(GameObject obj)
        {
            GameEntity gameEntity = null;
            int key = obj.GetInstanceID();
            if (EntityObjectKeyValuePairs.ContainsKey(key))
            {
                int value = EntityObjectKeyValuePairs[key];
                gameEntity = LoadedGameEntities.FirstOrDefault(entity => entity.Get_ID() == value);
            }
            return gameEntity;
        }

        public GameEntity GetEntityById(int id)
        {
            GameEntity returnentity = null;
            foreach(GameEntity E in LoadedGameEntities)
            {
                if (E.Get_ID() == id)
                {
                    returnentity = E;
                    break;
                }
            }
            return returnentity;
        }

        public void UpdateMovingEntities()
        {
            foreach(GameEntity E in LoadedGameEntities)
            {
                E.RunContinuousMovementStep();
            }
        }

        public void RunActionsOnEntityByID(int EntityID)
        {
            GameEntity ent = GetEntityById(EntityID);
            if (ent != null)
            {
                ent.RunAllActions();
            }
        }

        public void PassActionToEntityByID(int EntityID, Action<GameEntity> A)
        {
            GameEntity ent = GetEntityById(EntityID);
            ent.AddAction(A);
        }

        public void Despawn_Queued()
        {
            foreach(GameEntity E in LoadedGameEntities)
            {
                if (E.needs_despawn() == true)
                {
                    Despawn(E.Get_ID());
                }
            }
            Remove_Despawned();
        }

        public void RunActionsOnEntityByType(int internal_type)
        {
            //foreach(GameEntity E in LoadedGameEntities)
            //{
            //if (E.GetInteralType() == internal_type)
            //{
            // E.RunAllActions();
            //}
            //}

            //Should make this list safe?
            Action<GameEntity> run_action = (GameEntity E) => { E.RunAllActionsListSafe();};
            LoadedGameEntities.Where(x => x.NeedActionsRun() == true && x.GetInteralType() == internal_type).ToList().ForEach(run_action);
        }

    }
}