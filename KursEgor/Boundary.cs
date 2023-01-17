namespace course
{
    public class Boundary
    {
        public Boundary(int edge)
        {
            this.e = edge;
        }

        public int e { get; init; }
    }

    public class Boundary1 : Boundary
    {
        public Func<double, double, double> ugCallback;

        public Boundary1(int edge, Func<double, double, double> ugCallback) : base(edge)
        {
            this.ugCallback = ugCallback;
        }
    }

    public class Boundary2 : Boundary
    {
        public Func<double, double, double> theta;

        public Boundary2(int edge, Func<double, double, double> theta) : base(edge)
        {
            this.theta = theta;
        }
    }
    public class Boundary3 : Boundary
    {
        public double betta;
        public Func<double, double, double> ubettaCallback;

        public Boundary3(int edge, double betta, Func<double, double, double> ubettaCallback) : base(edge)
        {
            this.betta = betta;
            this.ubettaCallback = ubettaCallback;
        }
    }

}