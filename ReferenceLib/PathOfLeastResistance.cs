using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace CogSim
{


    public static class PathOfLeastResistance
    {
        private class Node : IHeapItem<Node>
        {
            public Vector3Int position;
            public float gCost, hCost;
            public float fCost => gCost + hCost;
            public Node parent;
            private int heapIndex;

            public int HeapIndex
            {
                get { return heapIndex; }
                set { heapIndex = value; }
            }

            public Node(Vector3Int pos, float g, float h, Node parent)
            {
                this.position = pos;
                this.gCost = g;
                this.hCost = h;
                this.parent = parent;
            }

            public int CompareTo(Node nodeToCompare)
            {
                int compare = fCost.CompareTo(nodeToCompare.fCost);
                if (compare == 0)
                    compare = hCost.CompareTo(nodeToCompare.hCost);
                return -compare;
            }
        }

        public static Queue<Vector3Int> FindPath(Vector3Int start, Vector3Int end, float[,] grid, float tolerance = 0.1f)
        {
            if (grid == null)
                throw new System.ArgumentNullException(nameof(grid));

            int gridWidth = grid.GetLength(0);
            int gridHeight = grid.GetLength(1);

            // Check if start or end is out of bounds
            if (start.x < 0 || start.x >= gridWidth || start.y < 0 || start.y >= gridHeight ||
                end.x < 0 || end.x >= gridWidth || end.y < 0 || end.y >= gridHeight)
                return null;

            Heap<Node> openSet = new Heap<Node>(gridWidth * gridHeight);
            HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();
            openSet.Add(new Node(start, 0, Vector3Int.Distance(start, end), null));

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode.position);

                if (currentNode.position == end)
                {
                    return RetracePath(start, currentNode);
                }

                foreach (Vector3Int neighbor in GetNeighbors(currentNode.position, gridWidth, gridHeight))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    float newMovementCostToNeighbor = currentNode.gCost + grid[neighbor.x, neighbor.y];

                    Node neighborNode = openSet.Find(n => n.position == neighbor);
                    if (neighborNode == null || newMovementCostToNeighbor < neighborNode.gCost + tolerance)
                    {
                        float hCost = Vector3Int.Distance(neighbor, end);
                        Node newNode = new Node(neighbor, newMovementCostToNeighbor, hCost, currentNode);
                        if (neighborNode == null)
                            openSet.Add(newNode);
                        else
                            openSet.UpdateItem(newNode);
                    }
                }
            }

            // No path found
            return null;
        }

        private static Queue<Vector3Int> RetracePath(Vector3Int startPos, Node endNode)
        {
            Queue<Vector3Int> path = new Queue<Vector3Int>();
            Node currentNode = endNode;
            Vector3Int lastDirection = Vector3Int.zero;

            while (currentNode != null)
            {
                Vector3Int currentDirection = currentNode.position - (currentNode.parent?.position ?? currentNode.position);

                // Add the waypoint if it's the start or if the direction changes
                if (currentNode.position == startPos || currentDirection != lastDirection)
                {
                    path.Enqueue(currentNode.position);
                    lastDirection = currentDirection;
                }

                currentNode = currentNode.parent;
            }

            // Reverse the path as we've built it backward
            Queue<Vector3Int> reversedPath = new Queue<Vector3Int>();
            while (path.Count > 0)
            {
                reversedPath.Enqueue(path.Dequeue());
            }

            return reversedPath;
        }

        private static IEnumerable<Vector3Int> GetNeighbors(Vector3Int pos, int width, int height)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    Vector3Int neighbor = new Vector3Int(pos.x + x, pos.y + y, pos.z);
                    if (neighbor.x >= 0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
                        yield return neighbor;
                }
            }
        }
    }

    // Custom Heap implementation for performance
    public class Heap<T> where T : IHeapItem<T>
    {
        private T[] items;
        private int currentItemCount;

        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
        }

        public T RemoveFirst()
        {
            T firstItem = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        public int Count => currentItemCount;

        public T Find(System.Func<T, bool> predicate)
        {
            for (int i = 0; i < currentItemCount; i++)
            {
                if (predicate(items[i]))
                {
                    return items[i];
                }
            }
            return default(T);
        }

        private void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < currentItemCount)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                    parentIndex = (item.HeapIndex - 1) / 2;
                }
                else
                {
                    break;
                }
            }
        }

        private void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;
            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }

}