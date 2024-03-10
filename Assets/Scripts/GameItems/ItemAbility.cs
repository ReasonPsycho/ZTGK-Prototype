using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameItems {
    public class ItemAbility {
        public string Name;
        public string Description;
        public Image icon;
        public float Cooldown;
        public GameItemAttackProperties AbilityProperties;

        // only extract what is necessary
        public class Args {
            public GameObject cause = null;
            public Unit cause_unit = null;
            public Tile cause_tile = null;
            public GameItem cause_item = null;
            
            public GameObject target = null;
            public Unit target_unit = null;
            public Tile target_tile = null;
            public GameItem target_item = null;
            public Building target_building = null;

            public List<GameObject> hits = null;
            public List<Unit> hit_units = null;
            public List<Tile> hit_tiles = null;
            public List<GameItem> hit_items = null;
            public List<Building> hit_buildings = null;
        }

        public Action<Args> Use = null;

    }
}