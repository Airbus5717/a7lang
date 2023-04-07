namespace A7
{
    public enum ErrKind
    {
        Unknown,
    }


    class Err
    {

        public static string GetErrString(ErrKind kind)
        {
            switch (kind)
            {
                case ErrKind.Unknown: return "Unknown";
                default:
                    return "";
            }
        }

        public static void ErrMsg()
        {

        }
    }
}
