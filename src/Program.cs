namespace A7.CLI;

class Program
{

    static void Main(string[] args)
    {
        var i = A7.Compiler.compile(new A7.CompileOptions("main.a7"));
        Utils.Err.PrintStage(i.item2);
        if (i.item1 == Utils.Status.Failure)
        {
            Utils.Utilities.LogErr("FAILED");
            Environment.Exit(1);
        }
        else
        {
            Utils.Utilities.LogInfo("SUCCESS");
        }
    }
}
