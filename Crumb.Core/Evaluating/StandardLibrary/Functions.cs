namespace Crumb.Core.Evaluating.StandardLibrary;
internal class Functions
{
    internal static string PrintLine => $$"""
        {
            (
                fun [ line ]
                {
                    ( print line )
                    ( print "\r\n" )
                }
            )
        }
        """;

    internal static string LessThanOrEqual => $$"""
        {
            (
                fun [ a b ]
                {
                    ( or
                        ( < a b )
                        ( = a b )
                    )
                }
            )
        }
        """;

    internal static string GreaterThanOrEqual => $$"""
        {
            (
                fun [ a b ]
                {
                    ( or
                        ( > a b )
                        ( = a b )
                    )
                }
            )
        }
        """;
}
