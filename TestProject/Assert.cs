using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject
{
    class Assert
    {
        public static void AreEqual(object a,object b)
        {
            if (!a.Equals(b))
            {
                throw new Exception($"expected is {a.ToString()} but have {b.ToString()}");
            }
        }
        public static void Fail()
        {
            throw new Exception("assert fail reached");
        }
    }
}
