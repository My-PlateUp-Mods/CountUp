using KitchenMods;
using MessagePack;
using Unity.Entities;

namespace Kitchen
{
    [MessagePackObject]
    public struct CCountUpAppliance : IModComponent
    {
        [Key(0)] public int Count;
        [Key(1)] public bool UseCount;
    }

    [MessagePackObject]
    public struct CCountUpItem : IModComponent
    {
        [Key(0)] public int ItemID;
        [Key(1)] public int Count;
        [Key(2)] public bool UndergoingProcess;
        [Key(3)] public bool IsPartial;
    }
}
