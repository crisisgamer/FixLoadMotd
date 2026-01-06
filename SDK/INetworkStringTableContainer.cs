using CounterStrikeSharp.API;

namespace FixLoadMotd;

internal unsafe class INetworkStringTableContainer : NativeObject
{
    private readonly nint* _vtable;

    public INetworkStringTableContainer(nint pointer) : base(pointer)
    {
        if (Handle == nint.Zero)
        {
            throw new Exception("Failed to create INetworkStringTableContainer");
        }

        _vtable = *(nint**)Handle;
    }

    public INetworkStringTable? FindTable(string tableName)
    {
        nint pTable = ((delegate* unmanaged<nint, string, nint>)_vtable[14 + Plugin.LINUX_OFFSET_PREDICT])(Handle, tableName);
        return pTable != nint.Zero ? new INetworkStringTable(pTable) : null;
    }
}