namespace Library
{

    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual)
        {
            if(!Equals(expected, actual))
                throw new AssertExcep("Not equal");
        }

        public static void AreNotEqual<T>(T expected, T actual)
        {
            if(Equals(expected, actual))
                throw new AssertExcep("Equal");
        }
        public static void IsTrue(bool condition)
        {
            if(!condition)
                throw new AssertExcep("False");
        }

        public static void IsFalse(bool condition)
        {
            if(condition)
                throw new AssertExcep("True");
        }

        public static void IsNull(object obj)
        {
            if(obj != null)
                throw new AssertExcep("Not null");
        }

        public static void IsNotNull(object obj)
        {
            if(obj == null)
                throw new AssertExcep("Null");
        }
        public static void AreSame(object expected, object actual)
        {
            if (!ReferenceEquals(expected, actual))
                throw new AssertExcep("Not the same reference");
        }

        public static void AreNotSame(object expected, object actual)
        {
            if (ReferenceEquals(expected, actual))
                throw new AssertExcep("The same reference");
        }
        public static void Throws<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ex is TException)
                    return;

                throw new AssertExcep($"Expected {typeof(TException).Name}, got {ex.GetType().Name}");
            }

            throw new AssertExcep($"Expected {typeof(TException).Name}, but no exception was thrown");
        }
    }
}
