namespace A7.CLI;

class Program
{

    static void Main(string[] args)
    {
        A7.Compiler.compile(new A7.CompileOptions("main.a7"));
    }
}
