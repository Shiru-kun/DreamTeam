using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamTeam.E2ETests
{
    internal class Class1
    {
        public void test()
        {
            var page = new Tests();
            page.Page.GotoAsync("");
        }
        public void main (string[] args) {
        }
    }
}
