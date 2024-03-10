using UnityEngine;

namespace GameItems {
    //  this could be a union in c++
    public class GameItemAttackProperties {
        public enum Area {
            SINGLE_UNIT,            // for explicit, immediate attacks like swords, arrows?
            SINGLE_TILE,            // for delayed attacks or traps like slow projectiles (rockets?), mines, etc
            SQUARE_AROUND_TARGET,
            CIRCLE_AROUND_TARGET,
            CONE_FROM_UNIT,
            CONE_BEHIND_TARGET
        }

        //  needs to be same as Cone_Height in case of cone from unit
        public float TargettingRange;
        //  type
        public Area EffectiveArea;

        //  SINGLE_TILE
        public Vector2Int Tile_Indices;     // use this for traps, for delayed attacks likely prefer to set a timer on the Tile object itself
        //  SQUARE_AROUND_TARGET
        public float Around_Range;
        //  both CONEs
        public float Cone_Angle;
        public float Cone_Height;

        public float BaseDamage;
        public bool AOEFalloff;
        public float AOEMinDamagePercent = 0.5f;      // For damage falloff over AOE, if applicable
    }
}