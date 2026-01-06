using System.Runtime.CompilerServices;
using CounterStrikeSharp.API;

namespace FixLoadMotd;

internal partial struct SetStringUserDataRequest_t;

internal unsafe class INetworkStringTable : NativeObject
{
    private readonly nint* _vtable;

    public const uint INVALID_STRING_INDEX = uint.MaxValue;

    public INetworkStringTable(nint pointer) : base(pointer)
    {
        _vtable = *(nint**)Handle;
    }

    public uint AddString(bool bIsServer, string value, ref SetStringUserDataRequest_t pUserData)
    {
        return ((delegate* unmanaged<nint, bool, string, nint, uint>)_vtable[7 + Plugin.LINUX_OFFSET_PREDICT])(Handle, bIsServer, value, (nint)Unsafe.AsPointer(ref pUserData));
    }
}