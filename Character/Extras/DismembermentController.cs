using UnityEngine;
using System.Collections.Generic;

public class DismembermentController : MonoBehaviour
{
    [Header("Modular Parts")]
    public Transform head;
    public Transform headCoveringsRoot;
    public Transform hairRoot;
    public Transform headAttachmentRoot;
    public Transform eyebrowsRoot;
    public Transform facialHairRoot;

    [Header("Runtime Data")]
    private List<GameObject> activeHeadParts = new List<GameObject>();
    public Transform headDetachSpawnPoint;

    [Header("Physics Settings")]
    public float throwForce = 3f;
    public float destroyAfter = 10f;

    [ContextMenu("Test Dismember Head")]
    public void DismemberHead()
    {
        if (head == null || headDetachSpawnPoint == null)
        {
            Debug.LogWarning("Missing head or spawn point!");
            return;
        }

        // 1. Find all active parts
        activeHeadParts.Clear();
        AddIfActive(head.gameObject);
        FindAndAddActiveChild(headCoveringsRoot);
        FindAndAddActiveChild(hairRoot);
        FindAndAddActiveChild(headAttachmentRoot);
        FindAndAddActiveChild(eyebrowsRoot);
        FindAndAddActiveChild(facialHairRoot);

        // 2. Disable original parts
        foreach (var part in activeHeadParts)
        {
            part.SetActive(false);
        }

        // 3. Create detached root object.
        GameObject detachedRoot = new GameObject("DetachedHead");
        detachedRoot.transform.position = headDetachSpawnPoint.position;
        detachedRoot.transform.rotation = headDetachSpawnPoint.rotation;

        // 4. Bake and clone each part
        foreach (var part in activeHeadParts)
        {
            GameObject baked = new GameObject(part.name + "_Baked");
            baked.transform.parent = detachedRoot.transform;
            baked.transform.position = part.transform.position;
            baked.transform.rotation = part.transform.rotation;
            baked.transform.localScale = part.transform.localScale;

            ConvertSkinnedMeshToMeshRenderer(part, baked);
        }

        // 5. Add physics
        Rigidbody rb = detachedRoot.AddComponent<Rigidbody>();
        CapsuleCollider col = detachedRoot.AddComponent<CapsuleCollider>();
        col.radius = 0.2f;
        col.height = 0.4f;

        rb.AddForce(transform.forward * throwForce + Vector3.up * 2f, ForceMode.Impulse);

        // 6. Auto-cleanup
        Destroy(detachedRoot, destroyAfter);
    }

    private void AddIfActive(GameObject obj)
    {
        if (obj != null && obj.activeInHierarchy)
        {
            activeHeadParts.Add(obj);
        }
    }

    private void FindAndAddActiveChild(Transform categoryRoot)
    {
        if (categoryRoot == null) return;

        foreach (Transform child in categoryRoot)
        {
            if (child.gameObject.activeSelf)
            {
                activeHeadParts.Add(child.gameObject);
                break; // Only one active per group
            }
        }
    }

    private void ConvertSkinnedMeshToMeshRenderer(GameObject source, GameObject target)
    {
        var skinned = source.GetComponent<SkinnedMeshRenderer>();
        if (skinned == null)
        {
            Debug.LogWarning($"No SkinnedMeshRenderer found on {source.name}");
            return;
        }

        // Bake the mesh
        Mesh bakedMesh = new Mesh();
        skinned.BakeMesh(bakedMesh);

        // Apply to MeshRenderer
        var filter = target.AddComponent<MeshFilter>();
        filter.sharedMesh = bakedMesh;

        var renderer = target.AddComponent<MeshRenderer>();
        renderer.sharedMaterials = skinned.sharedMaterials;
    }
}