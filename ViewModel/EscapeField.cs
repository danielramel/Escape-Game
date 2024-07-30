

namespace ELTE.Sudoku.ViewModel
{
    /// <summary>
    /// Escape játékmező típusa.
    /// </summary>
    public class EscapeField : ViewModelBase
    {

        private String _text = String.Empty;

        /// <summary>
        /// Felirat lekérdezése, vagy beállítása.
        /// </summary>
        public String Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Vízszintes koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Függőleges koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Sorszám lekérdezése.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Lépés parancs lekérdezése, vagy beállítása.
        /// </summary>
        public DelegateCommand? StepCommand { get; set; }
    }
}
