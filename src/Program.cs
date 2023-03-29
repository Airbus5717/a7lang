
class Program
{
    static void Main(string[] args)
    {
        var opts = new CompileOptions();
        opts.path = "main.vr";
        Compiler.compile(opts);
    }
}