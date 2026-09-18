using System;
using System.Runtime.CompilerServices;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

// ReSharper disable ALL

namespace NativeSockets
{
    /// <summary>
    ///     Ipv4/Ipv6 address parsing and formatting.
    /// </summary>
    /// <remarks>https://github.com/dotnet/runtime/blob/main/src/libraries/System.Net.Primitives/src/System/Net/IPAddressParser.cs</remarks>
    internal static class IpAddressParser
    {
        /// <summary>
        ///     Parses an Ipv4 address string into 4 bytes in network byte order.
        /// </summary>
        /// <param name="ipv4AddrText">
        ///     The address text to parse.
        ///     Must contain only the address, with no trailing whitespace or port.
        /// </param>
        /// <param name="destination">
        ///     When this method returns, receives the 4 address bytes in network byte order.
        ///     Must be at least 4 bytes long.
        /// </param>
        /// <typeparam name="TChar">
        ///     The character type,
        ///     either <see cref="char" /> or <see cref="byte" /> (ASCII).
        /// </typeparam>
        /// <returns>
        ///     <see langword="true" /> if parsing succeeded;
        ///     otherwise <see langword="false" />.
        /// </returns>
        /// <remarks>
        ///     Accepts the canonical dotted-quad form (e.g. <c>192.168.1.1</c>) as well as the
        ///     non-canonical forms historically accepted by inet_aton:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 One to three dot-separated fields,
        ///                 with shorter forms padded with zero bytes (e.g. <c>127.1</c> → 127.0.0.1, <c>1.2.3</c> → 1.2.0.3).
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>Hexadecimal fields prefixed with <c>0x</c> (e.g. <c>0x7f000001</c>).</description>
        ///         </item>
        ///         <item>
        ///             <description>Octal fields prefixed with a leading zero (e.g. <c>0177.0.0.1</c>).</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static bool TryParseIpv4<TChar>(ReadOnlySpan<TChar> ipv4AddrText, Span<byte> destination) where TChar : unmanaged
#if NET7_0_OR_GREATER
            , IBinaryInteger<TChar>
#endif
        {
            if (typeof(TChar) != typeof(char) && typeof(TChar) != typeof(byte))
                return false;

            if (destination.Length < 4)
                return false;

            long ipv4Addr = ParseIpv4NonCanonical(ipv4AddrText, out int end);
            if (ipv4Addr == -1 || end != ipv4AddrText.Length)
                return false;

            BinaryPrimitivesHelpers.WriteUInt32BigEndian(destination, (uint)ipv4Addr);
            return true;
        }

        /// <summary>
        ///     Parses an Ipv6 address string into 16 bytes in network byte order.
        /// </summary>
        /// <param name="ipv6AddrText">
        ///     The address text to parse.
        ///     Must contain only the address, with no surrounding brackets, no scope id, and no port.
        /// </param>
        /// <param name="destination">
        ///     When this method returns, receives the 16 address bytes in network byte order.
        ///     Must be at least 16 bytes long.
        /// </param>
        /// <typeparam name="TChar">
        ///     The character type,
        ///     either <see cref="char" /> or <see cref="byte" /> (ASCII).
        /// </typeparam>
        /// <returns>
        ///     <see langword="true" /> if parsing succeeded;
        ///     otherwise <see langword="false" />.
        /// </returns>
        /// <remarks>
        ///     Accepts the standard colon-separated hexadecimal form with <c>::</c> compression
        ///     of the longest zero run, and supports embedded Ipv4 addresses in the final two
        ///     16-bit groups (e.g. <c>::ffff:192.168.1.1</c>).
        ///     Does <b>not</b> accept surrounding brackets (<c>[::1]</c>), scope IDs (<c>%eth0</c>),
        ///     or trailing CIDR prefixes (<c>/64</c>).
        /// </remarks>
        public static bool TryParseIpv6<TChar>(ReadOnlySpan<TChar> ipv6AddrText, Span<byte> destination) where TChar : unmanaged
#if NET7_0_OR_GREATER
            , IBinaryInteger<TChar>
#endif
        {
            if (typeof(TChar) != typeof(char) && typeof(TChar) != typeof(byte))
                return false;

            if (destination.Length < 16 || !IsValidIpv6(ipv6AddrText))
                return false;

            Span<ushort> numbers = stackalloc ushort[8];
            numbers.Clear();
            ParseIpv6(ipv6AddrText, numbers);

            for (int i = 0; i < 8; ++i)
                BinaryPrimitivesHelpers.WriteUInt16BigEndian(destination.Slice(i * 2), numbers[i]);

            return true;
        }

