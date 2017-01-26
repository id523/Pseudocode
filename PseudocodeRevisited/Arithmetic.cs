using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudocodeRevisited
{
    /// <summary>
    /// Performs type conversions to do arithmetic.
    /// </summary>
    public static class Arithmetic
    {
        /// <summary>
        /// Called to signal an arithmetic error.
        /// </summary>
        private static int Error()
        {
            throw new RuntimeException("Unable to perform math operation");
        }
        /// <summary>
        /// Returns True if value is of an integer type, or False otherwise.
        /// </summary>
        public static bool IsInteger(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong;
        }
        /// <summary>
        /// Returns True if value is of a numeric type, or False otherwise.
        /// </summary>
        public static bool IsReal(object value)
        {
            return IsInteger(value)
                || value is float
                || value is double
                || value is decimal;
        }
        public static double ConvertDouble(object val)
        {
            try
            {
                return Convert.ToDouble(val);
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// An array of candidate lowest-common-denominator types for numeric comparisons.
        /// </summary>
        private static Type[] CommonTypes = 
            { typeof(long), typeof(double), typeof(bool), typeof(string), typeof(decimal) };
        /// <summary>
        /// Gets the right type to use for comparison.
        /// </summary>
        private static Type GetMostPreciseType(Type a, Type b)
        {
            foreach (Type t in CommonTypes)
            {
                if (a == t || b == t)
                {
                    return t;
                }
            }

            return a;
        }
        /// <summary>
        /// Returns a negative number if a is less than b, a positive number of a is greater than b or zero if they are
        /// numerically equal.
        /// </summary>
        public static int Compare(object a, object b)
        {
            try
            {
                Type mpt = GetMostPreciseType(a.GetType(), b.GetType());
                return System.Collections.Comparer.Default.Compare(Convert.ChangeType(a, mpt), Convert.ChangeType(b, mpt));
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// Adds two numbers or concatenates two strings.
        /// </summary>
        public static object Add(object a, object b)
        {
            if (IsInteger(a) && IsInteger(b))
                return Convert.ToInt64(a) + Convert.ToInt64(b);
            else if (IsReal(a) && IsReal(b))
                return Convert.ToDouble(a) + Convert.ToDouble(b);
            else if (a != null && b != null)
                return a.ToString() + b.ToString();
            else
                return Error();
        }
        /// <summary>
        /// Increments a number.
        /// </summary>
        public static object Increment(object a)
        {
            try
            {
                if (IsInteger(a))
                    return Convert.ToInt64(a) + 1;
                else
                    return Convert.ToDouble(a) + 1.0;
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// Subtracts one number (b) from another (a).
        /// </summary>
        public static object Subtract(object a, object b)
        {
            try
            {
                if (IsInteger(a) && IsInteger(b))
                    return Convert.ToInt64(a) - Convert.ToInt64(b);
                else
                    return Convert.ToDouble(a) - Convert.ToDouble(b);
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// Multiplies two numbers together.
        /// </summary>
        public static object Multiply(object a, object b)
        {
            try
            {
                if (IsInteger(a) && IsInteger(b))
                    return Convert.ToInt64(a) * Convert.ToInt64(b);
                else
                    return Convert.ToDouble(a) * Convert.ToDouble(b);
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// Divides one number (a) by another (b).
        /// </summary>
        public static object Divide(object a, object b)
        {
            try
            {
                return Convert.ToDouble(a) / Convert.ToDouble(b);
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// Returns the logical inverse (NOT) of a boolean value.
        /// </summary>
        public static object LogicalNot(object a)
        {
            try
            {
                return !Convert.ToBoolean(a);
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// Returns the logical AND of two boolean values.
        /// </summary>
        public static object LogicalAnd(object a, object b)
        {
            try
            {
                return Convert.ToBoolean(a) && Convert.ToBoolean(b);
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// Returns the logical OR of two boolean values.
        /// </summary>
        public static object LogicalOr(object a, object b)
        {
            try
            {
                return Convert.ToBoolean(a) || Convert.ToBoolean(b);
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// Returns the remainder from dividing an integer (a) by another integer (b).
        /// </summary>
        public static object Modulo(object a, object b)
        {
            try
            {
                return Convert.ToInt64(a) % Convert.ToInt64(b);
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// Returns the result of dividing an integer (a) by another integer (b).
        /// </summary>
        public static object IntDivide(object a, object b)
        {
            try
            {
                return Convert.ToInt64(a) / Convert.ToInt64(b);
            }
            catch (Exception)
            {
                return Error();
            }
        }
        /// <summary>
        /// Returns the greater of two numerical values.
        /// </summary>
        public static object Max(object a, object b)
        {
            return Compare(a, b) >= 0 ? a : b;
        }
        /// <summary>
        /// Returns the lesser of two numerical values.
        /// </summary>
        public static object Min(object a, object b)
        {
            return Compare(a, b) <= 0 ? a : b;
        }
        /// <summary>
        /// Returns the distance of the given value from zero (the absolute value).
        /// </summary>
        public static object Abs(object a)
        {
            if (Compare(a, 0) < 0)
                return Subtract(0, a);
            else
                return a;
        }
    }
}
