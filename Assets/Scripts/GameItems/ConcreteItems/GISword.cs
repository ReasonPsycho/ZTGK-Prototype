﻿using UnityEngine;

namespace GameItems.ConcreteItems {
    public class GISword : GameItem {
        public GISword() {
            Name = "Foam sword";
            Description = "Major damage";
            icon = Resources.Load<Sprite>("sword-svgrepo-com");
            AttackProperties = new GameItemAttackProperties {
                TargettingRange = 0f,
                EffectiveArea = GameItemAttackProperties.Area.SINGLE_UNIT,
                BaseDamage = 20
            };

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