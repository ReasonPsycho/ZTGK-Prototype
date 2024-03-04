using System;

namespace Buildings {
    public class BuildingAction {
        public BuildingAction(string name, Func<Tile, bool> function, bool available = true) {
            Name = name;
            Function = function;
            Available = available;
        }

        public string Name;
        public bool Available;
        // requirements?
        // public Image icon;
        public Func<Tile, bool> Function;
    }
}