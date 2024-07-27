namespace Task3
{
    internal class Product
    {
        int state;
        long value;

        public long Value
        {
            get
            {
                if(state == 0)
                    return value;
                return 0;
            }
        }

        public Product(int value)
        {
            this.value = value;
        }

        public void Multyply(int value)
        {
            if (value == 0)
            {
                state++;
            }
            else
            { 
                this.value *= value;
            }
        }

        public void Divide(int value)
        {
            if (value == 0)
                state--;
            else
                this.value /= value;
        }

        public void Clear(int newValue)
        { 
            this.state = 0;
            this.value = newValue;
        }
    }
}
