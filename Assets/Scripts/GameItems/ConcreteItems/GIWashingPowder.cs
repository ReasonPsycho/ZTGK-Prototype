using UnityEngine;

namespace GameItems.ConcreteItems {
    public class GIWashingPowder : GameItem {
        public GIWashingPowder() {
            Name = "Washing Powder";
            Description = "Adds a bit of dmg and a knockback";
            icon = Resources.Load<Sprite>("shield-alt-1-svgrepo-com");
            AttackProperties = new GameItemAttackProperties {
                EffectiveArea = GameItemAttackProperties.Area.SINGLE_UNIT,
                BaseDamage = 5
            };
            KnockBack = 2f;
            //HealthBuff = 400;
            //ArmorBuff = 50;
       
            
            ItemAbility ability = new() {
                Name = "Extra Foam",
                Description = "Introduces an extra strong washing solution and causes a small area-of-effect foam burst, dealing half damage to enemies around the target.",
                Cooldown = 20,
                AbilityProperties = new () {
                    EffectiveArea = GameItemAttackProperties.Area.CIRCLE_AROUND_TARGET,
                    TargettingRange = 1,
                    Around_Range = 1,
                    AOEFalloff = true,
                    AOEMinDamagePercent = 0.5f
                },
                Use = args => {
                    for (int i = 0; i < args.hits.Count; i++) {
                        // todo check range from target.tile and deal damage with falloff as necessary
                    }
                }
            };

            Abilities.Add(ability);
        }
    }
}