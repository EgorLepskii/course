using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace course
{
    public class Matr
    {
        public int n;
        public List<int> ia = new();
        public List<int> ja = new();
        public List<double> di = new();
        public List<double> al = new();
        public List<double> b = new();
        public List<double> di_for_LU = new();
        public List<double> au_for_LU = new();
        public List<double> al_for_LU = new();
        public Matr(int n)
        {
            this.n = n;
        }
        public void Clear()
        {
            for (int i = 0; i < n; i++)
            {
                di[i] = 0;
                di_for_LU[i] = 0;
                b[i] = 0;
            }
            int m = al.Count;
            for (int i = 0; i < m; i++)
            {
                al[i] = 0;
                al_for_LU[i] = 0;
                au_for_LU[i] = 0;
            }
        }
        public void LU()
        {
            foreach (var item in di)
            {
                di_for_LU.Add(item);
            }
            foreach (var item in al)
            {
                au_for_LU.Add(item);
                al_for_LU.Add(item);
            }
            for (int i = 0; i < n; i++)
            {
                di_for_LU[i] = di[i];
            }
            var q = al.Count;
            for (int i = 0; i < q; i++)
            {
                al_for_LU[i] = al[i];
                au_for_LU[i] = al[i];
            }
            for (int i = 0; i < n; i++)
            {
                double sumdi = 0.0;

                int i0 = ia[i];
                int i1 = ia[i + 1];


                for (int k = i0; k < i1; k++)
                {
                    int j = ja[k];
                    int j0 = ia[j];
                    int j1 = ia[j + 1];
                    int ik = i0;
                    int kj = j0;

                    double suml = 0.0;
                    double sumu = 0.0;

                    while (ik < k)
                    {

                        if (ja[ik] == ja[kj])
                        {

                            suml += al_for_LU[ik] * au_for_LU[kj];
                            sumu += au_for_LU[ik] * al_for_LU[kj];
                            ik++;
                            kj++;
                        }

                        else
                        {
                            if (ja[ik] > ja[kj])
                            {
                                kj++;
                            }
                            else
                            {
                                ik++;
                            }
                        }
                    }

                    al_for_LU[k] = al_for_LU[k] - suml;
                    au_for_LU[k] = (au_for_LU[k] - sumu) / di_for_LU[j];
                    sumdi += al_for_LU[k] * au_for_LU[k];
                }

                di_for_LU[i] = di_for_LU[i] - sumdi;
            }
        }
        List<double> Direct(List<double> rpart)
        {
            List<double> res = new();
            for (int i = 0; i < n; i++)
            {
                res.Add(rpart[i]);
            }

            for (int i = 0; i < n; i++)
            {
                double sum = 0.0;
                for (int j = ia[i]; j < ia[i + 1]; j++)
                    sum += al_for_LU[j] * res[ja[j]];
                res[i] -= sum;
                res[i] /= di_for_LU[i];
            }
            return res;
        }
        List<double> Reverse(List<double> rpart)
        {
            List<double> res = new();
            for (int i = 0; i < n; i++)
            {
                res.Add(rpart[i]);
            }

            for (int i = n - 1; i >= 0; i--)
            {
                for (int j = ia[i]; j < ia[i + 1]; j++)
                    res[ja[j]] -= au_for_LU[j] * res[i];
            }
            return res;
        }
        public List<double> LOS(List<double> x0, double eps, int maxiter)
        {
            int k = 1;
            List<double> buf = MatrixMult(x0);

            for (int i = 0; i < n; i++)
            {
                buf[i] = b[i] - buf[i];
            }


            List<double> directRes = Direct(buf);
            List<double> reverseRes = Reverse(directRes);

            buf = MatrixMult(reverseRes);
            List<double> p = Direct(buf);
            double epsk = 1;
            while (epsk > eps && k < maxiter)
            {
                double pp = MultVector(p, p);
                double pr = MultVector(p, directRes);
                double alpha = pr / pp;
                for (int i = 0; i < n; i++)
                {
                    x0[i] += alpha * reverseRes[i];
                    directRes[i] -= alpha * p[i];
                }

                List<double> Ur = Reverse(directRes);
                buf = MatrixMult(Ur);
                buf = Direct(buf);
                double betta = -(MultVector(p, buf) / pp);
                for (int i = 0; i < n; i++)
                {
                    reverseRes[i] = Ur[i] + betta * reverseRes[i];
                    p[i] = buf[i] + betta * p[i];
                }
                double a1 = 0;
                double a2 = 0;
                var asd = MatrixMult(x0);

                for (int i = 0; i < n; i++)
                {
                    a1 += (asd[i] - b[i]) * (asd[i] - b[i]);
                    a2 += b[i] * b[i];
                }
                epsk = Math.Sqrt(a1 / a2);
                k++;
            }

            return x0;
        }
      
        double MultVector(List<double> vec1, List<double> vec2)
        {
            if (vec1.Count != vec2.Count)
                throw new Exception();
            double res = 0;
            int m = vec1.Count;
            for (int i = 0; i < m; i++)
            {
                res += vec1[i] * vec2[i];
            }
            return res;
        }
        List<double> MatrixMult(List<double> x)
        {
            if (x.Count != n)
                throw new Exception();
            List<double> res = new List<double>(x.Count);
            for (int i = 0; i < n; i++)
            {
                res.Add(0);
            }
            for (int i = 0; i < n; i++)
            {
                res[i] = x[i] * di[i];
                for (int k = ia[i]; k < ia[i + 1]; k++)
                {
                    int j = ja[k];
                    res[i] += al[k] * x[j];
                    res[j] += al[k] * x[i];
                }
            }
            return res;
        }
        public List<double> LOSSolve(double eps,int maxiter)
        {
            List<double> x0 = new();
            for (int i = 0; i < n; i++)
            {
                x0.Add(0);
            }
            LU();
            return LOS(x0, eps, maxiter);
        }
    }
}

