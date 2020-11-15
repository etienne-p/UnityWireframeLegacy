﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(WireframeVisualizer))]
public class MeshWireframeVisualizer : MonoBehaviour
{
    [SerializeField]
    Mesh m_Mesh;

    WireframeVisualizer m_WireframeVisualizer;

    void Awake()
    {
        m_WireframeVisualizer = GetComponent<WireframeVisualizer>();
    }

    [ContextMenu("Update Wireframe")]
    void UpdateWireframe()
    {
        var vertices = m_Mesh.vertices;
        var indices = new List<int>();
        m_Mesh.GetIndices(indices, 0);

        var nativeVertices = new NativeArray<Vector3>(vertices.Length, Allocator.Temp);
        var nativeIndices = new NativeArray<int>(indices.Count, Allocator.Temp);

        nativeVertices.CopyFrom(vertices);
        nativeIndices.CopyFrom(indices.ToArray());

        m_WireframeVisualizer.UpdateGeometry(nativeVertices, nativeIndices);
    }
}
