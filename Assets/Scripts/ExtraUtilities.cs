using UnityEngine;
using System.Collections;

public class ExtraUtilities {
    public static void helloWorld()
    {
        Debug.Log("Hello World!");
    }

    public static T GetComponentByTag<T>(string tag, GameObject obj) where T : Component
    {
        T[] components = obj.GetComponents<T>();
        Debug.Log(components[0].ToString());
        foreach(T component in components)
        {
            Debug.Log(component);
            if (component.tag == tag)
            {
                return component;
            }
        }
        return null;
    }
   
}


/* public struct ClosestPointOnMeshInfo
    {
        public float distanceFromPointSquared;
        public float distanceFromPoint
        {
            get
            {
                return Mathf.Sqrt(this.distanceFromPointSquared);
            }
        }
        public int triangleOnMesh;
        public Vector3 normal;
        public Vector3 centerOfMesh;
        public Vector3 closestPoint;
    }

    public static ClosestPointOnMeshInfo closestPointOnMeshTriangle(Vector3 startingPoint, MeshFilter meshFilter, int triangle)
    {
        Vector3 localStartingPoint = meshFilter.transform.InverseTransformPoint(startingPoint);
        ClosestPointOnMeshInfo result = new ClosestPointOnMeshInfo();
        result.triangleOnMesh = triangle;
        result.distanceFromPointSquared = Mathf.Infinity;

        if (triangle >= meshFilter.mesh.triangles.Length / 3)
        {
            return result;
        }

        Vector3 p1 = meshFilter.mesh.vertices[ meshFilter.mesh.triangles[0 + triangle*3] ];
        Vector3 p2 = meshFilter.mesh.vertices[ meshFilter.mesh.triangles[1 + triangle*3] ];
        Vector3 p3 = meshFilter.mesh.vertices[ meshFilter.mesh.triangles[2 + triangle*3] ];

        result.normal = Vector3.Cross((p2-p1).normalized, (p3-p1).normalized);

        Vector3 projected = localStartingPoint + Vector3.Dot((p1 - localStartingPoint), result.normal) * result.normal;

        Debug.Log(projected);
        Debug.Log(((p1.x * p2.y) - (p1.x * p3.y) - (p2.x * p1.y) + (p2.x * p3.y) + (p3.x * p1.y) - (p3.x * p2.y)));
         //Calculate the barycentric coordinates
         float u = ((projected.x * p2.y) - (projected.x * p3.y) - (p2.x * projected.y) + (p2.x * p3.y) + (p3.x * projected.y) - (p3.x  * p2.y)) / ((p1.x * p2.y)  - (p1.x * p3.y)  - (p2.x * p1.y) + (p2.x * p3.y) + (p3.x * p1.y)  - (p3.x * p2.y));
         float v = ((p1.x * projected.y) - (p1.x * p3.y) - (projected.x * p1.y) + (projected.x * p3.y) + (p3.x * p1.y) - (p3.x * projected.y)) / ((p1.x * p2.y)  - (p1.x * p3.y)  - (p2.x * p1.y) + (p2.x * p3.y) + (p3.x * p1.y)  - (p3.x * p2.y));
         float w = ((p1.x * p2.y) - (p1.x * projected.y) - (p2.x * p1.y) + (p2.x * projected.y) + (projected.x * p1.y) - (projected.x * p2.y)) / ((p1.x * p2.y)  - (p1.x * p3.y)  - (p2.x * p1.y) + (p2.x * p3.y) + (p3.x * p1.y)  - (p3.x * p2.y));
         Debug.Log(u);
        Debug.Log(v);
        Debug.Log(w);
         result.centerOfMesh = meshFilter.transform.TransformPoint(p1 * 0.3333f + p2 * 0.3333f + p3 * 0.3333f);
         
         //Find the nearest point
          var vector = (new Vector3(u,v,w)).normalized;
         
         //work out where that point is
         var nearest = p1 * vector.x + p2 * vector.y + p3 * vector.z;
        Debug.Log(nearest);
         result.closestPoint = meshFilter.transform.TransformPoint(nearest);
         result.distanceFromPointSquared = (nearest - localStartingPoint).sqrMagnitude;
         
         if(float.IsNaN(result.distanceFromPointSquared))
         {
             result.distanceFromPointSquared = float.PositiveInfinity;
         }
         return result;
    }*/