////////////////////////////////////////////////////
///////////////////////////////////////////////////
/// Written using: https://www.youtube.com/watch?v=CSeUMTaNFYk 
////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private int raycount;
    [SerializeField] private float FoV;
    [SerializeField] private LayerMask collidable;
    [SerializeField] private float viewDistance;
    private float startingAngle = 0.0f;

    private Mesh viewMesh;
    private Vector3 origin = Vector3.zero;
    private void Start()
    {
        viewMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = viewMesh;
    }
    private void LateUpdate()
    {
        SetOrigin(transform.position);
        SetAimDirrection(transform.right);

        float angleIncrease = FoV / raycount;
        float internalAngle = startingAngle;

        int vertexIndex = 1;
        int triangleIndex = 0;

        Vector3[] meshVertices = new Vector3[raycount + 1 /*origin*/ + 1 /*ray zero*/];
        Vector2[] uvMap = new Vector2[meshVertices.Length];
        int[] triangles = new int[raycount *3];

        //meshVertices[0] = origin;
        meshVertices[0] = new Vector3(0.0f, 0.0f, 0.0f);


        for (int i = 0; i <= raycount; i++)
        {
            Vector3 vertex;
            RaycastHit2D hit = Physics2D.Raycast(origin, getVectorFromAngle(internalAngle), viewDistance, collidable);
            if (hit.collider == null)
            {
                vertex = new Vector3(0.0f, 0.0f, 0.0f) + getVectorFromAngle(internalAngle) * viewDistance;
            }
            else
            {
                vertex = new Vector3(0.0f, 0.0f, 0.0f) + getVectorFromAngle(internalAngle) * hit.distance;
            }

            meshVertices[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;
            internalAngle -= angleIncrease;
        }


        viewMesh.vertices = meshVertices;
        viewMesh.uv = uvMap;
        viewMesh.triangles = triangles;
    }

    private Vector3 getVectorFromAngle(float incomingAngle)
    {
        float degreesToRadians = incomingAngle * (Mathf.PI / 180f);
        var tmp = new Vector3(Mathf.Cos(degreesToRadians), Mathf.Sin(degreesToRadians));
        return tmp;
    }

    public void SetOrigin(Vector3 incomingOrigin)
    {
        origin = incomingOrigin;
    }
    public void SetAimDirrection(Vector3 incomingAimDirrection)
    {
        incomingAimDirrection = incomingAimDirrection.normalized;
        float angle = Mathf.Atan2(incomingAimDirrection.x, incomingAimDirrection.y ) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            angle += 360.0f;
        }
        startingAngle = angle - FoV / 2f;
    }
}
