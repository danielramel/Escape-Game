using ELTE.Sudoku.Persistence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELTE.Sudoku.Model
{

    public class EscapeGameModel
    {
        private EscapeTable _table = null!;
        private int _fieldSize;
        private int _gameTime;
        private IEscapeDataAccess _dataAccess = null!;
        private bool _isWon = false;
        private bool _isOver = false;



        public event EventHandler<EscapeEventArgs>? PlayerMoved;

        public event EventHandler<EscapeEventArgs>? GameAdvanced;

        public event EventHandler<EscapeEventArgs>? GameOver;

        public bool IsWon { get { return _isWon; } }

        public bool IsOver { get { return _isOver; } }

        public int GameTime { get { return _gameTime; } }

        public EscapeTable Table { get { return _table; } }


        public EscapeGameModel(IEscapeDataAccess dataAccess)
        {
            _dataAccess = dataAccess;

        }
        public void NewGame()
        {
            NewGame(11);
        }

        public void NewGame(int fieldSize)
        {
            _isOver = false;
            _gameTime = 0;
            _fieldSize = fieldSize;


            int bombsCount = fieldSize-3;

            Random random = new Random();

            List<(int, int)> chasers = new List<(int, int)> { (_fieldSize - 1, 0), (_fieldSize - 1, _fieldSize - 1) };

            HashSet<(int, int)> bombs = new HashSet<(int, int)>();

            while (bombs.Count < bombsCount)
            {

                int x = random.Next(_fieldSize); 
                int y = random.Next(_fieldSize);

                if (x == 0 && y == _fieldSize / 2 || chasers.Contains((x, y)))
                { continue; }

                Console.WriteLine(x.ToString(), y.ToString());

                bombs.Add((x, y));
            }

            /*
            int chaserCount = 1;
            List<(int, int)> chasers = new List<(int, int)>();

            while (chasers.Count < chaserCount)
            {
                int x = random.Next(_fieldSize);
                int y = random.Next(_fieldSize);

                if (((Math.Abs(0 - x) + Math.Abs(_fieldSize / 2 - y))) < 4
                    || bombs.Contains((x,y)) 
                    || chasers.Contains((x,y)))
                { continue; }


                chasers.Add((x, y));
            }*/

            _table = new EscapeTable(_fieldSize, new int[] { 0, _fieldSize / 2 }, chasers, bombs, 0);
            
        }

        public void NewGame(int tableSize, int[] playerPosition, List<(int, int)> chasers, HashSet<(int, int)> bombs, int gameTime)
        {
            _isOver = false;
            _gameTime = gameTime;
            _fieldSize = tableSize;
            _table = new EscapeTable(_fieldSize, new int[] { 0, _fieldSize / 2 }, new List<(int, int)> { (_fieldSize - 1, 0), (_fieldSize - 1, _fieldSize - 1) }, bombs, 0);
        }


        public void AdvanceTime()
        {
            _gameTime++;
            _table.GameTime++;

            int rValue = _table.moveChasers();
            if (rValue != 0)
            {

                OnGameOver(rValue == 1 ? true : false);
            }

            OnGameAdvanced();
        }

        public void MovePlayer(int direction)
        {
            if ((direction == 0 && _table.PlayerPosition[1] == _fieldSize - 1) ||
                (direction == 1 && _table.PlayerPosition[1] == 0) ||
                (direction == 2 && _table.PlayerPosition[0] == 0) ||
                (direction == 3 && _table.PlayerPosition[0] == _fieldSize - 1))
            {
                return;
            }
            int[] delta = direction switch
            {
                0 => new int[] { 0, 1 },
                1 => new int[] { 0, -1 },
                2 => new int[] { -1, 0 },
                3 => new int[] { 1, 0 },
                _ => new int[2]
            };

            if (!_table.movePlayer(delta))
            {
                OnGameOver(false);
            }
            OnPlayerMoved();
        }

        private void OnPlayerMoved()
        {
            PlayerMoved?.Invoke(this, new EscapeEventArgs(_gameTime, false));
        }

        private void OnGameAdvanced()
        {
            GameAdvanced?.Invoke(this, new EscapeEventArgs(_gameTime, false));
        }

        private void OnGameOver(Boolean isWon)
        {
            _isWon = isWon;
            _isOver = true;
            GameOver?.Invoke(this, new EscapeEventArgs(_gameTime, isWon));
        }

        public async Task SaveGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await _dataAccess.SaveAsync(path, _table);
        }

        public async Task LoadGameAsync(String path)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("No data access is provided.");
            }
            _table = await _dataAccess.LoadAsync(path);
            _gameTime = _table.GameTime;
            _fieldSize = Table.Size;

        }
    }
}
