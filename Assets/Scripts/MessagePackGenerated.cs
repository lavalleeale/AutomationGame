// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Resolvers
{
    using System;

    public class GeneratedResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new GeneratedResolver();

        private GeneratedResolver()
        {
        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    Formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        private static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(9)
            {
                { typeof(global::SavedBuilding[]), 0 },
                { typeof(global::SavedItemStack[]), 1 },
                { typeof(global::SavedWorldItemStack[]), 2 },
                { typeof(global::Item.Type), 3 },
                { typeof(global::SavedBuilding.Type), 4 },
                { typeof(global::SaveData), 5 },
                { typeof(global::SavedBuilding), 6 },
                { typeof(global::SavedItemStack), 7 },
                { typeof(global::SavedWorldItemStack), 8 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key))
            {
                return null;
            }

            switch (key)
            {
                case 0: return new global::MessagePack.Formatters.ArrayFormatter<global::SavedBuilding>();
                case 1: return new global::MessagePack.Formatters.ArrayFormatter<global::SavedItemStack>();
                case 2: return new global::MessagePack.Formatters.ArrayFormatter<global::SavedWorldItemStack>();
                case 3: return new MessagePack.Formatters.Item_TypeFormatter();
                case 4: return new MessagePack.Formatters.SavedBuilding_TypeFormatter();
                case 5: return new MessagePack.Formatters.SaveDataFormatter();
                case 6: return new MessagePack.Formatters.SavedBuildingFormatter();
                case 7: return new MessagePack.Formatters.SavedItemStackFormatter();
                case 8: return new MessagePack.Formatters.SavedWorldItemStackFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1649 // File name should match first type name


// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters
{
    using System;
    using System.Buffers;
    using MessagePack;

    public sealed class Item_TypeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Item.Type>
    {
        public void Serialize(ref MessagePackWriter writer, global::Item.Type value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((Byte)value);
        }

        public global::Item.Type Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Item.Type)reader.ReadByte();
        }
    }

    public sealed class SavedBuilding_TypeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::SavedBuilding.Type>
    {
        public void Serialize(ref MessagePackWriter writer, global::SavedBuilding.Type value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((Byte)value);
        }

        public global::SavedBuilding.Type Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::SavedBuilding.Type)reader.ReadByte();
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name



// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters
{
    using System;
    using System.Buffers;
    using MessagePack;

    public sealed class SaveDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::SaveData>
    {


        private readonly global::MessagePack.Internal.AutomataDictionary ____keyMapping;
        private readonly byte[][] ____stringByteKeys;

        public SaveDataFormatter()
        {
            this.____keyMapping = new global::MessagePack.Internal.AutomataDictionary()
            {
                { "buildings", 0 },
                { "items", 1 },
            };

            this.____stringByteKeys = new byte[][]
            {
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("buildings"),
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("items"),
            };
        }

        public void Serialize(ref MessagePackWriter writer, global::SaveData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteMapHeader(2);
            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SavedBuilding[]>().Serialize(ref writer, value.buildings, options);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SavedWorldItemStack[]>().Serialize(ref writer, value.items, options);
        }

        public global::SaveData Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadMapHeader();
            var __buildings__ = default(global::SavedBuilding[]);
            var __items__ = default(global::SavedWorldItemStack[]);

            for (int i = 0; i < length; i++)
            {
                ReadOnlySpan<byte> stringKey = global::MessagePack.Internal.CodeGenHelpers.ReadStringSpan(ref reader);
                int key;
                if (!this.____keyMapping.TryGetValue(stringKey, out key))
                {
                    reader.Skip();
                    continue;
                }

                switch (key)
                {
                    case 0:
                        __buildings__ = formatterResolver.GetFormatterWithVerify<global::SavedBuilding[]>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __items__ = formatterResolver.GetFormatterWithVerify<global::SavedWorldItemStack[]>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::SaveData(__buildings__, __items__);
            ____result.buildings = __buildings__;
            ____result.items = __items__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class SavedBuildingFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::SavedBuilding>
    {


        private readonly global::MessagePack.Internal.AutomataDictionary ____keyMapping;
        private readonly byte[][] ____stringByteKeys;

        public SavedBuildingFormatter()
        {
            this.____keyMapping = new global::MessagePack.Internal.AutomataDictionary()
            {
                { "type", 0 },
                { "rotation", 1 },
                { "xPos", 2 },
                { "yPos", 3 },
                { "inputs", 4 },
                { "outputs", 5 },
            };

            this.____stringByteKeys = new byte[][]
            {
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("type"),
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("rotation"),
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("xPos"),
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("yPos"),
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("inputs"),
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("outputs"),
            };
        }

        public void Serialize(ref MessagePackWriter writer, global::SavedBuilding value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteMapHeader(6);
            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SavedBuilding.Type>().Serialize(ref writer, value.type, options);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.Write(value.rotation);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.Write(value.xPos);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.Write(value.yPos);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SavedItemStack[]>().Serialize(ref writer, value.inputs, options);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SavedItemStack[]>().Serialize(ref writer, value.outputs, options);
        }

        public global::SavedBuilding Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadMapHeader();
            var __type__ = default(global::SavedBuilding.Type);
            var __rotation__ = default(byte);
            var __xPos__ = default(float);
            var __yPos__ = default(float);
            var __inputs__ = default(global::SavedItemStack[]);
            var __outputs__ = default(global::SavedItemStack[]);

            for (int i = 0; i < length; i++)
            {
                ReadOnlySpan<byte> stringKey = global::MessagePack.Internal.CodeGenHelpers.ReadStringSpan(ref reader);
                int key;
                if (!this.____keyMapping.TryGetValue(stringKey, out key))
                {
                    reader.Skip();
                    continue;
                }

                switch (key)
                {
                    case 0:
                        __type__ = formatterResolver.GetFormatterWithVerify<global::SavedBuilding.Type>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __rotation__ = reader.ReadByte();
                        break;
                    case 2:
                        __xPos__ = reader.ReadSingle();
                        break;
                    case 3:
                        __yPos__ = reader.ReadSingle();
                        break;
                    case 4:
                        __inputs__ = formatterResolver.GetFormatterWithVerify<global::SavedItemStack[]>().Deserialize(ref reader, options);
                        break;
                    case 5:
                        __outputs__ = formatterResolver.GetFormatterWithVerify<global::SavedItemStack[]>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::SavedBuilding(__type__, __rotation__, __xPos__, __yPos__, __inputs__, __outputs__);
            ____result.type = __type__;
            ____result.rotation = __rotation__;
            ____result.xPos = __xPos__;
            ____result.yPos = __yPos__;
            ____result.inputs = __inputs__;
            ____result.outputs = __outputs__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class SavedItemStackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::SavedItemStack>
    {


        private readonly global::MessagePack.Internal.AutomataDictionary ____keyMapping;
        private readonly byte[][] ____stringByteKeys;

        public SavedItemStackFormatter()
        {
            this.____keyMapping = new global::MessagePack.Internal.AutomataDictionary()
            {
                { "amount", 0 },
                { "type", 1 },
            };

            this.____stringByteKeys = new byte[][]
            {
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("amount"),
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("type"),
            };
        }

        public void Serialize(ref MessagePackWriter writer, global::SavedItemStack value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteMapHeader(2);
            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.Write(value.amount);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::Item.Type>().Serialize(ref writer, value.type, options);
        }

        public global::SavedItemStack Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadMapHeader();
            var __amount__ = default(byte);
            var __type__ = default(global::Item.Type);

            for (int i = 0; i < length; i++)
            {
                ReadOnlySpan<byte> stringKey = global::MessagePack.Internal.CodeGenHelpers.ReadStringSpan(ref reader);
                int key;
                if (!this.____keyMapping.TryGetValue(stringKey, out key))
                {
                    reader.Skip();
                    continue;
                }

                switch (key)
                {
                    case 0:
                        __amount__ = reader.ReadByte();
                        break;
                    case 1:
                        __type__ = formatterResolver.GetFormatterWithVerify<global::Item.Type>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::SavedItemStack(__amount__, __type__);
            ____result.amount = __amount__;
            ____result.type = __type__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class SavedWorldItemStackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::SavedWorldItemStack>
    {


        private readonly global::MessagePack.Internal.AutomataDictionary ____keyMapping;
        private readonly byte[][] ____stringByteKeys;

        public SavedWorldItemStackFormatter()
        {
            this.____keyMapping = new global::MessagePack.Internal.AutomataDictionary()
            {
                { "amount", 0 },
                { "type", 1 },
                { "xPos", 2 },
                { "yPos", 3 },
            };

            this.____stringByteKeys = new byte[][]
            {
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("amount"),
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("type"),
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("xPos"),
                global::MessagePack.Internal.CodeGenHelpers.GetEncodedStringBytes("yPos"),
            };
        }

        public void Serialize(ref MessagePackWriter writer, global::SavedWorldItemStack value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteMapHeader(4);
            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.Write(value.amount);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::Item.Type>().Serialize(ref writer, value.type, options);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.Write(value.xPos);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.Write(value.yPos);
        }

        public global::SavedWorldItemStack Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadMapHeader();
            var __amount__ = default(byte);
            var __type__ = default(global::Item.Type);
            var __xPos__ = default(float);
            var __yPos__ = default(float);

            for (int i = 0; i < length; i++)
            {
                ReadOnlySpan<byte> stringKey = global::MessagePack.Internal.CodeGenHelpers.ReadStringSpan(ref reader);
                int key;
                if (!this.____keyMapping.TryGetValue(stringKey, out key))
                {
                    reader.Skip();
                    continue;
                }

                switch (key)
                {
                    case 0:
                        __amount__ = reader.ReadByte();
                        break;
                    case 1:
                        __type__ = formatterResolver.GetFormatterWithVerify<global::Item.Type>().Deserialize(ref reader, options);
                        break;
                    case 2:
                        __xPos__ = reader.ReadSingle();
                        break;
                    case 3:
                        __yPos__ = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::SavedWorldItemStack(__amount__, __type__, __xPos__, __yPos__);
            ____result.amount = __amount__;
            ____result.type = __type__;
            ____result.xPos = __xPos__;
            ____result.yPos = __yPos__;
            reader.Depth--;
            return ____result;
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name
