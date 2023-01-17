
namespace course
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Boundary> Boundaries = new();

            // --------------------------------- test
            /*Boundaries.Add(new Boundary1(1, (x, y) => 1));
            Boundaries.Add(new Boundary3(3, 1, (x, y) => 21));
            Boundaries.Add(new Boundary2(0, (x, y) => 0));
            Boundaries.Add(new Boundary2(4, (x, y) => 0));*/

            // ------------------------------------




            // 61
            //-----------------------------------

            /*Boundaries.Add(new Boundary1(2, (x, y) => 1));
            Boundaries.Add(new Boundary1(9, (x, y) => 1));
            Boundaries.Add(new Boundary1(6, (x, y) => 3));
            Boundaries.Add(new Boundary1(13, (x, y) => 3));

            Boundaries.Add(new Boundary2(0, (x, y) => 0));
            Boundaries.Add(new Boundary2(1, (x, y) => 0));
            Boundaries.Add(new Boundary2(14, (x, y) => 0));
            Boundaries.Add(new Boundary2(15, (x, y) => 0));*/


            //--------------------------------------

            // 6.2

            /*Boundaries.Add(new Boundary2(2, (x, y) => -1));
            Boundaries.Add(new Boundary2(9, (x, y) => -1));

            Boundaries.Add(new Boundary2(0, (x, y) => 0));
            Boundaries.Add(new Boundary2(1, (x, y) => 0));

            Boundaries.Add(new Boundary2(14, (x, y) => 0));
            Boundaries.Add(new Boundary2(15, (x, y) => 0));


            Boundaries.Add(new Boundary3(6,1, (x, y) => 4 ));
            Boundaries.Add(new Boundary3(13, 1, (x, y) => 4 ));*/


            //--------------------------


            // 63
            /*Boundaries.Add(new Boundary2(2, (x, y) => -2));
            Boundaries.Add(new Boundary2(9, (x, y) => -2));
            Boundaries.Add(new Boundary2(0, (x, y) => 0));
            Boundaries.Add(new Boundary2(1, (x, y) => 0));
            Boundaries.Add(new Boundary2(14, (x, y) => 0));
            Boundaries.Add(new Boundary2(15, (x, y) => 0));
            Boundaries.Add(new Boundary3(6,1, (x, y) =>15));
            Boundaries.Add(new Boundary3(13, 1, (x, y) => 15));*/
            // ---------------------


             

            // 64
            /*Boundaries.Add(new Boundary1(2, (x, y) => 1));
            Boundaries.Add(new Boundary1(9, (x, y) => 1));
            Boundaries.Add(new Boundary1(6, (x, y) => 9));
            Boundaries.Add(new Boundary1(13, (x, y) => 9));

            Boundaries.Add(new Boundary2(14, (x, y) => 0));
            Boundaries.Add(new Boundary2(15, (x, y) => 0));
            Boundaries.Add(new Boundary2(0, (x, y) => 0));
            Boundaries.Add(new Boundary2(1, (x, y) => 0));*/

            //--------------------------


            //---------------------------------- 65 для вложенной
            /* Boundaries.Add(new Boundary1(4, (x, y) => 1));
             Boundaries.Add(new Boundary1(17, (x, y) => 1));
             Boundaries.Add(new Boundary1(30, (x, y) => 1));
             Boundaries.Add(new Boundary1(43, (x, y) => 1));

             Boundaries.Add(new Boundary1(12, (x, y) => 27));
             Boundaries.Add(new Boundary1(25, (x, y) => 27));
             Boundaries.Add(new Boundary1(38, (x, y) => 27));
             Boundaries.Add(new Boundary1(51, (x, y) => 27));

             Boundaries.Add(new Boundary2(0, (x, y) => 0));
             Boundaries.Add(new Boundary2(1, (x, y) => 0));
             Boundaries.Add(new Boundary2(2, (x, y) => 0));
             Boundaries.Add(new Boundary2(3, (x, y) => 0));

             Boundaries.Add(new Boundary2(52, (x, y) => 0));
             Boundaries.Add(new Boundary2(53, (x, y) => 0));
             Boundaries.Add(new Boundary2(54, (x, y) => 0));
             Boundaries.Add(new Boundary2(55, (x, y) => 0));*/

            //--------------------------

            //---------------------------------- 66 для обычной
             Boundaries.Add(new Boundary1(2, (x, y) => 1));
             Boundaries.Add(new Boundary1(9, (x, y) => 1));
             Boundaries.Add(new Boundary1(6, (x, y) => 27));
             Boundaries.Add(new Boundary1(13, (x, y) => 27));

           
             Boundaries.Add(new Boundary2(0, (x, y) => 0));
             Boundaries.Add(new Boundary2(1, (x, y) => 0));
             Boundaries.Add(new Boundary2(14, (x, y) => 0));
             Boundaries.Add(new Boundary2(15, (x, y) => 0));

        

            //--------------------------

            // test
            //Finit fem = new Finit((x, y) => -4 + 4 * x * x, Boundaries, (x, y) => 2, (x, y) => 4);

            // 61 62
            // Finit fem = new Finit((x, y) => 0, Boundaries, (x, y) => 1, (x, y) => 0);

            // 63
            //Finit fem = new Finit((x, y) => -2 + x * x, Boundaries, (x, y) => 1, (x, y) => 1);

            // 64
            //Finit fem = new Finit((x, y) => -4 * x + x * x, Boundaries, (x, y) => x, (x, y) => 1);

            //65
            Finit fem = new Finit((x, y) => -6 * x, Boundaries, (x, y) => 1, (x, y) => 0);

            fem.Run(3e-15, 10000);

            Console.WriteLine(fem.sollution(1 + 1.0 / 8.0, 1 / 8.0));

            Console.WriteLine("-------------------");

            //Console.WriteLine($"{fem.Getsollution(1, 0)} {fem.Getsollution(2, 0)} {fem.Getsollution(3, 0)} {fem.Getsollution(1, 1)} {fem.Getsollution(2, 1)} {fem.Getsollution(3, 1)} {fem.Getsollution(1, 2)} {fem.Getsollution(2, 2)} {fem.Getsollution(3, 2)}");
            //Console.WriteLine($"{fem.Getsollution(1 + 1 / 8, 1/8)} {fem.Getsollution(1.5 - 1/8, 1/8)} {fem.Getsollution(1.5 + 1/8, 0)} {fem.Getsollution(2 - 1/8, 0)} {fem.Getsollution(2+1/8, 0.5+1/8)} {fem.Getsollution(1.5+1/8, 0.5+1/8)} {fem.Getsollution(2+1/8, 0.5+1/8)} {fem.Getsollution(2.5+1/8, 0.5+1/8)} {fem.Getsollution(1+1/8, 0.5+1/8)} {fem.Getsollution(1.5+1/8, 0.5+1/8)} {fem.Getsollution(2+1/8, 0.5+1/8)} {fem.Getsollution(2.5+1/8, 0.5+1/8)}" +
            //   $"" +
            //  $"" +
            // $"");

            double x1 = 1.0;
            double x2 = 1.0;
            double x3 = 1.0;
            double x4 = 1.0;
          

            for(int i =0;i<16;i++)
            {
                if (i < 4)
                {

                    Console.WriteLine(fem.sollution(x1 + 1 / 8.0, 0 + 1 / 8.0));
                    Console.WriteLine(fem.sollution((x1 + 0.5) - 1 / 8.0, 0.5 - 1 / 8.0));
                    x1 += 0.5;
                }

                if (i >= 4 && i < 8)
                {

                    Console.WriteLine(fem.sollution(x2 + 1 / 8.0, 0.5 + 1 / 8.0));
                    Console.WriteLine(fem.sollution((x2 + 0.5) - 1 / 8.0, 1 - 1 / 8.0));
                    x2 += 0.5;
                }

                if (i >=8 && i < 12)
                {

                    Console.WriteLine(fem.sollution(x3 + 1 / 8.0, 1 + 1 / 8.0));
                    Console.WriteLine(fem.sollution((x3 + 0.5) - 1 / 8.0, 1.5 - 1 / 8.0));
                    x3 += 0.5;
                }

                if (i >= 12)
                {

                    Console.WriteLine(fem.sollution(x4 + 1 / 8.0, 1.5 + 1 / 8.0));
                    Console.WriteLine(fem.sollution((x4 + 0.5) - 1 / 8.0, 2 - 1 / 8.0));
                    x4 += 0.5;
                }
            }


            Console.WriteLine("-------------------");


            double[] arr = {
      2.5078E-02,
4.2891E-02,
4.0547E-02,
4.1484E-02,
4.1484E-02,
3.7734E-02,
4.2891E-02,
3.4922E-02





};

            double[] real = {
          1.423828125,
6.591796875000000000,
9.595703125000000000,
23.763671875000000000,
1.423828125,
6.591796875000000000,
9.595703125000000000,
23.763671875000000000




 };

            double s1 = 0.0;
            double s2 = 0.0;


            for(int i=0;i< arr.Length; i++)
            {
                s1 += arr[i] * arr[i];
                s2 += real[i];
            }


            Console.WriteLine(Math.Sqrt(s1)  / s2);


            double[] err =
            {
                1.3987500000000133,
6.634687500000035,
9.55515625000006,
23.805156250000014,
1.3823437500000033,
6.629531250000033,
9.552812500000043,
23.798593750000006

            };

            double[] ok =
            {
                  1.423828125,
6.591796875000000000,
9.595703125000000000,
23.763671875000000000,
1.423828125,
6.591796875000000000,
9.595703125000000000,
23.763671875000000000

            };


            for(int i = 0; i < ok.Length; i++)
            {
                //Console.WriteLine(Math.Abs(ok[i] - err[i]));
            }
        }
    }
}