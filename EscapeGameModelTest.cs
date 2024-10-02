using ELTE.Sudoku.Model;
using ELTE.Sudoku.Persistence;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Escape.Test
{
    [TestClass]
    public class EscapeGameModelTest
    {
        private EscapeGameModel _model = null!;
        private EscapeTable _mockedTable = null!;
        private Mock<IEscapeDataAccess> _mock = null!;


        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new EscapeTable(
                11,
                new int[] { 0, 6 },
                new List<(int, int)> { (10, 0), (10, 10) },
                new HashSet<(int, int)> { }, 0
                );

            _mock = new Mock<IEscapeDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(_mockedTable));

            _model = new EscapeGameModel(_mock.Object);

            _model.GameAdvanced += new EventHandler<EscapeEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<EscapeEventArgs>(Model_GameOver);
        }


        [TestMethod]
        public void EscapeGameModelNewGame15x15Test()
        {
            _model.NewGame(15);

            Assert.AreEqual(0, _model.GameTime);
            Assert.AreEqual(15, _model.Table.Size);

            int chasersCount = 0;

            for (int i = 0; i < _model.Table.Size; i++)
            {
                for (int j = 0; j < _model.Table.Size; j++)
                {
                    if (_model.Table[i, j] == 'C')
                    {
                        chasersCount++;
                    }
                }
            }
            Assert.AreEqual(2, chasersCount);
        }

        [TestMethod]
        public void EscapeGameModelNewGame11x11Test()
        {
            _model.NewGame();

            Assert.AreEqual(0, _model.GameTime);
            Assert.AreEqual(11, _model.Table.Size);

            int chasersCount = 0;

            for (int i = 0; i < _model.Table.Size; i++)
            {
                for (int j = 0; j < _model.Table.Size; j++)
                {
                    if (_model.Table[i, j] == 'C')
                    {
                        chasersCount++;
                    }
                }
            }
            Assert.AreEqual(2, chasersCount);
        }

        [TestMethod]
        public void EscapeGameModelNewGameHardTest()
        {
            _model.NewGame(21);

            Assert.AreEqual(0, _model.GameTime);
            Assert.AreEqual(21, _model.Table.Size);

            int chasersCount = 0;

            for (int i = 0; i < _model.Table.Size; i++)
            {
                for (int j = 0; j < _model.Table.Size; j++)
                {
                    if (_model.Table[i, j] == 'C')
                    {
                        chasersCount++;
                    }
                }
            }
            Assert.AreEqual(2, chasersCount);
        }

        [TestMethod]
        public void EscapeGameModelTimeTest()
        {
            _model.NewGame();


            Assert.AreEqual(0, _model.GameTime);

            _model.AdvanceTime();
            _model.AdvanceTime();
            _model.AdvanceTime();
            _model.AdvanceTime();
            _model.AdvanceTime();

            Assert.AreEqual(5, _model.GameTime);

            _model.NewGame();


            Assert.AreEqual(0, _model.GameTime);
        }





        [TestMethod]
        public async Task PlayerMovementTestAsync()
        {

            await _model.LoadGameAsync(string.Empty);


            for (int i = 2; i < 1E3; i++)
            {
                _model.MovePlayer(i % 2);
            }

            Assert.AreEqual(0, _model.Table.PlayerPosition[0]);
            Assert.AreEqual(6, _model.Table.PlayerPosition[1]);


            // check for player moving out of bounds
            _model.MovePlayer(0);
            _model.MovePlayer(0);
            _model.MovePlayer(0);
            _model.MovePlayer(0);
            _model.MovePlayer(0);
            _model.MovePlayer(0);
            _model.MovePlayer(0);
            _model.MovePlayer(0);
            _model.MovePlayer(0);

            Assert.AreEqual(10, _model.Table.PlayerPosition[1]);



            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);
            _model.MovePlayer(1);

            Assert.AreEqual(0, _model.Table.PlayerPosition[1]);


            _model.MovePlayer(0);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);
            _model.MovePlayer(3);

            Assert.AreEqual(10, _model.Table.PlayerPosition[0]);

            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);
            _model.MovePlayer(2);

            Assert.AreEqual(0, _model.Table.PlayerPosition[0]);


        }

        [TestMethod]
        public async Task EscapeGameModelLoadTest()
        {

            // majd betöltünk egy játékot
            await _model.LoadGameAsync(string.Empty);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Assert.AreEqual(_mockedTable[i, j], _model.Table[i, j]);
                    // ellenőrizzük, valamennyi mező értéke megfelelő-e
                }


            // ellenőrizzük, hogy meghívták-e a Load műveletet a megadott paraméterrel
            _mock.Verify(dataAccess => dataAccess.LoadAsync(string.Empty), Times.Once());
        }

        [TestMethod]
        public void PlayerWinningTest()
        {
            _model.NewGame(
                11,
                new int[] { 9, 0 },
                new List<(int, int)> { (10, 0), (10, 10) },
                new HashSet<(int, int)> { (9, 0), (9, 10) }, 0);

            _model.AdvanceTime();
            _model.AdvanceTime();
            _model.AdvanceTime();
            _model.AdvanceTime();

            Assert.AreEqual(true, _model.IsOver);
            Assert.AreEqual(true, _model.IsWon);

        }


        [TestMethod]
        public void oneChaserLeft()
        {


            _model.NewGame(
    11,
    new int[] { 8, 0 },
    new List<(int, int)> { (10, 0), (10, 10) },
    new HashSet<(int, int)> { (9, 0) }, 0);

            _model.AdvanceTime();
            _model.AdvanceTime();
            _model.AdvanceTime();
            _model.AdvanceTime();


            int chasersCount = 0;

            for (int i = 0; i < _model.Table.Size; i++)
            {
                for (int j = 0; j < _model.Table.Size; j++)
                {
                    if (_model.Table[i, j] == 'C')
                    {
                        chasersCount++;
                    }
                }
            }
            Assert.AreEqual(1, chasersCount);

        }


        private void Model_GameAdvanced(object? sender, EscapeEventArgs e)
        {
            Assert.IsTrue(_model.GameTime >= 0); // a játékidő nem lehet negatív

            Assert.AreEqual(e.GameTime, _model.GameTime);

            Assert.IsFalse(e.IsWon); // még nem nyerték meg a játékot
        }

        private void Model_GameOver(object? sender, EscapeEventArgs e)
        {
            Assert.AreEqual(_model.Table.Chasers.Count == 0, e.IsWon);

        }
    }
}