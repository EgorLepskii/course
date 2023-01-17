using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace course
{
    public class FinitElem
    {
        public IReadOnlyDictionary<int, int> Global;

        public int vert1, vert2, vert3;
        public int edge1, edge2, edge3;
      
        // Create finit element
        public FinitElem(int v1, int v2, int v3, int e1, int e2, int e3, int countOfVerts)
        {
            this.vert1 = v1;
            this.vert2 = v2;
            this.vert3 = v3;
            this.edge1 = e1;
            this.edge2 = e2;
            this.edge3 = e3;
            Global = new Dictionary<int, int>() 
            {
                { 0, v1 }, 
                { 1, v2 }, 
                { 2, v3 }, 
                { 3, e1 + countOfVerts }, 
                { 4, e2 + countOfVerts }, 
                { 5, e3 + countOfVerts } 
            };
        }

     
    }
}