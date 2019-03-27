using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class EntityActionMethods
    {
        public List<Action<GameEntity>> provided_actions = new List<Action<GameEntity>>();
        EntityHandler GenericEntities = new EntityHandler();

        public void MethodA(GameEntity e)
        {
            Debug.Log("The passed action ran in the context of the class?");
            e.SetTransform(12 * 0.32f, 7 * 0.32f, 0);
        }

        public void Init()
        {
            Action<GameEntity> tempaction = new Action<GameEntity>(MethodA);
            provided_actions.Add(tempaction);
            Action<GameEntity> fireball_action = new Action<GameEntity>(ShootFireBallAt);
            provided_actions.Add(fireball_action);
        }

        public List<Action<GameEntity>> GetActions()
        {
            return this.provided_actions;
        }

        public void ShootFireBallAt(GameEntity E)
        {
            int id = UnityEngine.Random.Range(0, 65535);
            //Spawn a fireball with a known id
            GenericEntities.SpawnEntity(0, 0, 0, Resources.Load<Texture2D>("Textures/FireTestSprite"), id);
            //Change it's location on screen since we cannot do that with ints
            GameEntity spawned_fireball = GenericEntities.GetEntityById(id);
            //spawned_fireball.GetObj().name = "FireBall Shooted";
            spawned_fireball.SetTransform(E.GetCurrentPos().x, E.GetCurrentPos().y, 0);
            //Set the target of the fireball to another entity.
            spawned_fireball.SetTarget(E.GetTarget());
            float total_distance = Vector2.Distance(spawned_fireball.GetCurrentPos(), spawned_fireball.GetTarget());
            Vector2 XPointStart = new Vector2(spawned_fireball.GetCurrentPos().x, 0);
            Vector2 XPointTarget = new Vector2(spawned_fireball.GetTarget().x, 0);
            float X_dist = Vector2.Distance(XPointStart, XPointTarget);
            Vector2 YPointStart = new Vector2(0, spawned_fireball.GetCurrentPos().y);
            Vector2 YPointTarget = new Vector2(0, spawned_fireball.GetTarget().y);
            float Y_dist = Vector2.Distance(YPointStart, YPointTarget);
            bool negx = false;
            bool negy = false;
            if (XPointStart.x > XPointTarget.x) { negx = true; }
            if (YPointStart.y > YPointTarget.y) { negy = true; }

            //Neg/pos values
            float finalydist = 0;
            float finalxdist = 0;
            if (negx) { finalxdist = X_dist * -1; } else { finalxdist = X_dist; }
            if (negy) { finalydist = Y_dist * -1; }else { finalydist = Y_dist; }
            spawned_fireball.set_internal_type(99); //Setting the type so it can be recognized by RunAllActionsByInternalType ^_^
            spawned_fireball.ApplyGenericMovement(finalxdist, finalydist, total_distance * 15);
            Action<GameEntity> generic_movement = new Action<GameEntity>(RunGenericMovement);
            spawned_fireball.AddAction(generic_movement);
            Action<GameEntity> Despawn_After_Move = new Action<GameEntity>(DespawnIfTargetReached);
            spawned_fireball.AddAction(Despawn_After_Move);

        }

        public void DespawnIfTargetReached(GameEntity E)
        {
            Debug.Log("The despawn if target reached function ran");
            if (E.GetMovingState() == false)
            {
                //Since modifying the entity directly while
                //it's running an action is bad
                //We queue up despawn and let the generic engine handle it.
                Debug.Log("A despawn was queued");
                E.queue_despawn();
            }
        }

        public void RunGenericMovement(GameEntity E)
        {
            E.RunContinuousMovementStep();
        }

    }
}
