using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WireframeVisualizer : MonoBehaviour
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct Vertex
    {
        public Vector3 position;
        public Vector2 barycentricCoords;
    }
    
    Material m_Material;
    Mesh m_Mesh;
    MeshFilter m_MeshFilter;
    MeshRenderer m_MeshRenderer;
    NativeArray<uint> m_Indices;
    NativeArray<Vertex> m_Vertices;
        
    void OnEnable()
    {
        m_Mesh = new Mesh();
        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        
        var shader = Shader.Find("Unlit/Wireframe");
        Assert.IsNotNull(shader);
        m_Material = new Material(shader)
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        m_MeshRenderer.sharedMaterial = m_Material;
        m_MeshFilter.sharedMesh = m_Mesh;
    }

    void DisposeBuffersIfNeeded()
    {
        if (m_Indices.IsCreated)
            m_Indices.Dispose();
        if (m_Vertices.IsCreated)
            m_Vertices.Dispose();
    }

    void OnDisable()
    {
        DisposeBuffersIfNeeded();
        
        if (Application.isPlaying)
        {
            Destroy(m_Material);
            Destroy(m_Mesh);
        }
        else
        {
            DestroyImmediate(m_Material);
            DestroyImmediate(m_Mesh);
        }
    }

    public void UpdateGeometry(NativeArray<Vector3> vertices, NativeArray<int> indices)
    {
        var totalVertices = indices.Length;

        if (!m_Indices.IsCreated || m_Indices.Length != totalVertices)
        {
            DisposeBuffersIfNeeded();
            
            m_Indices = new NativeArray<uint>(totalVertices, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_Vertices = new NativeArray<Vertex>(totalVertices, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            for (var i = 0; i != totalVertices; ++i) m_Indices[i] = (uint)i;
        }
        
        for (var i = 0; i != totalVertices; ++i)
        {            
            Vector2 barycentricCoords = Vector2.zero; 
            switch (i % 3)
            {
                case 1:
                    barycentricCoords = new Vector2(0, 1);
                    break;
                case 2:
                    barycentricCoords = new Vector2(1, 0);
                    break;
            }

            m_Vertices[i] = new Vertex
            {
                position = vertices[indices[i]],
                barycentricCoords = barycentricCoords
            };
        }
        
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
        };

        m_Mesh.SetVertexBufferParams(totalVertices, layout);
        m_Mesh.SetVertexBufferData(m_Vertices, 0, 0, totalVertices);
        
                
        m_Mesh.SetIndexBufferParams(totalVertices, IndexFormat.UInt32);
        m_Mesh.SetIndexBufferData(m_Indices, 0, 0, totalVertices);
        
        m_Mesh.SetSubMesh(0, new SubMeshDescriptor(0, totalVertices));
        m_Mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

        m_MeshFilter.sharedMesh = m_Mesh;

    }
}
