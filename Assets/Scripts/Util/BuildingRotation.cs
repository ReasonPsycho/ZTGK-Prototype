namespace Buildings {
    /// <summary>
    /// Utility class for managing a rotation integer.
    /// The rotation should be stored as an integer (or set of 4 enums) for future mesh rotation and related properties (such as rally point) placing purposes.
    /// </summary>
    public static class BuildingRotation {
        public static void cycleUp(ref int rotation) {
            rotation += 90;
            if ( rotation >= 360 ) rotation = 0;
        }

        public static void cycleDown(ref int rotation) {
            rotation -= 90;
            if ( rotation <= -90 ) rotation = 270;
        }

        public static bool isNormal(int rotation) {
            return rotation is 0 or 180;
        }

        public static bool isFlipped(int rotation) {
            return !isNormal(rotation);
        }
    }
}