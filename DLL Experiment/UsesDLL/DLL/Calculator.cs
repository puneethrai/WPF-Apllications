using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLL
{
    public class Calculator
    {
        /// <summary>
        /// Returns sum of two number
        /// </summary>
        public long Sum(long x, long y)
        {
            return (x + y);
        }
        /// <summary>
        /// Returns product of two number
        /// </summary>
        public long Multiply(long x, long y)
        {
            return (x * y);
        }
        /// <summary>
        /// Returns diff of two number
        /// </summary>
        public long Difference(long x, long y)
        {
            return (x - y);
        }
        /// <summary>
        /// Returns quotient of two number
        /// </summary>
        public long Divide(long x, long y)
        {
            if (x == 0)
                throw new DivideByZeroException();
            else
                return(x / y);
        }
    }
}
