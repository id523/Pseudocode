using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIF = PseudocodeRevisited.BuiltInFunctions;

namespace PseudocodeRevisited
{
    public partial class ExecutionState
    {
        /// <summary>
        /// Adds built-in functions.
        /// </summary>
        private void InitCoreBuiltIns()
        {
            #region Data Structure Creation
            AddFunction("array", BIF.MakeArray);
            AddFunction("stack", BIF.MakeObject<Stack<object>>);
            AddFunction("queue", BIF.MakeObject<Queue<object>>);
            AddFunction("collection", BIF.MakeObject<Pseudocollection>);
            #endregion
            #region Data Structure Operations
            AddFunction(".isEmpty", BIF.IsEmpty);
            AddFunction(".length", BIF.Length);
            AddFunction(".push", BIF.Push);
            AddFunction(".pop", BIF.Pop);
            AddFunction(".enqueue", BIF.Enqueue);
            AddFunction(".dequeue", BIF.Dequeue);
            AddFunction(".addItem", BIF.AddItems);
            AddFunction(".addItems", BIF.AddItems);
            AddFunction(".getNext", BIF.GetNext);
            AddFunction(".resetNext", BIF.ResetNext);
            AddFunction(".hasNext", BIF.HasNext);
            #endregion
            #region Input/Output
            AddFunction("writeLine", BIF.WriteLine);
            #endregion
            AddFunction(".__index", BIF.Index);
        }

