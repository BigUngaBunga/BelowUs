using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private SquareGrid squareGrid;
    [SerializeField] private MeshFilter meshFilter;
    
    [Range (1, 20)]
    [SerializeField] private int tileAmount;

    private List<Vector3> vertices;
    private List<int> triangles;
    private List<List<int>> outlines;
    private HashSet<int> checkedVertices;

    private Dictionary<int, List<Triangle>> triangleDictionary;


    public void GenerateMesh(int[,] Map, float SquareSize, int WallTile)
    {
        squareGrid = new SquareGrid(Map, SquareSize, WallTile);
        vertices = new List<Vector3>();
        triangles = new List<int>();
        checkedVertices = new HashSet<int>();
        triangleDictionary = new Dictionary<int, List<Triangle>>();
        outlines = new List<List<int>>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                TriangulateSquare(squareGrid.squares[x, y]);

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        mesh.uv = CreateUV(Map, SquareSize);
        Generate2DCollider();
    }

    private void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
            //TODO Simplify this
            //One point active
            case 1:
                MeshFromPoints(square.centreLeft, square.centreBottom, square.bottomLeft); //BottomLeft
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centreBottom, square.centreRight); //BottomRight
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centreRight, square.centreTop); //TopRight
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft); //TopLeft
                break;
            //Two points active
            case 3:
                MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft); //BottomLeft & BottomRight
                break;
            case 6:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom); //BottomRight & TopRight
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft); //BottomLeft & TopLeft
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft); //TopRight & TopLeft
                break;
            case 5:
                MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft); //BottomLeft & TopRight
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft); //BottomRight & TopLeft
                break;
            //Three points active
            case 7:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft); //TopRight & BottomLeft & BottomRight
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft); //BottomLeft & BottomRight & TopLeft
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft); //BottomLeft & TopRight & TopLeft
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft); //BottomRight & TopRight & TopLeft
                break;
            //All points active
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                checkedVertices.Add(square.topLeft.vertexIndex);
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;
            default:
                break;
        }
    }

    private void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);
        int lastIndex = points.Length - 1;
        CreateTriangle(points[0], points[lastIndex - 1], points[lastIndex]);
    }

    private void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }

    public void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex,c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }

    private void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
    {
        if (triangleDictionary.ContainsKey(vertexIndexKey))
            triangleDictionary[vertexIndexKey].Add(triangle);
        else
        {
            List<Triangle> listOfTriangles = new List<Triangle> { triangle };
            triangleDictionary.Add(vertexIndexKey, listOfTriangles);
        }
    }

    private Vector2[] CreateUV(int[,] map, float squareSize)
    {
        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].x);
            float percentY = Mathf.InverseLerp(-map.GetLength(1) / 2 * squareSize, map.GetLength(1) / 2 * squareSize, vertices[i].y);
            uvs[i] = new Vector2(percentX, percentY) / tileAmount;
        }

        return uvs;
    }

    private void CalculateMeshOutlines()
    {
        for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
        {
            if (!checkedVertices.Contains(vertexIndex))
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if (newOutlineVertex != -1)
                {
                    checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);

                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vertexIndex);
                }
            }
        }
    }

    private void FollowOutline(int vertexIndex, int outlineIndex)
    {
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if (nextVertexIndex != -1)
            FollowOutline(nextVertexIndex, outlineIndex);

    }

    private int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];

            for (int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];

                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB) && IsOutlineEdge(vertexIndex, vertexB))
                    return vertexB;
            }
        }
        return -1;
    }

    private bool IsOutlineEdge(int vertexA, int vertexB)
    {
        List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
        int sharedTriangleCount = 0;

        for (int i = 0;  i < trianglesContainingVertexA.Count;  i++)
            if (trianglesContainingVertexA[i].ContainsVertexIndex(vertexB))
            {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1)
                    break;
            }
        return sharedTriangleCount == 1;
    }

    private void Generate2DCollider()
    {
        EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();
        for (int i = 0; i < currentColliders.Length; i++)
            Destroy(currentColliders[i]);
        
        CalculateMeshOutlines();

        foreach (List<int> outline in outlines)
        {
            EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            Vector2[] edgePoints = new Vector2[outline.Count];

            for (int i = 0; i < outline.Count; i++)
                edgePoints[i] = vertices[outline[i]];

            edgeCollider.points = edgePoints;
        }
    }

    public struct Triangle
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        readonly int[] vertices;

        public Triangle(int vertexIndexA, int vertexIndexB, int vertexIndexC)
        {
            this.vertexIndexA = vertexIndexA;
            this.vertexIndexB = vertexIndexB;
            this.vertexIndexC = vertexIndexC;

            vertices = new int[] { vertexIndexA, vertexIndexB, vertexIndexC };
        }

        public int this[int i]
        {
            get
            {
                return vertices[i];
            }
        }

        public bool ContainsVertexIndex(int vertexIndex)
        {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }


    public class SquareGrid
    {
        public Square[,] squares;
        public SquareGrid(int[,] Map, float SquareSize, int WallTile)
        {
            int nodeCountX = Map.GetLength(0);
            int nodeCountY = Map.GetLength(1);
            float mapWidth = nodeCountX * SquareSize;
            float mapHeight = nodeCountY * SquareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 position = new Vector3(-mapWidth / 2 + x * SquareSize + SquareSize / 2, -mapHeight / 2 + y * SquareSize + SquareSize / 2, 0);
                    controlNodes[x, y] = new ControlNode(position, Map[x, y] == WallTile, SquareSize);
                }

            squares = new Square[nodeCountX -1, nodeCountY -1];
            for (int x = 0; x < nodeCountX - 1; x++)
                for (int y = 0; y < nodeCountY - 1; y++)
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x, y], controlNodes[x + 1, y]);
        }
    }

    public class Square
    {
        public ControlNode topLeft, topRight, bottomLeft, bottomRight;
        public Node centreTop, centreRight, centreBottom, centreLeft;
        public int configuration;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomLeft, ControlNode bottomRight)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;

            centreTop = topLeft.right;
            centreBottom = bottomLeft.right;
            centreLeft = bottomLeft.above;
            centreRight = bottomRight.above;

            if (topLeft.isActive)
                configuration += 8;
            if (topRight.isActive)
                configuration += 4;
            if (bottomLeft.isActive)
                configuration += 1;
            if (bottomRight.isActive)
                configuration += 2;
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;
        
        public Node (Vector3 position)
        {
            this.position = position;
        }
    }

    public class ControlNode : Node
    {
        public bool isActive;
        public Node above, right;

        public ControlNode(Vector3 position, bool isActive, float squareSize) : base (position)
        {
            this.isActive = isActive;
            above = new Node(position + Vector3.up * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}
