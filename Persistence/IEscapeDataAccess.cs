using System;
using System.Threading.Tasks;

namespace ELTE.Sudoku.Persistence
{
    /// <summary>
    /// Escape fájl kezelő felülete.
    /// </summary>
    public interface IEscapeDataAccess
    {
        /// <summary>
        /// Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="table">A fájlba kiírandó játéktábla.</param>
        Task SaveAsync(String path, EscapeTable table);

        Task<EscapeTable> LoadAsync(String path);
    }
}
