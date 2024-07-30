using ELTE.Sudoku.Model;
using ELTE.Sudoku.Persistence;
using ELTE.Sudoku.ViewModel;

namespace ELTE.Sudoku
{
    public partial class App : Application
    {
        private const string SuspendedGameSavePath = "SuspendedGame";

        private readonly AppShell _appShell;
        private readonly EscapeGameModel _escapeGameModel;
        private readonly EscapeViewModel _escapeViewModel;
        private readonly IEscapeDataAccess _escapeDataAccess;
        private readonly IStore _escapeStore;


        public App()
        {
            InitializeComponent();

            _escapeStore = new EscapeStore();
            _escapeDataAccess = new EscapeFileDataAccess(FileSystem.AppDataDirectory);

            _escapeGameModel = new EscapeGameModel(_escapeDataAccess);
            _escapeGameModel.NewGame();

            _escapeViewModel = new EscapeViewModel(_escapeGameModel);

            _appShell = new AppShell(_escapeStore, _escapeDataAccess, _escapeGameModel, _escapeViewModel)
            {
                BindingContext = _escapeViewModel
            };

            MainPage = _appShell;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);

            // amikor az alkalmazás fókuszba kerül
            window.Activated += (s, e) =>
            {
                if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath)))
                    return;

                Task.Run(async () =>
                {
                    // betöltjük a felfüggesztett játékot, amennyiben van
                    try
                    {
                        await _escapeGameModel.LoadGameAsync(SuspendedGameSavePath);
                    }
                    catch
                    {
                    }
                });
            };

            // amikor az alkalmazás fókuszt veszt
            window.Deactivated += (s, e) =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        // elmentjük a jelenleg folyó játékot
                        _escapeViewModel.OnPauseGame();
                        await _escapeGameModel.SaveGameAsync(SuspendedGameSavePath);
                    }
                    catch
                    {
                    }
                });
            };


            window.Destroying += (s, e) =>
            {
                if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath)))
                    return;
                try
                {
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath));
                }
                catch
                {
                }
            };

            return window;
        }
    }
}