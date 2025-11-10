using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace PozemiuRobotas
{
    public enum TileType
    {
        Empty, Wall, Key, Door, Trap, Exit
    }

    public class Tile
    {
        public TileType Type { get; private set; }
        public bool IsDiscovered { get; set; }
        public Tile(TileType type)
        {
            Type = type;
            IsDiscovered = false;
        }
        public void TurnInto(TileType newtype) => Type = newtype;
    }

    public class Map
    {
        public int Width { get; }
        public int Height { get; }
        private readonly Tile[,] _grid;

        public Map(string[] layout)
        {
            Height = layout.Length;
            Width = layout.Max(row => row.Length);
            _grid = new Tile[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char c = (x < layout[y].Length) ? layout[y][x] : '#';
                    _grid[x, y] = new Tile(CharToTile(c));
                }
            }
        }

        private static TileType CharToTile(char c)
        {
            return c switch
            {
                '#' => TileType.Wall,
                'K' => TileType.Key,
                'D' => TileType.Door,
                '^' => TileType.Trap,
                'E' => TileType.Exit,
                _ => TileType.Empty
            };
        }

        public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;
        public Tile Get(int x, int y) => _grid[x, y];

        public void DiscoverRadius(int cx, int cy, int radius)
        {
            int r2 = radius * radius;
            for (int y = Math.Max(0, cy - radius); y <= Math.Min(Height - 1, cy + radius); y++)
            {
                for (int x = Math.Max(0, cx - radius); x <= Math.Min(Width - 1, cx + radius); x++)
                {
                    int dx = x - cx; int dy = y - cy;
                    if (dx * dx + dy * dy <= r2)
                        _grid[x, y].IsDiscovered = true;
                }
            }
        }
    }

    public abstract class RobotModule
    {
        public string Name { get; }
        protected RobotModule(string name) => Name = name;
        public bool Enabled { get; private set; }
        public void Toggle() => Enabled = !Enabled;
        public virtual int ModifyVisionRadius(int baseRadius) => baseRadius;
        public virtual int ModifyMoveEnergyCost(int baseCost) => baseCost;
    }

    public class SensorModule : RobotModule
    {
        public SensorModule() : base("Jutiklių modulis") { }
        public override int ModifyVisionRadius(int baseRadius) => Enabled ? baseRadius + 2 : baseRadius;
    }

    public class EfficiencyModule : RobotModule
    {
        public EfficiencyModule() : base("Efektyvumo modulis") { }
        public override int ModifyMoveEnergyCost(int baseCost) => Enabled ? Math.Max(1, baseCost / 2) : baseCost;
    }

    public class Player
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Keys { get; private set; }
        public int Energy { get; private set; }
        public bool Alive { get; private set; } = true;

        public int BaseVisionRadius { get; } = 3;
        public int BaseMoveEnergyCost { get; } = 2;

        private readonly List<RobotModule> _modules = new();

        public Player(int x, int y, int energy)
        {
            X = x; Y = y; Energy = energy;
            _modules.Add(new SensorModule());
            _modules.Add(new EfficiencyModule());
        }

        public IEnumerable<RobotModule> Modules => _modules;

        public int CurrentVisionRadius()
        {
            int r = BaseVisionRadius;
            foreach (var m in _modules) r = m.ModifyVisionRadius(r);
            return r;
        }

        public int CurrentMoveEnergyCost()
        {
            int c = BaseMoveEnergyCost;
            foreach (var m in _modules) c = m.ModifyMoveEnergyCost(c);
            return c;
        }

        public void AddKey() => Keys++;
        public bool UseKey()
        {
            if (Keys > 0) { Keys--; return true; }
            return false;
        }

        public void DamageByTrap() => Alive = false;

        public void ConsumeEnergy(int amount)
        {
            Energy -= amount;
            if (Energy <= 0) { Energy = 0; Alive = false; }
        }

        public void MoveTo(int nx, int ny)
        {
            X = nx; Y = ny;
        }

        public void ToggleModule(int index)
        {
            if (index >= 0 && index < _modules.Count)
                _modules[index].Toggle();
        }
    }

    public enum GameState { Running, Won, Lost }

    public class Game
    {
        private readonly Map _map;
        private readonly Player _player;
        private readonly (int x, int y) _exitPos;

        public Game(string[] layout)
        {
            int startX = 1, startY = 1;
            int exitX = 1, exitY = 1;

            var normalized = new string[layout.Length];
            for(int y=0; y<layout.Length; y++)
            {
                var row = layout[y].ToCharArray();
                for(int x=0; x<row.Length; x++)
                {
                    if (row[x] == 'S') { startX = x; startY = y; row[x] = '.'; }
                    if (row[x] == 'E') { exitX = x; exitY = y; }
                }
                normalized[y] = new string(row);
            }

            _map = new Map(normalized);
            _player = new Player(startX, startY, 100);
            _exitPos = (exitX, exitY);

            _map.DiscoverRadius(_player.X, _player.Y, _player.CurrentVisionRadius());
        }

        public void Run()
        {
            Console.CursorVisible = false;
            GameState state = GameState.Running;

            while (state == GameState.Running)
            {
                Render();
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (HandleInput(key, out state))
                    continue;
            }

            Render();
            Console.WriteLine();
            Console.WriteLine(state == GameState.Won ? "\nSveikinimai! Radote išėjimą." : "\nDeja, pralaimėjote.");
            Console.CursorVisible = true;
        }

        private bool HandleInput(ConsoleKeyInfo key, out GameState state)
        {
            state = GameState.Running;

            if (key.Key == ConsoleKey.D1) _player.ToggleModule(0);
            if (key.Key == ConsoleKey.D2) _player.ToggleModule(1);

            int nx = _player.X, ny = _player.Y;
            bool moved = false;
            switch (key.Key)
            {
                case ConsoleKey.UpArrow: ny--; moved = true; break;
                case ConsoleKey.DownArrow: ny++; moved = true; break;
                case ConsoleKey.LeftArrow: nx--; moved = true; break;
                case ConsoleKey.RightArrow: nx++; moved = true; break;
                case ConsoleKey.Escape: state = GameState.Lost; return true;
            }

            if(moved)
            {
                TryMove(nx, ny, ref state);
                return true;
            }

            return false;
        }

        private void TryMove(int nx, int ny, ref GameState state)
        {
            if(!_map.InBounds(nx, ny)) return;

            var dest = _map.Get(nx, ny);
            if (dest.Type == TileType.Wall) return;

            if (dest.Type == TileType.Door)
            {
                if (_player.UseKey()) dest.TurnInto(TileType.Empty);
                else return;
            }

            _player.ConsumeEnergy(_player.CurrentMoveEnergyCost());
            if(!_player.Alive) { state = GameState.Lost; return; }

            _player.MoveTo(nx, ny);
            dest = _map.Get(nx, ny);

            switch (dest.Type)
            {
                case TileType.Key:
                    _player.AddKey();
                    dest.TurnInto(TileType.Empty);
                    break;
                case TileType.Trap:
                    _player.DamageByTrap();
                    state = GameState.Lost;
                    break;
                case TileType.Exit:
                    state = GameState.Won;
                    break;
            }

            _map.DiscoverRadius(_player.X, _player.Y, _player.CurrentVisionRadius());
        }

        private void Render()
        {
            Console.SetCursorPosition(0, 0);
            for(int y= 0; y<_map.Height; y++)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    bool visible = IsInVision(x, y);
                    var t = _map.Get(x, y);
                    char ch;
                    if (_player.X == x && _player.Y == y) ch = 'R';
                    else if (!t.IsDiscovered && !visible) ch = ' ';
                    else if (!visible) ch = ' ';
                    else ch = TileChar(t.Type);

                    Console.Write(ch);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine($"Energy: {_player.Energy}   Raktai: {_player.Keys}");
            var mods = _player.Modules.Select((m, i) => $"{i + 1}:{m.Name}[{(m.Enabled ? "Įjungtas" : "Išjungtas")}] ");
            Console.WriteLine(string.Join("  ", mods));
            Console.WriteLine("Valdymas: Rodyklės – judėti, 1/2 – perjungti modulius, ESC – baigti");
        }
        
        private bool IsInVision(int x, int y)
        {
            int r = _player.CurrentVisionRadius();
            int dx = x - _player.X; int dy = y - _player.Y;
            return dx * dx + dy * dy <= r * r;
        }

        private static char TileChar(TileType t)
        {
            return t switch
            {
                TileType.Empty => '.',
                TileType.Wall => '#',
                TileType.Key => 'K',
                TileType.Door => 'D',
                TileType.Trap => '^',
                TileType.Exit => 'E',
                _ => '?'

            };
        }
        
    }

    public static class Levels
    {
        public static readonly string[] Level1 = new[]
        {
    "########################",
    "#S....#.......#.......E#",
    "#.##.#.#####.#.#####.###",
    "#....#.....#.#.....#...#",
    "###.#####.#.#.###.#.#.#",
    "#...#..K..#.#...#.#.#.#",
    "#.#.#.###.#.###.#.#.#.#",
    "#.#...#...#...#.#...#.#",
    "#.#####.#####.#.#####.#",
    "#.....#.....D.#.....#.#",
    "###.#.###.###.#.###.#.#",
    "#...#.....^.....#.....#",
    "########################"
        };

    }

    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var game = new Game(Levels.Level1);
            game.Run();
        }
    }
}