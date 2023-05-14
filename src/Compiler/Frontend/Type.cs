namespace A7.Frontend;


public class TypeIndex
{
    uint indexer_index { get; set; }
    TypeBaseKind kind { get; set; }
}


public struct FunctionType
{
    // uint num_of_args; use args_type's count
    List<TypeIndex> args_types { get; set; }
    TypeIndex return_type { get; set; }
}

public struct RecordType
{
    List<TypeIndex> children_types { get; set; }
}

public struct IntType
{
    uint size_in_bits { get; set; }
    bool is_signed { get; set; }
}

public struct FloatType
{
    uint size_in_bits { get; set; }
}

public struct ArrayType
{
    uint count { get; set; }
    TypeIndex child_type { get; set; }
}

public struct StringType
{
    uint count { get; set; }
}

public struct VariantType
{
    // TODO: disable variant type embedded in variants
    List<TypeIndex> variants { get; set; }
}

public struct TypeSegment
{
    List<TypeIndex> indexer { get; set; }
    List<IntType> ints_types { get; set; }
    List<FloatType> float_types { get; set; }
    List<ArrayType> array_types { get; set; }
    List<FunctionType> function_type { get; set; }
}

internal enum TypeBaseKind : byte
{
    Void,
    Int,
    Enum,
    Float,
    Bool,
    String,
    Pointer,
    Function,
    Record,
    Array,
    Variant,
    Null,
    Any,
}

