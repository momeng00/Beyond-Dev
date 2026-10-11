using System.Collections.Generic;
using UnityEngine;

public class BuildingScroller : MonoBehaviour
{
    [SerializeField] private List<BuildingBlock> blockPrefabs = new List<BuildingBlock>();
    [SerializeField] private Transform upperBoundary;
    [SerializeField] private Transform lowerBoundary;
    [SerializeField, Min(0f)] private float speed = 2f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private BuildingBlock entrancePrefab;
    [SerializeField] private Transform entranceStopTarget;
    [SerializeField, Min(0)] private int minimumBlocksBeforeEntrance = 5;
    [SerializeField, Min(0.01f)] private float entranceDuration = 5f;
    [SerializeField, Range(0.1f, 10f)] private float decelerationStrength = 4f;

    private sealed class Entry
    {
        public BuildingBlock block;
        public int type;
    }

    private readonly List<Entry> active = new List<Entry>();
    private readonly List<Stack<Entry>> pools = new List<Stack<Entry>>();
    private readonly List<BuildingBlock> types = new List<BuildingBlock>();
    private readonly List<int> bag = new List<int>();
    private Transform poolRoot;
    private int bagPosition;
    private int lastType = -1;
    private bool initialized;
    private bool scrolling;
    private readonly Queue<int> entranceRoute = new Queue<int>();
    private Entry entranceEntry;
    private bool entranceRequested;
    private bool entrancePlaced;
    private bool arrived;
    private float entranceElapsed;
    private float entranceDistance;
    private float entranceMoved;
    private float arrivalY;
    private float travelDuration;
    private float travelStrength;
    // A configuration error must not produce an unbounded fill loop.
    private const int MaxAdditionsPerFill = 2048;

    private void Start()
    {
        if (playOnStart) StartScroll();
    }

    public void StartScroll()
    {
        if (arrived) return;
        if (!initialized && !Initialize()) return;
        scrolling = true;
    }

    public void StopScroll()
    {
        scrolling = false;
    }

    public void ResetScroll()
    {
        scrolling = false;
        for (int i = 0; i < active.Count; i++) ReturnToPool(active[i]);
        active.Clear();
        bag.Clear();
        bagPosition = 0;
        lastType = -1;
        initialized = false;
        entranceRoute.Clear();
        entranceRequested = false;
        entrancePlaced = false;
        arrived = false;
        entranceElapsed = 0f;
        entranceMoved = 0f;
    }

    // Connect a UI Button or other input handler to this method.
    public void RequestEntrance()
    {
        if (entranceRequested || arrived) return;
        if (!initialized && !Initialize()) return;
        if (!ValidBoundaries()) return;
        if (entrancePrefab == null || !entrancePrefab.HasValidImage ||
            !entrancePrefab.HasEntranceStopPoint || types.Contains(entrancePrefab) ||
            entranceStopTarget == null)
        {
            Fail("Assign a separate entrance prefab, its stop point and a stop target.");
            return;
        }
        arrivalY = entranceStopTarget.position.y;
        if (arrivalY <= lowerBoundary.position.y || arrivalY >= upperBoundary.position.y)
        {
            Fail("The entrance stop target must be between the upper and lower boundaries.");
            return;
        }
        int count = Mathf.Max(Mathf.Max(0, minimumBlocksBeforeEntrance), bag.Count - bagPosition);
        if (count > MaxAdditionsPerFill)
        {
            Fail("The entrance route exceeds the block safety limit.");
            return;
        }

        Entry entrance = GetEntrance();
        float stopOffset = entrance.block.EntranceStopPosition.y - entrance.block.TopCenter.y;
        float entranceHeight = entrance.block.TopCenter.y - entrance.block.BottomCenter.y;
        ReturnToPool(entrance);
        if (stopOffset > 0f || stopOffset < -entranceHeight)
        {
            Fail("The entrance stop point must be within the connection image's vertical bounds.");
            return;
        }

        // Measure in the same parent space used by active blocks; samples stay pooled.
        var heights = new float[types.Count];
        for (int i = 0; i < types.Count; i++)
        {
            Entry sample = TakeFromPool(i);
            heights[i] = sample.block.TopCenter.y - sample.block.BottomCenter.y;
            ReturnToPool(sample);
            if (heights[i] <= 0.001f)
            {
                Fail("A building block has an invalid height under the scroller transform.");
                return;
            }
        }
        float connectionY = active[active.Count - 1].block.BottomCenter.y;
        entranceRoute.Clear();
        for (int i = 0; i < count; i++)
        {
            int type = NextType();
            entranceRoute.Enqueue(type);
            connectionY -= heights[type];
        }
        entranceDistance = arrivalY - (connectionY + stopOffset);
        travelDuration = Mathf.Max(0.01f, entranceDuration);
        travelStrength = Mathf.Clamp(decelerationStrength, 0.1f, 10f);
        entranceElapsed = 0f;
        entranceMoved = 0f;
        entranceRequested = true;
        scrolling = true;
    }

    private Entry GetEntrance()
    {
        if (entranceEntry == null)
        {
            BuildingBlock block = Instantiate(entrancePrefab, poolRoot);
            block.gameObject.SetActive(false);
            entranceEntry = new Entry { block = block, type = -1 };
        }
        entranceEntry.block.transform.SetParent(transform, false);
        return entranceEntry;
    }

