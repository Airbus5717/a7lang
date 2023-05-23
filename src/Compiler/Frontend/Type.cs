namespace A7.Frontend;


public class TypeIndex
{
    uint indexer_index { get; set; }
    TypeBaseKind kind { get; set; }

    public bool Equals(TypeIndex other)
    {
        return indexer_index == other.indexer_index &&
               kind == other.kind;
    }

    public override string ToString()
    {
        Utils.Utilities.Todo("to implement to string for types");
        return "unknown";
    }
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


public static class TypeDomains {
    public static bool IsValidFunctionReturnType(TypeBaseKind k)
        => (k != TypeBaseKind.Function);

    public static bool IsValidFunctionArgumentType(TypeBaseKind k)
        => IsValidFunctionReturnType(k);
}

public enum TypeBaseKind : byte
{
    Int,
    UInt,
    Float,
    Bool,
    Enum,
    String,
    Array,
    Pointer,
    Function,
    VoidFunction,  // NOTE: ????
    Record,
    Variant,
    //Null, // null is pointer type
    //Any, // NOTE: for now
}

