namespace RE_Editor.Generated.Models;

public interface ICastingSpeed {
    public interface IAddRate {
        public float AddRateForPreparingSpellWhileWalking     { get; set; }
        public float AddRateForPreparingSpellWhileRunning     { get; set; }
        public float AddRateForPreparingSpellWhileLevitating  { get; set; }
        public float AddRateForPreparingSpellWhileConcentrate { get; set; }
    }

    public interface IPrepTime {
        public float PrepareTime { get; set; }
    }

    public interface ISecPrepare {
        public float SecPrepare { get; set; }
    }

    public interface ISecPrepareAndFocus {
        public float SecPrepare            { get; set; }
        public float SecPrepareFocus       { get; set; }
        public float SecPrepareLv2         { get; set; }
        public float SecPrepareFocusAndLv2 { get; set; }
    }
}