    private bool Initialize()
    {
        if (!ValidBoundaries()) return false;
        if (poolRoot == null)
        {
            if (blockPrefabs.Count == 0) return Fail("Register building block prefabs.");
            var unique = new HashSet<BuildingBlock>();
            foreach (BuildingBlock prefab in blockPrefabs)
            {
                if (prefab == null || !prefab.HasValidImage || !unique.Add(prefab))
                    return Fail("Each prefab must be unique and have a valid connection image.");
            }
            poolRoot = new GameObject("BuildingPool").transform;
            poolRoot.SetParent(transform, false);
            poolRoot.gameObject.SetActive(false);
            foreach (BuildingBlock prefab in blockPrefabs)
            {
                types.Add(prefab);
                pools.Add(new Stack<Entry>());
            }
        }
        if (!FillBelow()) return false;
        initialized = true;
        return true;
    }

    private bool ValidBoundaries()
    {
        if (upperBoundary == null || lowerBoundary == null ||
            upperBoundary.position.y <= lowerBoundary.position.y)
            return Fail("Assign upper and lower boundaries with upper Y greater than lower Y.");
        return true;
    }

    private void Update()
    {
        if (!scrolling || !ValidBoundaries()) return;
        float distance = Mathf.Max(0f, speed) * Time.deltaTime;
        if (entranceRequested)
        {
            entranceElapsed = Mathf.Min(travelDuration, entranceElapsed + Time.deltaTime);
            float progress = entranceElapsed / travelDuration;
            float eased = (1f - Mathf.Exp(-travelStrength * progress)) /
                (1f - Mathf.Exp(-travelStrength));
            float nextDistance = entranceDistance * eased;
            distance = nextDistance - entranceMoved;
            entranceMoved = nextDistance;
        }
        Vector3 movement = Vector3.up * distance;
        foreach (Entry entry in active) entry.block.transform.position += movement;
        RecycleAbove();
        if (!FillBelow()) return;
        RecycleAbove();
        if (entranceRequested && entranceElapsed >= travelDuration && entrancePlaced)
        {
            float correction = arrivalY - entranceEntry.block.EntranceStopPosition.y;
            foreach (Entry entry in active)
                entry.block.transform.position += Vector3.up * correction;
            arrived = true;
            scrolling = false;
        }
    }

    private void RecycleAbove()
    {
        // Keep the last block as the connection anchor even after a long frame.
        while (active.Count > 1 && active[0].block.BottomCenter.y > upperBoundary.position.y)
        {
            Entry entry = active[0];
            active.RemoveAt(0);
            ReturnToPool(entry);
        }
    }

    private bool FillBelow()
    {
        if (entrancePlaced) return true;
        int additions = 0;
        while (active.Count == 0 ||
            active[active.Count - 1].block.BottomCenter.y > lowerBoundary.position.y)
        {
            if (++additions > MaxAdditionsPerFill)
                return Fail("Too many blocks needed. Check block height, boundaries and scroll speed.");
            Vector3 connection = upperBoundary.position;
            if (active.Count > 0) connection = active[active.Count - 1].block.BottomCenter;
            Entry entry;
            if (!entranceRequested) entry = TakeFromPool(NextType());
            else if (entranceRoute.Count > 0) entry = TakeFromPool(entranceRoute.Dequeue());
            else entry = GetEntrance();
            if (!entry.block.HasValidImage)
            {
                ReturnToPool(entry);
                return Fail("A spawned block has an invalid connection image or height.");
            }
            entry.block.AlignTopTo(connection);
            active.Add(entry);
            entry.block.gameObject.SetActive(true);
            if (entry == entranceEntry)
            {
                entrancePlaced = true;
                break;
            }
            // Reclaim blocks passed during a long frame before taking the next one.
            RecycleAbove();
        }
        return true;
    }

    private int NextType()
    {
        if (bagPosition >= bag.Count)
        {
            bag.Clear();
            for (int i = 0; i < types.Count; i++) bag.Add(i);
            for (int i = bag.Count - 1; i > 0; i--)
            {
                int swapIndex = Random.Range(0, i + 1);
                Swap(i, swapIndex);
            }
            if (bag.Count > 1 && bag[0] == lastType)
                Swap(0, Random.Range(1, bag.Count));
            bagPosition = 0;
        }
        lastType = bag[bagPosition++];
        return lastType;
    }

    private void Swap(int first, int second)
    {
        int saved = bag[first];
        bag[first] = bag[second];
        bag[second] = saved;
    }

    private Entry TakeFromPool(int type)
    {
        Entry entry;
        if (pools[type].Count > 0) entry = pools[type].Pop();
        else
        {
            BuildingBlock block = Instantiate(types[type], poolRoot);
            block.gameObject.SetActive(false);
            entry = new Entry { block = block, type = type };
        }
        entry.block.transform.SetParent(transform, false);
        return entry;
    }

    private void ReturnToPool(Entry entry)
    {
        entry.block.gameObject.SetActive(false);
        entry.block.transform.SetParent(poolRoot, false);
        if (entry.type >= 0) pools[entry.type].Push(entry);
    }

    private bool Fail(string message)
    {
        scrolling = false;
        Debug.LogError(message, this);
        return false;
    }
}
