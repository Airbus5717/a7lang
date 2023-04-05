

class Program
{
    static int add9(int x) { return x + 09; }
    static void Main(string[] args)
    {
        Compiler.compile(new CompileOptions("main.a7"));
    }
}
