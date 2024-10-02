using ELTE.Sudoku.Model;
using ELTE.Sudoku.Persistence;
using ELTE.Sudoku.View;
using ELTE.Sudoku.ViewModel;

namespace ELTE.Sudoku
{
    public partial class AppShell : Shell
    {

        private readonly EscapeGameModel _escapeGameModel;
        private readonly EscapeViewModel _escapeViewModel;

        private readonly IDispatcherTimer _timer;

        private readonly IStore _store;
        private readonly StoredGameBrowserModel _storedGameBrowserModel;
        private readonly StoredGameBrowserViewModel _storedGameBrowserViewModel;

        public AppShell(IStore escapeStore, IEscapeDataAccess escapeDataAccess, EscapeGameModel escapeGameModel, EscapeViewModel escapeViewModel)
        {
            InitializeComponent();

            _store = escapeStore;
            _escapeGameModel = escapeGameModel;
            _escapeViewModel = escapeViewModel;

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (_, _) => _escapeGameModel.AdvanceTime();

            
            _escapeGameModel.GameOver += EscapeGameModel_GameOver;

            _escapeViewModel.NewGame += new EventHandler<int>(EscapeViewModel_NewGame);
            _escapeViewModel.PauseGame += EscapeViewModel_PauseGame;
            _escapeViewModel.ExitGame += EscapeViewModel_ExitGame;
            _escapeViewModel.SaveGame += EscapeViewModel_SaveGame;
            _escapeViewModel.LoadGame += EscapeViewModel_LoadGame;

            _storedGameBrowserModel = new StoredGameBrowserModel(_store);
            _storedGameBrowserViewModel = new StoredGameBrowserViewModel(_storedGameBrowserModel);
            _storedGameBrowserViewModel.GameLoading += StoredGameBrowserViewModel_GameLoading;
            _storedGameBrowserViewModel.GameSaving += StoredGameBrowserViewModel_GameSaving;

        }

        private async void EscapeViewModel_LoadGame(object? sender, EventArgs e)
        {
            await _storedGameBrowserModel.UpdateAsync(); // frissítjük a tárolt játékok listáját
            await Navigation.PushAsync(new LoadGamePage
            {
                BindingContext = _storedGameBrowserViewModel
            }); // átnavigálunk a lapra
        }

        private async void StoredGameBrowserViewModel_GameSaving(object? sender, StoredGameEventArgs e)
        {
            await Navigation.PopAsync(); // visszanavigálunk
            StopTimer();

            try
            {
                // elmentjük a játékot
                await _escapeGameModel.SaveGameAsync(e.Name);
                await DisplayAlert("Escape játék", "Sikeres mentés.", "OK");
            }
            catch
            {

                await DisplayAlert("Escape játék", "Sikertelen mentés.", "OK");
            }
        }

        private async void StoredGameBrowserViewModel_GameLoading(object? sender, StoredGameEventArgs e)
        {
            await Navigation.PopAsync(); // visszanavigálunk

            // betöltjük az elmentett játékot, amennyiben van
            try
            {
                await _escapeGameModel.LoadGameAsync(e.Name);

                // sikeres betöltés
                await Navigation.PopAsync(); // visszanavigálunk a játék táblára
                await DisplayAlert("Escape játék", "Sikeres betöltés.", "OK");

                _escapeViewModel.OnGameLoaded();
            }
            catch
            {
                await DisplayAlert("Escape játék", "Sikertelen betöltés.", "OK");
            }
        }

        private async void EscapeViewModel_SaveGame(object? sender, EventArgs e)
        {
            await _storedGameBrowserModel.UpdateAsync(); // frissítjük a tárolt játékok listáját
            await Navigation.PushAsync(new SaveGamePage
            {
                BindingContext = _storedGameBrowserViewModel
            });
        }

        private async void EscapeViewModel_ExitGame(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage
            {
                BindingContext = _escapeViewModel
            });
        }

        private void EscapeViewModel_PauseGame(object? sender, EventArgs e)
        {
            if (_timer.IsRunning)
            {
                StopTimer();
            }
            else
            {
                StartTimer();
            }
        }


        /// <summary>
        ///     Elindtja a játék léptetéséhez használt időzítőt.
        /// </summary>
        internal void StartTimer() => _timer.Start();


        /// <summary>
        ///     Megállítja a játék léptetéséhez használt időzítőt.
        /// </summary>
        internal void StopTimer() => _timer.Stop();


        private async void EscapeGameModel_GameOver(object? sender, EscapeEventArgs e)
        {
            StopTimer();
            _escapeViewModel.IsGamePaused = true;
            _escapeViewModel.IsGameNotOver = false;

            if (e.IsWon)
            {
                await DisplayAlert("Escape játék",
                    "Gratulálok, győztél!",
                    "OK");
            }
            else
            {
                await DisplayAlert("Escape játék", "Sajnálom, vesztettél!", "OK");
            }
        }

        private void EscapeViewModel_NewGame(object? sender, int newFieldSize)
        {
            _escapeGameModel.NewGame(newFieldSize);

            StartTimer();
        }
    }
}