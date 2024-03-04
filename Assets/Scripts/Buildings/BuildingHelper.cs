using System;
using System.Collections.Generic;

namespace Buildings {
    public class BuildingHelper {
        public static Dictionary<BuildingType, List<BuildingAction>> BuildingActions = new();

        public BuildingHelper() {
            var any = BuildingActions[ BuildingType.ANY ];
            any.Add( new BuildingAction(
                "Destroy",
                tile => tile.Destroy()
            ) );
        }
    }
}