using System;
using System.Collections.Immutable;
using System.Reflection.Metadata.Cil.Decoder;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Metadata.Cil
{
    // TODO: This is a shim to adapt old code to new pattern. It is better to use the Decode helpers hanging off of 
    //       MethodDefinition, FieldDefinition, etc. but this was the easiest way to adapt the existing code to get
    //       a build off the ground.
    internal static class SignatureDecoder
    {
        private static SignatureDecoder<CilType> NewDecoder(CilTypeProvider provider)
        {
            return new SignatureDecoder<CilType>(provider, provider.Reader);
        }

        internal static MethodSignature<CilType> DecodeMethodSignature(BlobHandle handle, CilTypeProvider provider)
        {
            var blobReader = provider.Reader.GetBlobReader(handle);
            return NewDecoder(provider).DecodeMethodSignature(ref blobReader);
        }

        internal static CilType DecodeFieldSignature(BlobHandle handle, CilTypeProvider provider)
        {
            var blobReader = provider.Reader.GetBlobReader(handle);
            return NewDecoder(provider).DecodeFieldSignature(ref blobReader);
        }


        internal static ImmutableArray<CilType> DecodeLocalSignature(StandaloneSignatureHandle handle, CilTypeProvider provider)
        {
            var standaloneSignature = provider.Reader.GetStandaloneSignature(handle);
            var blobReader = provider.Reader.GetBlobReader(standaloneSignature.Signature);
            return NewDecoder(provider).DecodeLocalSignature(ref blobReader);
        }

        internal static ImmutableArray<CilType> DecodeMethodSpecificationSignature(BlobHandle handle, CilTypeProvider provider)
        {
            var blobReader = provider.Reader.GetBlobReader(handle);
            return NewDecoder(provider).DecodeMethodSpecificationSignature(ref blobReader);
        }

        internal static CilType DecodeType(EntityHandle handle, CilTypeProvider provider, bool? isValueType)
        {
            SignatureTypeHandleCode code;
            if (isValueType.HasValue)
            {
                code = isValueType.Value ? SignatureTypeHandleCode.ValueType : SignatureTypeHandleCode.Class;
            }
            else
            {
                code = SignatureTypeHandleCode.Unresolved;
            }

            switch (handle.Kind)
            {
                case HandleKind.TypeDefinition:
                    return provider.GetTypeFromDefinition(provider.Reader, (TypeDefinitionHandle)handle, (byte)code);
                case HandleKind.TypeReference:
                    return provider.GetTypeFromReference(provider.Reader, (TypeReferenceHandle)handle, (byte)code);
                case HandleKind.TypeSpecification:
                    return provider.GetTypeFromSpecification(provider.Reader, (TypeSpecificationHandle)handle, (byte)code);
                default:
                    throw new ArgumentException("Handle is not a type definition, reference, or specification.", nameof(handle));
            }
        }

        internal static CilType DecodeType(ref BlobReader reader, CilTypeProvider provider)
        {
            return NewDecoder(provider).DecodeType(ref reader);
        }
    }

    public enum SignatureTypeHandleCode : byte
    {
        /// <summary>
        /// It is not known in the current context if the type reference or definition is a class or value type.
        /// This will be the case when <see cref="SignatureDecoderOptions.DifferentiateClassAndValueTypes"/> is not specified.
        /// </summary>
        Unresolved = 0,

        /// <summary>
        /// The type definition or reference refers to a class.
        /// </summary>
        Class = 0x12, // TODO: CorElementType.ELEMENT_TYPE_CLASS,

        /// <summary>
        /// The type definition or reference refers to a value type.
        /// </summary>
        ValueType = 0x11, // TODO: CorElementType.ELEMENT_TYPE_VALUETYPE,
    }
}
