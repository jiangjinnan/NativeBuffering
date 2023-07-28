//#nullable enable
//using System;
//using System.Runtime.CompilerServices;
//using System.Collections.Generic;
//using NativeBuffering;
//using NativeBuffering.Dictionaries;
//using NativeBuffering.Collections;
//public unsafe readonly struct EntityBufferedMessage : IReadOnlyBufferedObject<EntityBufferedMessage>
//{
//    public NativeBuffer Buffer { get; }
//    public EntityBufferedMessage(NativeBuffer buffer) => Buffer = buffer;
//    public static EntityBufferedMessage Parse(NativeBuffer buffer) => new EntityBufferedMessage(buffer);
//    public System.Int64 Foo => Buffer.ReadUnmanagedField<System.Int64>(0);
//    public ref readonly UnmanangedStruct Bar => ref Buffer.ReadUnmanagedFieldAsRef<UnmanangedStruct>(1);
//    public BufferedBinary Baz => Buffer.ReadBufferedObjectField<BufferedBinary>(2);
//    public BufferedString Qux => Buffer.ReadBufferedObjectField<BufferedString>(3);
//}
//#nullable disable
