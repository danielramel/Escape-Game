using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace ELTE.Sudoku.Persistence
{
    public class EscapeFileDataAccess : IEscapeDataAccess
    {
        private String? _directory = String.Empty;
        public EscapeFileDataAccess(String? saveDirectory = null)
        {
            _directory = saveDirectory;
        }

        public async Task SaveAsync(String path, EscapeTable table)
        {
            if (!String.IsNullOrEmpty(_directory))
                path = Path.Combine(_directory, path);

            try {
                using (StreamWriter writer = new StreamWriter(path))
                {

                    await writer.WriteLineAsync(table.Size.ToString());

                    await writer.WriteLineAsync(table.GameTime.ToString());
                    for (int i = 0; i < table.Size; i++)
                    {
                        for (int j = 0; j < table.Size; j++)
                        {
                            await writer.WriteAsync(table[i, j].ToString());
                        }
                        await writer.WriteLineAsync();
                    }
                }
            } catch
            {
                throw new EscapeDataException();
            }
             

        }

        public async Task<EscapeTable> LoadAsync(String path)
        {


            if (!String.IsNullOrEmpty(_directory))
            {
                path = Path.Combine(_directory, path);
            }
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line = await reader.ReadLineAsync() ?? String.Empty;

                    int tableSize = int.Parse(line);

                    line = await reader.ReadLineAsync() ?? String.Empty;

                    int gameTime = int.Parse(line);

                    List<(int, int)> chasers = new List<(int, int)>();

                    HashSet<(int,int)> bombs = new HashSet<(int, int)>();
                    int[] playerPosition = new int[] {0,0};


                    for (int i = 0; i < tableSize; i++)
                    {
                        line = await reader.ReadLineAsync() ?? String.Empty;

                        for (int j = 0; j < tableSize; j++)
                        {
                            if (line[j] == 'C')
                            {
                                chasers.Add((i, j));

                            } else if (line[j] == 'X')
                            {
                                bombs.Add((i, j));

                            } else if (line[j] == 'O')
                            {
                                playerPosition = new int[] { i, j };
                            }
                        }
                    }
                    return new EscapeTable(tableSize, playerPosition, chasers, bombs, gameTime);
                }
            }
            catch
            {
                throw new EscapeDataException();
            }
        }
    }
}
