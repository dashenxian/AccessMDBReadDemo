using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleApp4
{
    class ExTest
    {
        public void Test()
        {
            Expression<Func<int, int, int>> aa = (int a, int b) => a + b * 2;
            var res = aa.Compile(); // 结果 : false
        }
    }
}
