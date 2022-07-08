namespace MHR_Editor.Common {
    public static class BitMasks {
        public const uint ARMOR_TYPE_BIT_MASK  = 0xFFF00000;
        public const uint ARMOR_ID_BIT_MASK    = ARMOR_TYPE_BIT_MASK ^ 0xFFFFFFFF;
        public const uint LAYERED_ID_BIT_MASK  = 0x0FFF;
        public const uint ITEM_TYPE_BIT_MASK   = 0xFFF00000;
        public const uint ITEM_ID_BIT_MASK     = ITEM_TYPE_BIT_MASK ^ 0xFFFFFFFF;
        public const uint WEAPON_TYPE_BIT_MASK = 0xFFF00000;
        public const uint WEAPON_ID_BIT_MASK   = WEAPON_TYPE_BIT_MASK ^ 0xFFFFFFFF;
    }
}