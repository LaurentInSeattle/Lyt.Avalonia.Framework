namespace Lyt.Reflector.IL;

/// <summary>
/// Extension methods for a <see cref="IReadOnlyList{T}"/>, principally to read
/// binary data from a read-only list of bytes.
/// </summary>
internal static class ReadOnlyListExtensions
{
	/// <summary>
	/// Read an unsigned 8 bit integer from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>An unsigned 8 bit value.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static byte ReadByte(this IReadOnlyList<byte> data, int offset)
	{
		CheckArguments(data, offset, sizeof(sbyte));
		return data[offset];
	}

	/// <summary>
	/// Read an array of bytes from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>An array of bytes.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/> -OR- <paramref name="count"/> is less than
	/// zero.
	/// </exception>
	internal static byte[] ReadBytes(this IReadOnlyList<byte> data, int offset, int count)
	{
		if (count <= 0)
			throw new ArgumentOutOfRangeException(nameof(count));

		CheckArguments(data, offset, count);
		var result = new byte[count];
		for (int index = 0; index < count; index++)
			result[index] = data[offset + index];
		return result;
	}

	/// <summary>
	/// Read an IEEE 64 bit floating point value from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>An IEEE 64 bit floating point value.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static double ReadDouble(this IReadOnlyList<byte> data, int offset)
	{
		CheckArguments(data, offset, sizeof(double));
		return BitConverter.ToDouble(BitConverter.IsLittleEndian ?
			data.GetRange(offset, sizeof(double)) :
			data.Reverse(offset, sizeof(double)), 0);
	}

	/// <summary>
	/// Read a compressed unsigned 32 bit integer from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <param name="count">The number of bytes that were read.</param>
	/// <returns>An unsigned 32 bit integer.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static uint ReadCompressedUInt32(this IReadOnlyList<byte> data, int offset,
		out int count)
	{
		CheckArguments(data, offset, 1);
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
		return (uint)(nextByte & 0x3F) << 24 |
			(uint)data.ReadByte(offset + 1) << 16 |
			(uint)data.ReadByte(offset + 2) << 8 |
			data.ReadByte(offset + 3);
	}

	/// <summary>
	/// Read a compressed token from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <param name="count">The number of bytes that were read.</param>
	/// <returns>An unsigned 32 bit integer.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentException">
	/// <paramref name="data"/> contains an unrecognized token type at the specified offset.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static Token ReadCompressedTypeDefOrRef(this IReadOnlyList<byte> data, int offset,
		out int count)
	{
		uint compressedToken = data.ReadCompressedUInt32(offset, out count);

		// Low two bits contain type and remainder contains bit-shifted RID
		int id = (int)(compressedToken >> 2);
		TokenType type;

		switch(compressedToken & 3)
		{
			case 0: type = TokenType.TypeDef; break;
			case 1: type = TokenType.TypeRef; break;
			case 2: type = TokenType.TypeSpec; break;
			case 3:
			default: throw new ArgumentException(nameof(offset));
		}

		return new Token(type, id);
	}

	/// <summary>
	/// Read a signed 16 bit integer from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>A signed 16 bit integer.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static short ReadInt16(this IReadOnlyList<byte> data, int offset)
	{
		CheckArguments(data, offset, sizeof(short));
		return (short)(data[offset] | data[offset + 1] << 8);
	}

	/// <summary>
	/// Read a signed 32 bit integer from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>A signed 32 bit integer.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static int ReadInt32(this IReadOnlyList<byte> data, int offset)
	{
		CheckArguments(data, offset, sizeof(int));
		return data[offset] | data[offset + 1] << 8 | data[offset + 2] << 16 |
			data[offset + 3] << 24;
	}

	/// <summary>
	/// Read a signed 64 bit integer from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>A signed 64 bit integer.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static long ReadInt64(this IReadOnlyList<byte> data, int offset)
	{
		CheckArguments(data, offset, sizeof(long));
		return data[offset] | data[offset + 1] << 8 | data[offset + 2] << 16 |
			data[offset + 3] << 24 | data[offset + 4] << 32 | data[offset + 5] << 40 |
			data[offset + 6] << 48 | data[offset + 7] << 56;
	}

	/// <summary>
	/// Read an operation code (opcode) value.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static short ReadOpCode(this IReadOnlyList<byte> data, ref int offset)
	{
		CheckArguments(data, offset, 1);

		short code = data[offset++];

		if (offset < data.Count && code == OpCodes.Prefix1.Value)
			code = (short)(code << 8 | data[offset++]);

		return code;
	}

	/// <summary>
	/// Read an IEEE 32 bit floating point value from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>An IEEE 32 bit floating point value.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static float ReadSingle(this IReadOnlyList<byte> data, int offset)
	{
		CheckArguments(data, offset, sizeof(float));
		return BitConverter.ToSingle(BitConverter.IsLittleEndian ?
			data.GetRange(offset, sizeof(float)) :
			data.Reverse(offset, sizeof(float)), 0);
	}

	/// <summary>
	/// Read a signed 8 bit integer from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>A signed 8 bit integer.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static sbyte ReadSByte(this IReadOnlyList<byte> data, int offset)
	{
		CheckArguments(data, offset, sizeof(sbyte));
		return (sbyte)data[offset];
	}

	/// <summary>
	/// Read a 32 bit token from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>A a 32 bit token.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static Token ReadToken(this IReadOnlyList<byte> data, int offset) =>
		new Token(ReadInt32(data, offset));

	/// <summary>
	/// Read an unsigned 16 bit integer from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>An unsigned 16 bit integer.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static ushort ReadUInt16(this IReadOnlyList<byte> data, int offset) =>
		(ushort)data.ReadInt16(offset);

	/// <summary>
	/// Read an unsigned 32 bit integer from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>An unsigned 32 bit integer.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static uint ReadUInt32(this IReadOnlyList<byte> data, int offset) =>
		(uint)data.ReadInt32(offset);

	/// <summary>
	/// Read an unsigned 64 bit integer from this list.
	/// </summary>
	/// <param name="data">The <see cref="IReadOnlyList{T}"/> that is extended.</param>
	/// <param name="offset">The zero-based offset of the data.</param>
	/// <returns>An unsigned 64 bit integer.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="data"/> is null.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// <paramref name="offset"/> is less than zero or beyond the end of
	/// <paramref name="data"/>.
	/// </exception>
	internal static ulong ReadUInt64(this IReadOnlyList<byte> data, int offset) =>
		(ulong)data.ReadInt64(offset);

	// Check the specified arguments for validity
	private static void CheckArguments(IReadOnlyList<byte> data, int offset, int size)
	{
		if (data == null)
			throw new ArgumentNullException(nameof(data));

		if (offset < 0 || offset + size > data.Count)
			throw new ArgumentOutOfRangeException(nameof(offset));
	}

	// Get an array that is a subset of this list
	private static byte[] GetRange(this IReadOnlyList<byte> data, int offset, int size)
	{
		var range = new byte[size];
		for (int index = 0; index < size; index++)
			range[index] = data[offset + index];
		return range;
	}

	// Get an array that is a reversed subset of this list
	private static byte[] Reverse(this IReadOnlyList<byte> data, int offset, int size)
	{
		var reversed = new byte[size];
		for (int index = 0; index < size; index++)
			reversed[size - index - 1] = data[offset + index];
		return reversed;
	}
}
