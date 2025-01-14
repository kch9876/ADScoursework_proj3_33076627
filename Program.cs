using System;
using System.Collections.Generic;
using System.Linq;

class Graph
{
    private readonly int _numNodes;
    private readonly List<(int neighbor, double weight)>[] _adjacencyList;

    // Constructor to initialize the graph
    public Graph(int numNodes)
    {
        _numNodes = numNodes;
        _adjacencyList = new List<(int, double)>[numNodes];
        for (int i = 0; i < numNodes; i++)
        {
            _adjacencyList[i] = new List<(int, double)>();
        }
    }

    // Method to add an edge for a weighted or unweighted graph
    public void AddEdge(int u, int v, double weight = 1.0)
    {
        _adjacencyList[u].Add((v, weight));
        _adjacencyList[v].Add((u, weight)); // Undirected graph
    }

    // Method to calculate influence score of a node (unweighted graph)
    public double CalculateInfluenceScoreUnweighted(int targetNode)
    {
        int[] distances = BFS(targetNode); // Get shortest distances using BFS
        double sumOfDistances = 0;

        for (int i = 0; i < _numNodes; i++)
        {
            if (i != targetNode)
            {
                if (distances[i] == int.MaxValue)
                {
                    // Node is unreachable, return 0 influence score
                    return 0;
                }
                sumOfDistances += distances[i];
            }
        }

        return (_numNodes - 1) / sumOfDistances; // Influence score formula
    }

    // Method to calculate influence score of a node (weighted graph)
    public double CalculateInfluenceScoreWeighted(int targetNode)
    {
        double[] distances = Dijkstra(targetNode); // Get shortest distances using Dijkstra's algorithm
        double sumOfDistances = 0;

        for (int i = 0; i < _numNodes; i++)
        {
            if (i != targetNode)
            {
                if (distances[i] == double.MaxValue)
                {
                    // Node is unreachable, return 0 influence score
                    return 0;
                }
                sumOfDistances += distances[i];
            }
        }

        return (_numNodes - 1) / sumOfDistances; // Influence score formula
    }

