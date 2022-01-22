using UnityEngine;

namespace Map
{
    public class PillarData : Movable
    {
        readonly Transform _top;
        readonly Transform _bot;
        readonly PillarPair _obj;
        public PillarData(PillarPair obj, ref MapMovementParams p)
        {
            _obj = obj;
            transform = obj.transform;
            mapParams = p;
            _top = obj.top;
            _bot = obj.bot;
        }

        public void SetSeparation(float dist)
        {
            // Remember, each unit is 16 pixels. If you want a 64 pixel gap, dist should be 4
            _top.localPosition = new Vector3(0, dist / 2, 0);
            _bot.localPosition = new Vector3(0, -dist / 2, 0);
            
            // Clear all destructible objects first!
            _obj.ResetHittable();
        }
        
        // Method to randomize destructible objects.
        // Have basically a random binary number, use the bits to indicate whether each one is on or off.
        // Do this for both of them
    }
}
