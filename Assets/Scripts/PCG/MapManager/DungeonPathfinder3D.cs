using System;
using System.Collections.Generic;
using UnityEngine;
using BlueRaja;

public class DungeonPathfinder3D {
    public class Node {
        public Vector3Int Position { get; private set; }
        public Node Previous { get; set; }
        public HashSet<Vector3Int> PreviousSet { get; private set; }
        public float Cost { get; set; }

        public Node(Vector3Int position) {
            Position = position;
            PreviousSet = new HashSet<Vector3Int>();
        }
    }

    public struct PathCost {
        public bool traversable;
        public float cost;
        public bool isStairs;
    }

    static readonly Vector3Int[] neighbors = {
        //平面移动
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        //向上移动 水平三格 垂直一格
        new Vector3Int(3, 1, 0),
        new Vector3Int(-3, 1, 0),
        new Vector3Int(0, 1, 3),
        new Vector3Int(0, 1, -3),
        //向下移动，水平三格，垂直一格
        new Vector3Int(3, -1, 0),
        new Vector3Int(-3, -1, 0),
        new Vector3Int(0, -1, 3),
        new Vector3Int(0, -1, -3),
    };

    Grid3D<Node> grid;
    SimplePriorityQueue<Node, float> queue;
    HashSet<Node> closed;
    Stack<Vector3Int> stack;

    public DungeonPathfinder3D(Vector3Int size) {
        grid = new Grid3D<Node>(size, Vector3Int.zero);

        queue = new SimplePriorityQueue<Node, float>();
        closed = new HashSet<Node>();
        stack = new Stack<Vector3Int>();

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {
                    grid[x, y, z] = new Node(new Vector3Int(x, y, z));
                }
            }
        }
    }

    void ResetNodes() {
        var size = grid.Size;

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {
                    var node = grid[x, y, z];
                    node.Previous = null;
                    //node.Cost = float.PositiveInfinity;
                    node.Cost = 1000;
                    node.PreviousSet.Clear();
                }
            }
        }
    }
    
    //A*算法 基于优先队列和代价函数
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end, float xOffset,float zOffset, Func<Node, Node, PathCost> costFunction) {
        ResetNodes();
        queue.Clear();
        closed.Clear();

        queue = new SimplePriorityQueue<Node, float>();
        closed = new HashSet<Node>();

        grid[start].Cost = 0;
        // Vector3Int up = new Vector3Int(start.x, start.y, (int)(start.z + zOffset));
        // Vector3Int down = new Vector3Int(start.x, start.y, (int)(start.z - zOffset));
        // Vector3Int left = new Vector3Int((int)(start.x-xOffset), start.y, start.z);
        // Vector3Int right = new Vector3Int((int)(start.x+xOffset), start.y, start.z);
        // grid[up].Cost = 0;
        // grid[down].Cost = 0;
        // grid[left].Cost = 0;
        // grid[right].Cost = 0;
        // queue.Enqueue(grid[up],0);
        // queue.Enqueue(grid[down],0);
        // queue.Enqueue(grid[left],0);
        // queue.Enqueue(grid[right],0);
        queue.Enqueue(grid[start], 0);

        while (queue.Count > 0) {
            Node node = queue.Dequeue();
            closed.Add(node);

            if (node.Position == end) {
                return ReconstructPath(node);
            }

            foreach (var offset in neighbors) {
                //超出地图范围
                if (!grid.InBounds(node.Position + offset)) continue;
                var neighbor = grid[node.Position + offset];
                //该邻居已经在闭集中
                if (closed.Contains(neighbor)) continue;

                if (node.PreviousSet.Contains(neighbor.Position)) {
                    continue;
                }
                
                var pathCost = costFunction(node, neighbor);
                if (!pathCost.traversable) continue;

                if (pathCost.isStairs) {
                    int xDir = Mathf.Clamp(offset.x, -1, 1);
                    int zDir = Mathf.Clamp(offset.z, -1, 1);
                    Vector3Int verticalOffset = new Vector3Int(0, offset.y, 0);
                    Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                    if (node.PreviousSet.Contains(node.Position + horizontalOffset)
                        || node.PreviousSet.Contains(node.Position + horizontalOffset * 2)
                        || node.PreviousSet.Contains(node.Position + verticalOffset + horizontalOffset)
                        || node.PreviousSet.Contains(node.Position + verticalOffset + horizontalOffset * 2)) {
                        continue;
                    }
                }

                float newCost = node.Cost + pathCost.cost;
                // if (node.Previous.Position.x != neighbor.Position.x && node.Previous.Position.z != neighbor.Position.z)
                // {
                //     //拐弯情况 增加cost
                //     newCost += 1;
                // }

                if (newCost < neighbor.Cost) {
                    neighbor.Previous = node;
                    neighbor.Cost = newCost;

                    if (queue.TryGetPriority(neighbor, out float existingPriority)) {
                        queue.UpdatePriority(neighbor, newCost);
                    } else {
                        queue.Enqueue(neighbor, neighbor.Cost);
                    }

                    neighbor.PreviousSet.Clear();
                    neighbor.PreviousSet.UnionWith(node.PreviousSet);
                    neighbor.PreviousSet.Add(node.Position);

                    if (pathCost.isStairs){
                        int xDir = Mathf.Clamp(offset.x, -1, 1);
                        int zDir = Mathf.Clamp(offset.z, -1, 1);
                        Vector3Int verticalOffset = new Vector3Int(0, offset.y, 0);
                        Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                        neighbor.PreviousSet.Add(node.Position + horizontalOffset);
                        neighbor.PreviousSet.Add(node.Position + horizontalOffset * 2);
                        neighbor.PreviousSet.Add(node.Position + verticalOffset + horizontalOffset);
                        neighbor.PreviousSet.Add(node.Position + verticalOffset + horizontalOffset * 2);
                    }
                }
            }
        }

        return null;
    }

    List<Vector3Int> ReconstructPath(Node node) {
        List<Vector3Int> result = new List<Vector3Int>();

        while (node != null) {
            stack.Push(node.Position);
            node = node.Previous;
        }

        while (stack.Count > 0) {
            result.Add(stack.Pop());
        }

        return result;
    }
}
