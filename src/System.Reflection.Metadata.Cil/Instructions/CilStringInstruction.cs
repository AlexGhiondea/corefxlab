using System.Reflection.Emit;
using System.Reflection.Metadata.Cil.Decoder;
using System.Reflection.Metadata.Cil.Visitor;

namespace System.Reflection.Metadata.Cil.Instructions
{
    public class CilStringInstruction : CilInstructionWithValue<string>, ICilVisitable
    {
        public CilType ParentType { get; set; }
        private readonly bool _isPrintable;
        internal CilStringInstruction(OpCode opCode, string value, int token, int size, CilType parentType, bool isPrintable = true)
            : base(opCode, value, token, size)
        {
            ParentType = parentType;
            _isPrintable = isPrintable;
        }

        public bool IsPrintable
        {
            get
            {
                return _isPrintable;
            }
        }

        public override void Accept(ICilVisitor visitor)
        {
            visitor.Visit(this);
        }

    }
}
