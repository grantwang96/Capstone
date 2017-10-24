using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour {

    public List<SpellEffect> spellEffects = new List<SpellEffect>();
    public List<SideEffect> sideEffects = new List<SideEffect>();

    public List<Transform> spellSpawns = new List<Transform>();
    public static SpellManager Instance;

    bool gameIsRunning = true;

    public Transform spellBookPrefab;
    public LayerMask spellBooks;

    [Range(5f, 30f)]
    public float refreshTime;

    bool rechargingSpellSpawn;
    public bool isSpawning;

    void Awake()
    {
        Instance = this;
        if (isSpawning)
        {
            StartCoroutine(spawnSpell());
        }
    }

    void Update()
    {

    }

    IEnumerator spawnSpell()
    {
        while(gameIsRunning)
        {
            shuffle(spellSpawns);
            for(int i = 0; i < spellSpawns.Count; i++)
            {
                Collider[] colls = Physics.OverlapSphere(spellSpawns[i].position, 3f, spellBooks, QueryTriggerInteraction.Collide);
                if(colls.Length == 0)
                {
                    Instantiate(spellBookPrefab, spellSpawns[i].position, Quaternion.identity);
                    break;
                }
            }
            yield return new WaitForSeconds(refreshTime);
        }
    }

    void shuffle<T>(List<T> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int rand = Random.Range(0, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    public void dropSpellBook(Vector3 position)
    {
        Vector3 modifiedPosition = new Vector3(position.x, 1.5f, position.z);
        Instantiate(spellBookPrefab, modifiedPosition, Quaternion.identity);
    }
}