        /// <summary>
        ///     Tries to format 4 bytes of a network-order Ipv4 address into the destination span.
        /// </summary>
        /// <param name="ipv4Addr">The 4 address bytes in network byte order.</param>
        /// <param name="destination">
        ///     The character span to receive the formatted dotted-quad string;
        ///     resized to the actual length on success.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> on success; <see langword="false" /> if the address is shorter than 4 bytes or the
        ///     destination is too small.
        /// </returns>
        public static bool TryFormatIpv4(ReadOnlySpan<byte> ipv4Addr, ref Span<char> destination)
        {
            if (ipv4Addr.Length < 4)
                return false;

            Span<char> chars = stackalloc char[16];
            int length = FormatIpv4(ipv4Addr, chars);
            chars = chars.Slice(0, length);
            if (chars.TryCopyTo(destination))
            {
                destination = destination.Slice(0, chars.Length);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Tries to format 16 bytes of a network-order Ipv6 address into the destination span.
        /// </summary>
        /// <param name="ipv6Addr">The 16 address bytes in network byte order.</param>
        /// <param name="destination">
        ///     The character span to receive the formatted dotted-quad string;
        ///     resized to the actual length on success.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> on success;
        ///     <see langword="false" /> if the address is shorter than 16 bytes
        ///     or the destination is too small.
        /// </returns>
        /// <remarks>
        ///     The longest run of zero 16-bit groups is compressed as <c>::</c>. Ipv4-mapped,
        ///     Ipv4-compatible, and ISATAP addresses are emitted with an embedded dotted-quad
        ///     suffix. No scope id is appended.
        /// </remarks>
        public static bool TryFormatIpv6(ReadOnlySpan<byte> ipv6Addr, ref Span<char> destination)
        {
            if (ipv6Addr.Length < 16)
                return false;

            Span<char> chars = stackalloc char[64];
            int length = FormatIpv6(ipv6Addr, chars);
            chars = chars.Slice(0, length);
            if (chars.TryCopyTo(destination))
            {
                destination = destination.Slice(0, chars.Length);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Formats a 4-byte Ipv4 address in dotted-decimal notation (e.g. <c>"192.168.0.1"</c>).
        /// </summary>
        /// <param name="ipv4Addr">The 4-byte address in network byte order.</param>
        /// <param name="destination">
        ///     The buffer to write into. Must be at least 15 characters long (the worst case is
        ///     <c>"255.255.255.255"</c>). Each octet is written without leading zeroes.
        /// </param>
        /// <returns>The number of characters written to <paramref name="destination" />.</returns>
        private static int FormatIpv4(ReadOnlySpan<byte> ipv4Addr, Span<char> destination)
        {
            int length = 0;

            length = FormatByte(ipv4Addr[0], destination, length);
            destination[length++] = '.';
            length = FormatByte(ipv4Addr[1], destination, length);
            destination[length++] = '.';
            length = FormatByte(ipv4Addr[2], destination, length);
            destination[length++] = '.';
            length = FormatByte(ipv4Addr[3], destination, length);

            return length;
        }

        /// <summary>
        ///     Formats a 16-byte Ipv6 address in canonical compressed form (RFC 5952).
        /// </summary>
        /// <param name="ipv6Addr">The 16-byte address in network byte order.</param>
        /// <param name="destination">
        ///     The buffer to write into. Must be large enough to hold the longest possible output
        ///     (up to ~64 characters, including a dotted-quad suffix for embedded Ipv4 forms).
        ///     Each 16-bit group is written as lowercase hexadecimal without leading zeroes, and the
        ///     longest run of zero groups is compressed with <c>"::"</c>. Ipv4-mapped, Ipv4-compatible,
        ///     and ISATAP addresses emit the trailing four bytes as a dotted-quad suffix. No scope id
        ///     is appended.
        /// </param>
        /// <returns>The number of characters written to <paramref name="destination" />.</returns>
        private static int FormatIpv6(ReadOnlySpan<byte> ipv6Addr, Span<char> destination)
        {
            Span<ushort> numbers = stackalloc ushort[8];
            for (int i = 0; i < 8; ++i)
                numbers[i] = BinaryPrimitivesHelpers.ReadUInt16BigEndian(ipv6Addr.Slice(i * 2));

            int length = 0;
            if (ShouldHaveIpv4Embedded(numbers))
            {
                AppendSections(numbers.Slice(0, 6), destination, ref length);
                if (destination[length - 1] != ':')
                    destination[length++] = ':';

                length += FormatIpv4(ipv6Addr.Slice(12, 4), destination.Slice(length));
            }
            else
            {
                AppendSections(numbers, destination, ref length);
            }

            return length;
        }

        /// <summary>
        ///     Parses an Ipv4 address with non-canonical forms (octal/hex prefixes, shortened field counts).
        /// </summary>
        /// <param name="ipv4AddrText">The input span.</param>
        /// <param name="end">
        ///     Receives the index just past the last consumed address character.
        ///     A terminator such as '/', '\\', ':', '?', or '#' is treated as ending the address without being consumed.
        /// </param>
        /// <returns>The parsed address as a 32-bit integer in host byte order, or -1 on failure.</returns>
        private static long ParseIpv4NonCanonical<TChar>(ReadOnlySpan<TChar> ipv4AddrText, out int end) where TChar : unmanaged
        {
            end = 0;
            int ch = 0;
            Span<long> parts = stackalloc long[3];
            long currentValue = 0;
            bool atLeastOneChar = false;
            int dotCount = 0;
            int current = 0;

            for (; current < ipv4AddrText.Length; ++current)
            {
                ch = CharValue(ipv4AddrText[current]);
                currentValue = 0;
                int numberBase = 10;

                if (ch == '0')
                {
                    ++current;
                    atLeastOneChar = true;
                    if (current < ipv4AddrText.Length)
                    {
                        ch = CharValue(ipv4AddrText[current]);
                        if (ch is 'x' or 'X')
                        {
                            numberBase = 16;
                            ++current;
                            atLeastOneChar = false;
                        }
                        else
                        {
                            numberBase = 8;
                        }
                    }
                }

                for (; current < ipv4AddrText.Length; ++current)
                {
                    ch = CharValue(ipv4AddrText[current]);
                    int digitValue = HexConverter.FromChar(ch);
                    if (digitValue >= numberBase)
                        break;

                    currentValue = currentValue * numberBase + digitValue;
                    if (currentValue > uint.MaxValue)
                        return -1;

                    atLeastOneChar = true;
                }

                if (current < ipv4AddrText.Length && ch == '.')
                {
                    if (dotCount >= 3 || !atLeastOneChar || currentValue > 0xFF)
                        return -1;

                    parts[dotCount] = currentValue;
                    ++dotCount;
                    atLeastOneChar = false;
                    continue;
                }

                break;
            }

            if (!atLeastOneChar)
                return -1;

            if (current >= ipv4AddrText.Length)
                end = ipv4AddrText.Length;
            else if (ch == '/' || ch == '\\' || ch is ':' or '?' or '#')
                end = current;
            else
                return -1;

            return dotCount switch
            {
                0 => currentValue,
                1 => currentValue > 0xffffff ? -1 : (parts[0] << 24) | currentValue,
                2 => currentValue > 0xffff ? -1 : (parts[0] << 24) | (parts[1] << 16) | currentValue,
                3 => currentValue > 0xff ? -1 : (parts[0] << 24) | (parts[1] << 16) | (parts[2] << 8) | currentValue,
                _ => -1
            };
        }

        /// <summary>
        ///     Finds the longest run of consecutive zero 16-bit groups for <c>::</c> compression.
        /// </summary>
        /// <param name="numbers">The 8 parsed Ipv6 groups in host byte order.</param>
        /// <param name="start">
        ///     Receives the inclusive start of the longest zero run;
        ///     or -1 if no run of length &gt; 1 exists.
        /// </param>
        /// <param name="end">
        ///     Receives the exclusive end of the longest zero run;
        ///     or 0 if no run of length &gt; 1 exists.
        /// </param>
        private static void FindCompressionRange(ReadOnlySpan<ushort> numbers, out int start, out int end)
        {
            int longestLength = 0, longestStart = -1, currentLength = 0;
            for (int i = 0; i < numbers.Length; ++i)
            {
                if (numbers[i] == 0)
                {
                    ++currentLength;
                    if (currentLength > longestLength)
                    {
                        longestLength = currentLength;
                        longestStart = i - currentLength + 1;
                    }
                }
                else
                {
                    currentLength = 0;
                }
            }

            if (longestLength > 1)
            {
                start = longestStart;
                end = longestStart + longestLength;
            }
            else
            {
                start = -1;
                end = 0;
            }
        }

        /// <summary>
        ///     Determines whether the address should be emitted with an embedded dotted-quad Ipv4 suffix.
        /// </summary>
        /// <param name="numbers">The 8 parsed Ipv6 groups in host byte order.</param>
        /// <returns>
        ///     <see langword="true" /> for Ipv4-mapped (<c>::ffff:a.b.c.d</c>),
        ///     Ipv4-compatible (<c>::a.b.c.d</c>),
        ///     or ISATAP (<c>::5EFE:a.b.c.d</c>) addresses.
        ///     otherwise <see langword="false" />.
        /// </returns>
        private static bool ShouldHaveIpv4Embedded(ReadOnlySpan<ushort> numbers)
        {
            if (numbers[0] == 0 && numbers[1] == 0 && numbers[2] == 0 && numbers[3] == 0 && numbers[6] != 0)
            {
                if (numbers[4] == 0 && (numbers[5] == 0 || numbers[5] == 0xFFFF))
                    return true;

                if (numbers[4] == 0xFFFF && numbers[5] == 0)
                    return true;
            }

            return numbers[4] == 0 && numbers[5] == 0x5EFE;
        }

        /// <summary>
        ///     Appends colon-separated hexadecimal groups to the destination,
        ///     compressing the longest zero run as <c>::</c>.
        /// </summary>
        /// <param name="numbers">The groups to emit (in host byte order).</param>
        /// <param name="destination">The output span.</param>
        /// <param name="length">The current write offset; advanced by the number of characters written.</param>
        private static void AppendSections(ReadOnlySpan<ushort> numbers, Span<char> destination, ref int length)
        {
            FindCompressionRange(numbers, out int zeroStart, out int zeroEnd);
            bool needsColon = false;

            if (zeroStart >= 0)
            {
                for (int i = 0; i < zeroStart; ++i)
                {
                    if (needsColon)
                        destination[length++] = ':';

                    needsColon = true;
                    AppendHex(numbers[i], destination, ref length);
                }

                destination[length++] = ':';
                destination[length++] = ':';
                needsColon = false;
            }

            for (int i = zeroEnd; i < numbers.Length; ++i)
            {
                if (needsColon)
                    destination[length++] = ':';

                needsColon = true;
                AppendHex(numbers[i], destination, ref length);
            }
        }

        /// <summary>
        ///     Appends a single 16-bit Ipv6 group as lowercase hexadecimal without leading zeros.
        /// </summary>
        /// <param name="value">The group in host byte order.</param>
        /// <param name="destination">The output span.</param>
        /// <param name="length">The current write offset; advanced by the number of hex digits written (1-4).</param>
        private static void AppendHex(ushort value, Span<char> destination, ref int length)
        {
            if ((value & 0xFFF0) != 0)
            {
                if ((value & 0xFF00) != 0)
                {
                    if ((value & 0xF000) != 0)
                        destination[length++] = HexConverter.ToCharLower((value >> 12) & 0xF);

                    destination[length++] = HexConverter.ToCharLower((value >> 8) & 0xF);
                }

                destination[length++] = HexConverter.ToCharLower((value >> 4) & 0xF);
            }

            destination[length++] = HexConverter.ToCharLower(value & 0xF);
        }

        /// <summary>
        ///     Performs a structural validation pass over an Ipv6 address string.
        /// </summary>
        /// <param name="ipv6AddrText">The input span.</param>
        /// <typeparam name="TChar">
        ///     The character type,
        ///     either <see cref="char" /> or <see cref="byte" /> (ASCII).
        /// </typeparam>
        /// <returns>
        ///     <see langword="true" /> if the string has a syntactically valid Ipv6 shape;
        ///     otherwise <see langword="false" />.
        /// </returns>
        /// <remarks>
        ///     Validates group count, single <c>::</c> compression, embedded Ipv4 forms, and rejects
        ///     brackets, scope IDs, and CIDR prefixes.
        /// </remarks>
        private static bool IsValidIpv6<TChar>(ReadOnlySpan<TChar> ipv6AddrText) where TChar : unmanaged
        {
            int sequenceCount = 0;
            int sequenceLength = 0;
            bool haveCompressor = false;
            bool haveIpv4Address = false;
            bool expectingNumber = true;
            int lastSequence = 1;
            int end = ipv6AddrText.Length;

            if (end == 0)
                return false;

            if (CharValue(ipv6AddrText[0]) == ':' && (end < 2 || CharValue(ipv6AddrText[1]) != ':'))
                return false;

            for (int i = 0; i < end; ++i)
            {
                int currentCh = CharValue(ipv6AddrText[i]);
                if (HexConverter.IsHexChar(currentCh))
                {
                    ++sequenceLength;
                    expectingNumber = false;
                    continue;
                }

                if (sequenceLength > 4)
                    return false;

                if (sequenceLength != 0)
                {
                    ++sequenceCount;
                    lastSequence = i - sequenceLength;
                    sequenceLength = 0;
                }

                switch (currentCh)
                {
                    case ':':
                        if (i > 0 && CharValue(ipv6AddrText[i - 1]) == ':')
                        {
                            if (haveCompressor)
                                return false;

                            haveCompressor = true;
                            expectingNumber = false;
                        }
                        else
                        {
                            expectingNumber = true;
                        }

                        break;

                    case '.':
                        if (haveIpv4Address)
                            return false;

                        if (!IsValidIpv4Embedded(ipv6AddrText.Slice(lastSequence, end - lastSequence)))
                            return false;

                        i = lastSequence;
                        while (i < end && CharValue(ipv6AddrText[i]) != '/')
                            ++i;

                        --i;
                        ++sequenceCount;
                        haveIpv4Address = true;
                        break;

                    default:
                        return false;
                }
            }

            if (sequenceLength != 0)
            {
                if (sequenceLength > 4)
                    return false;

                ++sequenceCount;
            }

            return !expectingNumber && (haveCompressor ? sequenceCount < 8 : sequenceCount == 8);
        }

        /// <summary>
        ///     Validates the dotted-quad suffix of an embedded Ipv4 address inside an Ipv6 literal.
        /// </summary>
        /// <param name="ipv6AddrText">The span starting at the first digit of the Ipv4 suffix.</param>
        /// <typeparam name="TChar">The character type.</typeparam>
        /// <returns>
        ///     <see langword="true" /> if the suffix is a valid dotted-quad with three dots and no octal leading zeros.
        ///     otherwise <see langword="false" />.
        /// </returns>
        private static bool IsValidIpv4Embedded<TChar>(ReadOnlySpan<TChar> ipv6AddrText) where TChar : unmanaged
        {
            int dots = 0;
            long number = 0;
            bool haveNumber = false;

            for (int start = 0; start < ipv6AddrText.Length; ++start)
            {
                int ch = CharValue(ipv6AddrText[start]);
                if (ch == '/')
                    break;

                uint digit = (uint)(ch - '0');
                if (digit < 10)
                {
                    if (!haveNumber && digit == 0 && (uint)(start + 1) < (uint)ipv6AddrText.Length && IsAsciiDigit(CharValue(ipv6AddrText[start + 1])))
                        return false;

                    haveNumber = true;
                    number = number * 10 + digit;

                    if (number > byte.MaxValue)
                        return false;
                }
                else if (ch == '.')
                {
                    if (!haveNumber)
                        return false;

                    ++dots;
                    haveNumber = false;
                    number = 0;
                }
                else
                {
                    return false;
                }
            }

            return dots == 3 && haveNumber;
        }

        /// <summary>
        ///     Populates the 8 Ipv6 groups from a validated address string.
        /// </summary>
        /// <param name="ipv6AddrText">The input span (already accepted by <see cref="IsValidIpv6{TChar}" />).</param>
        /// <param name="numbers">
        ///     Receives the 8 groups in host byte order.
        ///     The longest zero run is filled with zeros according to the <c>::</c> compressor position.
        /// </param>
        /// <typeparam name="TChar">The character type.</typeparam>
        private static void ParseIpv6<TChar>(ReadOnlySpan<TChar> ipv6AddrText, Span<ushort> numbers) where TChar : unmanaged
        {
            int number = 0;
            int index = 0;
            int compressorIndex = -1;
            bool numberIsValid = true;

            for (int i = 0; i < ipv6AddrText.Length;)
            {
                int ch = CharValue(ipv6AddrText[i]);
                switch (ch)
                {
                    case ':':
                        numbers[index++] = (ushort)number;
                        number = 0;
                        ++i;
                        if (CharValue(ipv6AddrText[i]) == ':')
                        {
                            compressorIndex = index;
                            ++i;
                        }
                        else if (compressorIndex < 0 && index < 6)
                        {
                            break;
                        }

                        for (int j = i; j < ipv6AddrText.Length && CharValue(ipv6AddrText[j]) != ':' && j < i + 4; ++j)
                        {
                            if (CharValue(ipv6AddrText[j]) == '.')
                            {
                                while (j < ipv6AddrText.Length && CharValue(ipv6AddrText[j]) != '/')
                                    ++j;

                                int ipv4 = ParseIpv4HostNumber(ipv6AddrText, i, j);
                                numbers[index++] = (ushort)(ipv4 >> 16);
                                numbers[index++] = (ushort)(ipv4 & 0xFFFF);
                                i = j;
                                number = 0;
                                numberIsValid = false;
                                break;
                            }
                        }

                        break;

                    case '/':
                        if (numberIsValid)
                        {
                            numbers[index++] = (ushort)number;
                            numberIsValid = false;
                        }

                        i = ipv6AddrText.Length;
                        break;

                    default:
                        number = number * 16 + HexConverter.FromChar(ch);
                        ++i;
                        break;
                }
            }

            if (numberIsValid)
                numbers[index++] = (ushort)number;

            if (compressorIndex > 0)
            {
                int toIndex = 7;
                int fromIndex = index - 1;
                if (fromIndex != toIndex)
                {
                    for (int i = index - compressorIndex; i > 0; --i)
                    {
                        numbers[toIndex--] = numbers[fromIndex];
                        numbers[fromIndex--] = 0;
                    }
                }
            }
        }

        /// <summary>
        ///     Parses a dotted-quad Ipv4 suffix into a 32-bit integer in host byte order.
        /// </summary>
        /// <param name="ipv6AddrText">The full input span.</param>
        /// <param name="start">Index of the first digit of the suffix.</param>
        /// <param name="end">Index one past the last character of the suffix.</param>
        /// <typeparam name="TChar">The character type.</typeparam>
        /// <returns>The Ipv4 address as a 32-bit integer in host byte order (big-endian bytes on disk).</returns>
        private static int ParseIpv4HostNumber<TChar>(ReadOnlySpan<TChar> ipv6AddrText, int start, int end) where TChar : unmanaged
        {
            Span<byte> numbers = stackalloc byte[4];
            int ni = 0;
            for (int i = start; i < end && ni < 4; ++i)
            {
                int b = 0;
                int ch;
                while (i < end && (ch = CharValue(ipv6AddrText[i])) != '.' && ch != ':')
                {
                    b = b * 10 + ch - '0';
                    ++i;
                }

                numbers[ni++] = (byte)b;
            }

            return BinaryPrimitivesHelpers.ReadInt32BigEndian(numbers);
        }

        /// <summary>
        ///     Formats a byte as a decimal number into the destination buffer, without leading zeros.
        /// </summary>
        /// <param name="value">The byte to format (0-255).</param>
        /// <param name="destination">The buffer to write into.</param>
        /// <param name="offset">The starting offset within <paramref name="destination" />.</param>
        /// <returns>The offset after the last written character.</returns>
        private static int FormatByte(byte value, Span<char> destination, int offset)
        {
            if (value >= 100)
            {
                int hundreds = Math.DivRem(value, 100, out int tensAndOnes);
                int tens = Math.DivRem(tensAndOnes, 10, out int ones);

                destination[offset] = (char)('0' + hundreds);
                destination[offset + 1] = (char)('0' + tens);
                destination[offset + 2] = (char)('0' + ones);
                return offset + 3;
            }

            if (value >= 10)
            {
                int tens = Math.DivRem(value, 10, out int ones);

                destination[offset] = (char)('0' + tens);
                destination[offset + 1] = (char)('0' + ones);
                return offset + 2;
            }

            destination[offset] = (char)('0' + value);
            return offset + 1;
        }

        /// <summary>
        ///     Returns whether the given integer code point is an ASCII digit (0-9).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAsciiDigit(int value) => (uint)(value - '0') <= 9;

        /// <summary>
        ///     Reads a single character as an integer code point.
        /// </summary>
        /// <typeparam name="TChar">
        ///     Must be <see cref="char" /> or <see cref="byte" />;
        ///     any other type yields <see cref="int.MaxValue" />.
        /// </typeparam>
        /// <returns>The unicode code point of the character.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CharValue<TChar>(TChar value) where TChar : unmanaged
        {
            if (typeof(TChar) == typeof(char))
                return Unsafe.As<TChar, char>(ref value);

            if (typeof(TChar) == typeof(byte))
                return Unsafe.As<TChar, byte>(ref value);

            return int.MaxValue;
        }
    }
}