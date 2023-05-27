using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using static Player;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        int numberOfCells = int.Parse(Console.ReadLine()); // amount of hexagonal cells in this map
        var cellManager = new CellManager();
        for (int i = 0; i < numberOfCells; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            var cell = new Cell(i, inputs);
            Console.Error.WriteLine($"Cell:{cell}");
            cellManager.Add(cell);
        }
        int numberOfBases = int.Parse(Console.ReadLine());
        Console.Error.WriteLine($"Number of bases:{numberOfBases}");
        inputs = Console.ReadLine().Split(' ');
        int myBaseIndex = -1;
        for (int i = 0; i < numberOfBases; i++)
        {
            myBaseIndex = int.Parse(inputs[i]);
        }

        int oppBaseIndex = -1;
        inputs = Console.ReadLine().Split(' ');
        for (int i = 0; i < numberOfBases; i++)
        {
            oppBaseIndex = int.Parse(inputs[i]);
        }
        var gameManager = new GameManger(myBaseIndex, oppBaseIndex, cellManager);
        ICell? currentlyHarvesting = null;
        // game loop
        while (true)
        {
            for (int i = 0; i < numberOfCells; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int resources = int.Parse(inputs[0]); // the current amount of eggs/crystals on this cell
                int myAnts = int.Parse(inputs[1]); // the amount of your ants on this cell
                int oppAnts = int.Parse(inputs[2]); // the amount of opponent ants on this cell
                gameManager.Update(i, resources, myAnts, oppAnts);
            }
            if(currentlyHarvesting == null || currentlyHarvesting.CurrentResources == 0)
                currentlyHarvesting = cellManager.ProfitableCells.FirstOrDefault();
            if(currentlyHarvesting != null)
            {
                Console.Error.WriteLine($"Current resource {currentlyHarvesting.CurrentResources}");
                Console.WriteLine($"LINE {gameManager.MyBaseIndex} {currentlyHarvesting.CellIndex} 1");
            }
            else
            {
                Console.WriteLine("WAIT");
            }
            
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");


            // WAIT | LINE <sourceIdx> <targetIdx> <strength> | BEACON <cellIdx> <strength> | MESSAGE <text>
            
        }
    }

    public interface IGameManager
    {
        public int MyBaseIndex { get; }

        public int EnemyBaseIndex { get; }

        public ICellManager CellManager { get; }
    }

    public class GameManger : IGameManager
    {
        public GameManger(int myBaseIndex, int enemyBaseIndex, ICellManager cellManager)
        {
            MyBaseIndex = myBaseIndex;
            EnemyBaseIndex = enemyBaseIndex;
            CellManager = cellManager;
        }

        public int MyBaseIndex {get; private set;}

        public int EnemyBaseIndex {get; private set; }

        public ICellManager CellManager {get; private set;}

        internal void Update(int cellIndex, int resources, int myAnts, int oppAnts) => 
            CellManager.Update(cellIndex, resources, myAnts, oppAnts);
    }

    public interface ICellManager
    {
        void Update(int cellIndex, int resources, int myAnts, int oppAnts);
    }

    public class CellManager : List<ICell>, ICellManager
    {
        public ICollection<ICell> ProfitableCells => this.Where(x => x.CellType == CellType.Crystal && x.CurrentResources > 0).OrderByDescending(x => x.CurrentResources).ToList();

        public void Update(int cellIndex, int resources, int myAnts, int oppAnts)
        {
            var cell = this.Single(x => x.CellIndex == cellIndex);
            cell.Update(resources, myAnts, oppAnts);
        }
    }

    public interface ICell
    {
        CellType CellType { get; }
        int CellIndex { get; }
        int InitialResources { get; }
        ICollection<int> Neighbours { get; }
        int CurrentResources { get; }
        int MyAnts { get; }
        int OpponentAnts { get; }

        void Update(int resources, int myAnts, int oppAnts);
    }

    public class Cell : ICell
    {
        public Cell(int cellIndex, string[] inputs)
        {
            CellIndex = cellIndex;
            CellType = (CellType)int.Parse(inputs[0]); // 0 for empty, 1 for eggs, 2 for crystal
            InitialResources = int.Parse(inputs[1]); // the initial amount of eggs/crystals on this cell
            Neighbours.Add(int.Parse(inputs[2])); // the index of the neighbouring cell for each direction
            Neighbours.Add(int.Parse(inputs[3]));
            Neighbours.Add(int.Parse(inputs[4]));
            Neighbours.Add(int.Parse(inputs[5]));
            Neighbours.Add(int.Parse(inputs[6]));
            Neighbours.Add(int.Parse(inputs[7]));
            CurrentResources = InitialResources;
            MyAnts = 0;
            OpponentAnts = 0;
        }
        public int CellIndex { get; private set;}

        public int InitialResources {get; private set;}

        public ICollection<int> Neighbours {get; private set;} = new HashSet<int>();

        public CellType CellType { get; private set;}
        public int CurrentResources { get; private set; }
        public int MyAnts { get; private set; }
        public int OpponentAnts { get; private set;}

        public void Update(int resources, int myAnts, int oppAnts)
        {
            CurrentResources = resources;
            MyAnts = myAnts;
            OpponentAnts = oppAnts;
        }

        public override string ToString() => 
            $"{nameof(CellIndex)}: {CellIndex}, {nameof(InitialResources)}: {InitialResources}, {nameof(CellType)}: {CellType}, {nameof(CurrentResources)}: {CurrentResources}, {nameof(MyAnts)}: {MyAnts}, {nameof(OpponentAnts)}: {OpponentAnts} {nameof(Neighbours)}: {string.Join(",", Neighbours.ToArray())}";
    }

    public enum CellType
    {
        Empty = 0,
        Ignored = 1,
        Crystal = 2,
    }


}