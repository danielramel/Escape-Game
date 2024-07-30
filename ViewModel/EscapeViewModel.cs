using System.Collections.ObjectModel;
using ELTE.Sudoku.Model;

namespace ELTE.Sudoku.ViewModel
{
    /// <summary>
    /// Escape nézetmodell típusa.
    /// </summary>
    public class EscapeViewModel : ViewModelBase
    {


        private EscapeGameModel _model;
        private int _fieldSize;

        private bool _isGamePaused = true;
        private bool _isGameOver = false;


        public int FieldSize { get { return _fieldSize; } set { _fieldSize = value; } }
        public bool IsGamePaused { get { return _isGamePaused; } set { _isGamePaused = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsGameNotPaused)); } }

        public bool IsGameNotPaused { get { return !IsGamePaused; } }

        public bool IsGameNotOver { get => !_isGameOver; set { _isGameOver = !value; OnPropertyChanged(); } }

        #region Properties
        public ObservableCollection<EscapeField> Fields { get; set; }

        public String GameTime { get { return TimeSpan.FromSeconds(_model.GameTime).ToString("g"); } }
        public String PauseGameText { get { return IsGamePaused ? "&#x25B6;" : "&#x23F8;";  } }

        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand PauseGameCommand { get; private set; }

        public DelegateCommand MoveRightCommand { get; private set; }
        public DelegateCommand MoveLeftCommand { get; private set; }
        public DelegateCommand MoveUpCommand { get; private set; }
        public DelegateCommand MoveDownCommand { get; private set; }

        public DelegateCommand LoadGameCommand { get; private set; }

        public DelegateCommand SaveGameCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }
        #endregion


        #region Constructors

        /// <summary>
        /// Escape nézetmodell példányosítása.
        /// </summary>
        /// <param name="model">A modell típusa.</param>
        public EscapeViewModel(EscapeGameModel model)
        {

            // játék csatlakoztatása
            _model = model;
            _model.GameAdvanced += new EventHandler<EscapeEventArgs>(Model_GameAdvanced);
            _model.PlayerMoved += new EventHandler<EscapeEventArgs>(Model_PlayerMoved);

            NewGameCommand = new DelegateCommand(param => OnNewGame(param));
            PauseGameCommand = new DelegateCommand(param => OnPauseGame());

            MoveRightCommand = new DelegateCommand(param => MovePlayer(0));
            MoveLeftCommand = new DelegateCommand(param => MovePlayer(1));
            MoveUpCommand = new DelegateCommand(param => MovePlayer(2));
            MoveDownCommand = new DelegateCommand(param => MovePlayer(3));

            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());



            FieldSize = _model.Table.Size;
            Fields = new ObservableCollection<EscapeField>();

            GenerateTable();
            RefreshTable();

        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        /// <summary>
        /// Új játék eseménye.
        /// </summary>
        public event EventHandler<int>? NewGame;

        public event EventHandler? PauseGame;

        /// <summary>
        /// Játék betöltésének eseménye.
        /// </summary>
        public event EventHandler? LoadGame;

        public event EventHandler? SaveGame;

        public event EventHandler? ExitGame;


        /// <summary>
        /// Új játék indításának eseménykiváltása.
        /// </summary>
        private void OnNewGame(object? newFieldSize)
        {
            _isGameOver = false;
            OnPropertyChanged(nameof(IsGameNotOver));
            if (newFieldSize == null) newFieldSize = _fieldSize;
            int temp;
            int.TryParse(newFieldSize.ToString(), out temp);


            NewGame?.Invoke(this, temp);

            FieldSize = _model.Table.Size;
            GenerateTable();
            RefreshTable();
            OnPropertyChanged(nameof(_fieldSize));
            OnPropertyChanged(nameof(Fields));
            OnPropertyChanged(nameof(GameTableRows));
            OnPropertyChanged(nameof(GameTableColumns));
            IsGamePaused = false;
            OnPropertyChanged(nameof(IsGamePaused));
            OnPropertyChanged(nameof(PauseGameText));

        }

        public void OnPauseGame()
        {
            IsGamePaused = !IsGamePaused;
            OnPropertyChanged(nameof(IsGamePaused));
            OnPropertyChanged(nameof(PauseGameText));
            PauseGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        public void OnGameLoaded()
        {
            FieldSize = _model.Table.Size;
            GenerateTable();
            RefreshTable();
            OnPropertyChanged(nameof(_fieldSize));
            OnPropertyChanged(nameof(Fields));
            OnPropertyChanged(nameof(GameTableRows));
            OnPropertyChanged(nameof(GameTableColumns));
            OnPropertyChanged(nameof(IsGamePaused));
            OnPropertyChanged(nameof(PauseGameText));
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }



        #region Command Methods

        private void MovePlayer(int direction)
        {
            if (IsGamePaused) return;

            _model.MovePlayer(direction);
        }


        #endregion

        #region Private methods

        private void GenerateTable()
        {
            Fields = new ObservableCollection<EscapeField>();
            for (int i = 0; i < _model.Table.Size; i++)
            {
                for (int j = 0; j < _model.Table.Size; j++)
                {
                    Fields.Add(new EscapeField
                    {
                        Text = String.Empty,
                        X = i,
                        Y = j,
                        Number = i * _model.Table.Size + j
                    });;
                }
            }
        }

        /// <summary>
        /// Tábla frissítése.
        /// </summary>
        public void RefreshTable()
        {
            foreach (EscapeField field in Fields)
            {
                field.Text = _model.Table[field.X, field.Y].ToString();
            }
        }

        private void Model_GameAdvanced(object? sender, EscapeEventArgs e)
        {
            RefreshTable();
            OnPropertyChanged(nameof(GameTime));
        }

        private void Model_PlayerMoved(object? sender, EscapeEventArgs e)
        {
            RefreshTable();
        }


        /// <summary>
        /// Segédproperty a tábla méretezéséhez
        /// </summary>
        public RowDefinitionCollection GameTableRows
        {
            
            get => new RowDefinitionCollection(Enumerable.Repeat(new RowDefinition(GridLength.Star), FieldSize).ToArray());
        }

        /// <summary>
        /// Segédproperty a tábla méretezéséhez
        /// </summary>
        public ColumnDefinitionCollection GameTableColumns
        {
            get => new ColumnDefinitionCollection(Enumerable.Repeat(new ColumnDefinition(GridLength.Star), FieldSize).ToArray());
        }


        #endregion
    }
}
