﻿

using System;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public class Delaunay3D {
    
    //四面体类
    public class Tetrahedron : IEquatable<Tetrahedron> {
        public Vertex A { get; set; }
        public Vertex B { get; set; }
        public Vertex C { get; set; }
        public Vertex D { get; set; }

        public bool IsBad { get; set; }

        Vector3 Circumcenter { get; set; }
        float CircumradiusSquared { get; set; }

        public Tetrahedron(Vertex a, Vertex b, Vertex c, Vertex d) {
            A = a;
            B = b;
            C = c;
            D = d;
            CalculateCircumsphere();
        }

        void CalculateCircumsphere() {

            float a = new Matrix4x4(
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z),
                new Vector4(1, 1, 1, 1)
            ).determinant;

            float aPosSqr = A.Position.sqrMagnitude;
            float bPosSqr = B.Position.sqrMagnitude;
            float cPosSqr = C.Position.sqrMagnitude;
            float dPosSqr = D.Position.sqrMagnitude;

            float Dx = new Matrix4x4(
                new Vector4(aPosSqr, bPosSqr, cPosSqr, dPosSqr),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z),
                new Vector4(1, 1, 1, 1)
            ).determinant;

            float Dy = -(new Matrix4x4(
                new Vector4(aPosSqr, bPosSqr, cPosSqr, dPosSqr),
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z),
                new Vector4(1, 1, 1, 1)
            ).determinant);

            float Dz = new Matrix4x4(
                new Vector4(aPosSqr, bPosSqr, cPosSqr, dPosSqr),
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(1, 1, 1, 1)
            ).determinant;

            float c = new Matrix4x4(
                new Vector4(aPosSqr, bPosSqr, cPosSqr, dPosSqr),
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z)
            ).determinant;

            Circumcenter = new Vector3(
                Dx / (2 * a),
                Dy / (2 * a),
                Dz / (2 * a)
            );

            CircumradiusSquared = ((Dx * Dx) + (Dy * Dy) + (Dz * Dz) - (4 * a * c)) / (4 * a * a);
        }

        public bool ContainsVertex(Vertex v) {
            return AlmostEqual(v, A)
                || AlmostEqual(v, B)
                || AlmostEqual(v, C)
                || AlmostEqual(v, D);
        }

        public bool CircumCircleContains(Vector3 v) {
            Vector3 dist = v - Circumcenter;
            return dist.sqrMagnitude <= CircumradiusSquared;
        }

        public static bool operator ==(Tetrahedron left, Tetrahedron right) {
            return (left.A == right.A || left.A == right.B || left.A == right.C || left.A == right.D)
                && (left.B == right.A || left.B == right.B || left.B == right.C || left.B == right.D)
                && (left.C == right.A || left.C == right.B || left.C == right.C || left.C == right.D)
                && (left.D == right.A || left.D == right.B || left.D == right.C || left.D == right.D);
        }

        public static bool operator !=(Tetrahedron left, Tetrahedron right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Tetrahedron t) {
                return this == t;
            }

            return false;
        }

        public bool Equals(Tetrahedron t) {
            return this == t;
        }

        public override int GetHashCode() {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode() ^ D.GetHashCode();
        }
    }

    public class Triangle {
        public Vertex U { get; set; }
        public Vertex V { get; set; }
        public Vertex W { get; set; }

        public bool IsBad { get; set; }

        public Triangle() {

        }

        public Triangle(Vertex u, Vertex v, Vertex w) {
            U = u;
            V = v;
            W = w;
        }

        public static bool operator ==(Triangle left, Triangle right) {
            return (left.U == right.U || left.U == right.V || left.U == right.W)
                && (left.V == right.U || left.V == right.V || left.V == right.W)
                && (left.W == right.U || left.W == right.V || left.W == right.W);
        }

        public static bool operator !=(Triangle left, Triangle right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Triangle e) {
                return this == e;
            }

            return false;
        }

        public bool Equals(Triangle e) {
            return this == e;
        }

        public override int GetHashCode() {
            return U.GetHashCode() ^ V.GetHashCode() ^ W.GetHashCode();
        }

        public static bool AlmostEqual(Triangle left, Triangle right) {
            return (Delaunay3D.AlmostEqual(left.U, right.U) || Delaunay3D.AlmostEqual(left.U, right.V) || Delaunay3D.AlmostEqual(left.U, right.W))
                && (Delaunay3D.AlmostEqual(left.V, right.U) || Delaunay3D.AlmostEqual(left.V, right.V) || Delaunay3D.AlmostEqual(left.V, right.W))
                && (Delaunay3D.AlmostEqual(left.W, right.U) || Delaunay3D.AlmostEqual(left.W, right.V) || Delaunay3D.AlmostEqual(left.W, right.W));
        }
    }

    public class Edge {
        public Vertex U { get; set; }
        public Vertex V { get; set; }

        public bool IsBad { get; set; }

        public Edge() {

        }

        public Edge(Vertex u, Vertex v) {
            U = u;
            V = v;
        }

        public static bool operator ==(Edge left, Edge right) {
            return (left.U == right.U || left.U == right.V)
                && (left.V == right.U || left.V == right.V);
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

        public static bool AlmostEqual(Edge left, Edge right) {
            return (Delaunay3D.AlmostEqual(left.U, right.U) || Delaunay3D.AlmostEqual(left.V, right.U))
                && (Delaunay3D.AlmostEqual(left.U, right.V) || Delaunay3D.AlmostEqual(left.V, right.U));
        }
    }

    static bool AlmostEqual(Vertex left, Vertex right) {
        return (left.Position - right.Position).sqrMagnitude < 0.01f;
    }

    public List<Vertex> Vertices { get; private set; }
    public List<Edge> Edges { get; private set; }
    public List<Triangle> Triangles { get; private set; }
    public List<Tetrahedron> Tetrahedra { get; private set; }

    Delaunay3D() {
        Edges = new List<Edge>();
        Triangles = new List<Triangle>();
        Tetrahedra = new List<Tetrahedron>();
    }

    public static Delaunay3D Triangulate(List<Vertex> vertices) {
        Delaunay3D delaunay = new Delaunay3D();
        delaunay.Vertices = new List<Vertex>(vertices);
        delaunay.Triangulate();

        return delaunay;
    }

    void Triangulate() {
        float minX = Vertices[0].Position.x;
        float minY = Vertices[0].Position.y;
        float minZ = Vertices[0].Position.z;
        float maxX = minX;
        float maxY = minY;
        float maxZ = minZ;

        foreach (var vertex in Vertices) {
            if (vertex.Position.x < minX) minX = vertex.Position.x;
            if (vertex.Position.x > maxX) maxX = vertex.Position.x;
            if (vertex.Position.y < minY) minY = vertex.Position.y;
            if (vertex.Position.y > maxY) maxY = vertex.Position.y;
            if (vertex.Position.z < minZ) minZ = vertex.Position.z;
            if (vertex.Position.z > maxZ) maxZ = vertex.Position.z;
        }

        float dx = maxX - minX;
        float dy = maxY - minY;
        float dz = maxZ - minZ;
        float deltaMax = Mathf.Max(dx, dy, dz) * 2;
        
        //Bowyer-Watson算法
        //1、构建一个初始的超级大四面体包裹所有点集
        Vertex p1 = new Vertex(new Vector3(minX - 1         , minY - 1          , minZ - 1          ));
        Vertex p2 = new Vertex(new Vector3(maxX + deltaMax  , minY - 1          , minZ - 1          ));
        Vertex p3 = new Vertex(new Vector3(minX - 1         , maxY + deltaMax   , minZ - 1          ));
        Vertex p4 = new Vertex(new Vector3(minX - 1         , minY - 1          , maxZ + deltaMax   ));

        Tetrahedra.Add(new Tetrahedron(p1, p2, p3, p4));
        
        //2、将点一个个加入超级大四面体中，执行算法流程
        foreach (var vertex in Vertices) {
            List<Triangle> triangles = new List<Triangle>();
            
            //遍历所有四面体
            foreach (var t in Tetrahedra) {
                //如果四面体的外接球包含当前点，则标记四面体，之后要清除共同的三角面
                if (t.CircumCircleContains(vertex.Position)) {
                    t.IsBad = true;
                    //将四面体分为4个三角面
                    triangles.Add(new Triangle(t.A, t.B, t.C));
                    triangles.Add(new Triangle(t.A, t.B, t.D));
                    triangles.Add(new Triangle(t.A, t.C, t.D));
                    triangles.Add(new Triangle(t.B, t.C, t.D));
                }
            }
            
            
            //判断哪些三角面是重叠的
            for (int i = 0; i < triangles.Count; i++) {
                for (int j = i + 1; j < triangles.Count; j++) {
                    if (Triangle.AlmostEqual(triangles[i], triangles[j])) {
                        triangles[i].IsBad = true;
                        triangles[j].IsBad = true;
                    }
                }
            }
            
            //移除标记的四面体和重叠的三角面
            Tetrahedra.RemoveAll((Tetrahedron t) => t.IsBad);
            triangles.RemoveAll((Triangle t) => t.IsBad);
            
            //用当前点与其他所有剩下的三角面构成新的四面体
            foreach (var triangle in triangles) {
                Tetrahedra.Add(new Tetrahedron(triangle.U, triangle.V, triangle.W, vertex));
            }
        }

        //遍历完所有点之后，删除最开始的超级大四面体
        Tetrahedra.RemoveAll((Tetrahedron t) => t.ContainsVertex(p1) || t.ContainsVertex(p2) || t.ContainsVertex(p3) || t.ContainsVertex(p4));

        HashSet<Triangle> triangleSet = new HashSet<Triangle>();
        HashSet<Edge> edgeSet = new HashSet<Edge>();
        //根据剩下的四面体保存三角面和边的数据
        foreach (var t in Tetrahedra) {
            var abc = new Triangle(t.A, t.B, t.C);
            var abd = new Triangle(t.A, t.B, t.D);
            var acd = new Triangle(t.A, t.C, t.D);
            var bcd = new Triangle(t.B, t.C, t.D);

            if (triangleSet.Add(abc)) {
                Triangles.Add(abc);
            }

            if (triangleSet.Add(abd)) {
                Triangles.Add(abd);
            }

            if (triangleSet.Add(acd)) {
                Triangles.Add(acd);
            }

            if (triangleSet.Add(bcd)) {
                Triangles.Add(bcd);
            }

            var ab = new Edge(t.A, t.B);
            var bc = new Edge(t.B, t.C);
            var ca = new Edge(t.C, t.A);
            var da = new Edge(t.D, t.A);
            var db = new Edge(t.D, t.B);
            var dc = new Edge(t.D, t.C);

            if (edgeSet.Add(ab)) {
                Edges.Add(ab);
            }

            if (edgeSet.Add(bc)) {
                Edges.Add(bc);
            }

            if (edgeSet.Add(ca)) {
                Edges.Add(ca);
            }

            if (edgeSet.Add(da)) {
                Edges.Add(da);
            }

            if (edgeSet.Add(db)) {
                Edges.Add(db);
            }

            if (edgeSet.Add(dc)) {
                Edges.Add(dc);
            }
        }
    }
}