    // BFS to calculate shortest path distances for unweighted graph
    private int[] BFS(int startNode)
    {
        int[] distances = new int[_numNodes];
        for (int i = 0; i < _numNodes; i++)
        {
            distances[i] = int.MaxValue; // Initialize distances to "infinity"
        }
        distances[startNode] = 0;

        Queue<int> queue = new Queue<int>();
        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();

            foreach (var (neighbor, _) in _adjacencyList[current])
            {
                if (distances[neighbor] == int.MaxValue)
                {
                    distances[neighbor] = distances[current] + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return distances;
    }

    // Dijkstra's algorithm to calculate shortest path distances for weighted graph
    private double[] Dijkstra(int startNode)
    {
        double[] distances = new double[_numNodes];
        for (int i = 0; i < _numNodes; i++)
        {
            distances[i] = double.MaxValue; // Initialize distances to "infinity"
        }
        distances[startNode] = 0;

        PriorityQueue<int, double> priorityQueue = new PriorityQueue<int, double>();
        priorityQueue.Enqueue(startNode, 0);

        while (priorityQueue.Count > 0)
        {
            int current = priorityQueue.Dequeue();

            foreach (var (neighbor, weight) in _adjacencyList[current])
            {
                double newDistance = distances[current] + weight;
                if (newDistance < distances[neighbor])
                {
                    distances[neighbor] = newDistance;
                    priorityQueue.Enqueue(neighbor, newDistance);
                }
            }
        }

        return distances;
    }

    // Method to print the adjacency list
    public void PrintAdjacencyList(Dictionary<int, string> indexToName)
    {
        Console.WriteLine("Adjacency List:");
        for (int i = 0; i < _numNodes; i++)
        {
            Console.Write($"{indexToName[i]} -> ");
            Console.WriteLine(string.Join(", ", _adjacencyList[i].Select(neighbor => $"{indexToName[neighbor.neighbor]} ({neighbor.weight})")));
        }
    }
}

class Program
{
    static void Main()
    {
        int choice = -1;
        Dictionary<string, int>? nameToIndex = null;
        Dictionary<int, string>? indexToName = null;

        while (choice != 0)
        {
            Console.Clear();
            Console.Write("""
+---------------------------------------------------+
|                                                   |
|       Influence scores of nodes in a graph        |
|                                                   |
|---------------------------------------------------|
| Please select an option:                          |
| 1. Unweighted Graph (example 1)                   |
| 2. Weighted graph (example 2)                     |
|                                                   |
| 0. Exit                                           |
+---------------------------------------------------+
>> 
""");

            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("\nERROR! Please enter an integer.\n\nPress any key to continue...");
                Console.ReadKey();
                continue;
            }

            Graph? graph = null;

            switch (choice)
            {
                case 1:
                    // Define the names and their corresponding indices
                    nameToIndex = new Dictionary<string, int>
                    {
                        { "Alicia", 0 },
                        { "Britney", 1 },
                        { "Claire", 2 },
                        { "Diana", 3 },
                        { "Edward", 4 },
                        { "Harry", 5 },
                        { "Gloria", 6 },
                        { "Fred", 7 }
                    };

                    // Reverse mapping for indices to names
                    indexToName = nameToIndex.ToDictionary(pair => pair.Value, pair => pair.Key);

                    // Create the graph
                    graph = new Graph(nameToIndex.Count);

                    // Define the connections (add weights for weighted graph)
                    graph.AddEdge(nameToIndex["Alicia"], nameToIndex["Britney"]);
                    graph.AddEdge(nameToIndex["Britney"], nameToIndex["Claire"]);
                    graph.AddEdge(nameToIndex["Claire"], nameToIndex["Diana"]);
                    graph.AddEdge(nameToIndex["Diana"], nameToIndex["Edward"]);
                    graph.AddEdge(nameToIndex["Diana"], nameToIndex["Harry"]);
                    graph.AddEdge(nameToIndex["Edward"], nameToIndex["Harry"]);
                    graph.AddEdge(nameToIndex["Edward"], nameToIndex["Gloria"]);
                    graph.AddEdge(nameToIndex["Edward"], nameToIndex["Fred"]);
                    graph.AddEdge(nameToIndex["Gloria"], nameToIndex["Fred"]);
                    graph.AddEdge(nameToIndex["Harry"], nameToIndex["Gloria"]);

                    // Print the adjacency list
                    graph.PrintAdjacencyList(indexToName);

                    // Calculate and display the influence score for each person (unweighted and weighted)
                    Console.WriteLine("\nInfluence scores for all nodes (unweighted):");
                    for (int i = 0; i < nameToIndex.Count; i++)
                    {
                        double influenceScore = Math.Round(graph.CalculateInfluenceScoreUnweighted(i), 2);
                        Console.WriteLine($"{indexToName[i]}: {influenceScore}");
                    }

                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();

                    break;
                case 2:
                    // Define the names and their corresponding indices
                    nameToIndex = new Dictionary<string, int>
                    {
                        { "A", 0 },
                        { "B", 1 },
                        { "C", 2 },
                        { "D", 3 },
                        { "E", 4 },
                        { "F", 5 },
                        { "G", 6 },
                        { "H", 7 },
                        { "I", 8 },
                        { "J", 9 }
                    };

                    // Reverse mapping for indices to names
                    indexToName = nameToIndex.ToDictionary(pair => pair.Value, pair => pair.Key);

                    // Create the graph
                    graph = new Graph(nameToIndex.Count);

                    // Define the connections (add weights for weighted graph)
                    graph.AddEdge(nameToIndex["A"], nameToIndex["B"], 1);
                    graph.AddEdge(nameToIndex["A"], nameToIndex["C"], 1);
                    graph.AddEdge(nameToIndex["A"], nameToIndex["E"], 5);
                    graph.AddEdge(nameToIndex["B"], nameToIndex["C"], 4);
                    graph.AddEdge(nameToIndex["B"], nameToIndex["E"], 1);
                    graph.AddEdge(nameToIndex["B"], nameToIndex["G"], 1);
                    graph.AddEdge(nameToIndex["B"], nameToIndex["H"], 1);
                    graph.AddEdge(nameToIndex["C"], nameToIndex["D"], 3);
                    graph.AddEdge(nameToIndex["C"], nameToIndex["E"], 1);
                    graph.AddEdge(nameToIndex["D"], nameToIndex["E"], 2);
                    graph.AddEdge(nameToIndex["D"], nameToIndex["F"], 1);
                    graph.AddEdge(nameToIndex["D"], nameToIndex["G"], 5);
                    graph.AddEdge(nameToIndex["E"], nameToIndex["G"], 2);
                    graph.AddEdge(nameToIndex["F"], nameToIndex["G"], 1);
                    graph.AddEdge(nameToIndex["G"], nameToIndex["H"], 2);
                    graph.AddEdge(nameToIndex["H"], nameToIndex["I"], 3);
                    graph.AddEdge(nameToIndex["I"], nameToIndex["J"], 3);

                    // Print the adjacency list
                    graph.PrintAdjacencyList(indexToName);

                    // Calculate and display the influence score for each person (unweighted and weighted)
                    Console.WriteLine("\nInfluence scores for all nodes (weighted):");
                    for (int i = 0; i < nameToIndex.Count; i++)
                    {
                        double influenceScore = Math.Round(graph.CalculateInfluenceScoreWeighted(i), 2);
                        Console.WriteLine($"{indexToName[i]}: {influenceScore}");
                    }
                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();

                    break;
                case 0:
                    Console.WriteLine("\nExiting program...");
                    continue;
                default:
                    Console.WriteLine("\nERROR! Invalid option.\n\nPress any key to continue...");
                    Console.ReadKey();
                    choice = -1;
                    continue;
            }
        }


    }
}
