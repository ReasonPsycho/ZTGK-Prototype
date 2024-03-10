using UnityEngine;

namespace GameItems.ConcreteItems {
    public class GITidePodLauncher : GameItem {
        public GITidePodLauncher() {
            Name = "Tide Pod Launcher";
            Description =
                "High-tech piece of equipment, allowing to launch highly explosive TidePods from afar to wreak havoc upon The Dust.";
            icon = Resources.Load<Sprite>("axe-icon");

            AttackProperties = new GameItemAttackProperties() {
                TargettingRange = 5,
                EffectiveArea = GameItemAttackProperties.Area.CIRCLE_AROUND_TARGET,
                Around_Range = 1,
                BaseDamage = 15
            };

            ItemAbility ability = new() {
                Name = "Weakening foam.",
                Description = "Launches a special detergent missile which halves the armor of all enemies hit.",
                Cooldown = 30,
                AbilityProperties = new GameItemAttackProperties() {
                    TargettingRange = 5,
                    EffectiveArea = GameItemAttackProperties.Area.CIRCLE_AROUND_TARGET,
                    Around_Range = 2,
                    BaseDamage = 0
                },
                Use = args => {
                    foreach (var target in args.hit_units) {
                        target.Armor /= 2;
                    }
                }
            };

            Abilities.Add(ability);
        }
    }
}