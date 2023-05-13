namespace A7;

class TypeIndex
{
    uint indexer_index;
    TypeBaseKind kind;
}


class FunctionType
{
    uint num_of_args;
    List<TypeIndex> args_types;
    TypeIndex return_type;
}

class RecordType
{
    List<TypeIndex> children_types;
}

class IntType
{
    uint size_in_bits;
    bool is_signed;
}

class FloatType
{
    uint size_in_bits;
}

class ArrayType
{
    uint count;
    TypeIndex child_type;
}

class StringType
{
    uint count;
}

class VariantType
{
    // TODO: disable variant type embedded in variants
    List<TypeIndex> variants;
}

class TypeSegment
{
    List<TypeIndex> indexer;
    List<IntType> ints_types;
    List<FloatType> float_types;
    List<ArrayType> array_types;
    List<FunctionType> function_type;
}

enum TypeBaseKind : byte
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

