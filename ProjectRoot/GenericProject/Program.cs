//var r = new Solution().MergeKLists([
//    new(1, new(4, new(5))),
//    new(1, new(3, new(4))),
//    new(2, new(6))
//    ]);

using GenericProject;

var r = new Solution().MergeKLists(ListNodeParser.ParseToLinkedLists(
    "[[1,3,4,6,8,9,12],[1,2,5,7,11,21,24],[-4,0,4,7,10,14,22,29]]"
    ));

Console.WriteLine(r);

public class ListNode
{
    public int val;
    public ListNode next;
    public ListNode(int val = 0, ListNode next = null)
    {
        this.val = val;
        this.next = next;
    }

    // Optional: for easy printing
    public override string ToString()
    {
        List<int> values = new();
        ListNode current = this;
        while (current != null)
        {
            values.Add(current.val);
            current = current.next;
        }
        return string.Join(",", values);
    }
}

public class Solution
{
    public ListNode MergeKLists(ListNode[] lists)
    {
        if (lists.Length == 0)
            return null!;
        if (lists.Length == 1)
            return lists[0];

        for (int i = 1; i < lists.Length; i++)
        {
            Merge(ref lists[0], lists[i]);
        }

        return lists[0];
    }

    private void Merge(ref ListNode a, ListNode b)
    {
        if (a is null)
        { 
            a = b;
            return;
        }
        if (b is null)
        {
            return;
        }
        if (a.val > b.val)
        {
            var t = a;
            a = b;
            b = t;
        }

        var head = a;
        while (true)
        {
            if (a.next is null)
            {
                a.next = b;
                break;
            }

            if (a.next.val > b.val)
            {
                var t = a.next;
                a.next = b;
                b = t;
            }

            a = a.next;
        }

        a = head;
    }
}