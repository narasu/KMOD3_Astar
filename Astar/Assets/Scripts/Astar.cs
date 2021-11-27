using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    enum Direction { N, E, S, W, NE, SE, NW, SW }
    
    List<Node> openNodes = new List<Node>();
    List<Vector2Int> closedNodes = new List<Vector2Int>();
    int maxOperations = 10000;
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        Node parent = null;
        float HScore = Vector2Int.Distance(startPos, endPos);
        Node startNode = new Node(startPos, null, 0f, HScore);
        Node current = startNode;
        openNodes.Add(startNode);

        int numOperations = 0;

        while (openNodes.Count > 0)
        {
            if (numOperations >= maxOperations)
            {
                Debug.LogError("Too many operations");
                openNodes.Clear();
                return null;
            }
            current = GetNodeWithLowestFScore();
            
            if (current.position == endPos)
            {
                while (current.parent != null)
                {
                    path.Push(current.position);
                    current = current.parent;
                }
                //path.Push(current.position);

                openNodes.Clear();
                closedNodes.Clear();
                return path.ToList();
            }
            openNodes.Remove(current);
            closedNodes.Add(current.position);
            parent = current;
            
            List<Node> adjacentNodes = GenerateAdjacentNodes(current, grid);
            foreach (Node n in adjacentNodes)
            {
                if (closedNodes.Contains(n.position))
                {
                    continue;
                }
                float tempGScore = current.GScore + 1f;
                if (tempGScore < n.GScore)
                {
                    n.parent = current;
                    n.GScore = tempGScore;
                    n.HScore = Vector2Int.Distance(n.position, endPos);
                    if (!openNodes.Contains(n))
                    {
                        openNodes.Add(n);
                    }
                }
            }

            Debug.Log(openNodes.Count);
            numOperations++;
        }
        openNodes.Clear();
        closedNodes.Clear();
        Debug.Log("no good");
        return null;
    }

    List<Node> GenerateAdjacentNodes(Node parent, Cell[,] grid)
    {
        //List<Node> adjacentNodes = new List<Node>()
        //{
        //    new Node(parent.position + Vector2Int.left, null, float.PositiveInfinity, 0f),
        //    new Node(parent.position + Vector2Int.right, null, float.PositiveInfinity, 0f),
        //    new Node(parent.position + Vector2Int.up, null, float.PositiveInfinity, 0f),
        //    new Node(parent.position + Vector2Int.down, null, float.PositiveInfinity, 0f)
        //};

        //foreach(Node n in adjacentNodes)
        //{
        //    if (grid[n.position.x, n.position.y].HasWall())
        //    {

        //    }
        //}
        List<Node> adjacentNodes = new List<Node>();

        if (!grid[parent.position.x, parent.position.y].HasWall(Wall.LEFT) && parent.position.x > 0)
        {
            adjacentNodes.Add(new Node(parent.position + Vector2Int.left, null, float.PositiveInfinity, 0f));
        }

        if (!grid[parent.position.x, parent.position.y].HasWall(Wall.RIGHT) && parent.position.x < grid.GetLength(0) - 1)
        {
            adjacentNodes.Add(new Node(parent.position + Vector2Int.right, null, float.PositiveInfinity, 0f));
        }

        if (!grid[parent.position.x, parent.position.y].HasWall(Wall.DOWN) && parent.position.y > 0)
        {
            adjacentNodes.Add(new Node(parent.position + Vector2Int.down, null, float.PositiveInfinity, 0f));
        }

        if (!grid[parent.position.x, parent.position.y].HasWall(Wall.UP) && parent.position.y < grid.GetLength(1) - 1)
        {
            adjacentNodes.Add(new Node(parent.position + Vector2Int.up, null, float.PositiveInfinity, 0f));
        }

        return adjacentNodes;
    }

    Node GetNodeWithLowestFScore()
    {
        if (openNodes.Count == 0)
        {
            return null;
        }

        Node lowest = null;
        foreach (Node n in openNodes)
        {
            if (lowest == null || n.FScore < lowest.FScore)
            {
                lowest = n;
            }
        }

        return lowest;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, float GScore, float HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