        private Library CreateBuiltInsLibrary()
        {
            Library lib = new Library();
            lib.Add("hashtables.hashtable", "hashtable", BIF.MakeObject<Hashtable>);
            lib.Add("hashtables.hasKey", ".hasKey", BIF.HasKey);
            lib.Add("hashtables.getKey", ".getKey", BIF.GetKey);
            lib.Add("hashtables.setKey", ".setKey", BIF.SetKey);
            lib.Add("inputOutput.input", "input", BIF.Input);
            lib.Add("inputOutput.write", "write", BIF.Write);
            lib.Add("inputOutput.error", "error", BIF.Error);
            #region Math Functions
            lib.Add("math.abs", "abs", BIF.DblFunction("abs", Math.Abs));
            lib.Add("math.acos", "acos", BIF.DblFunction("acos", Math.Acos));
            lib.Add("math.asin", "asin", BIF.DblFunction("asin", Math.Asin));
            lib.Add("math.atan", "atan", BIF.Atan); // Math.Atan() and Math.Atan2()
            lib.Add("math.atan2", "atan2", BIF.DblFunction("atan2", Math.Atan2));
            lib.Add("math.ceil", "ceil", BIF.DblFunction("ceil", Math.Ceiling));
            lib.Add("math.cos", "cos", BIF.DblFunction("cos", Math.Cos));
            lib.Add("math.cosh", "cosh", BIF.DblFunction("cosh", Math.Cosh));
            lib.Add("math.exp", "exp", BIF.DblFunction("exp", Math.Exp));
            lib.Add("math.floor", "floor", BIF.DblFunction("floor", Math.Floor));
            lib.Add("math.ln", "ln", BIF.DblFunction("ln", (Func<double, double>)Math.Log));
            lib.Add("math.log", "log", BIF.DblFunction("log", (Func<double, double, double>)Math.Log));
            lib.Add("math.log10", "log10", BIF.DblFunction("log10", Math.Log10));
            lib.Add("math.max", "max", BIF.Max); // Math.Max()
            lib.Add("math.min", "min", BIF.Min); // Math.Min()
            lib.Add("math.pow", "pow", BIF.DblFunction("pow", Math.Pow));
            lib.Add("math.round", "round", BIF.DblFunction("round", Math.Round));
            lib.Add("math.sign", "sign", BIF.DblFunction("sign", Math.Sign));
            lib.Add("math.sin", "sin", BIF.DblFunction("sin", Math.Sin));
            lib.Add("math.sinh", "sinh", BIF.DblFunction("sinh", Math.Sinh));
            lib.Add("math.sqrt", "sqrt", BIF.DblFunction("sqrt", Math.Sqrt));
            lib.Add("math.tan", "tan", BIF.DblFunction("tan", Math.Tan));
            lib.Add("math.tanh", "tanh", BIF.DblFunction("tanh", Math.Tanh));
            lib.Add("math.truncate", "truncate", BIF.DblFunction("truncate", Math.Truncate));
            #endregion
            #region Functions on Functions
            lib.Add("functionObjects.invoke", ".invoke", BIF.InvokeFunction);
            lib.Add("functionObjects.invokeUnpack", ".invokeUnpack", BIF.InvokeUnpack);
            lib.Add("functionObjects.bind", ".bind", BIF.Bind);
            #endregion
            lib.Add("concat", "concat", BIF.Concat);
            return lib;
        }
    }
    public static class BuiltInFunctions
    {
        /// <summary>
        /// Creates and returns an array with the specified dimensions, performing type-conversion.
        /// </summary>
        public static object MakeArray(ExecutionState s, object[] args)
        {
            int[] parsed_indices = new int[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                parsed_indices[i] = Convert.ToInt32(args[i]);
            }
            return MakeArray(parsed_indices);
        }
        /// <summary>
        /// Creates and returns an array with the specified dimensions.
        /// </summary>
        private static object MakeArray(int[] dims)
        {
            // Return an empty array if the dimensions list was empty
            if (dims.Length == 0) return new object[0];
            // dims_chopped will contain all of dims except the first item
            int[] dims_chopped = new int[dims.Length - 1];
            for (int i = 0; i < dims_chopped.Length; i++)
            {
                dims_chopped[i] = dims[i + 1];
            }
            // Create an array
            object[] result = new object[dims[0]];
            // Fill it with sub-arrays
            if (dims_chopped.Length > 0)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = MakeArray(dims_chopped);
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a new, empty instance of a type.
        /// </summary>
        public static object MakeObject<T>(ExecutionState s, object[] args) where T : new()
        {
            return new T();
        }
        /// <summary>
        /// Returns the length of the (first dimension of) the given array.
        /// </summary>
        public static object Length(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for length()");
            object[] coll = args[0] as object[];
            if (coll == null)
                throw new RuntimeException("length() can only be used on arrays");
            return coll.Length;
        }
        /// <summary>
        /// Checks whether a stack, queue or collection is empty.
        /// </summary>
        public static object IsEmpty(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for isEmpty()");
            if (args[0] is Pseudocollection)
            {
                return ((Pseudocollection)args[0]).IsEmpty();
            }
            IEnumerable<object> coll = args[0] as IEnumerable<object>;
            if (coll == null)
                throw new RuntimeException("isEmpty() can only be used on collection-like values");
            return coll.Take(1).Count() <= 0;
        }
        /// <summary>
        /// Pushes a value or values to a stack.
        /// </summary>
        public static object Push(ExecutionState s, object[] args)
        {
            if (args.Length < 2)
                throw new RuntimeException("Insufficient parameters for push()");
            Stack<object> coll = args[0] as Stack<object>;
            if (coll == null)
                throw new RuntimeException("push() can only be used on stacks");
            for (int i = 1; i < args.Length; i++)
            {
                coll.Push(args[i]);
            }
            return null;
        }
        /// <summary>
        /// Pops a value from a stack.
        /// </summary>
        public static object Pop(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for pop()");
            Stack<object> coll = args[0] as Stack<object>;
            if (coll == null)
                throw new RuntimeException("pop() can only be used on stacks");
            return coll.Pop();
        }
        /// <summary>
        /// Adds a value or values to a queue.
        /// </summary>
        public static object Enqueue(ExecutionState s, object[] args)
        {
            if (args.Length < 2)
                throw new RuntimeException("Insufficient parameters for enqueue()");
            Queue<object> coll = args[0] as Queue<object>;
            if (coll == null)
                throw new RuntimeException("enqueue() can only be used on queues");
            for (int i = 1; i < args.Length; i++)
            {
                coll.Enqueue(args[i]);
            }
            return null;
        }
        /// <summary>
        /// Removes a value from a queue.
        /// </summary>
        public static object Dequeue(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for dequeue()");
            Queue<object> coll = args[0] as Queue<object>;
            if (coll == null)
                throw new RuntimeException("dequeue() can only be used on queues");
            return coll.Dequeue();
        }
        /// <summary>
        /// Adds an item or items to a collection.
        /// </summary>
        public static object AddItems(ExecutionState s, object[] args)
        {
            if (args.Length < 2)
                throw new RuntimeException("Insufficient parameters for addItem()");
            Pseudocollection coll = args[0] as Pseudocollection;
            if (coll == null)
                throw new RuntimeException("addItem() can only be used on collections");
            for (int i = 1; i < args.Length; i++)
            {
                coll.AddItem(args[i]);
            }
            return null;
        }
        /// <summary>
        /// Gets the next item from a collection.
        /// </summary>
        public static object GetNext(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for getNext()");
            Pseudocollection coll = args[0] as Pseudocollection;
            if (coll == null)
                throw new RuntimeException("getNext() can only be used on collections");
            return coll.GetNext();
        }
        /// <summary>
        /// Moves back to the beginning of a collection.
        /// </summary>
        public static object ResetNext(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for resetNext()");
            Pseudocollection coll = args[0] as Pseudocollection;
            if (coll == null)
                throw new RuntimeException("resetNext() can only be used on collections");
            coll.ResetNext();
            return null;
        }
        /// <summary>
        /// Checks if getNext will succeed.
        /// </summary>
        public static object HasNext(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for hasNext()");
            Pseudocollection coll = args[0] as Pseudocollection;
            if (coll == null)
                throw new RuntimeException("hasNext() can only be used on collections");
            return coll.HasNext();
        }
        /// <summary>
        /// Concatenates all of its arguments together and prints that line of text to the screen.
        /// "output a, b, c" is internally converted to "writeline(a, b, c)"
        /// </summary>
        public static object WriteLine(ExecutionState s, object[] args)
        {
            Console.WriteLine(string.Concat(args));
            return null;
        }
        /// <summary>
        /// Concatenates all of its arguments together and prints that text to the screen,
        /// without a newline.
        /// </summary>
        public static object Write(ExecutionState s, object[] args)
        {
            Console.Write(string.Concat(args));
            return null;
        }
        /// <summary>
        /// Indexes into an array or a hashtable. Multiple indices can be used for multi-dimensional arrays.
        /// "array[a, b][c]" is internally converted to "array.__index(a, b).__index(c)"
        /// </summary>
        public static object Index(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Not possible to index nothing");
            object arr = args[0];
            for (int i = 1; i < args.Length; i++)
            {
                if (arr is Expressions.IGetValue)
                    arr = ((Expressions.IGetValue)arr).GetValue(s);
                if (arr is object[])
                {
                    arr = new Expressions.ArrayIndexVariable((object[])arr, args[i]);
                }
                else if (arr is IDictionary)
                {
                    arr = new Expressions.HashtableIndexVariable((IDictionary)arr, args[i]);
                }
            }
            return arr;
        }
        /// <summary>
        /// Reads and returns a line of user input.
        /// </summary>
        public static object Input(ExecutionState s, object[] args)
        {
            return Console.ReadLine();
        }
        /// <summary>
        /// Checks whether the specified key is present in a hashtable.
        /// </summary>
        public static object HasKey(ExecutionState s, object[] args)
        {
            if (args.Length < 2)
                throw new RuntimeException("Insufficient parameters for hasKey()");
            IDictionary coll = args[0] as IDictionary;
            if (coll == null)
                throw new RuntimeException("hasKey() can only be used on hashtables");
            try
            {
                return coll.Contains(args[1]);
            }
            catch (Exception)
            {
                throw new RuntimeException("Invalid key for a hashtable");
            }
        }
        /// <summary>
        /// Gets the value that corresponds to the specified key in a hashtable.
        /// </summary>
        public static object GetKey(ExecutionState s, object[] args)
        {
            if (args.Length < 2)
                throw new RuntimeException("Insufficient parameters for getKey()");
            IDictionary coll = args[0] as IDictionary;
            if (coll == null)
                throw new RuntimeException("getKey() can only be used on hashtables");
            try
            {
                return coll[args[1]];
            }
            catch (Exception)
            {
                throw new RuntimeException("Invalid key for a hashtable");
            }
        }
        /// <summary>
        /// Sets the value that corresponds to the specified key in a hashtable.
        /// </summary>
        public static object SetKey(ExecutionState s, object[] args)
        {
            if (args.Length < 3)
                throw new RuntimeException("Insufficient parameters for setKey()");
            IDictionary coll = args[0] as IDictionary;
            if (coll == null)
                throw new RuntimeException("setKey() can only be used on hashtables");
            try
            {
                coll[args[1]] = args[2];
            }
            catch (Exception)
            {
                throw new RuntimeException("The value could not be stored in the hashtable");
            }
            return null;
        }
        /// <summary>
        /// Creates a pseudocode function that returns a constant value.
        /// </summary>
        public static PseudocodeFunction Constant(object value)
        {
            return delegate (ExecutionState s, object[] args)
            {
                return value;
            };
        }
        /// <summary>
        /// Creates a pseudocode function from a mathematical operation with one argument.
        /// </summary>
        public static PseudocodeFunction DblFunction<TResult>(string name, Func<double, TResult> func)
        {
            string moreargs_message = "Insufficient parameters for " + name + "()";
            return delegate (ExecutionState s, object[] args)
            {
                if (args.Length < 1)
                    throw new RuntimeException(moreargs_message);
                return func(Arithmetic.ConvertDouble(args[0]));
            };
        }
        /// <summary>
        /// Creates a pseudocode function from a mathematical operation with two arguments.
        /// </summary>
        public static PseudocodeFunction DblFunction<TResult>(string name, Func<double, double, TResult> func)
        {
            string moreargs_message = "Insufficient parameters for " + name + "()";
            return delegate (ExecutionState s, object[] args)
            {
                if (args.Length < 2)
                    throw new RuntimeException(moreargs_message);
                return func(Arithmetic.ConvertDouble(args[0]), Arithmetic.ConvertDouble(args[1]));
            };
        }
        /// <summary>
        /// A wrapper for either <see cref="Math.Atan(double)"/> or <see cref="Math.Atan2(double, double)"/>
        /// depending on parameter count.
        /// </summary>
        public static object Atan(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for atan()");
            if (args.Length == 1)
                return Math.Atan(Arithmetic.ConvertDouble(args[0]));
            else
                return Math.Atan2(Arithmetic.ConvertDouble(args[0]), Arithmetic.ConvertDouble(args[1]));
        }
        /// <summary>
        /// Returns the value of the smallest parameter.
        /// </summary>
        public static object Min(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for min()");
            return args.Aggregate(Arithmetic.Min);
        }
        /// <summary>
        /// Returns the value of the largest parameter.
        /// </summary>
        public static object Max(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for max()");
            return args.Aggregate(Arithmetic.Max);
        }
        /// <summary>
        /// Raises a runtime error with the specified message.
        /// </summary>
        public static object Error(ExecutionState s, object[] args)
        {
            throw new RuntimeException(string.Concat(args));
        }
        /// <summary>
        /// Returns the concatenation of all of its arguments as a string.
        /// </summary>
        public static object Concat(ExecutionState s, object[] args)
        {
            return string.Concat(args);
        }
        /// <summary>
        /// Invokes a function on its arguments.
        /// func.invoke(x, y, z) is equivalent to func(x, y, z)
        /// </summary>
        public static object InvokeFunction(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for invoke()");
            object[] butFirst = new object[args.Length - 1];
            Array.Copy(args, 1, butFirst, 0, butFirst.Length);
            return s.CallFunction(args[0], butFirst);
        }
        /// <summary>
        /// Invokes a function with a specified collection of arguments.
        /// If 'arr' is an array containing [1, 2, 3], func.invokeUnpack(arr) is equivalent to func(1, 2, 3)
        /// </summary>
        public static object InvokeUnpack(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for invokeUnpack()");
            List<object> NewArgs = new List<object>();
            for (int i = 1; i < args.Length; i++)
            {
                IEnumerable<object> coll = args[i] as IEnumerable<object>;
                if (coll == null)
                    NewArgs.Add(args[i]);
                else
                    NewArgs.AddRange(coll);
            }
            return s.CallFunction(args[0], NewArgs.ToArray());
        }
        /// <summary>
        /// Returns a new function that behaves like another function but with parameters added to the beginning.
        /// func.bind(A, B).invoke(...) => func(A, B, ...)
        /// </summary>
        public static object Bind(ExecutionState s, object[] args)
        {
            if (args.Length < 1)
                throw new RuntimeException("Insufficient parameters for bind()");
            PseudocodeFunction func = args[0] as PseudocodeFunction;
            if (func == null)
                throw new RuntimeException("bind() can only be used on functions");
            int lengthExceptFirst = args.Length - 1;
            return new PseudocodeFunction(delegate (ExecutionState s2, object[] argsInner)
            {
                object[] modArgs = new object[argsInner.Length + lengthExceptFirst];
                Array.Copy(args, 1, modArgs, 0, lengthExceptFirst);
                Array.Copy(argsInner, 0, modArgs, lengthExceptFirst, argsInner.Length);
                return func.Invoke(s2, modArgs);
            });
        }
    }
}
