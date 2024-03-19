using System;

namespace Buildings {
    public class BuildingAction {
        public BuildingAction(string name, Func<Tile, bool> function, bool available = true) {
            Name = name;
            Function = function;
            Available = available;
        }

        // public BuildingAction(string name, Image icon, Func<Tile, bool> function, bool available = true) {
        //     Name = name;
        //     Function = function;
        //     Icon = icon;
        //     Available = available;
        // }

        public string Name;
        public bool Available;
        // requirements?
        //todo vvv, load via Resources.Load for constructor?
        // public Image Icon;
        //to/do this should take a list of tiles now instead, also Building should expose a function returning all of its outer neighbor tiles #prototyp
        public Func<Tile, bool> Function;
    }
}