using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace ELTE.Sudoku.Persistence
{
    public class EscapeTable
    {
        private char[,] _fieldValues;
        private int _tableSize;
        private int[] _playerPosition;
        private List<(int, int)> _chasers;
        private int _gameTime;


        public int Size { get { return _tableSize; } }

        public List<(int, int)> Chasers { get { return _chasers; } }

        public int[] PlayerPosition { get { return _playerPosition; } }

        public char this[int x, int y] { get { return _fieldValues[x, y]; } }

        public int GameTime { get { return _gameTime; } set { _gameTime = value; } }



        public EscapeTable(int tableSize, int[] playerPosition, List<(int, int)> chasers, HashSet<(int, int)> bombs, int gameTime)
        {

            _fieldValues = new char[tableSize, tableSize];
            _tableSize = tableSize;
            _playerPosition = playerPosition;
            _chasers = chasers;
            

            _gameTime = gameTime;


            for (int i = 0; i < tableSize; i++)
            {
                for (int j = 0; j < tableSize; j++)
                {
                    _fieldValues[i, j] = ' ';
                }
            }

            Random random = new Random();

            
            _fieldValues[_playerPosition[0], _playerPosition[1]] = 'O';
            
            foreach (var chaser in chasers)
            {
                _fieldValues[chaser.Item1, chaser.Item2] = 'C';
            }

            foreach (var bomb in bombs)
            {
                _fieldValues[bomb.Item1, bomb.Item2] = 'X';
            }
        }

        public void move(int[] startPosition, int[] delta)
        {
            char _character = _fieldValues[startPosition[0], startPosition[1]];
            _fieldValues[startPosition[0], startPosition[1]] = ' ';

            _fieldValues[startPosition[0]+delta[0], startPosition[1]+delta[1]] = _character;
        }

        public Boolean movePlayer(int[] delta)
        {

            if (IsBomb(_playerPosition[0]+delta[0], _playerPosition[1]+delta[1]))
            {
                return false;
            }
            for (int i = 0; i < _chasers.Count(); i++)
            {
                if (_fieldValues[_playerPosition[0]+delta[0], _playerPosition[1]+delta[1]]=='C'){
                    return false;
                }
            }

            this.move(_playerPosition, delta);

            _playerPosition = new int[2] { _playerPosition[0] + delta[0], _playerPosition[1] + delta[1] };
            return true;
        }

        public int moveChasers()
        {
            int[] newPos;

            int i = 0;
            while (i < _chasers.Count()) {

                newPos = new int[2] { _chasers[i].Item1, _chasers[i].Item2 };


                int[] direction = new int[2] { 0, 0 };
                int[] delta = new int[2] { _chasers[i].Item1 - _playerPosition[0], _chasers[i].Item2 - _playerPosition[1] };

                if (Math.Abs(delta[0]) > Math.Abs(delta[1]))
                {
                    direction[0] = -Math.Sign(delta[0]);
                    newPos[0] = _chasers[i].Item1 + direction[0];
                }
                else
                {
                    direction[1] = -Math.Sign(delta[1]);
                    newPos[1] = _chasers[i].Item2 + direction[1];
                }

                if (IsBomb(newPos[0], newPos[1]) || _fieldValues[newPos[0], newPos[1]] == 'C')  {
                    _fieldValues[_chasers[i].Item1, _chasers[i].Item2] = ' ';
                    _chasers.RemoveAt(i);
                    continue;
                }

                this.move(new int[2] { _chasers[i].Item1, _chasers[i].Item2 }, direction);

                _chasers[i] = (newPos[0], newPos[1]);

                i++;
                if (newPos[0] == _playerPosition[0] && newPos[1] == _playerPosition[1])
                {
                    return 2;
                }
            }


            return _chasers.Count == 0 ? 1 : 0;
        }

        public bool IsBomb(int x, int y)
        {
            return _fieldValues[x, y] == 'X';
        }


    }
}
