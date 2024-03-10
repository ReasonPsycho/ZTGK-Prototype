using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameItems {
    public abstract class GameItem {
        public string Name = "";
        public string Description = "";
        public Sprite icon;

        // taczka
        // public bool Mixable;
        // public unsafe GameItem* MixedItem;

        public float HealthBuff = 0;
        public float ArmorBuff = 0;
        public float FlatDamageBuff = 0;
        public float PercentDamageBuff = 0;
        public GameItemAttackProperties AttackProperties = null;
        public List<ItemAbility> Abilities = new();
    }
}