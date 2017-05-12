using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapView : MonoBehaviour {

    public HeightMapManager heightMapManager;
    public ViewMode viewMode;

    // mesh fields
	private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector2[] _UV;
    private int[] _Triangles;

    // pillar fields
    public GameObject pillarPrefab;
    private GameObject[] pillars;
    
    private int width;
    private int height;

    // enum
    public enum ViewMode {
        Mesh,
        Pillar
    }

    void Start()
    {
        width = HeightMapManager.WIDTH;
        height = HeightMapManager.HEIGHT;

        if (viewMode == ViewMode.Mesh) {
            // Downsample to lower resolution
            CreateMesh(width, height);
        }
        else {
            // Downsample to lower resolution
            CreatePillars(width, height);
        }
    }

    void CreateMesh(int width, int height)
    {
        _Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _Mesh;
        Vector3 position = GetComponent<Transform>().position;

        _Vertices = new Vector3[width * height];
        _UV = new Vector2[width * height];
        _Triangles = new int[6 * ((width - 1) * (height - 1))];

        int triangleIndex = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width) + x;

                _Vertices[index] = new Vector3(x + position.x, position.y, -1 * (y + position.z));
                _UV[index] = new Vector2(((float)x / (float)width), ((float)y / (float)height));

                // Skip the last row/col
                if (x != (width - 1) && y != (height - 1))
                {
                    int topLeft = index;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + width;
                    int bottomRight = bottomLeft + 1;

                    _Triangles[triangleIndex++] = topLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomRight;
                }
            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals();
		//GetComponent<MeshCollider>().sharedMesh = _Mesh;
    }

    void CreatePillars(int width, int height)
    {
        pillars = new GameObject[width * height];
        Vector3 position = GetComponent<Transform>().position;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width) + x;
                float relativeX = x + position.x;
                float relativeZ = y + position.z;

                GameObject pillar = Instantiate(pillarPrefab, new Vector3(relativeX, position.y, relativeZ * -1), Quaternion.identity);
                pillars[index] = pillar;
            }
        }

    }

    void Update()
    {
		if (heightMapManager.HasPendingData()) {
			RefreshData(heightMapManager.GetData());
			// GetComponent<MeshCollider>().sharedMesh = _Mesh;
		}
    }
    
    private void RefreshData(float[] data)
    {
        Vector3 position = GetComponent<Transform>().position;
        Color[] colorData = heightMapManager.GetColorData();

        for (int i = 0; i < data.Length; i++)
        {    
            float dataValue = data[i];
            if (viewMode == ViewMode.Mesh) {
                _Vertices[i].y = dataValue;
            }
            else {
                Transform pillarTransform = pillars[i].GetComponent<Transform>();
                pillarTransform.localScale = new Vector3(1, dataValue, 1);
                pillarTransform.position = new Vector3(pillarTransform.position.x, position.y + ((float) (dataValue / 2)), pillarTransform.position.z);

                Renderer rend = pillars[i].GetComponent<Renderer>();
                rend.material.color = colorData[i];
            }
        }
        if (viewMode == ViewMode.Mesh) {
            _Mesh.vertices = _Vertices;
            _Mesh.uv = _UV;
            _Mesh.triangles = _Triangles;
            _Mesh.RecalculateNormals();
        }

    }

}
