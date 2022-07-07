namespace MHR_Editor.Generated {
    public class BitMasks {
        public const uint ARMOR_TYPE_BIT_MASK = 0xFFF00000;
        public const uint ARMOR_ID_BIT_MASK   = ARMOR_TYPE_BIT_MASK ^ 0xFFFFFFFF;
        public const uint ITEM_TYPE_BIT_MASK  = 0xFFF00000;
        public const uint ITEM_ID_BIT_MASK    = ITEM_TYPE_BIT_MASK ^ 0xFFFFFFFF;
    }
}