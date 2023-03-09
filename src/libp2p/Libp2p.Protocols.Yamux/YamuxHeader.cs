// SPDX-FileCopyrightText: 2023 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using System.Runtime.InteropServices;

namespace Nethermind.Libp2p.Protocols;

[StructLayout(LayoutKind.Explicit, Size = 12)]
internal struct YamuxHeader
{
    [FieldOffset(0)] public byte Version;
    [FieldOffset(1)] public YamuxHeaderType Type;
    [FieldOffset(2)] public YamuxHeaderFlags Flags;
    [FieldOffset(4)] public int StreamID;
    [FieldOffset(8)] public int Length;
}
