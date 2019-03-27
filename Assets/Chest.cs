using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class Chest
    {
       float Screenx;
       float Screeny;
       int ContainsItemId;
       bool openstate;
       GameEntity chestsprite;
       
        public Chest(bool open_state, int startx, int starty, int ItemInsideId)
        {
            open_state = openstate;
            Screenx = 0.32f * startx;
            Screeny = 0.32f * starty;
            ContainsItemId = ItemInsideId;
        }

        public GameEntity GetChestEntity()
        {
            return chestsprite;
        }

        public void SetEntity(GameEntity e)
        {
            this.chestsprite = e;
        }

    }
}