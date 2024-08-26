
namespace DreamTeam.UnitTests
{
    public class Tests
    {
        [Test]
        public void Test1()
            // arrange / preparar 
        {     int a = 1; int b = 2;
            // act     / agir 
            int resultado = Somar(a, b);
            // assert  verificar/ avaliar
            Assert.AreEqual(3,resultado);
        }


        private int Somar(int a, int b)
        {
            return a + b;
        }
    }
}