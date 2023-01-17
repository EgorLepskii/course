namespace course
{
    public class Finit
    {
        Func<double, double, double> fCallback;
        List<Boundary> Boundaries;
        Func<double, double, double> lambdaCallback;
        Func<double, double, double> gammaCallback;
        Matr mat;
        List<double> xv;
        List<double> yv;
        List<double> q;
        List<FinitElem> elems;
        List<Edge> edges;
        const int  LOCAL_SIZE= 6;

        public Finit(Func<double, double, double> f, List<Boundary> boundaryConditions, Func<double, double, double> lambda, Func<double, double, double> gamma)
        {
            this.fCallback = f;
            Boundaries = boundaryConditions;
            this.lambdaCallback = lambda;
            this.gammaCallback = gamma;
            elems = new();
            edges = new();
            xv = new();
            yv = new();
            Read();
        }

        public void Read()
        {
            var str = File.ReadAllLines("data/vertices.txt");
            int vertscount = str.Count();
            foreach (var item in str)
            {
                var el = item.Split(' ');
                xv.Add(double.Parse(el[0]));
                yv.Add(double.Parse(el[1]));
            }

            str = File.ReadAllLines("data/edges.txt");
            foreach (var item in str)
            {
                var el = item.Split(' ');
                edges.Add(new Edge(int.Parse(el[0]), int.Parse(el[1])));
            }
            str = File.ReadAllLines("data/finitElems.txt");
            foreach (var item in str)
            {
                var el = item.Split(' ');
                elems.Add(new FinitElem(int.Parse(el[0]), int.Parse(el[1]), int.Parse(el[2]), int.Parse(el[3]), int.Parse(el[4]), int.Parse(el[5]), vertscount));
            }
        }


        // Generate profile
        public void Profile()
        {
            mat = new(xv.Count + edges.Count());
            mat.di = new List<double>(mat.n);
            mat.ia = new List<int>(mat.n + 1);
            mat.ja = new List<int>();
            mat.al = new List<double>();
            mat.b = new List<double>(mat.n);
            List<SortedSet<int>> list = new List<SortedSet<int>>(mat.n);
            for (int i = 0; i < mat.n; i++)
            {
                list.Add(new SortedSet<int>());
            }
            foreach (var item in elems)
            {
                var global = item.Global;
                for (int i = 0; i < LOCAL_SIZE; i++)
                {
                    for (int j = 0; j < LOCAL_SIZE; j++)
                    {
                        list[global[i]].Add(global[j]);
                    }
                }
            }
            mat.ia.Add(0);
            mat.ia.Add(0);
            mat.di.Add(0);
            mat.b.Add(0);
            for (int i = 1; i < mat.n; i++)
            {
                mat.di.Add(0);
                mat.b.Add(0);
                int count = 0;
                foreach (var item in list[i])
                {
                    if (item < i)
                    {
                        mat.ja.Add(item);
                        mat.al.Add(0);
                        count++;
                    }
                }
                mat.ia.Add(mat.ia[i] + count);
            }
        }

        // Create local matrix and add it to global matrix
        public void LocalToGlobal(FinitElem elem)
        {
            var det = Det(elem);

            double[] ax = new double[6]
            {
                (yv[elem.vert2] - yv[elem.vert3]) / det * (yv[elem.vert2] - yv[elem.vert3]) / det,
                (yv[elem.vert3] - yv[elem.vert1]) / det * (yv[elem.vert2] - yv[elem.vert3]) / det,
                (yv[elem.vert1] - yv[elem.vert2]) / det * (yv[elem.vert2] - yv[elem.vert3]) / det,
                (yv[elem.vert3] - yv[elem.vert1]) / det * (yv[elem.vert3] - yv[elem.vert1]) / det,
                (yv[elem.vert1] - yv[elem.vert2]) / det * (yv[elem.vert3] - yv[elem.vert1]) / det,
                (yv[elem.vert1] - yv[elem.vert2]) / det * (yv[elem.vert1] - yv[elem.vert2]) / det
            };


            double[] ay = new double[6]
            {
                (xv[elem.vert3] - xv[elem.vert2]) / det * (xv[elem.vert3] - xv[elem.vert2]) / det,
                (xv[elem.vert1] - xv[elem.vert3]) / det * (xv[elem.vert3] - xv[elem.vert2]) / det,
                (xv[elem.vert2] - xv[elem.vert1]) / det * (xv[elem.vert3] - xv[elem.vert2]) / det,
                (xv[elem.vert1] - xv[elem.vert3]) / det * (xv[elem.vert1] - xv[elem.vert3]) / det,
                (xv[elem.vert2] - xv[elem.vert1]) / det * (xv[elem.vert1] - xv[elem.vert3]) / det,
                (xv[elem.vert2] - xv[elem.vert1]) / det * (xv[elem.vert2] - xv[elem.vert1]) / det
            };
            det = Math.Abs(det);


            double gamma = this.gammaCallback((xv[elem.vert1] + xv[elem.vert2] + xv[elem.vert3]) / 3, (yv[elem.vert1] + yv[elem.vert2] + yv[elem.vert3]) / 3);


            double[] lambdas = new double[3]
            {
                lambdaCallback(xv[elem.vert1], yv[elem.vert1]),
                lambdaCallback(xv[elem.vert2], yv[elem.vert2]),
                lambdaCallback(xv[elem.vert3], yv[elem.vert3])

            };
            double[] functions = new double[6]
            {
                fCallback(xv[elem.vert1], yv[elem.vert1]),
                fCallback(xv[elem.vert2], yv[elem.vert2]),
                fCallback(xv[elem.vert3], yv[elem.vert3]),
                fCallback((xv[elem.vert2] + xv[elem.vert3]) / 2, (yv[elem.vert2] + yv[elem.vert3]) / 2),
                fCallback((xv[elem.vert1] + xv[elem.vert3]) / 2, (yv[elem.vert1] + yv[elem.vert3]) / 2),
                fCallback((xv[elem.vert2] + xv[elem.vert1]) / 2, (yv[elem.vert2] + yv[elem.vert1]) / 2)
            };

            var globalElem = elem.Global;

            for (int i = 0; i < 6; i++)
            {
                // add to di and b global elem
                for (int m = 0; m < 6; m++)
                {
                    // add lambdas
                    for (int k = 0; k < 3; k++)
                    {
                        mat.di[globalElem[i]] += (ax[m] + ay[m]) * Matrices.GMatr[i][i][m][k] * det * lambdas[k];
                    }
                    mat.b[globalElem[i]] += functions[m] * Matrices.MMatr[i][m] * det;
                }



                mat.di[globalElem[i]] += Matrices.MMatr[i][i] * det * gamma;

                for (int j = 0; j < i; j++)
                {
                    // string number should be large than row number
                    int max = globalElem[i] > globalElem[j] ? globalElem[i] : globalElem[j];
                    int min = globalElem[i] > globalElem[j] ? globalElem[j] : globalElem[i];
                   
                    int index = mat.ja.BinarySearch(mat.ia[max], mat.ia[max + 1] - mat.ia[max], min, default);

                    for (int m = 0; m < 6; m++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            mat.al[index] += lambdas[k] * (ax[m] + ay[m]) * Matrices.GMatr[i][j][m][k] * det;
                        }
                    }
                    mat.al[index] += gamma * Matrices.MMatr[i][j] * det;
                }
            }

        }
        private void Boundary1()
        {
            foreach (var item in Boundaries.OfType<Boundary1>())
            {
                var dict = new Dictionary<int, int> { { 0, edges[item.e].v1 }, { 1, item.e + xv.Count }, { 2, edges[item.e].v2 } };

                var ug = new double[3] 
                { 
                    item.ugCallback(xv[dict[0]], yv[dict[0]]),
                    item.ugCallback((xv[dict[2]] + xv[dict[0]]) / 2, (yv[dict[2]] + yv[dict[0]]) / 2),
                    item.ugCallback(xv[dict[2]], yv[dict[2]]) 
                };

                for (int m = 0; m < 3; m++)
                {
                    // when apply 1 boundary, it need to set to 1 diagonal elements with boundary pointer
                    mat.di[dict[m]] = 1;
                    mat.b[dict[m]] = ug[m];

                    /*
                       In order not to break the symmetry of the matrix, the corresponding column is also set to zero, but, 
                       so that the SLAE does not change, the zeroed element is subtracted from each component of the right side, 
                       multiplied by the required value in the node
                    */
                    for (int k = mat.ia[dict[m]]; k < mat.ia[dict[m] + 1]; k++)
                    {
                        mat.b[mat.ja[k]] -= ug[m] * mat.al[k];
                        mat.al[k] = 0;
                    }

                    for (int i = dict[m] + 1; i < mat.n; i++)
                    {
                        int index = mat.ja.BinarySearch(mat.ia[i], mat.ia[i + 1] - mat.ia[i], dict[m], default);
                        if (index >= 0)
                        {
                            mat.b[i] -= mat.al[index] * ug[m];
                            mat.al[index] = 0;
                        }
                    }
                }
            }
        }
        private void Boundary2()
        {
            
            foreach (var item in Boundaries.OfType<Boundary2>())
            {
                // 0 - index of the first vertex for the boundary condition, 2 - index of the second vertex for the boundary condition, 1 - number of the node corresponding to the edge
                var data = new Dictionary<int, int> { { 0, edges[item.e].v1 }, { 1, item.e + xv.Count }, { 2, edges[item.e].v2 } };

                // write values ​​for theta, getting a specific value of x and y by index
                var thetas = new double[3] 
                { 
                    item.theta(xv[data[0]], yv[data[0]]),
                    item.theta((xv[data[2]] + xv[data[0]]) / 2, (yv[data[2]] + yv[data[0]]) / 2),                         
                    item.theta(xv[data[2]], yv[data[2]]) 
                };


                double len = Math.Sqrt((xv[data[2]] - xv[data[0]]) * (xv[data[2]] - xv[data[0]]) +
                                       (yv[data[2]] - yv[data[0]]) * (yv[data[2]] - yv[data[0]]));

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        mat.b[data[i]] += thetas[j] * Matrices.BCMatr[i][j] * len;
                    }
                }
            }

        }

        private void Boundary3()
        {
            foreach (var item in Boundaries.OfType<Boundary3>())
            {
                var data = new Dictionary<int, int> { { 0, edges[item.e].v1 }, { 1, item.e + xv.Count }, { 2, edges[item.e].v2 } };

                var ubetta = new double[3] 
                { 
                    item.ubettaCallback(xv[data[0]], yv[data[0]]),
                    item.ubettaCallback((xv[data[2]] + xv[data[0]]) / 2, (yv[data[2]] + yv[data[0]]) / 2),
                    item.ubettaCallback(xv[data[2]], yv[data[2]]) 
                };
                double len = Math.Sqrt((xv[data[2]] - xv[data[0]]) * (xv[data[2]] - xv[data[0]]) +
                                       (yv[data[2]] - yv[data[0]]) * (yv[data[2]] - yv[data[0]]));

                for (int i = 0; i < 3; i++)
                {
                    mat.di[data[i]] += Matrices.BCMatr[i][i] * item.betta * len;
                    for (int j = 0; j < 3; j++)
                    {
                        mat.b[data[i]] += ubetta[j] * Matrices.BCMatr[i][j] * len * item.betta;
                    }

                    for (int j = 0; j < i; j++)
                    {
                        int max = data[i] > data[j] ? data[i] : data[j];
                        int min = data[i] > data[j] ? data[j] : data[i];
                        int index = mat.ja.BinarySearch(mat.ia[max], mat.ia[max + 1] - mat.ia[max], min, default);
                        mat.al[index] += Matrices.BCMatr[i][j] * item.betta * len;
                    }
                }
            }
        }

        // Create profile, Add all local items to global, add all boundary conditions and get q
        public void Run(double eps, int maxiter)
        {
            Profile();
            foreach (var item in elems)
            {
                LocalToGlobal(item);
            }
            Boundary2();
            Boundary3();
            Boundary1();
            this.q = mat.LOSSolve(eps, maxiter);
        }

        // Get sollution in specific point
        public double sollution(double x, double y)
        {
            double result = 0;
            bool flag = true;
            int i = 0;
            for (i = 0; i < elems.Count && flag; i++)
            {
                double S23 = Math.Abs((xv[elems[i].vert3] - xv[elems[i].vert2]) * (y - yv[elems[i].vert2]) - (x - xv[elems[i].vert2]) * (yv[elems[i].vert3] - yv[elems[i].vert2]));
                double S31 = Math.Abs((xv[elems[i].vert1] - xv[elems[i].vert3]) * (y - yv[elems[i].vert3]) - (x - xv[elems[i].vert3]) * (yv[elems[i].vert1] - yv[elems[i].vert3]));
                double S12 = Math.Abs((xv[elems[i].vert2] - xv[elems[i].vert1]) * (y - yv[elems[i].vert1]) - (x - xv[elems[i].vert1]) * (yv[elems[i].vert2] - yv[elems[i].vert1]));

                // if point outside mesh
                if (Math.Abs(Math.Abs(Det(elems[i])) - (S23 + S31 + S12)) <= 1e-7)
                    flag = false;
            }
            i--;
            if (flag)
                throw new Exception();

            var dic = elems[i].Global;
            var L = XtoL(x, y, elems[i]);
            result += L.L1 * (2 * L.L1 - 1) * q[dic[0]];
            result += L.L2 * (2 * L.L2 - 1) * q[dic[1]];
            result += L.L3 * (2 * L.L3 - 1) * q[dic[2]];
            result += 4 * L.L2 * L.L3 * q[dic[3]];
            result += 4 * L.L1 * L.L3 * q[dic[4]];
            result += 4 * L.L1 * L.L2 * q[dic[5]];
            return result;
        }

        // calculate finit element determinate
        public double Det(FinitElem el)
        {
            return (xv[el.vert2] - xv[el.vert1]) * (yv[el.vert3] - yv[el.vert1]) - (xv[el.vert3] - xv[el.vert1]) * (yv[el.vert2] - yv[el.vert1]);
        }
        public (double L1, double L2, double L3) XtoL(double x, double z, FinitElem el)
        {
            return ((xv[el.vert2] * yv[el.vert3] - xv[el.vert3] * yv[el.vert2] + x * (yv[el.vert2] - yv[el.vert3]) + z * (xv[el.vert3] - xv[el.vert2])) / Det(el),
                ((xv[el.vert3] * yv[el.vert1] - xv[el.vert1] * yv[el.vert3] + x * (yv[el.vert3] - yv[el.vert1]) + z * (xv[el.vert1] - xv[el.vert3]))) / Det(el),
                ((xv[el.vert1] * yv[el.vert2] - xv[el.vert2] * yv[el.vert1] + x * (yv[el.vert1] - yv[el.vert2]) + z * (xv[el.vert2] - xv[el.vert1]))) / Det(el));
        }
      
        static class Matrices
        {
            public static double[][] MMatr = new double[6][]
            {
                new double[6]{ 0.01666666666666665, -0.002777777777777775, -0.002777777777777775, -0.01111111111111111, 0, 0, },
                new double[6]{ -0.002777777777777775, 0.01666666666666665, -0.002777777777777775, 0, -0.01111111111111111, 0, },
                new double[6]{ -0.002777777777777775, -0.002777777777777775, 0.01666666666666665, 0, 0, -0.01111111111111111, },
                new double[6]{ -0.01111111111111111, 0, 0, 0.08888888888888889, 0.044444444444444446, 0.044444444444444446,   },
                new double[6]{ 0, -0.01111111111111111, 0, 0.044444444444444446, 0.08888888888888889, 0.044444444444444446,   },
                new double[6]{ 0, 0, -0.01111111111111111, 0.044444444444444446, 0.044444444444444446, 0.08888888888888889,   },
            };


            // 1 - i elem, 2 - j elem, 3 - which L coordinates use, 4 - decomposition of the diffusion coefficient 
            public static double[][][][] GMatr = new double[6][][][] {
new double[6][][]
{
    new double[6][]
    {
        new double[3] { 0.30000000000000004, 0.1, 0.1, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0,},
        new double[3] { 0, 0, 0,},
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, -0.06666666666666665, -0.033333333333333326, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, -0.033333333333333326, -0.06666666666666665, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0.1, -0.033333333333333326, -0.06666666666666665, },
        new double[3] { 0.1, -0.06666666666666665, -0.033333333333333326, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0.1, -0.033333333333333326, -0.06666666666666665, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.46666666666666673, 0.1, 0.1, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0.1, -0.06666666666666665, -0.033333333333333326, },
        new double[3] { 0.46666666666666673, 0.1, 0.1, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
},
new double[6][][]
{
    new double[6][] 
    {
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, -0.06666666666666665, -0.033333333333333326, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.1, 0.30000000000000004, 0.1, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.033333333333333326, -0.06666666666666665, -0.06666666666666665, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, }, 
        new double[3] { 0, 0, 0, }, 
        new double[3] { 0, 0, 0, },
        new double[3] { -0.033333333333333326, 0.1, -0.06666666666666665, },
        new double[3] { 0.1, 0.46666666666666673, 0.1, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { -0.033333333333333326, 0.1, -0.06666666666666665, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, 0.1, -0.033333333333333326, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0.1, 0.46666666666666673, 0.1, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, 0.1, -0.033333333333333326, }, 
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
},
new double[6][][]
{
    new double[6][] 
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, -0.033333333333333326, -0.06666666666666665, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, }, 
        new double[3] { 0, 0, 0, }, 
        new double[3] { -0.033333333333333326, -0.06666666666666665, -0.06666666666666665, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    { 
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, }, 
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.1, 0.1, 0.30000000000000004, },
    },
    new double[6][]
    { 
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.1, 0.1, 0.46666666666666673, }, 
        new double[3] { -0.033333333333333326, -0.06666666666666665, 0.1, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.1, 0.1, 0.46666666666666673, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, -0.033333333333333326, 0.1, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, }, 
        new double[3] { 0, 0, 0, },
        new double[3] { -0.033333333333333326, -0.06666666666666665, 0.1, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, -0.033333333333333326, 0.1, },
        new double[3] { 0, 0, 0, },
    },
},
new double[6][][]
{
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0.1, -0.033333333333333326, -0.06666666666666665, }, 
        new double[3] { 0.1, -0.06666666666666665, -0.033333333333333326, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.033333333333333326, 0.1, -0.06666666666666665, },
        new double[3] { 0.1, 0.46666666666666673, 0.1, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][] 
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.1, 0.1, 0.46666666666666673, },
        new double[3] { -0.033333333333333326, -0.06666666666666665, 0.1, },
    }, 
    new double[6][] 
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.26666666666666666, 0.26666666666666666, 0.8, },
        new double[3] { 0.26666666666666666, 0.5333333333333333, 0.5333333333333333, },
        new double[3] { 0.26666666666666666, 0.8, 0.26666666666666666, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0.26666666666666666, 0.26666666666666666, 0.8, },
        new double[3] { 0.13333333333333333, 0.26666666666666666, 0.26666666666666666, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.26666666666666666, 0.13333333333333333, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.26666666666666666, 0.13333333333333333, },
    },
    new double[6][] 
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0.13333333333333333, 0.26666666666666666, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.8, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.13333333333333333, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.26666666666666666, 0.13333333333333333, },
        new double[3] { 0, 0, 0, },
    },
},
new double[6][][]
{
    new double[6][]
    {
        new double[3] { 0.1, -0.033333333333333326, -0.06666666666666665, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.46666666666666673, 0.1, 0.1, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][] 
    {
        new double[3] { 0, 0, 0, },
        new double[3] { -0.033333333333333326, 0.1, -0.06666666666666665, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, 0.1, -0.033333333333333326, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    { 
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.1, 0.1, 0.46666666666666673, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, -0.033333333333333326, 0.1, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0.26666666666666666, 0.26666666666666666, 0.8, },
        new double[3] { 0.13333333333333333, 0.26666666666666666, 0.26666666666666666, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.26666666666666666, 0.13333333333333333, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.26666666666666666, 0.13333333333333333, },
    },
    new double[6][]
    {
        new double[3] { 0.26666666666666666, 0.26666666666666666, 0.8, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.5333333333333333, 0.26666666666666666, 0.5333333333333333, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.8, 0.26666666666666666, 0.26666666666666666, },
    },
    new double[6][] 
    {
        new double[3] { 0.13333333333333333, 0.26666666666666666, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.13333333333333333, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.26666666666666666, 0.13333333333333333, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.8, 0.26666666666666666, 0.26666666666666666, },
        new double[3] { 0, 0, 0, },
    },
},
new double[6][][]
{
    new double[6][]
    {
        new double[3] { 0.1, -0.06666666666666665, -0.033333333333333326, },
        new double[3] { 0.46666666666666673, 0.1, 0.1, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][] 
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0.1, 0.46666666666666673, 0.1, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, 0.1, -0.033333333333333326, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.033333333333333326, -0.06666666666666665, 0.1, },
        new double[3] { 0, 0, 0, },
        new double[3] { -0.06666666666666665, -0.033333333333333326, 0.1, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][] 
    {
        new double[3] { 0, 0, 0, },
        new double[3] { 0.13333333333333333, 0.26666666666666666, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.8, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.13333333333333333, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.26666666666666666, 0.13333333333333333, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0.13333333333333333, 0.26666666666666666, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.13333333333333333, 0.26666666666666666, },
        new double[3] { 0.26666666666666666, 0.26666666666666666, 0.13333333333333333, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.8, 0.26666666666666666, 0.26666666666666666, },
        new double[3] { 0, 0, 0, },
    },
    new double[6][]
    {
        new double[3] { 0.26666666666666666, 0.8, 0.26666666666666666, },
        new double[3] { 0.5333333333333333, 0.5333333333333333, 0.26666666666666666, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0.8, 0.26666666666666666, 0.26666666666666666, },
        new double[3] { 0, 0, 0, },
        new double[3] { 0, 0, 0, },
    },
},
};
            public static double[][] BCMatr = new double[3][]
            {
                new double[3] { 4.0 / 30, 2.0 / 30, -1.0 / 30 },
                new double[3] { 2.0 / 30, 16.0 / 30, 2.0 / 30 },
                new double[3] { -1.0 / 30, 2.0 / 30, 4.0 / 30 }
            };
        }
    }
}