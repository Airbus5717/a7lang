namespace A7.Frontend;


public class TypeIndex
{
    public TypeBaseKind kind { get; set; } = TypeBaseKind.Invalid;
    public uint indexer { get; set; } = 0; // index in TypeBaseKind Specific slot

    public bool Equals(TypeIndex other)
    {
        return indexer == other.indexer &&
               kind == other.kind;
    }

    public override string ToString()
    {
        Utils.Utilities.Todo("to implement to string for types");
        return "unknown";
    }
}


public struct EnumType
{

}


public struct FunctionType
{
    // uint num_of_args; use args_type's count
    List<VariableDef> args_types { get; set; }
    TypeIndex return_type { get; set; }

    public FunctionType(List<VariableDef> args_types, TypeIndex return_type)
    {
        this.args_types = args_types;
        this.return_type = return_type;
    }
}

public struct RecordType
{
    List<TypeIndex> children_types { get; set; }
}

public struct ArrayType
{
    uint count { get; set; }
    TypeIndex child_type { get; set; }
}

public struct VariantType
{
    // TODO: disable variant type embedded in variants
    List<TypeIndex> variants { get; set; }
}

public struct TypeSegment
{
    List<ArrayType> array_types { get; set; }
    List<FunctionType> function_types { get; set; }
    List<VariantType> variant_types { get; set; }
    List<RecordType> record_types { get; set; }
    List<EnumType> enum_types { get; set; }
}

public static class TypeDomains
{
}

public enum TypeBaseKind : byte
{
    Invalid = 0,
    Int,
    UInt,
    Float,
    Bool,
    String,
    Char,

    // NOTE(5717): requires more checking
    Enum,
    Array,
    Pointer,
    Function,
    Record,
    Variant,
    VoidFunction,  // NOTE: only as statement, (expressions are not allowed)
}

