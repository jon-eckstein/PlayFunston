using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Globalization;
using System.Reflection;
using Nancy;
using System.IO;

namespace ShouldITakeMyDogToFortFunstonNow.Framework
{
    public static class Extensions
    {

        public static Response AsError(this IResponseFormatter formatter, HttpStatusCode statusCode, string message)
        {
            return new Response
            {
                StatusCode = statusCode,
                ContentType = "text/plain",
                Contents = stream => (new StreamWriter(stream) { AutoFlush = true }).Write(message)
            };
        }

        public static int[][] ToIntArray(this DataTable table, params string[] columnNames)
        {
            int[][] m = new int[table.Rows.Count][];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = new int[columnNames.Length];

            for (int j = 0; j < columnNames.Length; j++)
            {
                DataColumn col = table.Columns[columnNames[j]];

                for (int i = 0; i < table.Rows.Count; i++)
                    m[i][j] = convertToInt32(table.Rows[i][col]);
            }

            return m;
        }

        public static double[][] ToDoubleArray(this DataTable table, params string[] columnNames)
        {
            double[][] m = new double[table.Rows.Count][];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = new double[columnNames.Length];

            for (int j = 0; j < columnNames.Length; j++)
            {
                DataColumn col = table.Columns[columnNames[j]];

                for (int i = 0; i < table.Rows.Count; i++)
                    m[i][j] = convertToDouble(table.Rows[i][col]);
            }

            return m;
        }


        /// <summary>
        ///   Gets a column vector from a matrix.
        /// </summary>
        public static T[] GetColumn<T>(this T[,] m, int index)
        {
            T[] column = new T[m.GetLength(0)];

            for (int i = 0; i < column.Length; i++)
                column[i] = m[i, index];

            return column;
        }

        /// <summary>
        ///   Gets a column vector from a matrix.
        /// </summary>
        public static T[] GetColumn<T>(this T[][] m, int index)
        {
            T[] column = new T[m.Length];

            for (int i = 0; i < column.Length; i++)
                column[i] = m[i][index];

            return column;
        }

        private static int convertToInt32(object obj)
        {
            int d;

            if (obj is String)
            {
                d = int.Parse((String)obj, CultureInfo.InvariantCulture);
            }
            else if (obj is Boolean)
            {
                d = (Boolean)obj ? 1 : 0;
            }
            else
            {
                try
                {
                    d = System.Convert.ToInt32(obj);
                }
                catch (InvalidCastException)
                {
                    d = 0;
                }
            }

            return d;
        }


        private static double convertToDouble(object obj)
        {
            double d;

            if (obj is String)
            {
                d = Double.Parse((String)obj, CultureInfo.InvariantCulture);
            }
            else if (obj is Boolean)
            {
                d = (Boolean)obj ? 1.0 : 0.0;
            }
            else
            {
                try
                {
                    d = System.Convert.ToDouble(obj);
                }
                catch (InvalidCastException)
                {
                    d = 0;
                }
            }

            return d;
        }

        public static bool Contains(this string strVal, IEnumerable<string> vals)
        {
            foreach (var val in vals)
                if (strVal.Contains(val))
                    return true;

            return false;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                tb.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }
            return tb;
        }

    }
}