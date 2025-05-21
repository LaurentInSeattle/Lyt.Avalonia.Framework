namespace Lyt.Reflector.IL;

/// <summary> Extension methods for a byte array to read data from. </summary>
internal static class ByteArrayExtensions
{
    /// <summary> Read an unsigned 8 bit integer from this array .</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    internal static byte ReadByte(this byte[] data, int offset) => data[offset];

    /// <summary> Read a signed 8 bit integer from this array. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static sbyte ReadSByte(this byte[] data, int offset) => (sbyte)data[offset];

    /// <summary> Read an array of bytes from this array. </summary>
    /// <returns>An array of bytes.</returns>
    internal static byte[] ReadBytes(this IReadOnlyList<byte> data, int offset, int count)
    {
        byte[] result = new byte[count];
        for (int index = 0; index < count; ++index)
        {
            result[index] = data[offset + index];
        } 

        return result;
    }

    /// <summary> Read an IEEE 64 bit floating point value from this array. </summary>
    internal static double ReadDouble(this byte[] data, int offset)
        => BitConverter.ToDouble(
            BitConverter.IsLittleEndian ?
                data.GetRange(offset, sizeof(double)) :
                data.Reverse(offset, sizeof(double)), 0);

    /// <summary> Read a compressed unsigned 32 bit integer from this array. </summary>
    /// <returns> An unsigned 32 bit integer.</returns>
    internal static uint ReadCompressedUInt32(this byte[] data, int offset, out int count)
    {
        byte nextByte = data.ReadByte(offset);

        // 0XXXXXXX = single byte
        if ((nextByte & 0x80) == 0)
        {
            count = 1;
            return nextByte;
        }

        // 10XXXXXX XXXXXXXX = two bytes
        if ((nextByte & 0x40) == 0)
        {
            count = 2;
            return (uint)(nextByte & 0x7F) << 8 |
                data.ReadByte(offset + 1);
        }

        // 110XXXXX XXXXXXXX XXXXXXXX XXXXXXXX = four bytes
        // Technically, third bit should be clear but we'll return it anyway
        count = 4;
        return 
            (uint)(nextByte & 0x3F) << 24 |
            (uint)data.ReadByte(offset + 1) << 16 |
            (uint)data.ReadByte(offset + 2) << 8 |
            data.ReadByte(offset + 3);
    }

    /// <summary> Read a compressed token from this array. </summary>
    internal static Token ReadCompressedTypeDefOrRef(this byte[] data, int offset, out int count)
    {
        // Low two bits contain type and remainder contains bit-shifted RID
        uint compressedToken = data.ReadCompressedUInt32(offset, out count);
        int id = (int)(compressedToken >> 2);
        var type = (compressedToken & 3) switch
        {
            0 => TokenType.TypeDef,
            1 => TokenType.TypeRef,
            2 => TokenType.TypeSpec,
            _ => throw new ArgumentException(null, nameof(offset)),
        };

        return new Token(type, id);
    }

    /// <summary> Read a signed 16 bit integer from this array. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static short ReadInt16(this byte[] data, int offset)
        => (short)(data[offset] | data[offset + 1] << 8);

    /// <summary> Read a signed 32 bit integer from this array. </summary>
    internal static int ReadInt32(this byte[] data, int offset) => 
        data[offset] | 
        data[offset + 1] << 8 | 
        data[offset + 2] << 16 |
        data[offset + 3] << 24;

    /// <summary> Read a signed 64 bit integer from this array. </summary>
    internal static long ReadInt64(this byte[] data, int offset) =>
        data[offset] | data[offset + 1] << 8 | 
        data[offset + 2] << 16 | data[offset + 3] << 24 | 
        data[offset + 4] << 32 | data[offset + 5] << 40 |
        data[offset + 6] << 48 | data[offset + 7] << 56;

    /// <summary> Read an operation code (opcode) value.</summary>
    /// <remarks> offset is 'by ref' </remarks>
    internal static short ReadOpCode(this byte[] data, ref int offset)
    {
        short code = data[offset++];
        if (offset < data.Length && code == OpCodes.Prefix1.Value)
        {
            code = (short)(code << 8 | data[offset++]);
        }

        return code;
    }

    /// <summary> Read an IEEE 32 bit floating point value from this array. </summary>
    internal static float ReadSingle(this byte[] data, int offset)
        => BitConverter.ToSingle(
            BitConverter.IsLittleEndian ?
                data.GetRange(offset, sizeof(float)) :
                data.Reverse(offset, sizeof(float)), 0);

    /// <summary> Read a 32 bit token from this array.</summary>
    internal static Token ReadToken(this byte[] data, int offset) => new(ReadInt32(data, offset));

    /// <summary> Read an unsigned 16 bit integer from this array. </summary>
    internal static ushort ReadUInt16(this byte[] data, int offset) => (ushort)data.ReadInt16(offset);

    /// <summary> Read an unsigned 32 bit integer from this array. </summary>
    internal static uint ReadUInt32(this byte[] data, int offset) => (uint)data.ReadInt32(offset);

    /// <summary> Read an unsigned 64 bit integer from this array. </summary>
    internal static ulong ReadUInt64(this byte[] data, int offset) => (ulong)data.ReadInt64(offset);

    // Get an array that is a subset of this array
    private static byte[] GetRange(this byte[] data, int offset, int size)
    {
        byte[] range = new byte[size];
        for (int index = 0; index < size; index++)
        {
            range[index] = data[offset + index];
        }

        return range;
    }

    // Get an array that is a reversed subset of this array
    private static byte[] Reverse(this byte[] data, int offset, int size)
    {
        byte[] reversed = new byte[size];
        for (int index = 0; index < size; index++)
        {
            reversed[size - index - 1] = data[offset + index];
        } 

        return reversed;
    }
}
