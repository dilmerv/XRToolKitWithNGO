using Unity.Netcode;
using UnityEngine;

public class GrabbableCreator : NetworkBehaviour
{
    [SerializeField]
    private GameObject[] prefabs;

    [SerializeField]
    private Vector2 placementArea = new Vector2(-10.0f, 10.0f);

    [SerializeField]
    private int maxObjectsToSpawn = 10;

    void Start()
    {
        if (IsServer || IsHost)
        {
            for (int i = 0; i < maxObjectsToSpawn; i++)
            {
                //prefabs[Random.Range(0, prefabs.Length)]
                GameObject go = Instantiate(prefabs[0], Vector3.zero, Quaternion.identity);
                go.transform.position = new Vector3(Random.Range(placementArea.x, placementArea.y), 0,
                    Random.Range(placementArea.x, placementArea.y));

                go.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
