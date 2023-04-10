using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveToClick : MonoBehaviour
{
    /// <summary>
    /// Distance from the camera to the object when the movement is finished.</summary>
    public float DistanceToObject = 4;

    /// <summary>
    /// Speed of the camera movement.</summary>
    public float MovementSpeed = 1F;

    /// <summary>
    /// Camera to move when the object is clicked.</summary>
    private Camera CameraToMove;

    private Vector3 NormalVector;
    private Vector3 CameraStartPosition;
    private Vector3 CameraEndPosition;
    private Quaternion CameraStartRotation;
    private Quaternion CameraEndRotation;
    private bool DoMovement = false;

    private float LerpMovement = 0;

    /// <summary>
    /// Start function from Unity3D</summary>
    /// <remarks>
    /// For more information, see: http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
    /// </remarks>
    void Start()
    {
        Collider[] colliderArray;
        CameraToMove = Camera.main;
        MeshCollider collider = this.gameObject.GetComponent<MeshCollider>();
        colliderArray = this.gameObject.GetComponents<Collider>();

        foreach (Collider item in colliderArray)
        {
            if (item!=collider)
            {
                item.enabled = false;
            }
        }
        if (!collider)
        {
            this.gameObject.AddComponent<MeshCollider>();

        }
        //if (collider == null)
        //{

        //    //If there is no collider, add one so the raycast works.
        //    //Debug.Log("No collider found. Adding one.");
        //}
    }

    /// <summary>
    /// Update function from Unity3D</summary>
    /// <remarks>
    /// For more information, see: http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
    /// </remarks>
    void LateUpdate()
    {
        ClickRaycastUpdate();
        MoveToPointUpdate();
    }

    /// <summary>
    /// Function that checks if the left button is clicked on top of the object.</summary>
    /// <remarks>
    /// When the gameObject which has this script is clicked, it calculates the end position and rotation 
    /// of the camera and initialize the neccesary parameters.
    /// </remarks>
    private void ClickRaycastUpdate()
    {
        if (Input.GetMouseButtonDown(0) && Camera.main.pixelRect.Contains(Input.mousePosition))
        {
            Ray ray = CameraToMove.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.Equals(this.gameObject))
                {
                    NormalVector = GetMeshColliderNormal(hit);
                    CameraStartPosition = CameraToMove.transform.position;
                    CameraStartRotation = CameraToMove.transform.rotation;
                    CameraEndPosition = new Vector3(hit.point.x + NormalVector.x * DistanceToObject, hit.point.y + NormalVector.y * DistanceToObject, hit.point.z + NormalVector.z * DistanceToObject);
                    CameraEndRotation = Quaternion.LookRotation(hit.point - CameraEndPosition);
                    LerpMovement = 0F;
                    DoMovement = true;
                }
            }
        }
    }

    /// <summary>
    /// Function that moves the camera to the object.</summary>
    /// <remarks>
    /// If there any movement to do, this function moves progressivelly the camera to the end position
    /// and rotation calculated when the object was clicked.
    /// </remarks>
    private void MoveToPointUpdate()
    {
        if (DoMovement)
        {
            CameraToMove.transform.position = Vector3.Lerp(CameraStartPosition, CameraEndPosition, LerpMovement);
            CameraToMove.transform.rotation = Quaternion.Lerp(CameraStartRotation, CameraEndRotation, LerpMovement);
            LerpMovement += Time.deltaTime * MovementSpeed;
            if (LerpMovement >= 1F)
            {
                LerpMovement = 0F;
                DoMovement = false;
            }
        }
    }

    /// <summary>
    /// Gets the normal vector of a point on a mesh.</summary>
    /// <remarks>
    /// This function calculates the normal vector af a point obtanied by raycasting.
    /// </remarks>
    private Vector3 GetMeshColliderNormal(RaycastHit hit)
    {
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        Mesh mesh = collider.sharedMesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

        Vector3 baryCenter = hit.barycentricCoordinate;
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        interpolatedNormal.Normalize();
        interpolatedNormal = hit.transform.TransformDirection(interpolatedNormal);
        return interpolatedNormal;
    }
}
