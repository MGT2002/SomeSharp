namespace GenericProject
{
    internal class ListNodeParser
    {
        public static ListNode[] ParseToLinkedLists(string arrays)
        {
            return ParseToLinkedLists(ParseStringToArrays(arrays));
        }

        private static ListNode[] ParseToLinkedLists(List<int[]> arrays)
        {
            List<ListNode> result = new();

            foreach (var arr in arrays)
            {
                ListNode head = null;
                ListNode current = null;

                foreach (var num in arr)
                {
                    if (head == null)
                    {
                        head = new ListNode(num);
                        current = head;
                    }
                    else
                    {
                        current.next = new ListNode(num);
                        current = current.next;
                    }
                }

                result.Add(head);
            }

            return result.ToArray();
        }

        private static List<int[]> ParseStringToArrays(string input)
        {
            var result = new List<int[]>();

            // Remove outer brackets and split into array strings
            input = input.Trim();
            if (input.StartsWith("[[") && input.EndsWith("]]"))
                input = input.Substring(2, input.Length - 4);

            var arrayStrings = input.Split("],[");

            foreach (var arrayStr in arrayStrings)
            {
                var numbers = arrayStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var intArray = Array.ConvertAll(numbers, s => int.Parse(s.Trim()));
                result.Add(intArray);
            }

            return result;
        }

    }
}
