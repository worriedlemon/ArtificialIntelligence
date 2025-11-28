using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialIntelligenceIHW.Resources
{
    public static class GraphStrings
    {
        public static string FullGraph6 = """
            A,B,C,D,E,F

            0,80
            50,150
            120,150
            170,80
            120,10
            50,10

            Unoriented
            -,1,3,7,7,7
            1,-,1,1,2,2
            3,1,-,2,3,3
            7,1,2,-,1,4
            7,2,3,1,-,1
            7,2,3,4,1,-
            """;

        public static string AcyclicGraph7 = """
            A,B,C,D,E,F,G

            0,80
            50,150
            120,150
            170,80
            120,10
            50,10
            85,80

            -,1,-,-,-,7,3
            -,-,3,-,-,-,-
            -,-,-,2,-,-,-
            -,-,-,-,-,-,-
            -,-,-,4,-,-,-
            -,-,-,-,1,-,4
            -,1,-,-,2,-,-
            """;
    }
}
