using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class MeshGenerator : MonoBehaviour
    {
        [SerializeField] private SquareGrid squareGrid;
        [SerializeField] private MeshFilter meshFilter;

        public MeshFilter MeshFilter => meshFilter;

        [Range(10, 50)]
        [SerializeField] private int tileAmount = 20;

        private Vector2 mapSize;
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<int> triangles = new List<int>();
        private readonly List<List<int>> outlines = new List<List<int>>();
        private readonly HashSet<int> checkedVertices = new HashSet<int>();
        [SerializeField] private float timeToWait = 0.01f;

        private readonly Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();

        #pragma warning disable S2368 // Public methods should not have multidimensional array parameters
        public IEnumerator GenerateMesh(int[,] map, float squareSize, int wallTile)
        #pragma warning restore S2368 // Public methods should not have multidimensional array parameters
        {
            mapSize = new Vector2(map.GetLength(0), map.GetLength(1));

            squareGrid = new SquareGrid(map, squareSize, wallTile);
            for (int x = 0; x < squareGrid.Squares.GetLength(0); x++)
                for (int y = 0; y < squareGrid.Squares.GetLength(1); y++)
                    TriangulateSquare(squareGrid.Squares[x, y]);

            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.uv = CreateUV(squareSize);
            yield return Wait("Created mesh");

            yield return StartCoroutine(Generate2DCollider());
        }

        private void TriangulateSquare(Square square)
        {
            switch (square.Configuration)
            {
                //TODO Simplify this
                //One point active
                case 1:
                    MeshFromPoints(square.CentreLeft, square.CentreBottom, square.BottomLeft); //BottomLeft
                    break;
                case 2:
                    MeshFromPoints(square.BottomRight, square.CentreBottom, square.CentreRight); //BottomRight
                    break;
                case 4:
                    MeshFromPoints(square.TopRight, square.CentreRight, square.CentreTop); //TopRight
                    break;
                case 8:
                    MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreLeft); //TopLeft
                    break;
                //Two points active
                case 3:
                    MeshFromPoints(square.CentreRight, square.BottomRight, square.BottomLeft, square.CentreLeft); //BottomLeft & BottomRight
                    break;
                case 6:
                    MeshFromPoints(square.CentreTop, square.TopRight, square.BottomRight, square.CentreBottom); //BottomRight & TopRight
                    break;
                case 9:
                    MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreBottom, square.BottomLeft); //BottomLeft & TopLeft
                    break;
                case 12:
                    MeshFromPoints(square.TopLeft, square.TopRight, square.CentreRight, square.CentreLeft); //TopRight & TopLeft
                    break;
                case 5:
                    MeshFromPoints(square.CentreTop, square.TopRight, square.CentreRight, square.CentreBottom, square.BottomLeft, square.CentreLeft); //BottomLeft & TopRight
                    break;
                case 10:
                    MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreRight, square.BottomRight, square.CentreBottom, square.CentreLeft); //BottomRight & TopLeft
                    break;
                //Three points active
                case 7:
                    MeshFromPoints(square.CentreTop, square.TopRight, square.BottomRight, square.BottomLeft, square.CentreLeft); //TopRight & BottomLeft & BottomRight
                    break;
                case 11:
                    MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreRight, square.BottomRight, square.BottomLeft); //BottomLeft & BottomRight & TopLeft
                    break;
                case 13:
                    MeshFromPoints(square.TopLeft, square.TopRight, square.CentreRight, square.CentreBottom, square.BottomLeft); //BottomLeft & TopRight & TopLeft
                    break;
                case 14:
                    MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.CentreBottom, square.CentreLeft); //BottomRight & TopRight & TopLeft
                    break;
                //All points active
                case 15:
                    MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
                    break;
                default:
                    break;
            }
        }

        private void MeshFromPoints(params Node[] points)
        {
            AssignVertices(points);
            for (int i = 3; i <= points.Length; i++)
                CreateTriangle(points[0], points[i - 2], points[i - 1]);
        }

        private void AssignVertices(Node[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].VertexIndex == -1)
                {
                    points[i].VertexIndex = vertices.Count;
                    vertices.Add(points[i].Position);
                }
            }
        }

        public void CreateTriangle(Node a, Node b, Node c)
        {
            triangles.Add(a.VertexIndex);
            triangles.Add(b.VertexIndex);
            triangles.Add(c.VertexIndex);

            Triangle triangle = new Triangle(a.VertexIndex, b.VertexIndex, c.VertexIndex);
            AddTriangleToDictionary(triangle.VertexIndexA, triangle);
            AddTriangleToDictionary(triangle.VertexIndexB, triangle);
            AddTriangleToDictionary(triangle.VertexIndexC, triangle);
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

        private Vector2[] CreateUV(float squareSize)
        {
            Vector2[] uvs = new Vector2[vertices.Count];
            int divideMapSize = 2;
            for (int i = 0; i < vertices.Count; i++)
            {
                float fillPercentX = Mathf.InverseLerp(mapSize.x / divideMapSize * squareSize, -mapSize.x / divideMapSize * squareSize, vertices[i].x);
                float fillPercentY = Mathf.InverseLerp(mapSize.y / divideMapSize * squareSize, -mapSize.y / divideMapSize * squareSize, vertices[i].y);
                uvs[i] = new Vector2(fillPercentX, fillPercentY) * tileAmount;
            }

            return uvs;
        }

        private IEnumerator CalculateMeshOutlines()
        {
            for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
            {
                if (!checkedVertices.Contains(vertexIndex))
                {
                    int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                    if (newOutlineVertex != -1)
                    {
                        checkedVertices.Add(vertexIndex);

                        List<int> newOutline = new List<int>{vertexIndex};
                        outlines.Add(newOutline);

                        FollowOutline(newOutlineVertex, outlines.Count - 1);
                        outlines[outlines.Count - 1].Add(vertexIndex);
                    }
                }

                if (CorutineUtilities.WaitAmountOfTimes(vertexIndex, vertices.Count, 30))
                    yield return Wait("Calculating mesh outlines");
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

            for (int i = 0; i < trianglesContainingVertexA.Count; i++)
                if (trianglesContainingVertexA[i].ContainsVertexIndex(vertexB))
                {
                    sharedTriangleCount++;
                    if (sharedTriangleCount > 1)
                        break;
                }
            return sharedTriangleCount == 1;
        }

        private IEnumerator Generate2DCollider()
        {
            EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();
            for (int i = 0; i < currentColliders.Length; i++)
                Destroy(currentColliders[i]);

            yield return StartCoroutine(CalculateMeshOutlines());

            foreach (List<int> outline in outlines)
            {
                EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
                Vector2[] edgePoints = new Vector2[outline.Count];

                for (int i = 0; i < outline.Count; i++)
                    edgePoints[i] = vertices[outline[i]];

                edgeCollider.points = edgePoints;
            }
        }

        private WaitForSeconds Wait(string text = "") => CorutineUtilities.Wait(timeToWait, text);

        public struct Triangle
        {
            public int VertexIndexA { get; set; }
            public int VertexIndexB { get; set; }
            public int VertexIndexC { get; set; }
            readonly int[] vertices;

            public Triangle(int vertexIndexA, int vertexIndexB, int vertexIndexC)
            {
                VertexIndexA = vertexIndexA;
                VertexIndexB = vertexIndexB;
                VertexIndexC = vertexIndexC;

                vertices = new int[] { vertexIndexA, vertexIndexB, vertexIndexC };
            }

            public int this[int i] => vertices[i];

            public bool ContainsVertexIndex(int vertexIndex) => vertexIndex == VertexIndexA || vertexIndex == VertexIndexB || vertexIndex == VertexIndexC;
        }

        public class SquareGrid
        {
            public Square[,] Squares { get; set; }
            public SquareGrid(int[,] map, float squareSize, int wallTile)
            {
                int nodeCountX = map.GetLength(0);
                int nodeCountY = map.GetLength(1);
                float mapWidth = nodeCountX * squareSize;
                float mapHeight = nodeCountY * squareSize;

                ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

                for (int x = 0; x < nodeCountX; x++)
                    for (int y = 0; y < nodeCountY; y++)
                    {
                        Vector3 position = new Vector3(x: (-mapWidth / 2) + (x * squareSize) + (squareSize / 2), y: (-mapHeight / 2) + (y * squareSize) + (squareSize / 2), z: 0);
                        controlNodes[x, y] = new ControlNode(position, map[x, y] == wallTile, squareSize);
                    }

                Squares = new Square[nodeCountX - 1, nodeCountY - 1];
                for (int x = 0; x < nodeCountX - 1; x++)
                    for (int y = 0; y < nodeCountY - 1; y++)
                        Squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x, y], controlNodes[x + 1, y]);
            }
        }

        public class Square
        {
            public ControlNode TopLeft { get; set; }
            public ControlNode TopRight { get; set; }
            public ControlNode BottomLeft { get; set; }
            public ControlNode BottomRight { get; set; }

            public Node CentreTop { get; set; }
            public Node CentreRight { get; set; }
            public Node CentreBottom { get; set; }
            public Node CentreLeft { get; set; }
            public int Configuration { get; set; }

            public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomLeft, ControlNode bottomRight)
            {
                TopLeft = topLeft;
                TopRight = topRight;
                BottomLeft = bottomLeft;
                BottomRight = bottomRight;

                CentreTop = topLeft.Right;
                CentreBottom = bottomLeft.Right;
                CentreLeft = bottomLeft.Above;
                CentreRight = bottomRight.Above;

                if (topLeft.IsActive)
                    Configuration += 8;
                if (topRight.IsActive)
                    Configuration += 4;
                if (bottomLeft.IsActive)
                    Configuration += 1;
                if (bottomRight.IsActive)
                    Configuration += 2;
            }
        }

        public class Node
        {
            public Vector3 Position { get; set; }
            public int VertexIndex { get; set; } = -1;
            public Node(Vector3 position) => Position = position;
        }

        public class ControlNode : Node
        {
            public bool IsActive { get; set; }
            public Node Above { get; set; }
            public Node Right { get; set; }

            public ControlNode(Vector3 position, bool isActive, float squareSize) : base(position)
            {
                IsActive = isActive;
                Above = new Node(position + (Vector3.up * squareSize / 2f));
                Right = new Node(position + (Vector3.right * squareSize / 2f));
            }
        }
    }

}