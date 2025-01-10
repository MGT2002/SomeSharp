LFUCache lfu = new LFUCache(2);
lfu.Put(1, 1);
lfu.Put(2, 2);
lfu.Get(1);
lfu.Put(3, 3);
lfu.Get(2);
lfu.Get(3);
lfu.Put(4, 4);
lfu.Get(1);
lfu.Get(3);
lfu.Get(4);

public class LFUCache
{
    /// <summary>
    /// 1. cache: key, value, frequency
    /// 2. get -> frequency++
    /// 3. put -> frequency = 1, delete least frequency
    /// </summary>

    int capacity;
    int size = 0;
    // key, value
    Dictionary<int, int> cache = [];
    // frequency, keys
    Dictionary<int, HashSet<int>> frequency = [];
    // key, DateTime
    Dictionary<int, DateTime> lastAccessed = [];

    public LFUCache(int capacity)
    {
        this.capacity = capacity;
    }

    public int Get(int key)
    {
        if (!cache.TryGetValue(key, out int value))
            return -1;

        
        return value;
    }

    public void Put(int key, int value)
    {
        if (size == capacity)
        {
            if (!cache.TryGetValue(key, out int _))
            {
                Evict();
            }
        }
        else
        {
            size++;
        }

        cache[key] = value;
    }

    private void Evict()
    {
        
    }
}

/**
 * Your LFUCache object will be instantiated and called as such:
 * LFUCache obj = new LFUCache(capacity);
 * int param_1 = obj.Get(key);
 * obj.Put(key,value);
 */