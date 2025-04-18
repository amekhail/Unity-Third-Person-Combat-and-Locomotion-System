using UnityEngine;

/// <summary>
/// This class is specific for the Synty's Polygon Modular Fantasy Hero pack.
/// Will make this more modular for different body parts.
///
/// This works by getting the objects attached to the specific points on character
/// and making a static clone of the meshes at those locations while hiding the bones
/// for dismemberment on the character. 
/// </summary>
public class DismembermentController : MonoBehaviour
{
    [Header("Modular Parts (Skinned)")]
    public GameObject head;                     // e.g. head_04
    public GameObject[] accessories;            // e.g. eyebrows, beard, helmet

    [Header("Detach Settings")]
    public Transform detachSpawnPoint;          // Where to spawn the detached version
    public string staticMeshPath = "Characters_ModularParts_Static"; // Relative to Resources/

    [Header("Physics Settings")]
    public float throwForce = 6f;
    public float destroyAfter = 10f;
    public float drag = 3f;
    public float angularDrag = 2f;
    
    [Header("Debug")]
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    [ContextMenu("Test Dismember Head")]
    public void DismemberHead()
    {
        // TODO: If wearing a helmet, maybe sent the helmet flying off of the head
        if (head == null || detachSpawnPoint == null)
        {
            Debug.LogWarning("Missing head or detachSpawnPoint!");
            return;
        }

        // 1. Disable original parts
        head.SetActive(false);
        foreach (var accessory in accessories)
        {
            if (accessory != null)
                accessory.SetActive(false);
        }

        // 2. Create the root object
        GameObject detachedRoot = new GameObject("DetachedHead");
        detachedRoot.transform.position = detachSpawnPoint.position;
        detachedRoot.transform.rotation = detachSpawnPoint.rotation;

        // 3. Add static head
        SpawnStaticVersion(head, detachedRoot.transform);

        // 4. Add static accessories
        foreach (var acc in accessories)
        {
            if (acc != null)
                SpawnStaticVersion(acc, detachedRoot.transform);
        }

        // 5. Add physics
        Rigidbody rb = detachedRoot.AddComponent<Rigidbody>();
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;

        CapsuleCollider col = detachedRoot.AddComponent<CapsuleCollider>();
        col.radius = 0.2f;
        col.height = 0.4f;
        col.center = new Vector3(0, 0, 0);

        rb.AddForce(transform.forward * throwForce + Vector3.up * 2f, ForceMode.Impulse);
        
        // TODO: Play BloodFX and sounds
        animator.CrossFade("Die", 0.2f);

        // 6. Auto-destroy
        Destroy(detachedRoot, destroyAfter);
    }
    
    private void SpawnStaticVersion(GameObject originalSkinnedPart, Transform parent)
    {
        string staticName = originalSkinnedPart.name + "_Static";
        GameObject prefab = Resources.Load<GameObject>($"{staticMeshPath}/{staticName}");

        if (prefab == null)
        {
            Debug.LogWarning($"[Dismemberment] Static prefab not found for {staticName} in Resources/{staticMeshPath}");
            return;
        }

        GameObject instance = Instantiate(prefab, parent);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one * 0.01f;
    }
    
}