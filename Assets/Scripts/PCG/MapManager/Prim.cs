﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public static class Prim {
    public class Edge : Graphs.Edge {
        public float Distance { get; private set; }

        public Edge(Vertex u, Vertex v) : base(u, v) {
            Distance = Vector3.Distance(u.Position, v.Position);
        }
        
        //无视方向 AB和BA相等
        public static bool operator ==(Edge left, Edge right) {
            return (left.U == right.U && left.V == right.V)
                || (left.U == right.V && left.V == right.U);
        }

        public static bool operator !=(Edge left, Edge right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Edge e) {
                return this == e;
            }

            return false;
        }

        public bool Equals(Edge e) {
            return this == e;
        }

        public override int GetHashCode() {
            return U.GetHashCode() ^ V.GetHashCode();
        }
    }

    public static List<Edge> MinimumSpanningTree(List<Edge> edges, Vertex start) {
        //未访问顶点
        HashSet<Vertex> openSet = new HashSet<Vertex>();
        //已访问顶点
        HashSet<Vertex> closedSet = new HashSet<Vertex>();

        foreach (var edge in edges) {
            openSet.Add(edge.U);
            openSet.Add(edge.V);
        }

        closedSet.Add(start);

        List<Edge> results = new List<Edge>();

        while (openSet.Count > 0) {
            bool chosen = false;
            Edge chosenEdge = null;
            float minWeight = float.PositiveInfinity;

            foreach (var edge in edges) {
                int closedVertices = 0;
                //对于一条边，只有当其有一个顶点在已访问顶点集合中，另一个不在时才考虑
                if (!closedSet.Contains(edge.U)) closedVertices++;
                if (!closedSet.Contains(edge.V)) closedVertices++;
                if (closedVertices != 1) continue;

                if (edge.Distance < minWeight) {
                    chosenEdge = edge;
                    chosen = true;
                    minWeight = edge.Distance;
                }
            }
            //如果没有任何一个顶点被选中，则循环结束
            if (!chosen) break;
            results.Add(chosenEdge);
            openSet.Remove(chosenEdge.U);
            openSet.Remove(chosenEdge.V);
            closedSet.Add(chosenEdge.U);
            closedSet.Add(chosenEdge.V);
        }

        return results;
    }
}
