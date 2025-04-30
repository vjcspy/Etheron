namespace Etheron.Gameplay.Character.Player.Common.Components.MoveComp
{
    public enum MoveType
    {
        Walk,
        Run
    }
    public struct MoveCompData
    {
        public MoveType moveType;
        public float runSpeed;
        public float walkSpeed;
    }
}
