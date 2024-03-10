namespace GameItems.ConcreteItems {
    public class GILanceMop : GameItem {
        public GILanceMop() {
            Name = "Lance Mop";
            Description = "Standard mop on a long stick. Originally intended for use by cavalry soldiers, but in these trying times, we will use anything.";

            AttackProperties = new GameItemAttackProperties() {
                TargettingRange = 2,
                EffectiveArea = GameItemAttackProperties.Area.RECTANGLE_FROM_UNIT,
                Rect_Length = 2,
                Rect_Width = 1,
                BaseDamage = 8
            };

            ItemAbility ability = new ItemAbility() {
                Name = "Thrust",
                Description = "The brave sponge gathers all of its foam and performs a strong thrust, piercing enemies in a longer range than usual for double the damage.",
                Cooldown = 20,
                AbilityProperties = new GameItemAttackProperties() {
                    TargettingRange = 2,
                    BaseDamage = 16,
                    EffectiveArea = GameItemAttackProperties.Area.RECTANGLE_FROM_UNIT,
                    Rect_Length = 3,
                    Rect_Width = 1
                },
                Use = args => {
                    foreach (var target in args.hit_units) {
                        target.TakeDmg(16);
                    }
                }
            };

            Abilities.Add(ability);
        }
    }
}